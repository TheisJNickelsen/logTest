using System;
using LogTest.Aggregates;
using LogTest.Services;
using LogTest.ValueObjects;

namespace LogTest
{
    public class SynchronousLog : LogBase
    {
        public SynchronousLog(ILogService logger) : base(logger)
        {
        }

        public void Write(string text)
        {
            WriteToLog(new LogLine(text));
        }
    }
}
