using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FlowerGUIListener.Services
{
    public class GlobalHookManager : IDisposable
    {
        // Delegates til hooks
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        // Hook instances
        private IntPtr _keyboardHookID = IntPtr.Zero;
        private IntPtr _mouseHookID = IntPtr.Zero;
        private LowLevelKeyboardProc _keyboardProc;
        private LowLevelMouseProc _mouseProc;
        
        // Keep GC handles to prevent delegate collection
        private GCHandle _keyboardProcHandle;
        private GCHandle _mouseProcHandle;

        // State tracking
        private bool _ctrlPressed = false;
        private bool _disposed = false;

        // Constants
        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE_LL = 14;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_RBUTTONDOWN = 0x0204;

        // Events
        public event EventHandler<HotkeyEventArgs> HotkeyActivated;

        public bool IsInstalled { get; private set; }
        
        public static bool IsRunningElevated()
        {
            try
            {
                using var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
                var principal = new System.Security.Principal.WindowsPrincipal(identity);
                return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        public GlobalHookManager()
        {
            _keyboardProc = KeyboardProc;
            _mouseProc = MouseProc;
            
            // Pin delegates to prevent garbage collection
            _keyboardProcHandle = GCHandle.Alloc(_keyboardProc);
            _mouseProcHandle = GCHandle.Alloc(_mouseProc);
        }

        public bool InstallHooks()
        {
            try
            {
                if (IsInstalled)
                    return true;

                System.Diagnostics.Debug.WriteLine("Installing hooks...");
                
                _keyboardHookID = SetHook(_keyboardProc, WH_KEYBOARD_LL);
                System.Diagnostics.Debug.WriteLine($"Keyboard hook ID: {_keyboardHookID}");
                
                _mouseHookID = SetHook(_mouseProc, WH_MOUSE_LL);
                System.Diagnostics.Debug.WriteLine($"Mouse hook ID: {_mouseHookID}");

                IsInstalled = _keyboardHookID != IntPtr.Zero && _mouseHookID != IntPtr.Zero;
                System.Diagnostics.Debug.WriteLine($"Hooks installed: {IsInstalled}");
                
                if (!IsInstalled)
                {
                    var lastError = Marshal.GetLastWin32Error();
                    System.Diagnostics.Debug.WriteLine($"Hook installation failed. Last Win32 error: {lastError}");
                }
                
                return IsInstalled;
            }
            catch (Exception ex)
            {
                // Log exception (could be injected logger here)
                System.Diagnostics.Debug.WriteLine($"Failed to install hooks: {ex.Message}");
                return false;
            }
        }

        public void UninstallHooks()
        {
            if (_keyboardHookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_keyboardHookID);
                _keyboardHookID = IntPtr.Zero;
            }

            if (_mouseHookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_mouseHookID);
                _mouseHookID = IntPtr.Zero;
            }

            IsInstalled = false;
        }

        private IntPtr KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                System.Diagnostics.Debug.WriteLine($"Key event: vkCode={vkCode}, wParam={wParam}, isDown={wParam == (IntPtr)WM_KEYDOWN}");
                
                // Check for both left and right control keys
                if (wParam == (IntPtr)WM_KEYDOWN && (vkCode == (int)Keys.LControlKey || vkCode == (int)Keys.RControlKey || vkCode == (int)Keys.ControlKey))
                {
                    _ctrlPressed = true;
                    System.Diagnostics.Debug.WriteLine("Control key pressed");
                }
                else if (wParam == (IntPtr)WM_KEYUP && (vkCode == (int)Keys.LControlKey || vkCode == (int)Keys.RControlKey || vkCode == (int)Keys.ControlKey))
                {
                    _ctrlPressed = false;
                    System.Diagnostics.Debug.WriteLine("Control key released");
                }
            }
            return CallNextHookEx(_keyboardHookID, nCode, wParam, lParam);
        }

        private IntPtr MouseProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                System.Diagnostics.Debug.WriteLine($"Mouse event: wParam={wParam}, nCode={nCode}, ctrlPressed={_ctrlPressed}");
                
                if (wParam == (IntPtr)WM_RBUTTONDOWN)
                {
                    System.Diagnostics.Debug.WriteLine("Right mouse button clicked");
                    
                    if (_ctrlPressed)
                    {
                        System.Diagnostics.Debug.WriteLine("Hotkey activated: Ctrl + Right Click");
                        // Get cursor position
                        var cursorPos = GetCursorPosition();
                        HotkeyActivated?.Invoke(this, new HotkeyEventArgs(cursorPos.X, cursorPos.Y));
                    }
                }
            }
            return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
        }

        private IntPtr SetHook(Delegate proc, int hookType)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                IntPtr procAddress = Marshal.GetFunctionPointerForDelegate(proc);
                return SetWindowsHookEx(hookType, procAddress,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private (int X, int Y) GetCursorPosition()
        {
            GetCursorPos(out var point);
            return (point.X, point.Y);
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

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                UninstallHooks();
                
                // Free GC handles
                if (_keyboardProcHandle.IsAllocated)
                    _keyboardProcHandle.Free();
                if (_mouseProcHandle.IsAllocated)
                    _mouseProcHandle.Free();
                    
                _disposed = true;
            }
        }
    }

    public class HotkeyEventArgs : EventArgs
    {
        public int X { get; }
        public int Y { get; }

        public HotkeyEventArgs(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
