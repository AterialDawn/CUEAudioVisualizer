using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CUEAudioVisualizer.Plugin
{
    public class VisualizerModes
    {
        /// <summary>
        /// Delegate for updating the keyboard
        /// </summary>
        public delegate void UpdateKeyboardDelegate();

        /// <summary>
        /// Name of the mode. This is what's shown to the user in the toolstrip menu
        /// </summary>
        public string ModeName { get; private set; }

        /// <summary>
        /// The UpdateKeyboardDelegate associated with this mode
        /// </summary>
        public UpdateKeyboardDelegate UpdateDelegate { get; private set; }

        /// <summary>
        /// Class to indicate that a plugin supports a certain visualizer mode
        /// </summary>
        /// <param name="ModeName">The mode name to display to the user</param>
        /// <param name="UpdateDelegate">The UpdateKeyboardDelegate to call when the keyboard is to be updated</param>
        public VisualizerModes(string ModeName, UpdateKeyboardDelegate UpdateDelegate)
        {
            this.ModeName = ModeName;
            this.UpdateDelegate = UpdateDelegate;
        }
    }
}
