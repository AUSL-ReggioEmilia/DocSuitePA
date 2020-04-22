using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Threading;
using log4net;

namespace BiblosDS.Cloud.WorkerRole
{
    public abstract class ThreadedRoleEntryPoint : RoleEntryPoint
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ThreadedRoleEntryPoint));
        private List<Thread> Threads = new List<Thread>();
        private WorkerEntryPoint[] Workers;
        protected EventWaitHandle EventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

        public override void Run()
        {
            try
            {
                _logger.InfoFormat("Run..");
                foreach (WorkerEntryPoint worker in Workers)
                    Threads.Add(new Thread(worker.ProtectedRun));

                foreach (Thread thread in Threads)
                    thread.Start();

                while (!EventWaitHandle.WaitOne(0))
                {
                    // WWB: Restart Dead Threads
                    for (Int32 i = 0; i < Threads.Count; i++)
                    {
                        if (!Threads[i].IsAlive)
                        {
                            Threads[i] = new Thread(Workers[i].Run);
                            Threads[i].Start();
                        }
                    }

                    EventWaitHandle.WaitOne(1000);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }           

        }

        public bool OnStart(WorkerEntryPoint[] workers)
        {
            try
            {
                _logger.Info("OnStart");
                this.Workers = workers;

                foreach (WorkerEntryPoint worker in workers)
                {
                    _logger.InfoFormat("OnStart: {0}" , worker.GetType().Name);
                    worker.OnStart();
                }

                return base.OnStart();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }           
        }

        public override bool OnStart()
        {
            try
            {
                _logger.Warn("Call invalid ONSTART");
                throw (new InvalidOperationException());
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
            
        }

        public override void OnStop()
        {
            EventWaitHandle.Set();

            foreach (Thread thread in Threads)
                while (thread.IsAlive)
                    thread.Abort();

            // WWB: Check To Make Sure The Threads Are
            // Not Running Before Continuing
            foreach (Thread thread in Threads)
                while (thread.IsAlive)
                    Thread.Sleep(10);

            // WWB: Tell The Workers To Stop Looping
            foreach (WorkerEntryPoint worker in Workers)
                worker.OnStop();

            base.OnStop();
        }
    }

}
