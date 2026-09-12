using System;
using MicriCancelli.Data;

namespace MicriCancelli.Services
{
    /// <summary>
    /// Cancella i codici più vecchi di <see cref="Impostazioni.GiorniConservazione"/> giorni.
    /// Un biglietto vale pochi minuti: dopo la conservazione il numero torna libero, la tabella
    /// resta piccola e la generazione di nuovi codici non trova quasi mai un numero già occupato.
    /// </summary>
    public sealed class PuliziaCodici
    {
        private readonly CodiciRepository _codici;
        private readonly Impostazioni _imp;
        private readonly object _lock = new object();

        public DateTime? UltimaEsecuzione { get; private set; }

        public PuliziaCodici(CodiciRepository codici, Impostazioni imp)
        {
            _codici = codici;
            _imp = imp;
        }

        /// <summary>Esegue la pulizia; con <paramref name="compatta"/> recupera anche lo spazio su disco (VACUUM).</summary>
        public int Esegui(bool compatta)
        {
            lock (_lock)
            {
                try
                {
                    var limite = DateTime.Now.AddDays(-_imp.GiorniConservazione);
                    var eliminati = _codici.EliminaEmessiPrimaDi(limite);
                    UltimaEsecuzione = DateTime.Now;
                    if (eliminati > 0 || compatta)
                    {
                        // compattazione: richiesta esplicitamente (dopo una migrazione) o quando si è liberato molto
                        if (compatta || eliminati >= 5000) { _codici.Compatta(); compatta = true; }
                        Log.Info("Pulizia codici: eliminati " + eliminati + " codici emessi prima del " + limite.ToString("dd/MM/yyyy") +
                                 ", restano " + _codici.Conta() + (compatta ? ", database compattato" : ""));
                    }
                    return eliminati;
                }
                catch (Exception ex)
                {
                    Log.Errore("Pulizia codici non riuscita", ex);
                    return 0;
                }
            }
        }
    }
}
