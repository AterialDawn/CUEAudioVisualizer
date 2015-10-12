using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Un4seen.BassWasapi;

namespace CUEAudioVisualizer
{
    public partial class mainForm : Form
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

            SetCheckedVisualizerButton();
            AddValidWasapiDevices();
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
            visualizer.UpdateFromSettings();
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

        //Shows a balloon tip to let the user know to select a device
        private void mainForm_Load(object sender, EventArgs e)
        {
            
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
                    visualizer.UpdateFromSettings();
                }
            }
        }

        //Changes Vis mode to Spectrum
        private void spectrumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            vUToolStripMenuItem.Checked = false;
            spectrumToolStripMenuItem.Checked = true;
            visualizer.VisualizerMode = KeyboardVisualizerMode.Spectrum;
        }

        //Changes Vis mode to VU
        private void vUToolStripMenuItem_Click(object sender, EventArgs e)
        {
            vUToolStripMenuItem.Checked = true;
            spectrumToolStripMenuItem.Checked = false;
            visualizer.VisualizerMode = KeyboardVisualizerMode.VUMeter;
        }

        //Helper function to set checked visualizer button
        private void SetCheckedVisualizerButton()
        {
            if (Properties.Settings.Default.VisualizerMode == "Spectrum")
            {
                spectrumToolStripMenuItem.Checked = true;
            }
            else
            {
                vUToolStripMenuItem.Checked = true;
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
