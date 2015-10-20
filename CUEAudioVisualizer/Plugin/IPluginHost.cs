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
        float[] SmoothedBarData { get; }
        float AveragedVolume { get; }
        float ImmediateVolume { get; }
        float SongBeat { get; }
        double Time { get; }
        Color PrimaryColor { get; }
        Color SecondaryColor { get; }
        CorsairKeyboard Keyboard { get; }
    }
}
