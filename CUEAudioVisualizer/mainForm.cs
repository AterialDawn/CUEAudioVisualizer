using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CUEAudioVisualizer.Exceptions;
using CUEAudioVisualizer.Plugin;
using Un4seen.BassWasapi;

namespace CUEAudioVisualizer
{
    partial class mainForm : Form
    {
        private bool IsVisible = false;
        private KeyboardVisualizer visualizer;
        private bool IsExiting = false;

        public mainForm()
        {
            InitializeComponent();
            mainNotifyIcon.Icon = this.Icon;

            visualizer = new KeyboardVisualizer();
            if (!visualizer.InitKeyboard())
            {
                System.Windows.Forms.MessageBox.Show("Unable to initialize CUE support!", "Error", System.Windows.Forms.MessageBoxButtons.OK);
                Application.Exit();
            }
            try
            {
                visualizer.UpdateFromSettings();
            }
            catch (ModeNotFoundException)
            {
                //On a ModeNotFoundException, reset the default mode to "Spectrum,Spectrum". This mode is built-in, and should never not be found
                Properties.Settings.Default.VisualizerMode = "Spectrum,Spectrum";
                visualizer.UpdateFromSettings();
            }
            AddValidWasapiDevices();
            AddVisualizerModes();
            ShowNotifyIcon();
        }

        #region Event Handlers
        //Handles clicking of a Device Selection subitem
        void currentDeviceItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedItem = sender as ToolStripMenuItem;
            if (clickedItem == null) return;

            int deviceId = (int)clickedItem.Tag;

            if (deviceId == -2)
            {
                AddValidWasapiDevices();
                return;
            }

            //Uncheck all items, and check the clicked item
            foreach (ToolStripMenuItem item in deviceSelectionToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }

            clickedItem.Checked = true;

            //Update settings and notify visualizer
            Properties.Settings.Default.DeviceIndex = deviceId;
            Properties.Settings.Default.Save();
            visualizer.UpdateFromSettings();
        }

        void ChangeVisualizerModeItemClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedItem = sender as ToolStripMenuItem;
            if (clickedItem == null) return;

            string modeString = (string)clickedItem.Tag;

            Properties.Settings.Default.VisualizerMode = modeString;

            UncheckAllVisualizerModeItems();

            clickedItem.Checked = true;

