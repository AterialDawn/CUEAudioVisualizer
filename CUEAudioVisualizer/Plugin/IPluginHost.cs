using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using CUE.NET.Devices.Keyboard;

namespace CUEAudioVisualizer.Plugin
{
    public interface IPluginHost
    {
        /// <summary>
        /// Smoothed bar data, hardcoded to 1000 floats
        /// </summary>
        float[] SmoothedBarData { get; }

        /// <summary>
        /// Average volume over a few hundred milliseconds
        /// </summary>
        float AveragedVolume { get; }

        /// <summary>
        /// Immediate volume
        /// </summary>
        float ImmediateVolume { get; }

        /// <summary>
        /// Smoothed bassline beat
        /// </summary>
        float SongBeat { get; }

        /// <summary>
        /// Time since plugin host started, useful for time-dependant visualizations
        /// </summary>
        double Time { get; }

        /// <summary>
        /// Users set primary color, typically used for foreground or active key color
        /// </summary>
        Color PrimaryColor { get; }

        /// <summary>
        /// Users set secondary color, typically used as the background or inactive key color
        /// </summary>
        Color SecondaryColor { get; }

        /// <summary>
        /// CorsairKeyboard instance
        /// </summary>
        CorsairKeyboard Keyboard { get; }
    }
}
