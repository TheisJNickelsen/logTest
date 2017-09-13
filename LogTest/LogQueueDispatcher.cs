using LogTest.Aggregates;
using LogTest.Services;
using LogTest.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LogTest
{
    public class LogQueueDispatcher : LogBase, ILogQueueDispatcher
    {
        private readonly List<LogLine> _lines;

        private readonly BufferSize _bufferSize;
        private readonly LogWriteInterval _intervalMs;

        private bool _exit;
        private bool _quitWithFlush;

        public LogQueueDispatcher(ILogService logService, List<LogLine> lines, BufferSize bufferSize, LogWriteInterval intervalMs) : base(logService)
        {
            _lines = lines ?? throw new ArgumentNullException(nameof(lines));
            _bufferSize = bufferSize;
            _intervalMs = intervalMs;
        }

        public void Start()
        {
            Thread thread = new Thread(MessageLoop);
            thread.Start();
        }

        public void ForceStop()
        {
            Stop();
        }

        public void FlushAndStop()
        {
            _quitWithFlush = true;
        }

        private void MessageLoop()
        {
            while (!IsStopped())
            {
                if (NoLogLines()) continue;

                var linesCopy = CopyLinesToBuffer();

                foreach (var logLine in linesCopy)
                {
                    if (IsStopped() && !WriteOutstandingLogLines())
                        continue;

                    WriteToLog(logLine);
                }

                if (IsDoneFlusingLogs())
                    Stop();

                Thread.Sleep(_intervalMs.Value);
            }

        }

        private List<LogLine> CopyLinesToBuffer()
        {
            List<LogLine> linesCopy = new List<LogLine>();
            lock (_lines)
            {
                var toRemove = new List<LogLine>();
                foreach (var line in _lines.Take(_bufferSize.Value))
                {
                    linesCopy.Add(new LogLine(line));
                    toRemove.Add(line);
                }
                _lines.RemoveAll(x => toRemove.Contains(x));
            }
            return linesCopy;
        }

        private bool WriteOutstandingLogLines()
        {
            return _quitWithFlush;
        }

        private bool IsStopped()
        {
            return _exit;
        }

        private void Stop()
        {
            _exit = true;
        }

        private bool NoLogLines()
        {
            return _lines.Count <= 0;
        }

        private bool IsDoneFlusingLogs()
        {
            return _quitWithFlush && _lines.Count == 0;
        }

    }

}
