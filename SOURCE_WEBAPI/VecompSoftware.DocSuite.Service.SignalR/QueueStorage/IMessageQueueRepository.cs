namespace VecompSoftware.DocSuite.Service.SignalR
{
    /// <summary>
    /// Abstraction of a FIFO type of storage. 
    /// THis storage will contain the a queue list which is used to populate with messages from service bus.
    /// From this queue messages are sent with signalR to the client.
    /// <para>The RestoreOlder() functionality is used for the scenario of the client resuming after he disconected from
    /// this queue before all processes were complete. It will restore all messages so the client can have everything restored</para>
    /// </summary>
    /// <typeparam name="T">The type of message stored in the queue</typeparam>
    public interface IMessageQueueRepository<T>
    {
        T Dequeue();
        void Enqueue(T item);
        int Count { get; }
        void Clear();
        void RestoreOlder();
    }
}
