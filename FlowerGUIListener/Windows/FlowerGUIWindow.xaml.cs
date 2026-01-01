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
		public double PetalHeight { get; set; } = 220;

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
            double radius = 40 + (PetalHeight / 2); // 40 is half the width of the center ellipse

            AddPetalButton(0 * angleIncrement, "Search", "Search_Click", radius);
            AddPetalButton(1 * angleIncrement, "Drive", "Drive_Click", radius);
            AddPetalButton(2 * angleIncrement, "Total Commander", "TC_Click", radius);
            AddPetalButton(3 * angleIncrement, "Note", "TakeNote_Click", radius);
            AddPetalButton(4 * angleIncrement, "Screenshot", "TakeScreenshot_Click", radius);
            AddPetalButton(5 * angleIncrement, "Clipboard", "OpenClipboard_Click", radius);
            AddPetalButton(6 * angleIncrement, "Recent items", "RecentItems_Click", radius);
            AddPetalButton(7 * angleIncrement, "Help", "Help_Click", radius);
            AddPetalButton(8 * angleIncrement, "Info", "Info_Click", radius);
        }

        private void AddPetalButton(double angle, string content, string action, double radius)
        {
            var petalButton = new PetalButtonData
            {
                RotateAngle = angle,
                Content = content,
                ClickAction = action
            };
            petalButton.UpdateTransforms(radius);
            PetalButtons.Add(petalButton);
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
