using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CUEAudioVisualizer
{
    public static class Utility
    {
        private static Color[] rainbowColors = new Color[] { Color.FromArgb(255, 255, 0, 0), Color.FromArgb(255, 0, 255, 0), Color.FromArgb(255, 0, 0, 255), Color.FromArgb(255, 255, 0, 0) };

        public static float Clamp(float Value, float Min, float Max)
        {
            return Math.Min(Math.Max(Value, Min), Max);
        }

        public static double Clamp(double Value, double Min, double Max)
        {
            return Math.Min(Math.Max(Value, Min), Max);
        }

        public static float LinearInterpolate(float AbsMin, float AbsMax, float Value)
        {
            return (Value / (1f / (AbsMax - AbsMin))) + AbsMin;
        }

        public static double LinearInterpolate(double AbsMin, double AbsMax, double Value)
        {
            return (Value / (1.0 / (AbsMax - AbsMin))) + AbsMin;
        }

        public static Color CalculateRainbowGradient(double mix)
        {
            double scaledMix = mix * (rainbowColors.Length - 1);
            Color firstCol = rainbowColors[(int)scaledMix];
            Color secondCol = rainbowColors[(int)(scaledMix + 1.0)];
            double newMix = scaledMix - (float)((int)scaledMix);
            return Utility.CalculateGradient(firstCol, secondCol, newMix, 1f);
        }

        public static Color CalculateColorBrightness(Color color, double brightness)
        {
            int scaledA = (int)Math.Floor(color.A * brightness);
            int scaledR = (int)Math.Floor(color.R * brightness);
            int scaledG = (int)Math.Floor(color.G * brightness);
            int scaledB = (int)Math.Floor(color.B * brightness);

            return Color.FromArgb(scaledA, scaledR, scaledG, scaledB);
        }

        public static Color CalculateGradient(Color fromColor, Color toColor, double mix, double brightness)
        {
            int gradA = (int)Math.Floor(LinearInterpolate(fromColor.A, toColor.A, mix) * brightness);
            int gradR = (int)Math.Floor(LinearInterpolate(fromColor.R, toColor.R, mix) * brightness);
            int gradG = (int)Math.Floor(LinearInterpolate(fromColor.G, toColor.G, mix) * brightness);
            int gradB = (int)Math.Floor(LinearInterpolate(fromColor.B, toColor.B, mix) * brightness);

            return Color.FromArgb(gradA, gradR, gradG, gradB);
        }
    }
}
