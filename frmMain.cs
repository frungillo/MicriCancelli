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

        private const int CODE_LEN = 6;
        private const Keys keyEnd = Keys.E;

        // Inizializza un nuovo gestore SQLiteManager
        // string dbFilePath = @"D:\Sviluppo\MicriCancelli\DataBase\cancelli.db"; // Imposta il percorso del file del database SQLite
        string dbFilePath = MicriCancelli.Properties.Settings.Default.dbpathFile;
        //D:/Sviluppo/MicriCancelli/DataBase/cancelli.

        public frmMain()
        {
            InitializeComponent();
            keyboardProc = HookCallback;
            txtLogLettore.ScrollBars=ScrollBars.Vertical;
            
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
                if (key == Keys.Return) return CallNextHookEx(hookID, nCode, wParam, lParam);
               
                // Controlla se il tasto premuto è il tasto "E" ho finito la lettura
                // il barcode con codifica COD39 è fatto da * SEI cifre + "E" + *
                // ad esempio *123456E*  viene letto come codice 123456

                if (key == keyEnd)
                {
                    // Aggiungi la stringa bufferizzata alla TextBox
                    string codice = inputBuffer.ToString().Substring(0, CODE_LEN);
                    txtLogLettore.AppendText(DateTime.Now + " - Letto Codice: " + codice + Environment.NewLine);

                    // a questo punto ho un POTENZIALE codice a barre
                    // verifico se esiste in tabella Codici

                    if (Codici.isCodiceEsistente(Convert.ToInt32(codice)))
                    {
                        if (Codici.isCodiceValido(Convert.ToInt32(codice)))
                        {
                            // ALZO LA BARRIERA
                            // AGGIORNO LA TABELLA CODICI
                            Codici cod = new Codici();
                            string dbFilePath = MicriCancelli.Properties.Settings.Default.dbpathFile;
                            SQLiteManager manager = new SQLiteManager(dbFilePath);
                            cod = manager.ReadCodice(Convert.ToInt32(codice));
                            cod.Data_uso = DateTime.Now.ToString("dd/MM/yyyy");
                            cod.Ora_uso = DateTime.Now.ToString("HH:mm");
                            manager.UpdateCodice(cod.Id_codice, cod.Codice, cod.Data_emissione, cod.Ora_emissione, cod.Data_uso, cod.Ora_uso);
                            txtLogLettore.AppendText(DateTime.Now + " - Aperta barriera per: " + codice + Environment.NewLine);
                        }
                        else // codice esistente ma non utilizzabile
                        {
                            txtLogLettore.AppendText(DateTime.Now + " - Passaggio negato per: " + codice + Environment.NewLine);
                        }
                    }
                    else // è arrivato qualcosa che assomiglia ad un codice ma non esiste in tabella
                    {
                        txtLogLettore.Text = "";
                    }
                    // Pulisci il buffer
                    inputBuffer.Clear();
                }
                else
                {
                    // Altrimenti, aggiungi il carattere al buffer
                    char keyChar = (char)vkCode;
                    inputBuffer.Append(keyChar);
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


        private void btnApri_Click(object sender, EventArgs e)
        {
            // COMANDA L'APERTURA MANUALE 
                Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                System.Net.IPAddress ipAdd = System.Net.IPAddress.Parse("10.99.5.101");
                System.Net.IPEndPoint remoteEP = new IPEndPoint(ipAdd, 80);
                soc.Connect(remoteEP);
                byte[] byData = System.Text.Encoding.ASCII.GetBytes("apri*");
                soc.Send(byData);
                soc.Close();
        }

        private void btnChiudi_Click(object sender, EventArgs e)
        {
            // COMANDA LA CHIUSURA MANUALE 
            Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                System.Net.IPAddress ipAdd = System.Net.IPAddress.Parse("10.99.5.101");
                System.Net.IPEndPoint remoteEP = new IPEndPoint(ipAdd, 80);
                soc.Connect(remoteEP);
                byte[] byData = System.Text.Encoding.ASCII.GetBytes("chiudi*");
                soc.Send(byData);
                soc.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtLogLettore.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            MessageBox.Show(" Valido?" + Codici.isCodiceValido(654321).ToString());
        }

        private void btnGeneraTicket_Click(object sender, EventArgs e)
        {
            // genera e stampa un nuovo biglietto. Come string che poi memoorizzo come intero che non inizia con 0
            string codice;
            do codice = Commons.RandomDigits(6);
            while ( Codici.isCodiceEsistente(Convert.ToInt32(codice)) || codice.Substring(0, 1) == "0");

            // ho un nuovo codice, inserisco in tabella
            SQLiteManager manager = new SQLiteManager(dbFilePath);
            // Inserisci un nuovo codice con data e ora di uso specificate
            string ora = DateTime.Now.ToString("HH:mm"); 
            string data = DateTime.Now.ToString("dd/MM/yyyy");
            
            manager.InsertCodice(Convert.ToInt32(codice), data, ora, "", "");
        }
    }
}
