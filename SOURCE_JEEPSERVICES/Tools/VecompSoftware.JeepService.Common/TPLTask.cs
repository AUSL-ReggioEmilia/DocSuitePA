using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VecompSoftware.JeepService.Common
{
    public class TPLTask
    {
        public static Task<DateTimeOffset> Delay(int millisecondsTimeout)
        {
            TaskCompletionSource<DateTimeOffset> tcs = null;
            System.Threading.Timer timer = null;

            timer = new System.Threading.Timer(delegate
            {
                timer.Dispose();
                tcs.TrySetResult(DateTimeOffset.UtcNow);
            }, null, Timeout.Infinite, Timeout.Infinite);

            tcs = new TaskCompletionSource<DateTimeOffset>(timer);
            timer.Change(millisecondsTimeout, Timeout.Infinite);
            return tcs.Task;
        }
    }
}
