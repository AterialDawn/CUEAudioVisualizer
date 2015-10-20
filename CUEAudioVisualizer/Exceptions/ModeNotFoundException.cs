using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CUEAudioVisualizer.Exceptions
{
    class ModeNotFoundException : Exception
    {
        public ModeNotFoundException() : base() { }
        public ModeNotFoundException(string message) : base(message) { }
        public ModeNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
