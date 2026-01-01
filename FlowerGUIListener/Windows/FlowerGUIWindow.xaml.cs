using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

using System.Collections.ObjectModel;
using System.Reflection;
using FlowerGUIListener.Models;
using FlowerGUIListener.Services;

namespace FlowerGUIListener.Windows
{
	public partial class FlowerGUIWindow : Window
	{
		private Settings _settings;
		private PetalActionService _petalActionService;

		public ObservableCollection<PetalButtonData> PetalButtons { get; set; }

		public FlowerGUIWindow(Settings settings = null)
		{
			_settings = settings ?? new Settings();
			_petalActionService = new PetalActionService(this, _settings);
			PetalButtons = new ObservableCollection<PetalButtonData>();
			InitializePetalButtons();
			InitializeComponent();
			InitializeWindow();
		}

		private void InitializePetalButtons()
		{
            int totalButtons = 9;
            double angleIncrement = 360.0 / totalButtons;

			PetalButtons.Add(new PetalButtonData { RotateAngle = 0 * angleIncrement, Content = "Note", ClickAction = "TakeNote_Click" });
			PetalButtons.Add(new PetalButtonData { RotateAngle = 1 * angleIncrement, Content = "Screenshot", ClickAction = "TakeScreenshot_Click" });
			PetalButtons.Add(new PetalButtonData { RotateAngle = 2 * angleIncrement, Content = "Clipboard", ClickAction = "OpenClipboard_Click" });
			PetalButtons.Add(new PetalButtonData { RotateAngle = 3 * angleIncrement, Content = "Search", ClickAction = "Search_Click" });
			PetalButtons.Add(new PetalButtonData { RotateAngle = 4 * angleIncrement, Content = "Recent items", ClickAction = "RecentItems_Click" });
			PetalButtons.Add(new PetalButtonData { RotateAngle = 5 * angleIncrement, Content = "Help", ClickAction = "Help_Click" });
			PetalButtons.Add(new PetalButtonData { RotateAngle = 6 * angleIncrement, Content = "Info", ClickAction = "Info_Click" });
			PetalButtons.Add(new PetalButtonData { RotateAngle = 7 * angleIncrement, Content = "Drive", ClickAction = "Drive_Click" });
			PetalButtons.Add(new PetalButtonData { RotateAngle = 8 * angleIncrement, Content = "Total Commander", ClickAction = "TC_Click" });
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
			// Position the window near the cursor but
			// ensure it stays on screen
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
					_petalActionService.Info_Click(null, null);
					e.Handled = true;
					break;
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
				var methodInfo = typeof(PetalActionService).GetMethod(petalData.ClickAction);
				methodInfo?.Invoke(_petalActionService, new object[] { sender, e });
			}
		}
	}
}
