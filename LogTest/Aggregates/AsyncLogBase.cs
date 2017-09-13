using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LogTest.Services;
using LogTest.ValueObjects;

namespace LogTest.Aggregates
{
    public class AsyncLogBase : LogBase
    {
        private readonly List<LogLine> _lines = new List<LogLine>();

        private readonly BufferSize _bufferSize;
        private readonly LogWriteInterval _intervalMs;

        private bool _exit;
        private bool _quitWithFlush;
        public AsyncLogBase(ILogService logService, LogWriteInterval logWriteIntervalMs, BufferSize bufferSize) 
            : base(logService)
        {
            _intervalMs = logWriteIntervalMs;
            _bufferSize = bufferSize;
        }

        protected void AddLogLine(string text)
        {
            lock(_lines)
            { 
                _lines.Add(new LogLine(text));
            }
        }

        protected void WriteLog()
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

        protected void ForceStop()
        {
            Stop();
        }

        protected void FlushAndStop()
        {
            _quitWithFlush = true;
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
