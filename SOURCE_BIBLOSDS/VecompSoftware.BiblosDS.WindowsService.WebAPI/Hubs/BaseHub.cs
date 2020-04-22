using log4net;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.BiblosDS.WindowsService.WebAPI.Helpers;

namespace VecompSoftware.BiblosDS.WindowsService.WebAPI.Hubs
{
    public abstract class BaseHub : Hub
    {
        #region [ Fields ]
        private static readonly ILog _logger = LogManager.GetLogger(typeof(BaseHub));        
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]
        public override Task OnConnected()
        {
            string correlationId = Context.QueryString["correlationId"];
            _logger.Debug($"Connect with ConnectionId {Context.ConnectionId} with CorrelationId {correlationId}");
            if (!ConnectionShared.Connections.TryAdd(Context.ConnectionId, correlationId))
            {
                _logger.Warn($"Concurrent Exception in TryAdd ConnectionId {Context.ConnectionId} and CollerationId {correlationId}");
                throw new ArgumentException("WebSocket connection failed");
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (!ConnectionShared.Connections.TryRemove(Context.ConnectionId, out string correlationId))
            {
                _logger.Warn($"Concurrent Exception in TryRemove ConnectionId {Context.ConnectionId}");
            }
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            string correlationId = Context.QueryString["correlationId"];
            if (!ConnectionShared.Connections.ContainsKey(Context.ConnectionId))
            {
                if (!ConnectionShared.Connections.TryAdd(Context.ConnectionId, correlationId))
                {
                    _logger.Warn($"Concurrent Exception in TryAdd ConnectionId {Context.ConnectionId} and CollerationId {correlationId}");
                    throw new ArgumentException("WebSocket connection failed");
                }
            }
            return base.OnReconnected();
        }
        #endregion
    }
}
