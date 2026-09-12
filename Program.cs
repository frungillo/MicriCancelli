using System;
using System.Threading;
using System.Windows.Forms;
using MicriCancelli.Services;

namespace MicriCancelli
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Modalità strumento:
            //   MicriCancelli.exe --anteprima biglietto.png [QR|CODE39]   salva l'anteprima del biglietto ed esce
            //   MicriCancelli.exe --parametri                             apre solo il pannello Parametri
            if (args.Length >= 2 && args[0] == "--anteprima")
            {
                App.Inizializza();
                var codice = new Models.Codice { Numero = 865081, EmessoIl = DateTime.Now, Tipo = args.Length >= 3 ? args[2] : App.Impostazioni.TipoCodice };
                using (var bmp = App.Stampa.Anteprima(codice))
                    bmp.Save(args[1], System.Drawing.Imaging.ImageFormat.Png);
                return;
            }
            if (args.Length >= 1 && args[0] == "--parametri")
            {
                App.Inizializza();
                Application.Run(new frmParametri());
                return;
            }

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) =>
            {
                Log.Errore("Eccezione non gestita nella UI", e.Exception);
                MessageBox.Show("Si è verificato un errore: " + e.Exception.Message, "MicriCancelli", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                Log.Errore("Eccezione non gestita", e.ExceptionObject as Exception);
            TaskSchedulerOsserva();

            try
            {
                App.Inizializza();
            }
            catch (Exception ex)
            {
                Log.Errore("Avvio fallito", ex);
                MessageBox.Show("Impossibile avviare l'applicazione:\n\n" + ex.Message, "MicriCancelli", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.Run(new frmMain());
        }

        private static void TaskSchedulerOsserva()
        {
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                Log.Errore("Eccezione non osservata in un task", e.Exception);
                e.SetObserved();
            };
        }
    }
}
