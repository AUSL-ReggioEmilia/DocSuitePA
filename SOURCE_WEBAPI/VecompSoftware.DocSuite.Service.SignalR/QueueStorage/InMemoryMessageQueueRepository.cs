using System.Collections.Concurrent;
using System.Collections.Generic;

namespace VecompSoftware.DocSuite.Service.SignalR
{
    /// <summary>
    /// A default in-memory implementation of an <see cref="IMessageQueueRepository{T}"/>
    /// </summary>
    /// <typeparam name="T">The queue item type</typeparam>
    public class InMemoryMessageQueueRepository<T> : IMessageQueueRepository<T>
    {
        private readonly List<T> _allMessages;
        private readonly ConcurrentQueue<T> _messages;

        public InMemoryMessageQueueRepository()
        {
            _allMessages = new List<T>();
            _messages = new ConcurrentQueue<T>();
        }

        public int Count => _messages.Count;

        public void Clear()
        {
            _allMessages?.Clear();

            while (!_messages.IsEmpty)
            {
                _messages?.TryDequeue(out T _);
            }
        }

        /// <summary>
        /// Removes the first item from the queue
        /// </summary>
        /// <returns>Returns the removed item</returns>
        public T Dequeue()
        {
            _messages.TryDequeue(out T item);
            return item;
        }

        /// <summary>
        /// Adds one item on the top of the queue as the first item
        /// </summary>
        /// <param name="item">The item to be added</param>
        public void Enqueue(T item)
        {
            _allMessages.Add(item);
            _messages.Enqueue(item);
        }

        /// <summary>
        /// In the case of resume-subscription, a request which comes from the client,
        /// all messages which are saved in a sepparate list will be restored and the client will have
        /// all in the list
        /// </summary>
        public void RestoreOlder()
        {
            while (!_messages.IsEmpty)
            {
                _messages.TryDequeue(out T _);
            }

            foreach (T item in _allMessages)
            {
                _messages.Enqueue(item);
            }
        }
    }
}