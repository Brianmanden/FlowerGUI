using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

using System.Collections.ObjectModel;
using System.Reflection;
using FlowerGUIListener.Models;

namespace FlowerGUIListener.Windows
{
	public partial class FlowerGUIWindow : Window
	{
		private Settings _settings;

		public ObservableCollection<PetalButtonData> PetalButtons { get; set; }

		public FlowerGUIWindow(Settings settings = null)
		{
			_settings = settings ?? new Settings();
			PetalButtons = new ObservableCollection<PetalButtonData>();
			InitializePetalButtons();
			InitializeComponent();
			InitializeWindow();
		}

		private void InitializePetalButtons()
		{
            int totalButtons = 9;
            double angleIncrement = 360.0 / totalButtons;

			PetalButtons.Add(new PetalButtonData { RotateAngle = 0 * angleIncrement, Content = "Note", ClickAction = nameof(TakeNote_Click) });
			PetalButtons.Add(new PetalButtonData { RotateAngle = 1 * angleIncrement, Content = "Screenshot", ClickAction = nameof(TakeScreenshot_Click) });
			PetalButtons.Add(new PetalButtonData { RotateAngle = 2 * angleIncrement, Content = "Clipboard", ClickAction = nameof(OpenClipboard_Click) });
			PetalButtons.Add(new PetalButtonData { RotateAngle = 3 * angleIncrement, Content = "Search", ClickAction = nameof(Search_Click) });
			PetalButtons.Add(new PetalButtonData { RotateAngle = 4 * angleIncrement, Content = "Recent items", ClickAction = nameof(RecentItems_Click) });
			PetalButtons.Add(new PetalButtonData { RotateAngle = 5 * angleIncrement, Content = "Help", ClickAction = nameof(Help_Click) });
			PetalButtons.Add(new PetalButtonData { RotateAngle = 6 * angleIncrement, Content = "Info", ClickAction = nameof(Info_Click) });
			PetalButtons.Add(new PetalButtonData { RotateAngle = 7 * angleIncrement, Content = "Drive", ClickAction = nameof(Drive_Click) });
			PetalButtons.Add(new PetalButtonData { RotateAngle = 8 * angleIncrement, Content = "Total Commander", ClickAction = nameof(TC_Click) });
		}

		private void InitializeWindow()
		{
			// Handle keyboard shortcuts
			this.KeyDown += OnKeyDown;

			// Allow window dragging by clicking anywhere on it
			this.MouseLeftButtonDown += (s, e) => this.DragMove();
		}

		public void ShowAt(int x, int y)
		{
			// Position the window near the cursor but ensure it stays on screen
			var screen = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point(x, y));

			// Calculate position
			double windowX = Math.Max(0, Math.Min(x - this.Width / 2, screen.WorkingArea.Width - this.Width));
			double windowY = Math.Max(0, Math.Min(y - this.Height / 2, screen.WorkingArea.Height - this.Height));

			this.Left = windowX;
			this.Top = windowY;

			this.Show();
			this.Activate();
			this.Focus();
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Escape:
					this.Hide();
					e.Handled = true;
					break;

				case Key.H when Keyboard.Modifiers == ModifierKeys.Control:
					Info_Click(null, null);
					e.Handled = true;
					break;
			}
		}

		private void TakeNote_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				// Create a simple note file
				string noteFile = Path.Combine(_settings.NotesDirectory,
					$"FlowerGUI_Note_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

				File.WriteAllText(noteFile, $"FlowerGUI Notat - {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n\n");

				// Open in default text editor
				Process.Start(new ProcessStartInfo(noteFile) { UseShellExecute = true });

				this.Hide();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Kunne ikke oprette notat: {ex.Message}", "Fejl",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void TakeScreenshot_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				this.Hide(); // Hide window before taking screenshot

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

					MessageBox.Show($"Skærmbillede gemt til:\n{screenshotFile}", "FlowerGUI",
						MessageBoxButton.OK, MessageBoxImage.Information);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Kunne ikke tage skærmbillede: {ex.Message}", "Fejl",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void OpenClipboard_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				// Try to get clipboard content
				string clipboardText = "";
				if (Clipboard.ContainsText())
				{
					clipboardText = Clipboard.GetText();
				}

				MessageBox.Show($"Udklipsholder indhold:\n{clipboardText}", "FlowerGUI - Udklipsholder",
					MessageBoxButton.OK, MessageBoxImage.Information);
				this.Hide();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Kunne ikke læse udklipsholder: {ex.Message}", "Fejl",
					MessageBoxButton.OK, MessageBoxImage.Error);
				this.Hide();
			}
		}

		private void Search_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				// Open Google.com
				Process.Start(new ProcessStartInfo("https://www.google.com") { UseShellExecute = true });
				this.Hide();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Kunne ikke åbne Google.com: {ex.Message}", "Fejl",
					MessageBoxButton.OK, MessageBoxImage.Error);
				this.Hide();
			}
		}

		private void RecentItems_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string path = Environment.ExpandEnvironmentVariables(@"%APPDATA%\Microsoft\Windows\Recent");
				Process.Start("explorer.exe", path);
				this.Hide();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Could not open Recent Items: {ex.Message}", "Error",
					MessageBoxButton.OK, MessageBoxImage.Error);
				this.Hide();
			}
		}

		private void Help_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("FlowerGUI Hjælp\n\n" +
				  "Genveje:\n" +
				  "• Ctrl + Højreklik: Åbn FlowerGUI\n" +
				  "• Esc: Luk FlowerGUI\n" +
				  "• Ctrl + H: Vis hjælp\n\n" +
				  "Funktioner:\n" +
				  "• Notat: Opret hurtige notater\n" +
				  "• Skærmbillede: Tag skærmbilleder\n" +
				  "• Udklipsholder: Vis udklipsholder-indhold\n" +
				  "• Søg: Åbn Windows søgning",
				  "FlowerGUI Hjælp", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void Info_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				// Open the GitHub repository URL
				Process.Start(new ProcessStartInfo("https://github.com/Brianmanden/FlowerGUI") { UseShellExecute = true });
				this.Hide();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Could not open the URL: {ex.Message}", "Error",
					MessageBoxButton.OK, MessageBoxImage.Error);
				this.Hide();
			}
		}

		private void Drive_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				// Open the GitHub repository URL
				Process.Start(new ProcessStartInfo("https://drive.google.com/drive/my-drive") { UseShellExecute = true });
				this.Hide();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Could not open the URL: {ex.Message}", "Error",
					MessageBoxButton.OK, MessageBoxImage.Error);
				this.Hide();
			}
		}

		private void TC_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var processStartInfo = new ProcessStartInfo
				{
					FileName = "C:\\Program Files\\totalcmd\\TOTALCMD64.EXE",
					WindowStyle = ProcessWindowStyle.Maximized,
					UseShellExecute = true
				};

				Process.Start(processStartInfo);
				this.Hide();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Could not open Total Commander: {ex.Message}", "Error",
					MessageBoxButton.OK, MessageBoxImage.Error);
				this.Hide();
			}
		}

		protected override void OnDeactivated(EventArgs e)
		{
			base.OnDeactivated(e);
			// Optionally hide window when it loses focus
			this.Hide();
		}

		private void PetalButton_Click(object sender, RoutedEventArgs e)
		{
			if (sender is System.Windows.Controls.Button button && button.DataContext is PetalButtonData petalData)
			{
				var methodInfo = GetType().GetMethod(petalData.ClickAction, BindingFlags.NonPublic | BindingFlags.Instance);
				methodInfo?.Invoke(this, new object[] { sender, e });
			}
		}
	}
}
