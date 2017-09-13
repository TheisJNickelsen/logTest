using System;
using LogTest.ValueObjects;

namespace LogTest.Services
{
    public interface ILogService
    {
        void Write(LogLine logLine);
        void CreateLog();
    }
}
