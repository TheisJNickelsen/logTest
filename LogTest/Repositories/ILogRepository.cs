using LogTest.ValueObjects;

namespace LogTest.Repositories
{
    public interface ILogRepository
    {
        void CreateLogFile();
        void WriteToFile(LogLine logLine);
    }
}
