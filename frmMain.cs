using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using MicriCancelli.Classi;
using System.Data.SQLite;
using static log4net.Appender.RollingFileAppender;
using System.Threading;
using CrystalDecisions.CrystalReports.Engine;
using System.Reflection;
using System.Runtime.CompilerServices;



namespace MicriCancelli
{
    
    public partial class frmMain : Form
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        private IntPtr hookID = IntPtr.Zero;
        private LowLevelKeyboardProc keyboardProc;

        private StringBuilder inputBuffer = new StringBuilder();

        // alcuni parametri

        private  int code_len = 6;
        private  Keys keyEnd = Keys.E;
        private int minutiValidita = 30;
        private string ipArduino = "10.99.5.101";
        private int portaArduino = 80;
        private int inching = 1000;
        private int secondiRefresh = 5;
        private int countDown;

        // Inizializza un nuovo gestore SQLiteManager
        string dbFilePath = MicriCancelli.Properties.Settings.Default.dbpathFile;

        // parte dedicata all'autorecovery
        public delegate int RecoveryDelegate(IntPtr parameter);

        [DllImport("kernel32.dll")]
        private static extern int RegisterApplicationRecoveryCallback(
                 RecoveryDelegate recoveryCallback,
                 IntPtr parameter,
                 uint pingInterval,
                 uint flags);

        [DllImport("kernel32.dll")]
        private static extern void ApplicationRecoveryFinished(bool success);
        
        BindingSource bs_co = new BindingSource();
        
        private static void RegisterForRecovery()
         {
             var callback = new RecoveryDelegate(p =>
             {
                 Process.Start(Assembly.GetEntryAssembly().Location);
                 ApplicationRecoveryFinished(true);
                 return 0;
             });

             var interval = 100U;
             var flags = 0U;

             RegisterApplicationRecoveryCallback(callback, IntPtr.Zero, interval, flags);
         }
        
        // FINE PARTE AUTORECOVERY
        public frmMain()
        {
            RegisterForRecovery();
            this.Load += FrmMain_Load;
            InitializeComponent();
            keyboardProc = HookCallback;
            txtLogLettore.ScrollBars=ScrollBars.Vertical;
            caricaParametri();
            this.FormClosing += (s, e) => {
                var result = MessageBox.Show("Confermi Chiusura?", "Chiusura?", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    // Do some work such as closing connection with sqlite3 DB
                    Application.Exit();
                }
            };
            this.TopMost = true;
            
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            caricaCodici();
            timerRefresh.Interval= secondiRefresh * 1000;
            lblRefresh.Text = "AutoRefresh ogni " + secondiRefresh.ToString() + " secondi";
            timerRefresh.Start();
            timerCountDown.Start();
            countDown = secondiRefresh;
            lblRefresh.Text = "Prossimo refresh tra " + countDown.ToString() + " sec.";
        }

        private void caricaParametri() 
        {
            Parametri par=new Parametri();
            par = Parametri.GetParametro("code_len");
            code_len=Convert.ToInt32(par.Value);
            par = Parametri.GetParametro("KeyEnd");
            keyEnd= (Keys)Enum.Parse(typeof(Keys), par.Value, true);
            par = Parametri.GetParametro("minutiValidita");
            minutiValidita=Convert.ToInt32(par.Value);
            par = Parametri.GetParametro("ipArduino");
            ipArduino=par.Value.ToString();
            par = Parametri.GetParametro("portaArduino");
            portaArduino=Convert.ToInt32(par.Value);
            par = Parametri.GetParametro("inching");
            inching=Convert.ToInt32(par.Value);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            hookID = SetHook(keyboardProc);
            
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            UnhookWindowsHookEx(hookID);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (ProcessModule curModule = Process.GetCurrentProcess().MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                // Converti il codice del tasto in un valore Keys
                Keys key = (Keys)vkCode;

                // Controlla se il tasto premuto è il tasto "Invio", esco
                
                if (key == Keys.Return || inputBuffer.Length > code_len + 1)
                {
                    inputBuffer.Clear();
                    return CallNextHookEx(hookID, nCode, wParam, lParam);
                }

                // Controlla se il tasto premuto è il tasto "E" ho finito la lettura
                // il barcode con codifica COD39 è fatto da * SEI cifre + "E" + *
                // ad esempio *123456E*  viene letto come codice 123456   
                if (key == Keys.F12 || key == Keys.P) 
                {
                    GeneraTicket();
                    return CallNextHookEx(hookID, nCode, wParam, lParam);
                } 
               // txtLogLettore.AppendText(" - inputbuffer: " +inputBuffer.ToString() + Environment.NewLine);

                if (key == keyEnd && inputBuffer.ToString().Length != code_len + 1) 
                {
                    inputBuffer.Clear();
                    return CallNextHookEx(hookID, nCode, wParam, lParam);
                }

                if (key == keyEnd && inputBuffer.ToString().Length == code_len + 1)
                {
                     // Aggiungi la stringa bufferizzata alla TextBox
                    string buffer = inputBuffer.ToString();
                    txtLogLettore.AppendText(" - buffer: " + buffer + Environment.NewLine);

                    txtLogLettore.AppendText(DateTime.Now + " - Letto Codice: " + buffer.Substring(0, code_len) + Environment.NewLine);
                    inputBuffer.Clear();
                    // a questo punto ho un POTENZIALE codice a barre
                    // verifico se esiste in tabella Codici
                    try
                    {
                        string codice = buffer.Substring(0, code_len);
                        if(Codici.isCodiceEsistente(Convert.ToInt32(codice)))
                        {
                            if (Codici.isCodiceValido(Convert.ToInt32(codice), minutiValidita))
                            {

                                // AGGIORNO LA TABELLA CODICI
                                Codici cod = new Codici();
                                string dbFilePath = MicriCancelli.Properties.Settings.Default.dbpathFile;
                                SQLiteManager manager = new SQLiteManager(dbFilePath);
                                cod = manager.ReadCodice(Convert.ToInt32(codice));
                                cod.Data_uso = DateTime.Now.ToString("dd/MM/yyyy");
                                cod.Ora_uso = DateTime.Now.ToString("HH:mm");
                                manager.UpdateCodice(cod.Id_codice, cod.Codice, cod.Data_emissione, cod.Ora_emissione, cod.Data_uso, cod.Ora_uso);
                                txtLogLettore.AppendText(DateTime.Now + " - Aperta barriera per: " + codice + Environment.NewLine);
                                inputBuffer.Clear();

                                // ALZO LA BARRIERA

                                Commons.ApreChiude(inching);

                                return CallNextHookEx(hookID, nCode, wParam, lParam);
                            }
                            else // codice esistente ma non utilizzabile
                            {
                                txtLogLettore.AppendText(DateTime.Now + " - Passaggio negato per: " + codice + Environment.NewLine);
                                inputBuffer.Clear();
                                return CallNextHookEx(hookID, nCode, wParam, lParam);
                            }
                        }
                        // è arrivato qualcosa che assomiglia ad un codice ma non esiste in tabella
                        else 
                        {
                            txtLogLettore.AppendText(DateTime.Now + " - Codice non in tabella: " + codice + Environment.NewLine);
                            inputBuffer.Clear();

                        }
                    }
                    catch { inputBuffer.Clear(); }

                    // Pulisci il buffer
                    inputBuffer.Clear();
                    return CallNextHookEx(hookID, nCode, wParam, lParam);
                }
                else
                {
                    // Altrimenti, aggiungi il carattere al buffer
                    char keyChar = (char)vkCode;
                    if (inputBuffer.Length < 7) inputBuffer.Append(keyChar);
                    else inputBuffer.Clear();
                }
            }
            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }

