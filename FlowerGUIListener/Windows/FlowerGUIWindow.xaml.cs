using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        private readonly List<PetalAction> _petalActions;

        public ObservableCollection<PetalButtonData> PetalButtons { get; set; }
        public double PetalHeight { get; private set; } = 220; // Default petal height

        public FlowerGUIWindow(Settings settings, List<PetalAction> petalActions)
        {
            _settings = settings ?? new Settings();
            _petalActions = petalActions;
            _petalActionService = new PetalActionService(this, _settings, _petalActions);
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
            var allActions = new List<PetalButtonData>();

            if (_petalActions != null)
            {
                // Add actions from JSON
                foreach (var action in _petalActions)
                {
                    allActions.Add(new PetalButtonData { Content = action.Label, ClickAction = action.Id });
                }
            }

            // Add hardcoded actions
            allActions.Add(new PetalButtonData { Content = "Note", ClickAction = "TakeNote_Click" });
            allActions.Add(new PetalButtonData { Content = "Screenshot", ClickAction = "TakeScreenshot_Click" });
            allActions.Add(new PetalButtonData { Content = "Clipboard", ClickAction = "OpenClipboard_Click" });
            allActions.Add(new PetalButtonData { Content = "Help", ClickAction = "Help_Click" });


            int totalButtons = allActions.Count;
            double angleIncrement = 360.0 / totalButtons;
            double petalTipDistanceToCenter = 25 + (PetalHeight / 2);

            for (int i = 0; i < totalButtons; i++)
            {
                var action = allActions[i];
                AddPetalButton(i * angleIncrement, action.Content, action.ClickAction, petalTipDistanceToCenter);
            }
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
            if (sender is Button button && button.DataContext is PetalButtonData petalData)
            {
                // Check if the action is one of the hardcoded methods
                var methodInfo = typeof(PetalActionService).GetMethod(petalData.ClickAction);
                if (methodInfo != null)
                {
                    methodInfo.Invoke(_petalActionService, new object[] { sender, e });
                }
                else
                {
                    // Otherwise, it's an ID for the generic Execute method
                    _petalActionService.Execute(petalData.ClickAction);
                }
            }
        }
	}
}
