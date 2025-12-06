using System;

namespace FlowerGUIListener.Models
{
    public class PetalButtonData
    {
        public string Content { get; set; }
        public string ClickAction { get; set; } // Name of the method to call
        
        private double _rotateAngle;
        public double RotateAngle
        {
            get => _rotateAngle;
            set
            {
                _rotateAngle = value;
                CalculateTransforms();
            }
        }

        public double CalculatedTranslateX { get; private set; }
        public double CalculatedTranslateY { get; private set; }

        private const double Radius = 100; // Based on initial XAML for "Note" button Y transform

        private void CalculateTransforms()
        {
            // Convert angle to radians
            double angleRad = RotateAngle * (Math.PI / 180.0);

            // Calculate TranslateX and TranslateY
            // We want 0 degrees to point upwards, so Y is negative cos, X is positive sin
            CalculatedTranslateX = Radius * Math.Sin(angleRad);
            CalculatedTranslateY = -Radius * Math.Cos(angleRad);
        }
    }
}
