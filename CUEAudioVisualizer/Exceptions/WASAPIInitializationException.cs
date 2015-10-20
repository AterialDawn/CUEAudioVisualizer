using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Un4seen.Bass;

namespace CUEAudioVisualizer.Exceptions
{
    class WASAPIInitializationException : Exception
    {
        public BASSError? OptionalError { get; private set; }
        public WASAPIInitializationException() : base() { OptionalError = null; }
        public WASAPIInitializationException(BASSError error) : base() { OptionalError = error; }
        public WASAPIInitializationException(string message) : base(message) { OptionalError = null; }
        public WASAPIInitializationException(string message, BASSError error) : base(message) { OptionalError = error; }
        public WASAPIInitializationException(string message, Exception innerException) : base(message, innerException) { OptionalError = null; }
        public WASAPIInitializationException(string message, Exception innerException, BASSError error) : base(message, innerException) { OptionalError = error; }
    }
}
