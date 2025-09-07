using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows; // WPF
using WinForms = System.Windows.Forms; // Alias så vi kan bruge NotifyIcon

namespace FlowerGUIListener
{
    public partial class App : Application
    {
        private WinForms.NotifyIcon trayIcon;
        private static IntPtr _keyboardHookID = IntPtr.Zero;
        private static IntPtr _mouseHookID = IntPtr.Zero;

        private static LowLevelKeyboardProc _keyboardProc;
        private static LowLevelMouseProc _mouseProc;

        private static bool ctrlPressed = false;

        // Delegates til hooks
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        // Konstanter
        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE_LL = 14;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_RBUTTONDOWN = 0x0204;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Skjul hovedvinduet, så kun tray-ikonet vises
            if (Current.MainWindow != null)
                Current.MainWindow.Hide();

            // Opret tray-ikon
            trayIcon = new WinForms.NotifyIcon();
            trayIcon.Icon = System.Drawing.SystemIcons.Application;
            trayIcon.Visible = true;
            trayIcon.Text = "Blomster-GUI Listener";

            var menu = new WinForms.ContextMenuStrip();
            menu.Items.Add("Exit", null, OnExit);
            menu.Items.Add("Exit2", null, OnExit);
            trayIcon.ContextMenuStrip = menu;

            // Opsæt global hooks
            _keyboardProc = KeyboardProc;
            _keyboardHookID = SetHook(_keyboardProc, WH_KEYBOARD_LL);
            _mouseProc = MouseProc;
            _mouseHookID = SetHook(_mouseProc, WH_MOUSE_LL);
        }

        private void OnExit(object sender, EventArgs e)
        {
            UnhookWindowsHookEx(_keyboardHookID);
            UnhookWindowsHookEx(_mouseHookID);
            trayIcon.Visible = false;
            Shutdown();
        }

        private static IntPtr KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (wParam == (IntPtr)WM_KEYDOWN && vkCode == (int)WinForms.Keys.ControlKey)
                    ctrlPressed = true;
                else if (wParam == (IntPtr)WM_KEYUP && vkCode == (int)WinForms.Keys.ControlKey)
                    ctrlPressed = false;
            }
            return CallNextHookEx(_keyboardHookID, nCode, wParam, lParam);
        }

        private static IntPtr MouseProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_RBUTTONDOWN)
            {
                if (ctrlPressed)
                {
                    // 🚀 Aktiver GUI – lige nu bare en placeholder
                    MessageBox.Show("Blomster-GUI aktiveret!");
                }
            }
            return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
        }

        private static IntPtr SetHook(Delegate proc, int hookType)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                IntPtr procAddress = Marshal.GetFunctionPointerForDelegate(proc);
                return SetWindowsHookEx(hookType, procAddress,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        // WinAPI imports
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, IntPtr lpfn,
            IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
