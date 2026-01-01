using System;

namespace FlowerGUIListener.Models
{
    public class PetalButtonData
    {
        public string Content { get; set; }
        public string ClickAction { get; set; } // Name of the method to call
        
        public double RotateAngle { get; set; }

        public double CalculatedTranslateX { get; private set; }
        public double CalculatedTranslateY { get; private set; }

        public double Radius { get; private set; }

        public void UpdateTransforms(double radius)
        {
            this.Radius = radius;
            // Convert angle to radians
            double angleRad = RotateAngle * (Math.PI / 180.0);

            // Calculate TranslateX and TranslateY
            // We want 0 degrees to point upwards, so Y is negative cos, X is positive sin
            CalculatedTranslateX = Radius * Math.Sin(angleRad);
            CalculatedTranslateY = -Radius * Math.Cos(angleRad);
        }
    }
}

