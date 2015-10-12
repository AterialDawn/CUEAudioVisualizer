using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArduinoAudioLevel;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace CUEAudioVisualizer
{
    class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            BassNet.Registration("trial@trial.com", "2X1837515183722");
            BassNet.OmitCheckVersion = true;
            if (!Bass.LoadMe())
            {
                System.Windows.Forms.MessageBox.Show("Unable to load bass.dll!", "Error", System.Windows.Forms.MessageBoxButtons.OK);
                return;
            }
            if (!Bass.BASS_Init(0, 48000, 0, IntPtr.Zero))
            {
                System.Windows.Forms.MessageBox.Show("Unable to initialize the BASS library!", "Error", System.Windows.Forms.MessageBoxButtons.OK);
            }
            if (!BassWasapi.LoadMe())
            {
                System.Windows.Forms.MessageBox.Show("Unable to load BassWasapi.dll!", "Error", System.Windows.Forms.MessageBoxButtons.OK);
            }
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 0);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            mainForm mainForm = new mainForm();

            Application.Run(mainForm);
        }
    }
}
