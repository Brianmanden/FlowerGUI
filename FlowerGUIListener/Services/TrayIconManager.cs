using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace FlowerGUIListener.Services
{
    public class TrayIconManager : IDisposable
    {
        private NotifyIcon _trayIcon;
        private bool _disposed = false;

        public event EventHandler ShowMainWindow;
        public event EventHandler ExitApplication;

        public bool IsVisible => _trayIcon?.Visible ?? false;

        public void Initialize(string tooltip = "FlowerGUI Listener")
        {
            _trayIcon = new NotifyIcon
            {
                Icon = GetApplicationIcon(),
                Visible = false,
                Text = tooltip
            };

            _trayIcon.DoubleClick += OnTrayIconDoubleClick;
            CreateContextMenu();
        }

        private Icon GetApplicationIcon()
        {
            // Try to get the application icon, fallback to system icon
            try
            {
                var iconResource = Assembly.GetExecutingAssembly().GetManifestResourceNames()
                    .FirstOrDefault(x => x.EndsWith(".ico"));
                
                if (iconResource != null)
                {
                    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(iconResource))
                    {
                        return new Icon(stream);
                    }
                }
            }
            catch
            {
                // Fallback to system icon if custom icon fails
            }

            return SystemIcons.Application;
        }

        private void CreateContextMenu()
        {
            var contextMenu = new ContextMenuStrip();
            
            // Show main window option
            var showItem = new ToolStripMenuItem("Vis FlowerGUI", null, OnShowMainWindow);
            contextMenu.Items.Add(showItem);
            
            contextMenu.Items.Add(new ToolStripSeparator());
            
            // Status item
            var statusItem = new ToolStripMenuItem("Status: Aktiv")
            {
                Enabled = false
            };
            contextMenu.Items.Add(statusItem);
            
            contextMenu.Items.Add(new ToolStripSeparator());
            
            // Settings option
            var settingsItem = new ToolStripMenuItem("Indstillinger", null, OnSettings);
            contextMenu.Items.Add(settingsItem);
            
            // About option
            var aboutItem = new ToolStripMenuItem("Om FlowerGUI", null, OnAbout);
            contextMenu.Items.Add(aboutItem);
            
            contextMenu.Items.Add(new ToolStripSeparator());
            
            // Exit option
            var exitItem = new ToolStripMenuItem("Afslut", null, OnExit);
            contextMenu.Items.Add(exitItem);

            _trayIcon.ContextMenuStrip = contextMenu;
        }

        public void Show()
        {
            if (_trayIcon != null)
                _trayIcon.Visible = true;
        }

        public void Hide()
        {
            if (_trayIcon != null)
                _trayIcon.Visible = false;
        }

        public void ShowBalloonTip(string title, string text, ToolTipIcon icon = ToolTipIcon.Info, int timeout = 3000)
        {
            _trayIcon?.ShowBalloonTip(timeout, title, text, icon);
        }

        public void UpdateTooltip(string tooltip)
        {
            if (_trayIcon != null)
                _trayIcon.Text = tooltip;
        }

        private void OnTrayIconDoubleClick(object sender, EventArgs e)
        {
            ShowMainWindow?.Invoke(this, EventArgs.Empty);
        }

        private void OnShowMainWindow(object sender, EventArgs e)
        {
            ShowMainWindow?.Invoke(this, EventArgs.Empty);
        }

        private void OnSettings(object sender, EventArgs e)
        {
            // TODO: Implement settings dialog
            MessageBox.Show("Indstillinger kommer snart!", "FlowerGUI", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnAbout(object sender, EventArgs e)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            MessageBox.Show($"FlowerGUI Listener v{version}\\n\\nGlobal hotkey listener til FlowerGUI.\\nTryk Ctrl + højreklik for at aktivere.", 
                "Om FlowerGUI", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnExit(object sender, EventArgs e)
        {
            ExitApplication?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _trayIcon?.Dispose();
                _disposed = true;
            }
        }
    }
}
