using LogTest;
using LogTest.Repositories;
using LogTest.Services;
using LogTest.ValueObjects;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace LogComponent.Tests
{
    public class LogComponentTestBase : IDisposable
    {
        public Mock<ILogRepository> LogRepo;
        public Mock<ILogService> LogService;
        public List<LogLine> LogLines;
        public const int BufferSize = 2;

        protected LogComponentTestBase()
        {
            LogRepo = new Mock<ILogRepository>();
            LogService = new Mock<ILogService>();
            LogLines = new List<LogLine>();
        }

        public AsyncLog BuildAsyncLog()
        {
            return new AsyncLog(LogService.Object, new LogWriteInterval(0), new BufferSize(BufferSize));
        }

        public SynchronousLog BuildSynchronousLog()
        {
            return new SynchronousLog(LogService.Object);
        }

        public void Dispose()
        {
        }
    }
    public class AssyncLogTests : LogComponentTestBase
    {
        [Fact]  
        public void ShouldWriteToLogOnWrite()
        {
            var logger = BuildAsyncLog();
            var logMsg = "Test";
            var logTime = DateTime.Now;

            var logLine = new LogLine(logMsg, logTime);

            logger.Write(logMsg);

            LogService.Verify(mock => mock.Write(logLine), Times.Once);
        }

        // I was unable to control the asynchronous behavior for unit tests. See LogQueueDispatcherTest comments.
        [Fact]
        public void ShouldNotWriteOutstandingLogsOnStopWithoutFlush()
        {
            var logger = BuildAsyncLog();

            var logMsg = "Test";

            logger.Write(logMsg);
            logger.Write(logMsg);
            logger.Write(logMsg);
            logger.StopWithoutFlush();
            logger.Write(logMsg);
            logger.Write(logMsg);
            logger.Write(logMsg);

            LogService.Verify(mock => mock.Write(It.IsAny<LogLine>()), Times.Never);
        }

        [Fact]
        public void ShouldWriteOutstandingLogsOnStopWithFlush()
        {
            var logger = BuildAsyncLog();

            var logMsg = "Test";

            logger.Write(logMsg);
            logger.Write(logMsg);
            logger.StopWithFlush();
            logger.Write(logMsg);
            logger.Write(logMsg);
            logger.Write(logMsg);
            logger.Write(logMsg);

            LogService.Verify(mock => mock.Write(It.IsAny<LogLine>()), Times.Exactly(BufferSize));
        }
    }
}
