using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUE.NET.Devices.Keyboard.Keys;
using CUEAudioVisualizer;
using CUEAudioVisualizer.Plugin;

namespace ExamplePlugin
{
    public class ExamplePlugin : IPlugin
    {
        public string Name { get { return "Example Plugin"; } }

        public IPluginHost Host { get; set; }

        public VisualizerModes[] ModeList { get { return modeList; } }

        private VisualizerModes[] modeList;

        public ExamplePlugin()
        {
            modeList = new VisualizerModes[] { new VisualizerModes("Spectrum Responsive Background", SpectrumResponsiveBackground) };
        }

        public void SpectrumResponsiveBackground()
        {
            float kbWidth = Host.Keyboard.KeyboardRectangle.Location.X + Host.Keyboard.KeyboardRectangle.Width;
            float kbHeight = Host.Keyboard.KeyboardRectangle.Location.Y + Host.Keyboard.KeyboardRectangle.Height;
            float barCount = Host.SmoothedBarData.Length;

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
                    //'unlit' keys will change based on bar values
                    byte r, g, b;
                    r = (byte)Utility.Clamp(Host.SmoothedBarData[15] * 255, 0, 255);
                    g = (byte)Utility.Clamp(Host.SmoothedBarData[440] * 255, 0, 255);
                    b = (byte)Utility.Clamp(Host.SmoothedBarData[860] * 255, 0, 255);
                    key.Led.Color = Color.FromArgb(255, r, g, b);
                }

            }
        }
    }
}
