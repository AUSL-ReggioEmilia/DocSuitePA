using System;
using System.ServiceModel;
using System.Threading;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.DocSuiteWeb.Services.WSSeries.Helpers
{
    public class ActionHelper
    {
        public static T ImpersonatedAction<T>(Func<T> func)
        {
            FileLogger.Info(LogName.FileLog, $"Impersonate user from {DocSuiteContext.Current.User.FullUserName} to {ServiceSecurityContext.Current.WindowsIdentity.Name}");
            using (ServiceSecurityContext.Current.WindowsIdentity.Impersonate())
            using (ExecutionContext.SuppressFlow())
            {
                return func();
            }
        }
    }
}