using System.Collections.Concurrent;
using System.Threading;

namespace VecompSoftware.DocSuite.Service.SignalR
{
    public static class WorkflowHubLock
    {
        //https://www.infoworld.com/article/3094774/my-two-cents-on-spinlock-in-net.html
        //SpinLock is better for simple operations
        private static SpinLock gate = new SpinLock();
        private static ConcurrentDictionary<string, object> _locks = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// Locks are made for each correlation id. We don't want to lock threads that handle
        /// a client that is not disconnected
        /// </summary>
        public static object GetLock(string correlationId)
        {
            bool lockTacken = false;

            try
            {
                gate.Enter(ref lockTacken);
                return _locks.GetOrAdd(correlationId, _ => new object());
            }
            finally
            {
                if (lockTacken)
                {
                    gate.Exit();
                }
            }
        }
    }
}