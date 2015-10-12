using System;
using System.Threading;

namespace ArduinoAudioLevel
{
    public class ThreadedConsoleReader
    {
        Thread ConsoleThread;
        bool Closing = false;
        bool Started = false;
        public event EventHandler<ConsoleLineEventArgs> ConsoleLineRead;
        public ThreadedConsoleReader()
        {
        }
        public void Start()
        {
            if (Started) return;
            Started = true;
            ConsoleThread = new Thread(ConsoleReaderLoop);
            ConsoleThread.IsBackground = true;
            ConsoleThread.Start();
        }
        public void Stop()
        {
            if (!Started) return;
            Started = false;
            Closing = true;
            ConsoleThread.Abort();
        }
        private void ConsoleReaderLoop()
        {
            while (!Closing)
            {
                string line = Console.ReadLine();
                if (ConsoleLineRead != null)
                {
                    ConsoleLineEventArgs e = new ConsoleLineEventArgs(line);
                    ConsoleLineRead(this, e);
                }
            }
        }
    }
    public class ConsoleLineEventArgs : EventArgs
    {
        string _line;
        public string Line { get { return _line; } }
        public ConsoleLineEventArgs(string line)
        {
            _line = line;
        }
    }
}
