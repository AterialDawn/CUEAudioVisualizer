using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CUE.NET.Devices.Keyboard;

namespace CUEAudioVisualizer.Plugin
{
    public interface IPlugin
    {
        string Name { get; }
        IPluginHost Host { get; set; }
        VisualizerModes[] ModeList { get;}

        void OnModeActivated(VisualizerModes ActivatedMode);
        void OnModeDeactivated(VisualizerModes DeactivatedMode);
    }
}
