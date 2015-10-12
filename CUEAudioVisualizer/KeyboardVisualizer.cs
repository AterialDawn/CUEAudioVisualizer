using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using CUE.NET;
using CUE.NET.Devices.Generic.Enums;
using CUE.NET.Devices.Keyboard;
using CUE.NET.Devices.Keyboard.Keys;
using CUE.NET.Exceptions;

namespace CUEAudioVisualizer
{
    //This code was originally from a Console application so there's a bit of Console.WriteLines that I haven't bothered to clean up
    class KeyboardVisualizer
    {
        public KeyboardVisualizerMode VisualizerMode { get; set; }

        private CorsairKeyboard Keyboard;
        private SoundDataProcessor DataProcessor;
        private Color beatLow = Color.FromArgb(150, 150, 255);
        private Color beatHigh = Color.FromArgb(0, 0, 255);

        public KeyboardVisualizer()
        {
            VisualizerMode = KeyboardVisualizerMode.Spectrum;
            DataProcessor = new SoundDataProcessor(Properties.Settings.Default.DeviceIndex);
            UpdateFromSettings();
        }

        #region CUE Initialization Functions
        //Initializes CUE SDK with Exclusive Access
        public bool InitKeyboard()
        {
            return Init(true);
        }

        //Actually initializes CUE SDK with Exclusive Access
        private bool Init(bool ExclusiveAccess)
        {
            Console.WriteLine("Attempting to initialize CUE SDK...");
            //Initialize SDK
            try
            {
                if (!InitializeCueSdk(ExclusiveAccess)) return false;

                //Verify that a keyboard is connected
                if (!CheckForKeyboard()) return false;

                Console.WriteLine("CUE SDK initialized!");
                PrintDeviceInfo();

                return true;
            }
            catch (CUEException)
            {
                Console.WriteLine("Unable to initialize CUE SDK!");
                return false;
            }

        }

        //Prints device info to console, such as KB detected and the calculated rect
        private void PrintDeviceInfo()
        {
            var devInfo = Keyboard.DeviceInfo;
            var kbRect = Keyboard.KeyboardRectangle;
            string deviceModel = devInfo.Model;
            Console.WriteLine("Keyboard {0} detected.", deviceModel);
            Console.WriteLine("Device Rectangle {0}x{1} at {2},{3}", kbRect.Width, kbRect.Height, kbRect.X, kbRect.Y);
        }

        //Initializes CUESDK with optional Exclusive Access
        private bool InitializeCueSdk(bool ExclusiveAccess)
        {
            CueSDK.Initialize(ExclusiveAccess);
            if (!CheckForCUEError()) return false;
            var protocol = CueSDK.ProtocolDetails;
            Console.WriteLine("CUE SDK: SDK Version {0}, Server Version {1}", protocol.SdkVersion, protocol.ServerVersion);
            return true;
        }

        //Checks the Keyboard is not null
        private bool ValidateKeyboard()
        {
            if (Keyboard == null)
            {
                Console.WriteLine("Lost keyboard!");
                return false;
            }
            else
            {
                return true;
            }
        }

        //Checks for a CUE Error in the initialization steps
        private bool CheckForCUEError()
        {
            if (CueSDK.LastError != CorsairError.Success)
            {
                Console.WriteLine("Error initializing CueSDK! {0}.", CueSDK.LastError.ToString());
                return false;
            }
            else
            {
                return true;
            }
        }

