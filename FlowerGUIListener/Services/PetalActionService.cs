using FlowerGUIListener.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using FlowerGUIListener.Models;

namespace FlowerGUIListener.Services
{
    public class PetalActionService
    {
        private readonly FlowerGUIWindow _flowerGuiWindow;
        private readonly Settings _settings;
        private readonly List<PetalAction> _petalActions;

        public PetalActionService(FlowerGUIWindow flowerGuiWindow, Settings settings, List<PetalAction> petalActions)
        {
            _flowerGuiWindow = flowerGuiWindow;
            _settings = settings;
            _petalActions = petalActions;
        }

        public void Execute(string actionId)
        {
            var action = _petalActions.FirstOrDefault(a => a.Id == actionId);
            if (action != null)
            {
                try
                {
                    var processStartInfo = new ProcessStartInfo
                    {
                        FileName = Environment.ExpandEnvironmentVariables(action.LaunchableResource),
                        Arguments = action.Arguments != null ? Environment.ExpandEnvironmentVariables(action.Arguments) : null,
                        WindowStyle = action.WindowStyle,
                        UseShellExecute = action.UseShellExecute
                    };
                    Process.Start(processStartInfo);
                    _flowerGuiWindow.Hide();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not perform action: '{action.Label}' - {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void TakeNote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create a simple note file
                string noteFile = Path.Combine(_settings.NotesDirectory,
                    $"FlowerGUI_Note_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

                File.WriteAllText(noteFile, $"FlowerGUI Note - {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n\n");

                // Open in default text editor
                Process.Start(new ProcessStartInfo(noteFile) { UseShellExecute = true });

                _flowerGuiWindow.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not create note: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void TakeScreenshot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _flowerGuiWindow.Hide(); // Hide window before taking screenshot

                // Wait a moment for the window to hide
                System.Threading.Thread.Sleep(200);

                var bounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
                using (var bitmap = new System.Drawing.Bitmap(bounds.Width, bounds.Height))
                {
                    using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
                    {
                        graphics.CopyFromScreen(System.Drawing.Point.Empty, System.Drawing.Point.Empty, bounds.Size);
                    }

                    string screenshotFile = Path.Combine(_settings.ScreenshotsDirectory,
                        $"FlowerGUI_Screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");

                    bitmap.Save(screenshotFile, System.Drawing.Imaging.ImageFormat.Png);

                    MessageBox.Show($"Screenshot saved at:\n{screenshotFile}", "FlowerGUI",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not take screenshot: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void OpenClipboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Try to get clipboard content
                string clipboardText = "";
                if (Clipboard.ContainsText())
                {
                    clipboardText = Clipboard.GetText();
                }

                MessageBox.Show($"Clipboard contents:\n{clipboardText}", "FlowerGUI - Clipboard",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                _flowerGuiWindow.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not read clipboard: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                _flowerGuiWindow.Hide();
            }
        }

        public void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("FlowerGUI Help\n\n" +
                  "How to use:\n" +
                  "• [Ctrl] + Right Click: Open FlowerGUI\n" +
                  "• [Esc]: Close FlowerGUI\n",
                  "FlowerGUI Help", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
