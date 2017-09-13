namespace LogTest
{
    public interface ILogQueueDispatcher
    {
        void Start();
        void ForceStop();
        void FlushAndStop();
    }
}
