using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CUE.NET.Devices.Keyboard;

namespace CUEAudioVisualizer.Plugin
{
    public interface IPlugin
    {
        /// <summary>
        /// Plugin name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Plugin host instance, set by the host after plugin has been instantiated
        /// </summary>
        IPluginHost Host { get; set; }

        /// <summary>
        /// List of modes the plugin has, must have at least 1 mode
        /// </summary>
        VisualizerModes[] ModeList { get;}
    }
}
