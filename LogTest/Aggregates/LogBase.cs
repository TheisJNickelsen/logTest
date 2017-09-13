using System;
using LogTest.Services;
using LogTest.ValueObjects;

namespace LogTest.Aggregates
{
    public abstract class LogBase 
    {
        private readonly ILogService _logService;
        DateTime _curDate = SystemTime.Now();

        protected LogBase(ILogService logService)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
            _logService.CreateLog();
        }

        protected void WriteToLog(LogLine logLine)
        {
            if (HasPassedMidnight())
            {
                UpdateCurrentDate();
                _logService.CreateLog();
            }

            _logService.Write(logLine);
        }

        private bool HasPassedMidnight()
        {
            return SystemTime.Now().Day - _curDate.Day != 0;
        }
        private void UpdateCurrentDate()
        {
            _curDate = SystemTime.Now();
        }
    }
}
