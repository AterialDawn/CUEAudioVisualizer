using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using CUE.NET;
using CUE.NET.Devices.Generic.Enums;
using CUE.NET.Devices.Keyboard;
using CUE.NET.Devices.Keyboard.Keys;
using CUE.NET.Exceptions;
using CUEAudioVisualizer.Exceptions;
using CUEAudioVisualizer.Plugin;
using CUEAudioVisualizer.Plugins;

namespace CUEAudioVisualizer
{
    //This code was originally from a Console application so there's a bit of Console.WriteLines that I haven't bothered to clean up
    class KeyboardVisualizer : IPluginHost
    {
        #region IPluginHost interface implementation
        public float[] SmoothedBarData { get { return DataProcessor.BarValues; } }

        public float AveragedVolume { get { return DataProcessor.AveragedVolume; } }

        public float ImmediateVolume { get { return DataProcessor.ImmediateVolume; } }

        public float SongBeat { get { return DataProcessor.SongBeat; } }

        public double Time { get { return stopwatch.Elapsed.TotalSeconds; } }

        public Color PrimaryColor { get { return beatHigh; } }

        public Color SecondaryColor { get { return beatLow; } }

        public CorsairKeyboard Keyboard { get { return keyboard; } }
        #endregion
        public List<IPlugin> VisualizerPlugins { get; private set; }

        private CorsairKeyboard keyboard;
        private SoundDataProcessor DataProcessor;
        private Color beatLow = Color.FromArgb(150, 150, 255);
        private Color beatHigh = Color.FromArgb(0, 0, 255);
        private Stopwatch stopwatch = new Stopwatch();
        private VisualizerModes activeMode = null;

        public KeyboardVisualizer()
        {
            VisualizerPlugins = new List<IPlugin>();
            DataProcessor = new SoundDataProcessor(Properties.Settings.Default.DeviceIndex);
            LoadPlugins();
            stopwatch.Start();
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

        //Checks the Keyboard is not null, and hands the instance to any IPlugins
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
            keyboard = CueSDK.KeyboardSDK;
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
            DataProcessor.SmoothingScalar = Properties.Settings.Default.SmoothingModifier;

            TrySetActiveVisualizer();
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

            if (activeMode != null)
            {
                activeMode.UpdateDelegate();
            }

            Keyboard.UpdateLeds();
        }

        //Loads built-in plugins, and all plugins from Plugins folder, if it exists.
        private void LoadPlugins()
        {
            IPlugin SpectrumPlugin = new SpectrumPlugin();
            IPlugin VUPlugin = new VUPlugin();

            SpectrumPlugin.Host = this;
            VUPlugin.Host = this;

            VisualizerPlugins.Add(SpectrumPlugin);
            VisualizerPlugins.Add(VUPlugin);

            IPlugin[] plugins = PluginLoader.LoadPlugins();

            if (plugins == null) return;

            foreach (IPlugin currentPlugin in plugins)
            {
                currentPlugin.Host = this;
                VisualizerPlugins.Add(currentPlugin);
            }
        }

        //Tries to set the active visualizer from a stored settings key. Throws a ModeNotFoundException if the mode does not exist.
        private void TrySetActiveVisualizer()
        {
            string storedModeName = Properties.Settings.Default.VisualizerMode;
            foreach (IPlugin currentPlugin in VisualizerPlugins)
            {
                foreach (VisualizerModes currentMode in currentPlugin.ModeList)
                {
                    string currentModeKey = string.Format("{0},{1}", currentPlugin.Name, currentMode.ModeName);
                    if (storedModeName == currentModeKey)
                    {
                        //Found our saved mode, set as active mode
                        activeMode = currentMode;
                        return;
                    }
                }
            }

            if (storedModeName != "")
            {
                throw new ModeNotFoundException("Unable to find saved mode " + storedModeName);
            }
        }
        #endregion
    }
}
