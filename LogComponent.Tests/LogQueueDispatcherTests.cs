using LogTest;
using LogTest.Services;
using LogTest.ValueObjects;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace LogComponent.Tests
{
    public class LogQueueDispatcherTestBase : IDisposable
    {
        public Mock<ILogService> LogService;
        public const int BufferSize = 2;

        protected LogQueueDispatcherTestBase()
        {
            LogService = new Mock<ILogService>();
        }

        public LogQueueDispatcher BuildLogQueueDispatcher(List<LogLine> lines)
        {
            return new LogQueueDispatcher(LogService.Object, lines, new BufferSize(BufferSize), new LogWriteInterval(0));
        }

        public void Dispose()
        {
        }
    }

    // I was unable to control the asynchronous behavior of the dispatcher, thus the two last tests are unreliable.
    // A solution could be consider AsyncLog a process of its own. In that case,
    // I would create the service using TopShelf and Quartz, introduce a message bus (such as RabbitMQ), have it subscribe to 
    // WriteLog events and publish WriteLog events from Program.cs. That would abstract the asynchronous behavior away thus making
    // unit tests easier. In that case Start, Stop and FlushAndStop behavior would belong in an integration test. Writing to different 
    // files I would control using topics.
    public class LogQueueDispatcherTests : LogQueueDispatcherTestBase
    {
        [Fact]
        public void ShouldCreateNewLogFileWhenCrossingMidnight()
        {
            SystemTime.Now = () => new DateTime(2017, 9, 1, 23, 00, 00);
            var logLines = new List<LogLine>
            {
                new LogLine("Test"),
                new LogLine("Test"),
                new LogLine("Test"),
                new LogLine("Test")
            };
            var dispatcher = BuildLogQueueDispatcher(logLines);
            SystemTime.Now = () => new DateTime(2017, 9, 2, 00, 00, 00);
            dispatcher.Start();
            LogService.Verify(mock => mock.CreateLog(), Times.Exactly(2));
        }

        [Fact]
        public void ShouldNotWriteOutstandingLogsOnStopWithoutFlush()
        {
            var logLines = new List<LogLine>
            {
                new LogLine("Test"),
                new LogLine("Test"),
                new LogLine("Test"),
                new LogLine("Test")
            };
            var dispatcher = BuildLogQueueDispatcher(logLines);

            dispatcher.Start();
            dispatcher.ForceStop();

            LogService.Verify(mock => mock.Write(It.IsAny<LogLine>()), Times.Never);
        }

        [Fact]
        public void ShouldWriteOutstandingLogsOnStopWithFlush()
        {
            var logLines = new List<LogLine>
            {
                new LogLine("Test"),
                new LogLine("Test"),
                new LogLine("Test"),
                new LogLine("Test")
            };
            var dispatcher = BuildLogQueueDispatcher(logLines);

            dispatcher.Start();
            dispatcher.FlushAndStop();

            LogService.Verify(mock => mock.Write(It.IsAny<LogLine>()), Times.Exactly(BufferSize));

        }


    }
}
