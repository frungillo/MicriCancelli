using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MicriCancelli.Services
{
    /// <summary>
    /// Collegamento alla piattaforma MICRI AI: il PC del varco resta in long polling su
    /// GET {url}/api/varco/comandi (autenticato con l'header X-Varco-Chiave), esegue i comandi
    /// di apertura che la piattaforma accoda (dalla chat IA o dal pannello, solo Dirigenti e Admin)
    /// e ne riporta l'esito con POST {url}/api/varco/comandi/{id}/esito.
    /// La connessione è sempre in uscita: sul PC del varco non serve aprire porte.
    /// </summary>
    public sealed class ComandiRemoti : IDisposable
    {
        private const int SecondiLongPoll = 25;

        private readonly Impostazioni _imp;
        private readonly Cancello _cancello;
        private readonly CancellationTokenSource _stop = new CancellationTokenSource();
        private readonly HttpClient _http;
        private Task _ciclo;
        private bool _collegato;
        private string _ultimoErrore;

        public ComandiRemoti(Impostazioni imp, Cancello cancello)
        {
            _imp = imp;
            _cancello = cancello;
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            _http = new HttpClient { Timeout = TimeSpan.FromSeconds(SecondiLongPoll + 15) };
            _http.DefaultRequestHeaders.UserAgent.ParseAdd("MicriCancelli/2.0");
        }

        public bool Attivo => !string.IsNullOrWhiteSpace(_imp.PiattaformaChiave) && !string.IsNullOrWhiteSpace(_imp.PiattaformaUrl);

        public void Avvia()
        {
            if (_ciclo != null) return;
            _ciclo = Task.Run(() => CicloAsync(_stop.Token));
        }

        private async Task CicloAsync(CancellationToken stop)
        {
            var avvisatoInattivo = false;
            while (!stop.IsCancellationRequested)
            {
                if (!Attivo)
                {
                    if (!avvisatoInattivo) { Log.Info("Collegamento alla piattaforma non configurato (parametri piattaformaUrl / piattaformaChiave)"); avvisatoInattivo = true; }
                    _collegato = false;
                    await Attendi(5000, stop);
                    continue;
                }
                avvisatoInattivo = false;

                try
                {
                    var comando = await PrelevaComandoAsync(stop);
                    if (!_collegato)
                    {
                        _collegato = true;
                        _ultimoErrore = null;
                        Log.Info("Collegato alla piattaforma " + _imp.PiattaformaUrl);
                    }
                    if (comando != null) await EseguiAsync(comando);
                }
                catch (OperationCanceledException) when (stop.IsCancellationRequested) { }
                catch (Exception ex)
                {
                    _collegato = false;
                    var testo = ex.GetType().Name + ": " + ex.Message;
                    if (testo != _ultimoErrore) { Log.Errore("Piattaforma non raggiungibile", ex); _ultimoErrore = testo; }
                    await Attendi(ex is ChiaveRifiutataException ? 30000 : 10000, stop);
                }
            }
        }

        private async Task<ComandoRicevuto> PrelevaComandoAsync(CancellationToken stop)
        {
            using (var richiesta = new HttpRequestMessage(HttpMethod.Get, Url("/api/varco/comandi?attesa=" + SecondiLongPoll)))
            {
                richiesta.Headers.Add("X-Varco-Chiave", _imp.PiattaformaChiave);
                using (var risposta = await _http.SendAsync(richiesta, HttpCompletionOption.ResponseContentRead, stop).ConfigureAwait(false))
                {
                    if (risposta.StatusCode == HttpStatusCode.NoContent) return null;
                    if (risposta.StatusCode == HttpStatusCode.Unauthorized)
                        throw new ChiaveRifiutataException("la piattaforma rifiuta la chiave configurata (parametro piattaformaChiave)");
                    risposta.EnsureSuccessStatusCode();
                    var json = await risposta.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return new JavaScriptSerializer().Deserialize<ComandoRicevuto>(json);
                }
            }
        }

        private async Task EseguiAsync(ComandoRicevuto comando)
        {
            Log.Info("Comando remoto #" + comando.id + " '" + comando.comando + "' da " + comando.utente + " (" + comando.ruolo + ", " + comando.origine + ")");
            bool ok;
            string dettaglio;
            if (string.Equals(comando.comando, "apri", StringComparison.OrdinalIgnoreCase))
            {
                if (_cancello.InApertura)
                {
                    ok = true;
                    dettaglio = "Sbarra già in apertura";
                }
                else
                {
                    ok = await _cancello.ApriChiudiAsync("comando remoto di " + comando.utente).ConfigureAwait(false);
                    dettaglio = ok ? "Sequenza apri/chiudi completata" : "Arduino " + _imp.IpArduino + ":" + _imp.PortaArduino + " non ha risposto";
                }
            }
            else
            {
                ok = false;
                dettaglio = "Comando sconosciuto: " + comando.comando;
            }

            try
            {
                var corpo = new JavaScriptSerializer().Serialize(new { eseguito = ok, dettaglio });
                using (var richiesta = new HttpRequestMessage(HttpMethod.Post, Url("/api/varco/comandi/" + comando.id + "/esito")))
                {
                    richiesta.Headers.Add("X-Varco-Chiave", _imp.PiattaformaChiave);
                    richiesta.Content = new StringContent(corpo, Encoding.UTF8, "application/json");
                    using (var risposta = await _http.SendAsync(richiesta).ConfigureAwait(false))
                        risposta.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                Log.Errore("Esito del comando remoto #" + comando.id + " non consegnato alla piattaforma", ex);
            }
        }

        private string Url(string percorso) => _imp.PiattaformaUrl.TrimEnd('/') + percorso;

        private static async Task Attendi(int ms, CancellationToken stop)
        {
            try { await Task.Delay(ms, stop).ConfigureAwait(false); } catch (OperationCanceledException) { }
        }

        public void Dispose()
        {
            _stop.Cancel();
            _http.Dispose();
        }

        private sealed class ComandoRicevuto
        {
            public int id { get; set; }
            public string comando { get; set; }
            public string utente { get; set; }
            public string ruolo { get; set; }
            public string origine { get; set; }
        }

        private sealed class ChiaveRifiutataException : Exception
        {
            public ChiaveRifiutataException(string messaggio) : base(messaggio) { }
        }
    }
}
