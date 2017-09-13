using LogTest;
using LogTest.ValueObjects;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace LogComponent.Tests
{
    public class AsyncLog2TestBase : IDisposable
    {
        public Mock<ILogQueueDispatcher> LogQueueDispatcher;
        public List<LogLine> LogLines;

        protected AsyncLog2TestBase()
        {
            LogQueueDispatcher = new Mock<ILogQueueDispatcher>();
            LogLines = new List<LogLine>();
        }
        public AsyncLog2 BuildAsyncLog2()
        {
            return new AsyncLog2(LogQueueDispatcher.Object, LogLines);
        }

        public void Dispose()
        {
        }
    }


    public class AssyncLog2Tests : AsyncLog2TestBase
    {
        [Fact]
        public void ShouldQueueLogsOnWrite()
        {
            var logger = BuildAsyncLog2();
            logger.Write("Test");
            Assert.True(LogLines.Count == 1);
        }

        [Fact]
        public void ShouldForceStopDispatcherOnStopWithoutFlush()
        {
            var logger = BuildAsyncLog2();
            logger.StopWithoutFlush();
            LogQueueDispatcher.Verify(mock => mock.ForceStop(), Times.Once);
        }

        [Fact]
        public void ShouldWriteOutstandingLogsOnStopWithFlush()
        {
            var logger = BuildAsyncLog2();
            logger.StopWithFlush();
            LogQueueDispatcher.Verify(mock => mock.FlushAndStop(), Times.Once);
        }
    }

}
