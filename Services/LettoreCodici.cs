using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MicriCancelli.Services
{
    /// <summary>
    /// Intercetta lo scanner (emulazione tastiera) con un hook globale a basso livello.
    /// Dentro l'hook si fa solo il minimo indispensabile: accumulare caratteri e, al terminatore,
    /// mettere la sequenza in coda. La verifica, il DB e il cancello girano su un thread separato,
    /// così l'hook non supera mai il timeout di Windows e non viene rimosso in silenzio.
    /// </summary>
    public sealed class LettoreCodici : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int VK_RETURN = 0x0D;

        /// <summary>Oltre questa pausa tra due tasti il buffer si azzera: lo scanner "digita" in pochi ms, una persona no.</summary>
        private const int PausaMassimaMs = 400;

        private readonly Impostazioni _imp;
        private readonly StringBuilder _buffer = new StringBuilder();
        private readonly BlockingCollection<string> _coda = new BlockingCollection<string>();
        private readonly Stopwatch _cronometro = Stopwatch.StartNew();
        private long _ultimoTastoMs = -1;

        private IntPtr _hook = IntPtr.Zero;
        private LowLevelKeyboardProc _callback; // campo: evita che il GC raccolga il delegato passato a Windows
        private Task _worker;

        /// <summary>Sequenza numerica completa letta dallo scanner. Sollevato sul thread di lavoro, non sulla UI.</summary>
        public event Func<string, Task> CodiceLetto;

        public LettoreCodici(Impostazioni imp) { _imp = imp; }

        /// <summary>Va chiamato dal thread della UI (serve un message loop per l'hook).</summary>
        public void Avvia()
        {
            if (_hook != IntPtr.Zero) return;
            _callback = HookCallback;
            using (var modulo = Process.GetCurrentProcess().MainModule)
                _hook = SetWindowsHookEx(WH_KEYBOARD_LL, _callback, GetModuleHandle(modulo.ModuleName), 0);
            if (_hook == IntPtr.Zero)
                throw new InvalidOperationException("Impossibile installare l'hook tastiera (errore " + Marshal.GetLastWin32Error() + ")");

            _worker = Task.Run(() => CicloElaborazione());
            Log.Info("Lettore codici attivo (terminatore '" + _imp.TastoFine + "', " + _imp.LunghezzaCodice + " cifre)");
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
                    Elabora(Marshal.ReadInt32(lParam));
            }
            catch (Exception ex)
            {
                _buffer.Clear();
                Log.Errore("Errore nell'hook tastiera", ex);
            }
            return CallNextHookEx(_hook, nCode, wParam, lParam);
        }

        private void Elabora(int vk)
        {
            var adesso = _cronometro.ElapsedMilliseconds;
            if (_ultimoTastoMs >= 0 && adesso - _ultimoTastoMs > PausaMassimaMs) _buffer.Clear();
            _ultimoTastoMs = adesso;

            if (vk == VK_RETURN || vk == (int)_imp.TastoFine)
            {
                Completa();
                return;
            }

            var c = CarattereDa(vk);
            if (c == '\0') return; // shift, ctrl, frecce...: non contano

            if (_buffer.Length >= _imp.LunghezzaCodice + 4) _buffer.Clear(); // sequenza troppo lunga, non è nostra
            _buffer.Append(c);
        }

        private void Completa()
        {
            var sequenza = _buffer.ToString();
            _buffer.Clear();
            if (sequenza.Length != _imp.LunghezzaCodice) return;
            foreach (var ch in sequenza) if (!char.IsDigit(ch)) return;
            _coda.Add(sequenza);
        }

        private static char CarattereDa(int vk)
        {
            if (vk >= 0x30 && vk <= 0x39) return (char)vk;                 // 0-9 tastiera principale
            if (vk >= 0x60 && vk <= 0x69) return (char)('0' + vk - 0x60);  // 0-9 tastierino numerico
            if (vk >= 0x41 && vk <= 0x5A) return (char)vk;                 // A-Z
            return '\0';
        }

        private async Task CicloElaborazione()
        {
            foreach (var sequenza in _coda.GetConsumingEnumerable())
            {
                var gestore = CodiceLetto;
                if (gestore == null) continue;
                try { await gestore(sequenza).ConfigureAwait(false); }
                catch (Exception ex) { Log.Errore("Errore elaborando il codice " + sequenza, ex); }
            }
        }

        public void Dispose()
        {
            if (_hook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hook);
                _hook = IntPtr.Zero;
            }
            _coda.CompleteAdding();
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
