using System;
using System.Windows.Input;

namespace FlowerGUIListener.Models
{
    public class PetalButtonData
    {
        public string Content { get; set; }
        public string ClickAction { get; set; } // Name of the method to call
        public double RotateAngle { get; set; }
        public double TranslateX { get; set; }
        public double TranslateY { get; set; }
        public double CenterX { get; set; } = 50; // Default values based on XAML
        public double CenterY { get; set; } = 60; // Default values based on XAML
    }
}
