using System;

namespace VecompSoftware.DocSuiteWeb.Service.ServiceBus
{
    public interface IServiceBusConfiguration
    {
        string ConnectionString { get; set; }

        TimeSpan TimeToLive { get; set; }

        TimeSpan AutoDeleteOnIdle { get; set; }

        TimeSpan DefaultMessageTimeToLive { get; set; }

        TimeSpan LockDuration { get; set; }

        int MaxDeliveryCount { get; set; }

        TimeSpan AutoRenewTimeout { get; set; }

        TimeSpan? OperationTimeout { get; set; }

        TimeSpan ServerWaitTime { get; set; }

        int MaximumMessageSize { get; set; }

        int MaximumQueueSize { get; set; }
    }
}