        // Importazioni delle funzioni di Windows per l'utilizzo dei hook globali
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        // Delegato per la procedura di callback della tastiera
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);


        private async void btnApri_Click(object sender, EventArgs e)
        {
            Parametri par=new Parametri();
            par = Parametri.GetParametro("inching");
            await Commons.ApreChiude(Convert.ToInt32(par.Value));
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtLogLettore.Text = "";
            inputBuffer.Clear();
        }
        private void GeneraTicket() {
            // genera e stampa un nuovo biglietto. Come string che poi memoorizzo come intero che non inizia con 0
            string codice;
            do codice = Commons.RandomDigits(6);
            while (Codici.isCodiceEsistente(Convert.ToInt32(codice)) || codice.Substring(0, 1) == "0");

            // ho un nuovo codice, inserisco in tabella
            SQLiteManager manager = new SQLiteManager(dbFilePath);
            // Inserisci un nuovo codice con data e ora di uso specificate
            string ora = DateTime.Now.ToString("HH:mm");
            string data = DateTime.Now.ToString("dd/MM/yyyy");

            manager.InsertCodice(Convert.ToInt32(codice), data, ora, "", "");

            txtLogLettore.AppendText(DateTime.Now + " - Generato Codice: " + codice + Environment.NewLine);

            ReportDocument rep = new ReportDocument();
            rep.Load(Application.StartupPath + "\\biglietto.rpt");
            rep.SetParameterValue(0, "*" + codice + keyEnd + "*");
            rep.SetParameterValue(1, codice);
            rep.PrintToPrinter(1, false, 1, 1);
        }
        private void btnGeneraTicket_Click(object sender, EventArgs e)
        {
           GeneraTicket();

        }
        private void btnSettings_Click(object sender, EventArgs e)
        {
            frmParametri frm = new frmParametri();
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog(this);
            caricaParametri();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnTabella_Click(object sender, EventArgs e)
        {
            frmTabella frm = new frmTabella();
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog(this);
        }
        private void caricaCodici()
        {

            DataTable dt = new DataTable();
            List<Codici> list = new List<Codici>();
            list = Codici.getAll(" 1=1 order by data_emissione desc, ora_emissione desc");
            dt = Commons.ToDataTable(list);
            bs_co.DataSource = dt;
            grigliaCodici.DataSource = bs_co;

            grigliaCodici.Columns[1].Width = 70;
            grigliaCodici.Columns[2].Width = 95;
            grigliaCodici.Columns[3].Width = 70;
            grigliaCodici.Columns[4].Width = 95;
            grigliaCodici.Columns[5].Width = 70;

            grigliaCodici.Columns[1].HeaderText = "CODICE";
            grigliaCodici.Columns[2].HeaderText = "DATA EMISS.";
            grigliaCodici.Columns[3].HeaderText = "ORA EMISS.";
            grigliaCodici.Columns[4].HeaderText = "DATA USO";
            grigliaCodici.Columns[5].HeaderText = "ORA USO";
            grigliaCodici.Columns[0].Visible = false;

            grigliaCodici.ClearSelection();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            caricaCodici();
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            caricaCodici();
        }
        private void timerCountDown_Tick(object sender, EventArgs e)
        {
            countDown--;
            lblRefresh.Text = "Prossimo refresh tra " + countDown.ToString() + " sec.";
            if (countDown == 0) countDown = secondiRefresh;
        }
    }
}
