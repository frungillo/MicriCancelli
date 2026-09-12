using System;
using System.Threading.Tasks;
using MicriCancelli.Data;
using MicriCancelli.Models;

namespace MicriCancelli.Services
{
    public enum EsitoAccesso
    {
        Consentito,
        CodiceSconosciuto,
        Scaduto,
        GiaUsato,
        CancelloOccupato,
        ErroreCancello
    }

    /// <summary>Regole di validità dei biglietti e generazione di nuovi codici.</summary>
    public sealed class GestoreAccessi
    {
        private readonly CodiciRepository _codici;
        private readonly Impostazioni _imp;
        private readonly Cancello _cancello;

        public GestoreAccessi(CodiciRepository codici, Impostazioni imp, Cancello cancello)
        {
            _codici = codici;
            _imp = imp;
            _cancello = cancello;
        }

        /// <summary>Valuta un codice letto dallo scanner e, se valido, apre il cancello.</summary>
        public async Task<EsitoAccesso> ProcessaAsync(string letto)
        {
            int numero;
            if (!int.TryParse(letto, out numero))
            {
                Log.Avviso("Lettura non numerica ignorata: '" + letto + "'");
                return EsitoAccesso.CodiceSconosciuto;
            }

            var codice = _codici.Trova(numero);
            if (codice == null)
            {
                Log.Avviso("Codice " + letto + " non presente in tabella: passaggio negato");
                return EsitoAccesso.CodiceSconosciuto;
            }

            var adesso = DateTime.Now;
            if (codice.UsatoIl == null)
            {
                var scadenza = codice.EmessoIl.AddMinutes(_imp.MinutiValidita);
                if (adesso > scadenza)
                {
                    Log.Avviso("Codice " + letto + " scaduto alle " + scadenza.ToString("HH:mm") + ": passaggio negato");
                    return EsitoAccesso.Scaduto;
                }
                _codici.SegnaUsato(codice.Id, adesso);
                Log.Info("Codice " + letto + " valido, primo utilizzo");
            }
            else
            {
                var trascorsi = (adesso - codice.UsatoIl.Value).TotalSeconds;
                if (trascorsi > _imp.SecondiGrazia)
                {
                    Log.Avviso("Codice " + letto + " già usato alle " + codice.UsatoIl.Value.ToString("HH:mm:ss") + ": passaggio negato");
                    return EsitoAccesso.GiaUsato;
                }
                Log.Info("Codice " + letto + " riletto entro " + _imp.SecondiGrazia + " s dall'uso, consentito");
            }

            if (_cancello.InApertura)
            {
                Log.Info("Cancello già in apertura, il codice " + letto + " passa con l'apertura in corso");
                return EsitoAccesso.CancelloOccupato;
            }

            var ok = await _cancello.ApriChiudiAsync("codice " + letto).ConfigureAwait(false);
            return ok ? EsitoAccesso.Consentito : EsitoAccesso.ErroreCancello;
        }

        /// <summary>Genera un nuovo codice univoco e lo salva come emesso adesso.</summary>
        public Codice GeneraNuovo()
        {
            for (int tentativo = 0; tentativo < 1000; tentativo++)
            {
                var numero = Sicurezza.NumeroCasuale(_imp.LunghezzaCodice);
                if (_codici.Esiste(numero)) continue;
                var codice = new Codice { Numero = numero, EmessoIl = DateTime.Now, Tipo = _imp.TipoCodice };
                _codici.Inserisci(codice);
                Log.Info("Generato codice " + codice.NumeroFormattato(_imp.LunghezzaCodice) + " (" + codice.Tipo + ")");
                return codice;
            }
            throw new InvalidOperationException("Impossibile trovare un codice libero: tabella quasi piena?");
        }
    }
}