            try
            {
                visualizer.UpdateFromSettings();
            }
            catch (ModeNotFoundException)
            {
                //This shouldn't happen...
                //On a ModeNotFoundException, reset the default mode to "Spectrum,Spectrum". This mode is built-in, and should never not be found
                Properties.Settings.Default.VisualizerMode = "Spectrum,Spectrum";
                visualizer.UpdateFromSettings();
            }
            Properties.Settings.Default.Save();
        }

        //Updates the keyboard visualizer, and if we lose Exclusive Control, shuts the program down
        private void visUpdateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                visualizer.UpdateKeyboard();
            }
            catch (ApplicationException exc)
            {
                if (exc.Message == "We lost Exclusive Control!")
                {
                    visUpdateTimer.Enabled = false;
                    MessageBox.Show("Another program is controlling keyboard lighting! Shutting down.");
                    Application.Exit();
                }
                else throw;
            }
            catch (WASAPIInitializationException exc)
            {
                //Unable to initialize specified wasapi device! Set the device back to -1, and show the user a message saying so.
                int oldDeviceIndex = Properties.Settings.Default.DeviceIndex;
                Properties.Settings.Default.DeviceIndex = -1;
                visualizer.UpdateFromSettings();
                mainNotifyIcon.ShowBalloonTip(5000, "Unable to initialize device " + oldDeviceIndex.ToString(), string.Format("{0} BASS Error is : {1}", exc.Message, exc.OptionalError.HasValue ? exc.OptionalError.Value.ToString() : "No BASS error!"), ToolTipIcon.Error);
            }
        }

        //Prevents the form from closing unless the Exit button was clicked first
        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.WindowsShutDown)
            {
                if (!IsExiting)
                {
                    e.Cancel = true;
                }
            }
        }

        //Closes the form
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IsExiting = true;
            Application.Exit();
        }

        //Handles the changing of the Primary Color
        private void primaryColorMenuItem_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //Update our primary color
                    Color chosenColor = colorDialog.Color;
                    primaryColorMenuItem.BackColor = chosenColor;
                    Properties.Settings.Default.PrimaryColor = chosenColor;
                    Properties.Settings.Default.Save();
                    visualizer.UpdateFromSettings();
                }
            }
        }

        //Handles the changing of the secondary color
        private void secondaryColorMenuItem_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //Update our primary color
                    Color chosenColor = colorDialog.Color;
                    secondaryColorMenuItem.BackColor = chosenColor;
                    Properties.Settings.Default.SecondaryColor = chosenColor;
                    Properties.Settings.Default.Save();
                    visualizer.UpdateFromSettings();
                }
            }
        }

        #endregion

        #region Utility/Helper functions
        //Lets the form stay invisible
        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(IsVisible ? value : IsVisible);
        }

        //Adds all valid detected WASAPI devices to the Device Selection menu item
        private void AddValidWasapiDevices()
        {
            deviceSelectionToolStripMenuItem.DropDownItems.Clear();
            BASS_WASAPI_DEVICEINFO[] devices = BassWasapi.BASS_WASAPI_GetDeviceInfos();
            List<string> deviceNameList = new List<string>();
            for (int i = 0; i < devices.Length; i++)
            {
                var curDevice = devices[i];
                if (curDevice.IsEnabled && curDevice.SupportsRecording)
                {
                    string deviceName = curDevice.name;
                    if (curDevice.IsLoopback)
                    {
                        deviceName = deviceName + " (Loopback)";
                    }
                    if (deviceNameList.Contains(curDevice.name))
                    {
                        deviceName = string.Format("{0} ({1})", curDevice.name, i);
                    }
                    deviceNameList.Add(deviceName);

                    if (Properties.Settings.Default.DeviceIndex == i)
                    {
                        AddDeviceToMenuItem(deviceName, i, true);
                    }
                    else
                    {
                        AddDeviceToMenuItem(deviceName, i, false);
                    }
                }
            }

            AddDeviceToMenuItem("Reload Devices...", -2, false);
        }

        private void AddVisualizerModes()
        {
            //Add all loaded visualizers into the ContextMenuStrip and makes them selectable
            visualizerModeToolStripMenuItem.DropDownItems.Clear();
            foreach (IPlugin currentPlugin in visualizer.VisualizerPlugins)
            {
                //If the plugin has no modes, consider it a bugged plugin and skip it
                if (currentPlugin.ModeList == null || currentPlugin.ModeList.Length == 0)
                {
                    continue;
                }
                AddPluginToMenuItem(currentPlugin);
            }
        }

        //Builds a ToolStripMenuItem from a WASAPI device string/id
        private void AddDeviceToMenuItem(string deviceName, int deviceId, bool setChecked)
        {
            ToolStripMenuItem currentDeviceItem = new ToolStripMenuItem();
            currentDeviceItem.Name = "Device " + deviceName;
            currentDeviceItem.Tag = deviceId;
            currentDeviceItem.Text = deviceName;
            currentDeviceItem.Checked = setChecked;
            currentDeviceItem.Click += currentDeviceItem_Click;
            deviceSelectionToolStripMenuItem.DropDownItems.Add(currentDeviceItem);
        }

        private void AddPluginToMenuItem(IPlugin plugin)
        {
            string storedMode = Properties.Settings.Default.VisualizerMode;

            ToolStripMenuItem currentPluginItem = new ToolStripMenuItem();
            currentPluginItem.Name = "Plugin " + plugin.Name;
            if (plugin.ModeList.Length == 1)
            {
                //The main item will be clickable
                currentPluginItem.Text = plugin.ModeList[0].ModeName;
                string currentModeTag = string.Format("{0},{1}", plugin.Name, plugin.ModeList[0].ModeName);
                currentPluginItem.Tag = currentModeTag;
                currentPluginItem.Checked = currentModeTag == storedMode;
                currentPluginItem.Click += ChangeVisualizerModeItemClicked;
            }
            else
            {
                //The main item will not be clickable, only it's sub-items.
                currentPluginItem.Text = plugin.Name;
                foreach (VisualizerModes currentMode in plugin.ModeList)
                {
                    ToolStripMenuItem currentModeItem = new ToolStripMenuItem();
                    currentModeItem.Name = "Plugin " + plugin.Name + ", Mode " + currentMode.ModeName;
                    currentModeItem.Text = currentMode.ModeName;
                    string currentModeTag = string.Format("{0},{1}", plugin.Name, currentMode.ModeName);
                    currentModeItem.Tag = currentModeTag;
                    currentModeItem.Checked = currentModeTag == storedMode;
                    currentModeItem.Click += ChangeVisualizerModeItemClicked;

                    currentPluginItem.DropDownItems.Add(currentModeItem);
                }
            }

            visualizerModeToolStripMenuItem.DropDownItems.Add(currentPluginItem);
        }

        private void UncheckAllVisualizerModeItems()
        {
            foreach (ToolStripMenuItem currentItem in visualizerModeToolStripMenuItem.DropDownItems)
            {
                currentItem.Checked = false;
                foreach (ToolStripMenuItem currentSubItem in currentItem.DropDownItems)
                {
                    currentSubItem.Checked = false;
                }
            }
        }

        //Shows a notify icon if no device is selected
        private void ShowNotifyIcon()
        {
            if (Properties.Settings.Default.DeviceIndex == -1)
            {
                mainNotifyIcon.ShowBalloonTip(5000, "No Device Selected", "Select a Device to start visualizing!", ToolTipIcon.Info);
            }
        }
        #endregion
    }
}
