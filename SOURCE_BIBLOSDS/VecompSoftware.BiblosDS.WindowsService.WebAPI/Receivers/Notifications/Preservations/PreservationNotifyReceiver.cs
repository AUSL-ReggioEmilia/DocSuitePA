using log4net;
using Microsoft.AspNet.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.BiblosDS.Model.CQRS;
using VecompSoftware.BiblosDS.Model.CQRS.Notifications.Preservations;
using VecompSoftware.BiblosDS.WindowsService.WebAPI.Helpers;
using VecompSoftware.BiblosDS.WindowsService.WebAPI.Hubs;

namespace VecompSoftware.BiblosDS.WindowsService.WebAPI.Receivers.Notifications.Preservations
{
    public class PreservationNotifyReceiver : Receiver
    {
        #region [ Fields ]
        private static readonly ILog _logger = LogManager.GetLogger(typeof(PreservationNotifyReceiver));
        private readonly IHubContext _hubContext;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public PreservationNotifyReceiver(IReceiverMediator mediator)
            : base(mediator)
        {
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<PreservationHub>();
        }
        #endregion

        #region [ Methods ]
        public override async Task Execute(CommandModel commandModel)
        {
            await Task.Run(() =>
            {
                if (!(commandModel is CommandPreservationNotify))
                {
                    _logger.Error($"Command is not of type {nameof(CommandPreservationNotify)}");
                    return;
                }

                CommandPreservationNotify @command = commandModel as CommandPreservationNotify;
                if (string.IsNullOrEmpty(command.ReferenceId))
                {
                    return;
                }

                if (ConnectionShared.Connections.Any(x => x.Value.Equals(command.ReferenceId))
                    && _hubContext.Clients.Client(ConnectionShared.Connections.Single(x => x.Value.Equals(command.ReferenceId)).Key) != null)
                {
                    string connectionId = ConnectionShared.Connections.Single(x => x.Value.Equals(command.ReferenceId.ToString())).Key;
                    _hubContext.Clients.Client(connectionId).notify(command);
                }                
            });
        }
        #endregion
    }
}
