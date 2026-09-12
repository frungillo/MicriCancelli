using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MicriCancelli.Services
{
    /// <summary>
    /// Comanda il cancello tramite la scheda Arduino (TCP, comandi "apri*" e "chiudi*").
    /// Ogni chiamata ha timeout e non solleva mai eccezioni: l'esito è il valore di ritorno, il dettaglio va nel log.
    /// È il punto unico da cui passa qualunque apertura: pulsante, scanner e, in futuro, comandi remoti dalla piattaforma IA.
    /// </summary>
    public sealed class Cancello
    {
        private readonly Impostazioni _imp;
        private readonly SemaphoreSlim _sequenza = new SemaphoreSlim(1, 1);

        public Cancello(Impostazioni imp) { _imp = imp; }

        /// <summary>True mentre una sequenza apri/attesa/chiudi è in corso.</summary>
        public bool InApertura => _sequenza.CurrentCount == 0;

        /// <summary>Sequenza completa: apri, attesa "inching", chiudi. Se una sequenza è già in corso la richiesta viene ignorata.</summary>
        public async Task<bool> ApriChiudiAsync(string motivo)
        {
            if (!await _sequenza.WaitAsync(0).ConfigureAwait(false))
            {
                Log.Avviso("Apertura già in corso, richiesta ignorata (" + motivo + ")");
                return false;
            }
            try
            {
                Log.Info("Apertura cancello: " + motivo);
                if (!await InviaAsync("apri*").ConfigureAwait(false)) return false;
                await Task.Delay(_imp.InchingMs).ConfigureAwait(false);
                await InviaAsync("chiudi*").ConfigureAwait(false);
                return true;
            }
            finally
            {
                _sequenza.Release();
            }
        }

        public Task<bool> ApriAsync() => InviaAsync("apri*");

        public Task<bool> ChiudiAsync() => InviaAsync("chiudi*");

        private async Task<bool> InviaAsync(string comando)
        {
            var destinazione = _imp.IpArduino + ":" + _imp.PortaArduino;
            try
            {
                IPAddress ip;
                if (!IPAddress.TryParse(_imp.IpArduino, out ip))
                {
                    Log.Errore("Indirizzo IP Arduino non valido: '" + _imp.IpArduino + "'");
                    return false;
                }

                using (var client = new TcpClient(AddressFamily.InterNetwork))
                {
                    var connessione = client.ConnectAsync(ip, _imp.PortaArduino);
                    var completato = await Task.WhenAny(connessione, Task.Delay(_imp.TimeoutArduinoMs)).ConfigureAwait(false);
                    if (completato != connessione)
                    {
                        // osserva l'eventuale eccezione della connect abbandonata, così non resta "unobserved"
                        var _ = connessione.ContinueWith(t => { var e = t.Exception; }, TaskContinuationOptions.OnlyOnFaulted);
                        Log.Errore("Timeout (" + _imp.TimeoutArduinoMs + " ms) connettendo ad Arduino " + destinazione);
                        return false;
                    }
                    await connessione.ConfigureAwait(false); // rilancia eventuali errori di connessione

                    using (var stream = client.GetStream())
                    {
                        stream.WriteTimeout = _imp.TimeoutArduinoMs;
                        var dati = Encoding.ASCII.GetBytes(comando);
                        await stream.WriteAsync(dati, 0, dati.Length).ConfigureAwait(false);
                        await stream.FlushAsync().ConfigureAwait(false);
                    }
                }
                Log.Info("Comando '" + comando + "' inviato ad Arduino " + destinazione);
                return true;
            }
            catch (Exception ex)
            {
                Log.Errore("Errore inviando '" + comando + "' ad Arduino " + destinazione, ex);
                return false;
            }
        }
    }
}
