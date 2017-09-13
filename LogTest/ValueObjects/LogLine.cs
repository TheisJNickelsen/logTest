using System;
using System.Text;

namespace LogTest.ValueObjects
{
    /// <summary>
    /// This is the object that the diff. loggers (filelogger, consolelogger etc.) will operate on. The LineText() method will be called to get the text (formatted) to log
    /// </summary>
    public class LogLine
    {
        #region Private Fields

        #endregion

        #region Constructors

        public LogLine(string text)
        {
            Text = text;
            Timestamp = DateTime.Now;
        }

        public LogLine(string text, DateTime timestamp)
        {
            Text = text;
            Timestamp = timestamp;
        }

        public LogLine(LogLine logLine)
        {
            Text = logLine.Text;
            Timestamp = logLine.Timestamp;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Return a formatted line
        /// </summary>
        /// <returns></returns>
        public virtual string LineText()
        {
            var sb = new StringBuilder();

            if (Text.Length > 0)
            {
                sb.Append(Text);
                sb.Append(". ");
            }

            sb.Append(CreateLineText());

            return sb.ToString();
        }

        public virtual string CreateLineText()
        {
            return "";
        }


        #endregion

        #region Properties

        /// <summary>
        /// The text to be display in logline
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// The Timestamp is initialized when the log is added. Th
        /// </summary>
        public DateTime Timestamp { get; }


        #endregion

        public override bool Equals(object obj)
        {
            var item = obj as LogLine;
            return item != null 
                && item.Text == Text
                && item.Timestamp == Timestamp;
        }

        public override int GetHashCode()
        {
            return (Text + Timestamp).GetHashCode();
        }
    }
}