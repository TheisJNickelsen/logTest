
namespace LogTest.ValueObjects
{
    public class LogWriteInterval
    {
        public int Value { get; }
        public LogWriteInterval(int msValue)
        {
            //Can check for too low or too high values
            Value = msValue;
        }

        public override bool Equals(object obj)
        {
            var item = obj as LogWriteInterval;
            return item != null && item.Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
