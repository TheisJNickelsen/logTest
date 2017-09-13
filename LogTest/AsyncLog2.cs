using LogTest.ValueObjects;
using System;
using System.Collections.Generic;

namespace LogTest
{

    public class AsyncLog2 : ILog
    {
        private readonly ILogQueueDispatcher _logQueueDispatcher;
        private readonly List<LogLine> _lines;

        public AsyncLog2(ILogQueueDispatcher logQueueDispatcher, List<LogLine> lines)
        {
            _logQueueDispatcher = logQueueDispatcher ?? throw new ArgumentNullException(nameof(logQueueDispatcher));
            _lines = lines ?? throw new ArgumentNullException(nameof(lines));
            _logQueueDispatcher.Start();
        }

        public void StopWithoutFlush()
        {
            _logQueueDispatcher.ForceStop();
        }

        public void StopWithFlush()
        {
            _logQueueDispatcher.FlushAndStop();
        }

        public void Write(string text)
        {
            lock (_lines)
            {
                _lines.Add(new LogLine(text));
            }
        }
    }
}