using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogTest.Repositories;
using LogTest.ValueObjects;

namespace LogTest.Services
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;

        public LogService(ILogRepository logRepository)
        {
            _logRepository = logRepository ?? throw new ArgumentNullException(nameof(logRepository));
        }
        public void Write(LogLine logLine)
        {
            _logRepository.WriteToFile(logLine);
        }

        public void CreateLog()
        {
            _logRepository.CreateLogFile();
        }
    }
}
