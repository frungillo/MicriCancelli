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

namespace MicriCancelli
{
    public partial class frmMain : Form
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private IntPtr hookID = IntPtr.Zero;
        private LowLevelKeyboardProc keyboardProc;

        public frmMain()
        {
            InitializeComponent();
            keyboardProc = HookCallback;
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
                // Controlla se il tasto premuto è il tasto "Invio"
                if (key == Keys.Return)
                {
                    // Aggiungi un ritorno a capo alla TextBox
                    textBox1.AppendText(Environment.NewLine);
                }
                else
                {
                    // Altrimenti, aggiungi il carattere alla TextBox
                    char keyChar = (char)vkCode;
                    textBox1.AppendText(keyChar.ToString());
                }



                // char keyChar = (char)vkCode;

                // Aggiungi il carattere alla TextBox
               // textBox1.AppendText(DateTime.Now+"- Letto Codice: "+keyChar.ToString()+ "\r\n");
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
                Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                System.Net.IPAddress ipAdd = System.Net.IPAddress.Parse("10.99.5.101");
                System.Net.IPEndPoint remoteEP = new IPEndPoint(ipAdd, 80);
                soc.Connect(remoteEP);
                byte[] byData = System.Text.Encoding.ASCII.GetBytes("chiudi*");
                soc.Send(byData);
                soc.Close();
        }

    }
}
