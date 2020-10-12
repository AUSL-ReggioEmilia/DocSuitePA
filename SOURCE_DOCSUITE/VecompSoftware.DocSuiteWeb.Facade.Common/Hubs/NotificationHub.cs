using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using VecompSoftware.DocSuiteWeb.Facade.Common.Commons;

namespace VecompSoftware.DocSuiteWeb.Facade.Common.Hubs
{
    [HubName("notificationTickerCount")]
    public class NotificationHub : Hub
    {
        public void GetNotificationCounter(string data)
        {
            NotificationCounter counter = new NotificationCounter(this, Guid.Parse(data));
            counter.GetNotifications(Context.ConnectionId);
        }
    }
}
