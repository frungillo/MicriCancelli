using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using MicriCancelli.Services;

namespace MicriCancelli
{
    public partial class frmMain : Form
    {
        private LettoreCodici _lettore;
        private int _countDown;
        private const int RigheMassimeLog = 400;

        public frmMain()
        {
            RegisterForRecovery();
            InitializeComponent();
            txtLogLettore.ScrollBars = ScrollBars.Vertical;
            this.Text += "  v" + Application.ProductVersion;
            this.TopMost = true;

            Log.Scritto += SuLogScritto;
            this.Load += FrmMain_Load;
            this.FormClosing += FrmMain_FormClosing;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            try
            {
                _lettore = new LettoreCodici(App.Impostazioni);
                _lettore.CodiceLetto += SuCodiceLetto;
                _lettore.TastoStampa += () => EseguiSuUi(StampaNuovoBiglietto);
                _lettore.Avvia();
            }
            catch (Exception ex)
            {
                Log.Errore("Lettore codici non avviato", ex);
                MessageBox.Show("Il lettore codici non è attivo:\n" + ex.Message, "MicriCancelli", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            App.Remoti.Avvia(); // comandi di apertura dalla piattaforma IA (se configurata)

            caricaCodici();
            impostaTimer();
            timerRefresh.Start();
            timerCountDown.Start();
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Confermi la chiusura?", "Chiusura", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }
            Log.Scritto -= SuLogScritto;
            _lettore?.Dispose();
            App.Remoti?.Dispose();
            Log.Info("Applicazione chiusa dall'operatore");
        }

        // ---- eventi dai servizi (thread diversi dalla UI) ----

        private async Task SuCodiceLetto(string codice)
        {
            var esito = await App.Accessi.ProcessaAsync(codice);
            if (esito == EsitoAccesso.Consentito || esito == EsitoAccesso.GiaUsato || esito == EsitoAccesso.CancelloOccupato)
                EseguiSuUi(caricaCodici);
        }

        private void SuLogScritto(string riga)
        {
            EseguiSuUi(() =>
            {
                if (txtLogLettore.Lines.Length > RigheMassimeLog) txtLogLettore.Clear();
                txtLogLettore.AppendText(riga + Environment.NewLine);
            });
        }

        private void EseguiSuUi(Action azione)
        {
            if (IsDisposed || !IsHandleCreated) return;
            try
            {
                if (InvokeRequired) BeginInvoke(azione);
                else azione();
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }

        // ---- pulsanti ----

        private async void btnApri_Click(object sender, EventArgs e)
        {
            btnApri.Enabled = false;
            try
            {
                var ok = await App.Cancello.ApriChiudiAsync("pulsante manuale");
                if (!ok) MessageBox.Show("Il cancello non ha risposto: controlla il log e i parametri Arduino.", "Apertura manuale", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                btnApri.Enabled = true;
            }
        }

        private void btnGeneraTicket_Click(object sender, EventArgs e) => StampaNuovoBiglietto();

        /// <summary>Genera e stampa un biglietto: dal pulsante o dal tasto Invio (tastiera o tastierino).</summary>
        private void StampaNuovoBiglietto()
        {
            if (!btnGeneraTicket.Enabled) return; // stampa gia' in corso
            btnGeneraTicket.Enabled = false;
            try
            {
                var codice = App.Accessi.GeneraNuovo();
                App.Stampa.Stampa(codice);
                caricaCodici();
            }
            catch (Exception ex)
            {
                Log.Errore("Stampa biglietto fallita", ex);
                MessageBox.Show("Stampa non riuscita:\n" + ex.Message, "Biglietto", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnGeneraTicket.Enabled = true;
            }
        }

        private void btnClear_Click(object sender, EventArgs e) => txtLogLettore.Clear();

        private void btnSettings_Click(object sender, EventArgs e)
        {
            if (_lettore != null) _lettore.StampaConInvio = false; // l'Invio nel pannello non deve stampare
            using (var frm = new frmParametri())
            {
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowDialog(this);
            }
            if (_lettore != null) _lettore.StampaConInvio = true;
            App.RicaricaImpostazioni();
            impostaTimer();
            caricaCodici();
        }

        private void btnClose_Click(object sender, EventArgs e) => this.Close();

        private void btnTabella_Click(object sender, EventArgs e)
        {
            if (_lettore != null) _lettore.StampaConInvio = false;
            using (var frm = new frmTabella())
            {
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowDialog(this);
            }
            if (_lettore != null) _lettore.StampaConInvio = true;
        }

        private void btnRefresh_Click(object sender, EventArgs e) => caricaCodici();

        // ---- griglia e timer ----

        private void caricaCodici()
        {
            try
            {
                grigliaCodici.DataSource = TabellaCodici.Costruisci(App.Codici.Tutti(200), App.Impostazioni);
                TabellaCodici.ApplicaLayout(grigliaCodici);
            }
            catch (Exception ex)
            {
                Log.Errore("Aggiornamento tabella fallito", ex);
            }
        }

        private void impostaTimer()
        {
            timerRefresh.Interval = App.Impostazioni.SecondiRefresh * 1000;
            _countDown = App.Impostazioni.SecondiRefresh;
            lblRefresh.Text = "Prossimo refresh tra " + _countDown + " sec.";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            caricaCodici();
            _countDown = App.Impostazioni.SecondiRefresh;
        }

        private void timerCountDown_Tick(object sender, EventArgs e)
        {
            lblRefresh.Text = "Prossimo refresh tra " + _countDown + " sec.";
            if (_countDown > 0) _countDown--;
        }

        // ---- riavvio automatico in caso di crash (Windows Application Recovery) ----

        public delegate int RecoveryDelegate(IntPtr parameter);

        [DllImport("kernel32.dll")]
        private static extern int RegisterApplicationRecoveryCallback(RecoveryDelegate recoveryCallback, IntPtr parameter, uint pingInterval, uint flags);

        [DllImport("kernel32.dll")]
        private static extern void ApplicationRecoveryFinished(bool success);

        private static RecoveryDelegate _recoveryCallback; // tenuto vivo per il GC

        private static void RegisterForRecovery()
        {
            _recoveryCallback = p =>
            {
                Process.Start(Assembly.GetEntryAssembly().Location);
                ApplicationRecoveryFinished(true);
                return 0;
            };
            try { RegisterApplicationRecoveryCallback(_recoveryCallback, IntPtr.Zero, 100U, 0U); }
            catch (Exception ex) { Log.Avviso("Registrazione autorecovery non riuscita: " + ex.Message); }
        }
    }
}
