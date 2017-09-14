using LogTest.Repositories;
using LogTest.Services;
using System;

namespace LogUsers
{
    using LogTest;
    using System.Threading;

    /* I came up with two solutions trying to find a solution for controlling asynchronous behavior for unit tests.
     * I failed in doing so resulting in unreliable tests. Both solutions are equally good in that sense.
     * Solution 1: Encapsulates the functionality in a single class AsyncLog.
     * Solution 2: AsyncLog owns a LogQueueDispatcher which handles aynchronous behavior.  
     * 
     * Errors found:
     *   - The way the code calculates when midnight is crossed was wrong. 
     *   - An occasional runtime exception was thrown, when write and read to logline list happend at the same time. This has been fixed
     *     using locking of the loglines list.
     *     
     * Another solution would be to make AsyncLog a process of it's own, introduce a bus (like RabbitMQ) and have it subscripe 
     * to WriteLog events. The program below would then publish LogWrite events. This would abstract queue, start, stop and flush functionality 
     * away from the code and unit tests and into integration tests instead. 
     * Writing to different files would be handled using topics.
     * 
     * See more comments in test files.
     */

    class Program
    {
        static void Main(string[] args)
        {
            //Preferably DI / IOC Container setup
            
            // Solution 1
            ILogRepository logRepository = new LogRepository();
            ILogService logService = new LogService(logRepository);
            ILog  logger = new AsyncLog(logService);

            for (int i = 0; i < 15; i++)
            {
                logger.Write("Number with Flush: " + i);
                Thread.Sleep(50);
            }

            logger.StopWithFlush();

            ILogRepository logRepository2 = new LogRepository();
            ILogService logService2 = new LogService(logRepository2);
            ILog logger2 = new AsyncLog(logService2);

            for (int i = 50; i > 0; i--)
            {
                logger2.Write("Number with No flush: " + i);
                Thread.Sleep(20);
            }

            logger2.StopWithoutFlush();

            Console.ReadLine();


            // Solution 2
            /*
            var logLines = new List<LogLine>();
            var logRepository = new LogRepository();
            var logService = new LogService(logRepository); 
            var logQueueDispatcher = new LogQueueDispatcher(logService, logLines, new BufferSize(5), new LogWriteInterval(50));

            ILog logger = new AsyncLog2(logQueueDispatcher, logLines);

            for (int i = 0; i < 15; i++)
            {
                logger.Write("Number with Flush: " + i);
                Thread.Sleep(50);
            }

            logger.StopWithFlush();


            var logLines2 = new List<LogLine>();
            var logRepository2 = new LogRepository();
            var logService2 = new LogService(logRepository2);
            var logQueueDispatcher2 = new LogQueueDispatcher(logService2, logLines2, new BufferSize(5), new LogWriteInterval(50));
            ILog logger2 = new AsyncLog2(logQueueDispatcher2, logLines2);

            for (int i = 50; i > 0; i--)
            {
                logger2.Write("Number with No flush: " + i);
                Thread.Sleep(20);
            }

            logger2.StopWithoutFlush();

            Console.ReadLine();
             */
        }
    }
}
