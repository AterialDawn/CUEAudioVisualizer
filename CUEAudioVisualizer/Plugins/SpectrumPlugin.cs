using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using CUE.NET.Devices.Keyboard.Keys;
using CUEAudioVisualizer.Plugin;

namespace CUEAudioVisualizer.Plugins
{
    class SpectrumPlugin : IPlugin
    {
        public string Name { get { return "Spectrum"; } }

        public IPluginHost Host { get; set; }

        public VisualizerModes[] ModeList { get { return modeList; } }

        private VisualizerModes[] modeList;

        public SpectrumPlugin()
        {
            modeList = new VisualizerModes[] { new VisualizerModes("Spectrum", SpectrumUpdateDelegate),
                new VisualizerModes("Spectrum (VaryingColors)", SpectrumInvertingColorsDelegate),
                new VisualizerModes("Spectrum (Rainbow)", SpectrumRainbowDelegate)};
        }

        public void OnModeActivated(VisualizerModes ActivatedMode)
        {
            //Do nothing
        }

        public void OnModeDeactivated(VisualizerModes DeactivatedMode)
        {
            //Do nothing
        }

        //Updates the keyboard using the Spectrum Visualizer
        private void SpectrumUpdateDelegate()
        {
            double brightness = Utility.Clamp(0.03f + (Host.SongBeat * 0.1f), 0f, 1f); //Keep brightness at least to 3% and clamp to 100% (Should never get anywhere near 100%)
            Color backgroundColor = Utility.CalculateColorBrightness(Host.PrimaryColor, brightness);
            Host.Keyboard.Color = backgroundColor;

            float kbWidth = Host.Keyboard.KeyboardRectangle.Location.X + Host.Keyboard.KeyboardRectangle.Width;
            float kbHeight = Host.Keyboard.KeyboardRectangle.Location.Y + Host.Keyboard.KeyboardRectangle.Height;
            float barCount = SoundDataProcessor.BarCount;

            foreach (CorsairKey key in Host.Keyboard.Keys)
            {
                //Calculate the color for each individual key, and light it up if necessary
                RectangleF keyRect = key.KeyRectangle;
                PointF keyCenterPos = new PointF(keyRect.Location.X + (keyRect.Width / 2f), keyRect.Location.Y + (keyRect.Height / 2f)); //Sample center of key
                int barSampleIndex = (int)Math.Floor(barCount * (keyCenterPos.X / kbWidth)); //Calculate bar sampling index
                float curBarHeight = 1f - Utility.Clamp(Host.SmoothedBarData[barSampleIndex] * 1.5f, 0f, 1f); //Scale values up a bit and clamp to 1f. I also invert this value since the keyboard is laid out with topleft being point 0,0
                float keyVerticalPos = (keyCenterPos.Y / kbHeight);

                if (curBarHeight <= keyVerticalPos)
                {
                    //Key should be fully lit
                    key.Led.Color = Host.SecondaryColor;
                }
                else
                {
                    //Do whatever we do for unlit keys, which is currently nothing
                }

            }
        }

        //Updates the keyboard using the Spectrum visualizer, with the colors inverting based on average volume (v1.0-v1.1 default mode)
        private void SpectrumInvertingColorsDelegate()
        {
            double brightness = Utility.Clamp(0.03f + (Host.SongBeat * 0.1f), 0f, 1f); //Keep brightness at least to 3% and clamp to 100% (Should never get anywhere near 100%)
            Color backgroundColor = Utility.CalculateGradient(Host.SecondaryColor, Host.PrimaryColor, Host.AveragedVolume, brightness);
            Host.Keyboard.Color = backgroundColor;

            float kbWidth = Host.Keyboard.KeyboardRectangle.Location.X + Host.Keyboard.KeyboardRectangle.Width;
            float kbHeight = Host.Keyboard.KeyboardRectangle.Location.Y + Host.Keyboard.KeyboardRectangle.Height;
            float barCount = SoundDataProcessor.BarCount;

            foreach (CorsairKey key in Host.Keyboard.Keys)
            {
                //Calculate the color for each individual key, and light it up if necessary
                RectangleF keyRect = key.KeyRectangle;
                PointF keyCenterPos = new PointF(keyRect.Location.X + (keyRect.Width / 2f), keyRect.Location.Y + (keyRect.Height / 2f)); //Sample center of key
                int barSampleIndex = (int)Math.Floor(barCount * (keyCenterPos.X / kbWidth)); //Calculate bar sampling index
                float curBarHeight = 1f - Utility.Clamp(Host.SmoothedBarData[barSampleIndex] * 1.5f, 0f, 1f); //Scale values up a bit and clamp to 1f. I also invert this value since the keyboard is laid out with topleft being point 0,0
                float keyVerticalPos = (keyCenterPos.Y / kbHeight);

                if (curBarHeight <= keyVerticalPos)
                {
                    //Key should be fully lit
                    key.Led.Color = Utility.CalculateGradient(Host.PrimaryColor, Host.SecondaryColor, Host.AveragedVolume, 1f);
                }
                else
                {
                    //Do whatever we do for unlit keys, which is currently nothing
                }

            }
        }

        private void SpectrumRainbowDelegate()
        {
            double brightness = 1f; //Fixed full brightness on rainbow background

            float kbWidth = Host.Keyboard.KeyboardRectangle.Location.X + Host.Keyboard.KeyboardRectangle.Width;
            float kbHeight = Host.Keyboard.KeyboardRectangle.Location.Y + Host.Keyboard.KeyboardRectangle.Height;
            float barCount = SoundDataProcessor.BarCount;

            foreach (CorsairKey key in Host.Keyboard.Keys)
            {
                //Calculate the color for each individual key, and light it up if necessary
                RectangleF keyRect = key.KeyRectangle;
                PointF keyCenterPos = new PointF(keyRect.Location.X + (keyRect.Width / 2f), keyRect.Location.Y + (keyRect.Height / 2f)); //Sample center of key
                float keyVerticalPos = (keyCenterPos.Y / kbHeight);
                float keyHorizontalPos = (keyCenterPos.X / kbWidth);
                int barSampleIndex = (int)Math.Floor(barCount * (keyCenterPos.X / kbWidth)); //Calculate bar sampling index
                float curBarHeight = 1f - Utility.Clamp(Host.SmoothedBarData[barSampleIndex] * 1.5f, 0f, 1f); //Scale values up a bit and clamp to 1f. I also invert this value since the keyboard is laid out with topleft being point 0,0

                if (curBarHeight <= keyVerticalPos)
                {
                    //Key should be fully lit
                    key.Led.Color = Host.PrimaryColor;
                }
                else
                {
                    //'unlit' keys will be rainbow gradiented
                    float t = (float)(((keyHorizontalPos + (Host.Time * 0.4)) / 2f) + ((keyVerticalPos + (Host.Time * 0.4)) * 0.25f)) % 1f;
                    key.Led.Color = Utility.CalculateRainbowGradient(t);
                }

            }
        }
    }
}
