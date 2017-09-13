using System;

namespace LogTest
{
    //This has been created to control system time for unit testing
    public static class SystemTime
    {
        public static Func<DateTime> Now = () => DateTime.Now;
    }
}
