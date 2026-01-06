using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization; // Add this using statement
using System.Windows;
using FlowerGUIListener.Models;
using FlowerGUIListener.Services;
using FlowerGUIListener.Windows;

namespace FlowerGUIListener
{
    public partial class App : Application
    {
        private GlobalHookManager _hookManager;
        private TrayIconManager _trayManager;
        private FlowerGUIWindow _flowerWindow;
        private Settings _settings;
        private List<PetalAction> _petalActions;
        private bool _disposed = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Prevent app from shutting down when no windows are open
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            try
            {
                InitializeServices();

				// Prevent app from shutting down when no windows are open
				ShutdownMode = ShutdownMode.OnExplicitShutdown;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fejl ved opstart: {ex.Message}\n\nApplikationen vil lukke.", 
                    "FlowerGUI Fejl", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        private void InitializeServices()
        {
            // Load settings
            _settings = Settings.Load();

            // Load petal actions
            LoadPetalActions();
            
            // Initialize FlowerGUI window
            _flowerWindow = new FlowerGUIWindow(_settings, _petalActions);
            
            // Initialize and setup tray icon manager
            _trayManager = new TrayIconManager();
            _trayManager.Initialize("FlowerGUI Listener - Aktiv");
            _trayManager.ShowMainWindow += OnShowMainWindow;
            _trayManager.ExitApplication += OnExitApplication;
            _trayManager.Show();
            
            // Initialize and setup global hook manager
            _hookManager = new GlobalHookManager();
            _hookManager.HotkeyActivated += OnHotkeyActivated;
            
            bool isElevated = GlobalHookManager.IsRunningElevated();
            System.Diagnostics.Debug.WriteLine($"Running elevated: {isElevated}");
            
            if (!_hookManager.InstallHooks())
            {
                string message = "Kunne ikke installere globale genveje. Funktionalitet kan være begrænset.";
                if (!isElevated)
                {
                    message += "\n\nPrøv at køre som administrator for bedre kompatibilitet.";
                }
                
                _trayManager.ShowBalloonTip("FlowerGUI Advarsel", message, 
                    System.Windows.Forms.ToolTipIcon.Warning);
            }
            else
            {
                _trayManager.ShowBalloonTip("FlowerGUI Startet", 
                    "FlowerGUI lytter nu efter Ctrl + højreklik.", 
                    System.Windows.Forms.ToolTipIcon.Info);
            }
        }
        
        private void LoadPetalActions()
        {
            try
            {
                string filePath = "actions.json";
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new JsonStringEnumConverter() } // Add this converter
                    };
                    _petalActions = JsonSerializer.Deserialize<List<PetalAction>>(json, options);
                }
                else
                {
                    _petalActions = new List<PetalAction>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading actions.json: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _petalActions = new List<PetalAction>();
            }
        }

        private void OnHotkeyActivated(object sender, HotkeyEventArgs e)
        {
            try
            {
                // Show the FlowerGUI window at cursor position
                Dispatcher.BeginInvoke(() => 
                {
                    _flowerWindow.ShowAt(e.X, e.Y);
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing FlowerGUI: {ex.Message}");
            }
        }

        private void OnShowMainWindow(object sender, EventArgs e)
        {
            try
            {
                // Get current cursor position and show window there
                var cursorPos = System.Windows.Forms.Cursor.Position;
                _flowerWindow.ShowAt(cursorPos.X, cursorPos.Y);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing main window: {ex.Message}");
            }
        }

        private void OnExitApplication(object sender, EventArgs e)
        {
            Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            CleanupResources();
            base.OnExit(e);
        }

        private void CleanupResources()
        {
            if (_disposed)
                return;
                
            try
            {
                _hookManager?.Dispose();
                _trayManager?.Dispose();
                _flowerWindow?.Close();
                _disposed = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during cleanup: {ex.Message}");
            }
        }
    }
}
