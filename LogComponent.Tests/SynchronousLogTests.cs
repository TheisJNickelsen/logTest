using System;
using LogTest;
using Moq;
using Xunit;

namespace LogComponent.Tests
{
    public class SynchronousLogTests : LogComponentTestBase
    {
        // Synchronous logger was made first to test this behavior. 

        //This test revealed error in how 'Past midnight' is determined. 
        //(now - _curDate).Days != 0   to      SystemTime.Now().Day - _curDate.Day != 0;
        [Fact]
        public void ShouldCreateNewLogFileWhenCrossingMidnight()
        {
            SystemTime.Now = () => new DateTime(2017, 9, 1, 23, 00, 00);
            var logger = BuildSynchronousLog();
            logger.Write("Test1");

            SystemTime.Now = () => new DateTime(2017, 9, 2, 00, 00, 00);
            logger.Write("Test2");

            LogService.Verify(mock => mock.CreateLog(), Times.Exactly(2));
        }
    }
}
