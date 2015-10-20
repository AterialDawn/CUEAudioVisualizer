using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CUEAudioVisualizer.Plugin
{
    public class VisualizerModes
    {
        public delegate void UpdateKeyboardDelegate();

        public string ModeName { get; private set; }
        public UpdateKeyboardDelegate UpdateDelegate { get; private set; }

        public VisualizerModes(string ModeName, UpdateKeyboardDelegate UpdateDelegate)
        {
            this.ModeName = ModeName;
            this.UpdateDelegate = UpdateDelegate;
        }
    }
}
