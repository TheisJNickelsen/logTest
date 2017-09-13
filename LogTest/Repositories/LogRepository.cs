using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LogTest.ValueObjects;

namespace LogTest.Repositories
{
    public class LogRepository : ILogRepository
    {
        private StreamWriter _writer;

        public void CreateLogFile()
        {
            if (!Directory.Exists(@"C:\LogTest"))
                Directory.CreateDirectory(@"C:\LogTest");

            _writer = File.AppendText(@"C:\LogTest\Log" + DateTime.Now.ToString("yyyyMMdd HHmmss fff") + ".log");

            _writer.Write("Timestamp".PadRight(25, ' ') + "\t" + "Data".PadRight(15, ' ') + "\t" + Environment.NewLine);

            _writer.AutoFlush = true;
        }

        public void WriteToFile(LogLine logLine)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(logLine.Timestamp.ToString("yyyy-MM-dd HH:mm:ss:fff"));
            stringBuilder.Append("\t");
            stringBuilder.Append(logLine.LineText());
            stringBuilder.Append("\t");

            stringBuilder.Append(Environment.NewLine);

            _writer.Write(stringBuilder.ToString());
        }
    }
}
