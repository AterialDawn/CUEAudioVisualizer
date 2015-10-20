using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using CUE.NET.Devices.Keyboard.Keys;
using CUEAudioVisualizer.Plugin;

namespace CUEAudioVisualizer.Plugins
{
    class VUPlugin : IPlugin
    {
        public string Name { get { return "VU"; } }

        public IPluginHost Host { get; set; }

        public VisualizerModes[] ModeList { get { return modeList; } }

        private VisualizerModes[] modeList;

        public VUPlugin()
        {
            modeList = new VisualizerModes[] { new VisualizerModes("VU (Left)", VULeftDelegate),
                new VisualizerModes("VU (Right)", VURightDelegate),
                new VisualizerModes("VU (Rainbow Left)", VURainbowLeftDelegate),
                new VisualizerModes("VU (Rainbow Right)", VURainbowRightDelegate)};
        }

        public void OnModeActivated(VisualizerModes ActivatedMode)
        {
            //Do nothing
        }

        public void OnModeDeactivated(VisualizerModes DeactivatedMode)
        {
            //Do nothing
        }

        private void VULeftDelegate()
        {
            double brightness = Utility.Clamp(0.03f + (Host.SongBeat * 0.1f), 0f, 1f); //Keep brightness at least to 3% and clamp to 100% (Should never get anywhere near 100%)
            Color backgroundColor = Utility.CalculateColorBrightness(Host.SecondaryColor, brightness);
            Host.Keyboard.Color = backgroundColor;

            float kbWidth = Host.Keyboard.KeyboardRectangle.Location.X + Host.Keyboard.KeyboardRectangle.Width;
            float immediateVolume = Host.ImmediateVolume;
            foreach (CorsairKey key in Host.Keyboard.Keys)
            {
                //Determine if key should be lit based on overall volume
                RectangleF keyRect = key.KeyRectangle;
                PointF keyCenterPos = new PointF(keyRect.Location.X + (keyRect.Width / 2f), keyRect.Location.Y + (keyRect.Height / 2f)); //Sample center of key

                float keyHorVal = (keyCenterPos.X / kbWidth);

                if (immediateVolume >= keyHorVal)
                {
                    //Key should be fully lit
                    key.Led.Color = Host.PrimaryColor;
                }
                else
                {
                    //Do whatever we do for unlit keys, which is currently nothing
                }
            }
        }

        private void VURightDelegate()
        {
            double brightness = Utility.Clamp(0.03f + (Host.SongBeat * 0.1f), 0f, 1f); //Keep brightness at least to 3% and clamp to 100% (Should never get anywhere near 100%)
            Color backgroundColor = Utility.CalculateColorBrightness(Host.SecondaryColor, brightness);
            Host.Keyboard.Color = backgroundColor;

            float kbWidth = Host.Keyboard.KeyboardRectangle.Location.X + Host.Keyboard.KeyboardRectangle.Width;
            float immediateVolume = Host.ImmediateVolume;
            foreach (CorsairKey key in Host.Keyboard.Keys)
            {
                //Determine if key should be lit based on overall volume
                RectangleF keyRect = key.KeyRectangle;
                PointF keyCenterPos = new PointF(keyRect.Location.X + (keyRect.Width / 2f), keyRect.Location.Y + (keyRect.Height / 2f)); //Sample center of key

                float keyHorVal = 1f - (keyCenterPos.X / kbWidth);

                if (immediateVolume >= keyHorVal)
                {
                    //Key should be fully lit
                    key.Led.Color = Host.PrimaryColor;
                }
                else
                {
                    //Do whatever we do for unlit keys, which is currently nothing
                }
            }
        }

        private void VURainbowLeftDelegate()
        {
            double brightness = 1f; //Brightness is fixed at 100% for rainbow modes.

            float kbWidth = Host.Keyboard.KeyboardRectangle.Location.X + Host.Keyboard.KeyboardRectangle.Width;
            float kbHeight = Host.Keyboard.KeyboardRectangle.Location.Y + Host.Keyboard.KeyboardRectangle.Height;
            float immediateVolume = Host.ImmediateVolume;
            foreach (CorsairKey key in Host.Keyboard.Keys)
            {
                //Determine if key should be lit based on overall volume
                RectangleF keyRect = key.KeyRectangle;
                PointF keyCenterPos = new PointF(keyRect.Location.X + (keyRect.Width / 2f), keyRect.Location.Y + (keyRect.Height / 2f)); //Sample center of key
                float keyVerticalPos = (keyCenterPos.Y / kbHeight);
                float keyHorizontalPos = (keyCenterPos.X / kbWidth);

                float keyHorVal = (keyCenterPos.X / kbWidth);

                if (immediateVolume >= keyHorVal)
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

        private void VURainbowRightDelegate()
        {
            double brightness = 1f; //Brightness is fixed at 100% for rainbow modes.

            float kbWidth = Host.Keyboard.KeyboardRectangle.Location.X + Host.Keyboard.KeyboardRectangle.Width;
            float kbHeight = Host.Keyboard.KeyboardRectangle.Location.Y + Host.Keyboard.KeyboardRectangle.Height;
            float immediateVolume = Host.ImmediateVolume;
            foreach (CorsairKey key in Host.Keyboard.Keys)
            {
                //Determine if key should be lit based on overall volume
                RectangleF keyRect = key.KeyRectangle;
                PointF keyCenterPos = new PointF(keyRect.Location.X + (keyRect.Width / 2f), keyRect.Location.Y + (keyRect.Height / 2f)); //Sample center of key
                float keyVerticalPos = (keyCenterPos.Y / kbHeight);
                float keyHorizontalPos = (keyCenterPos.X / kbWidth);

                float keyHorVal = 1f - (keyCenterPos.X / kbWidth);

                if (immediateVolume >= keyHorVal)
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
