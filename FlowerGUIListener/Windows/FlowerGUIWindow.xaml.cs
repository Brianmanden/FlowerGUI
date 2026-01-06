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
		public double PetalHeight { get; private set; } = 220; // Default petal height

		public FlowerGUIWindow(Settings settings = null)
		{
			_settings = settings ?? new Settings();
			_petalActionService = new PetalActionService(this, _settings);
			PetalButtons = new ObservableCollection<PetalButtonData>();
			InitializePetalButtons();

			// Calculate required window size
			double petal_width = 100;
			double radius = 40 + (PetalHeight / 2);
			double max_petal_extent = Math.Sqrt(Math.Pow(petal_width / 2, 2) + Math.Pow(PetalHeight / 2, 2));
			double requiredSize = 2 * (radius + max_petal_extent);
			double padding = 50; // Add some padding

			this.Width = requiredSize + padding;
			this.Height = requiredSize + padding;

			InitializeComponent();
			InitializeWindow();
		}

		private void InitializePetalButtons()
		{
			int totalButtons = 9;
			double angleIncrement = 360.0 / totalButtons;
			double petalTipDistanceToCenter = 25 + (PetalHeight / 2);

			AddPetalButton(0 * angleIncrement, "Search", "Search_Click", petalTipDistanceToCenter);
			AddPetalButton(1 * angleIncrement, "GDrive", "Drive_Click", petalTipDistanceToCenter);
			AddPetalButton(2 * angleIncrement, "Total Commander", "TC_Click", petalTipDistanceToCenter);
			AddPetalButton(3 * angleIncrement, "Note", "TakeNote_Click", petalTipDistanceToCenter);
			AddPetalButton(4 * angleIncrement, "Screenshot", "TakeScreenshot_Click", petalTipDistanceToCenter);
			AddPetalButton(5 * angleIncrement, "Clipboard", "OpenClipboard_Click", petalTipDistanceToCenter);
			AddPetalButton(6 * angleIncrement, "Recent items", "RecentItems_Click", petalTipDistanceToCenter);
			AddPetalButton(7 * angleIncrement, "Help", "Help_Click", petalTipDistanceToCenter);
			AddPetalButton(8 * angleIncrement, "Info", "Info_Click", petalTipDistanceToCenter);
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
			double windowX = Math.Max(screen.WorkingArea.Left, Math.Min(x - this.Width / 2, screen.WorkingArea.Right - this.Width));
			double windowY = Math.Max(screen.WorkingArea.Top, Math.Min(y - this.Height / 2, screen.WorkingArea.Bottom - this.Height));

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
