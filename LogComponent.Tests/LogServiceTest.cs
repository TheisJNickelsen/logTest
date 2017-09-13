using System;
using LogTest.Repositories;
using LogTest.Services;
using LogTest.ValueObjects;
using Moq;
using Xunit;

namespace LogComponent.Tests
{
    public class LogServiceTestBase
    {
        //Repository tests would require a read method, thus it has been omitted.
        public Mock<ILogRepository> LogRepo;

        public LogService BuildLogService()
        {
            LogRepo = new Mock<ILogRepository>();
            return new LogService(LogRepo.Object);
        }
    }

    public class LogServiceTest : LogServiceTestBase
    {
        [Fact]
        public void ShouldWriteToLogWhenCallingWrite()
        {
            var logService = BuildLogService();

            var logMsg = "test";
            logService.Write(new LogLine("test"));

            LogRepo.Verify(mock => mock.WriteToFile(It.Is<LogLine>(l => l.Text.Equals(logMsg))), Times.Once);
        }
    }
}
