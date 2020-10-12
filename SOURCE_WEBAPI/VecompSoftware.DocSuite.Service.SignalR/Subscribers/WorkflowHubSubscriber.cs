using System;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;

namespace VecompSoftware.DocSuite.Service.SignalR
{
    /// <summary>
    /// This class will issue signal R message push to clients
    /// </summary>
    public class WorkflowHubSubscriber
    {
        private readonly Action<ServiceBusMessage> _statusDone;
        private readonly Action<ServiceBusMessage> _statusError;
        private readonly Action<ServiceBusMessage> _notificationInfo;
        private readonly Action<ServiceBusMessage> _notificationInfoModel;
        private readonly Action<ServiceBusMessage> _notificationWarning;
        private readonly Action<ServiceBusMessage> _notificationError;

        public WorkflowHubSubscriber(
            Action<ServiceBusMessage> statusDone,
            Action<ServiceBusMessage> statusError,
            Action<ServiceBusMessage> notificationInfo,
            Action<ServiceBusMessage> notificationInfoModel,
            Action<ServiceBusMessage> notificationWarning,
            Action<ServiceBusMessage> notificationError)
        {
            _statusDone = statusDone;
            _statusError = statusError;
            _notificationInfo = notificationInfo;
            _notificationInfoModel = notificationInfoModel;
            _notificationWarning = notificationWarning;
            _notificationError = notificationError;
        }

        public void SendResponseStatusDone(ServiceBusMessage result) => _statusDone(result);
        public void SendResponseStatusError(ServiceBusMessage result) => _statusError(result);
        public void SendResponseNotificationInfo(ServiceBusMessage result) => _notificationInfo(result);
        public void SendResponseNotificationInfoModel(ServiceBusMessage result) => _notificationInfoModel(result);
        public void SendResponseNotificationWarning(ServiceBusMessage result) => _notificationWarning(result);
        public void SendResponseNotificationError(ServiceBusMessage result) => _notificationError(result);
    }
}