        //Checks that we have a valid KeyboardSDK instance
        private bool CheckForKeyboard()
        {
            Keyboard = CueSDK.KeyboardSDK;
            if (!CheckForCUEError() || Keyboard == null)
            {
                Console.WriteLine("No Corsair keyboard connected!.");
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region Visualizer Functions
        //Updates visualizer parameters from Settings object
        public void UpdateFromSettings()
        {
            beatHigh = Properties.Settings.Default.PrimaryColor;
            beatLow = Properties.Settings.Default.SecondaryColor;
            DataProcessor.VolumeScalar = Properties.Settings.Default.VolumeModifier;
            DataProcessor.WASAPIDeviceIndex = Properties.Settings.Default.DeviceIndex;
            if (Properties.Settings.Default.VisualizerMode == "Spectrum")
            {
                VisualizerMode = KeyboardVisualizerMode.Spectrum;
            }
            else if (Properties.Settings.Default.VisualizerMode == "VU")
            {
                VisualizerMode = KeyboardVisualizerMode.VUMeter;
            }
        }

        //Visualizes keyboard from DataProcessor data
        public void UpdateKeyboard()
        {
            //Ensure we have Exclusive access, else throw.
            if (CueSDK.LastError == CorsairError.NoControl)
            {
                throw new ApplicationException("We lost Exclusive Control!");
            }

            DataProcessor.Process();
            
            //Do whatever and UpdateLeds
            CorsairKeyboard kb = Keyboard;
            
            kb.Color = Color.Black; //Clear keyboard colors

            if (VisualizerMode == KeyboardVisualizerMode.Spectrum)
            {
                VisualizeSpectrum();
            }
            else if (VisualizerMode == KeyboardVisualizerMode.VUMeter)
            {
                VisualizeVU();
            }

            Keyboard.UpdateLeds();
        }

        //Handles the Spectrum Visualization
        private void VisualizeSpectrum()
        {
            double brightness = Utility.Clamp(0.03f + (DataProcessor.SongBeat * 0.1f), 0f, 1f); //Keep brightness at least to 3% and clamp to 100% (Should never get anywhere near 100%)
            Color backgroundColor = Utility.CalculateGradient(beatLow, beatHigh, DataProcessor.AveragedVolume, brightness);
            Keyboard.Color = backgroundColor;

            float kbWidth = Keyboard.KeyboardRectangle.Location.X + Keyboard.KeyboardRectangle.Width;
            float kbHeight = Keyboard.KeyboardRectangle.Location.Y + Keyboard.KeyboardRectangle.Height;
            float barCount = SoundDataProcessor.BarCount;

            foreach (CorsairKey key in Keyboard.Keys)
            {
                //Calculate the color for each individual key, and light it up if necessary
                RectangleF keyRect = key.KeyRectangle;
                PointF keyCenterPos = new PointF(keyRect.Location.X + (keyRect.Width / 2f), keyRect.Location.Y + (keyRect.Height / 2f)); //Sample center of key
                int barSampleIndex = (int)Math.Floor(barCount * (keyCenterPos.X / kbWidth)); //Calculate bar sampling index
                float curBarHeight = 1f - Utility.Clamp(DataProcessor.BarValues[barSampleIndex] * 1.5f, 0f, 1f); //Scale values up a bit and clamp to 1f. I also invert this value since the keyboard is laid out with topleft being point 0,0
                float keyVerticalPos = (keyCenterPos.Y / kbHeight);

                if (curBarHeight <= keyVerticalPos)
                {
                    //Key should be fully lit
                    key.Led.Color = Utility.CalculateGradient(beatHigh, beatLow, DataProcessor.AveragedVolume, 1f);
                }
                else
                {
                    //Do whatever we do for unlit keys, which is currently nothing
                }

            }
        }

        //Handles the VU Visualization
        private void VisualizeVU()
        {
            double brightness = Utility.Clamp(0.03f + (DataProcessor.SongBeat * 0.1f), 0f, 1f); //Keep brightness at least to 3% and clamp to 100% (Should never get anywhere near 100%)
            Color backgroundColor = Utility.CalculateGradient(beatLow, beatHigh, DataProcessor.AveragedVolume, brightness);
            Keyboard.Color = backgroundColor;

            float kbWidth = Keyboard.KeyboardRectangle.Location.X + Keyboard.KeyboardRectangle.Width;
            float immediateVolume = DataProcessor.ImmediateVolume; //Scale down a bit since the kbWidth 
            foreach (CorsairKey key in Keyboard.Keys)
            {
                //Determine if key should be lit based on overall volume
                RectangleF keyRect = key.KeyRectangle;
                PointF keyCenterPos = new PointF(keyRect.Location.X + (keyRect.Width / 2f), keyRect.Location.Y + (keyRect.Height / 2f)); //Sample center of key

                float keyHorVal = (keyCenterPos.X / kbWidth);

                if (immediateVolume >= keyHorVal)
                {
                    //Key should be fully lit
                    key.Led.Color = Utility.CalculateGradient(beatHigh, beatLow, DataProcessor.AveragedVolume, 1f);
                }
                else
                {
                    //Do whatever we do for unlit keys, which is currently nothing
                }
            }
        }
        #endregion

    }
}
