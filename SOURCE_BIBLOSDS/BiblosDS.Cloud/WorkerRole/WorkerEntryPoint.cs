using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Cloud.WorkerRole
{
    public class WorkerEntryPoint
    {
        public virtual bool OnStart()
        {
            return (true);
        }

        /// <summary>
        /// This method prevents unhandled exceptions from being thrown
        /// from the worker thread.
        /// </summary>
        internal void ProtectedRun()
        {
            try
            {
                // Call the Workers Run() method
                Run();
            }
            catch (SystemException)
            {
                // Exit Quickly on a System Exception
                throw;
            }
            catch (Exception)
            {
            }
        }

        public virtual void Run()
        {
        }

        public virtual void OnStop()
        {
        }
    }

}
