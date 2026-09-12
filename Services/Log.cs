using System;
using System.IO;
using System.Text;

namespace MicriCancelli.Services
{
    /// <summary>
    /// Log minimale su file (cartella "logs" accanto all'eseguibile, un file al giorno)
    /// con notifica all'interfaccia tramite l'evento <see cref="Scritto"/>.
    /// </summary>
    public static class Log
    {
        private static readonly object _lock = new object();
        private static readonly string _cartella = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

        /// <summary>Sollevato per ogni riga scritta, sul thread chiamante. La UI deve fare Invoke.</summary>
        public static event Action<string> Scritto;

        public static void Info(string messaggio) => Scrivi("INFO ", messaggio);

        public static void Avviso(string messaggio) => Scrivi("WARN ", messaggio);

        public static void Errore(string messaggio, Exception ex = null)
        {
            if (ex != null) messaggio += " -> " + ex.GetType().Name + ": " + ex.Message;
            Scrivi("ERROR", messaggio);
        }

        private static void Scrivi(string livello, string messaggio)
        {
            var adesso = DateTime.Now;
            var riga = adesso.ToString("HH:mm:ss") + " [" + livello.Trim() + "] " + messaggio;
            try
            {
                lock (_lock)
                {
                    Directory.CreateDirectory(_cartella);
                    File.AppendAllText(Path.Combine(_cartella, "micricancelli-" + adesso.ToString("yyyyMMdd") + ".log"),
                        adesso.ToString("yyyy-MM-dd ") + riga + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch
            {
                // il log non deve mai far cadere l'applicazione
            }
            try { Scritto?.Invoke(riga); } catch { }
        }
    }
}
