using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CUEAudioVisualizer
{
    public static class Utility
    {
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
