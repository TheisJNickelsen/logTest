
namespace LogTest.ValueObjects
{
    public class BufferSize
    {
        public int Value { get; }
        public BufferSize(int bufferSize)
        {
            //Can check for too low or too high values
            Value = bufferSize;
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
