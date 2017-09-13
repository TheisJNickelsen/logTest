using System.Threading;
using LogTest.Aggregates;
using LogTest.Services;
using LogTest.ValueObjects;

namespace LogTest
{

    public class AsyncLog : AsyncLogBase, ILog
    {
        public AsyncLog(ILogService logService) 
            : base(logService, new LogWriteInterval(50), new BufferSize(5))
        {
            Start();
        }
        public AsyncLog(ILogService logService, LogWriteInterval logWriteInterval, BufferSize bufferSize)
            : base(logService, logWriteInterval, bufferSize)
        {
            Start();
        }

        private void Start()
        {
            var runThread = new Thread(WriteLog);
            runThread.Start();
        }

        public void StopWithoutFlush()
        {
            ForceStop();
        }

        public void StopWithFlush()
        {
            FlushAndStop();
        }

        public void Write(string text)
        {
            AddLogLine(text);
        }
    }
}