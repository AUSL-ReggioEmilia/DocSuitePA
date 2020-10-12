using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VecompSoftware.BiblosDS.Model.CQRS;
using VecompSoftware.BiblosDS.Model.CQRS.Notifications.Preservations;
using VecompSoftware.BiblosDS.Model.CQRS.Preservations;
using VecompSoftware.BiblosDS.WindowsService.WebAPI.Receivers.Notifications.Preservations;
using VecompSoftware.BiblosDS.WindowsService.WebAPI.Receivers.Preservations;

namespace VecompSoftware.BiblosDS.WindowsService.WebAPI.Receivers
{
    public class ReceiverMediator : IReceiverMediator
    {
        #region [ Fields ]
        private readonly IDictionary<Type, Func<IReceiver>> _receiverInstances;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public ReceiverMediator()
        {
            _receiverInstances = new Dictionary<Type, Func<IReceiver>>()
            {
                { typeof(CommandPurgePreservation), () => new PurgePreservationReceiver(this) },
                { typeof(CommandPreservationNotify), () => new PreservationNotifyReceiver(this) },
                { typeof(CommandExecutePreservation), () => new ExecutePreservationReceiver(this) },
                { typeof(CommandInsertPreservationPDV), () => new InsertPreservationPDVReceiver(this) },
                { typeof(CommandInsertPreservationRDV), () => new InsertPreservationRDVReceiver(this) },
                { typeof(CommandConfigureArchiveForPreservation), () => new ConfigureArchiveForPreservation(this) }
            };
        }
        #endregion

        #region [ Methods ]
        public async Task Send(CommandModel commandModel)
        {
            IReceiver receiver = GetReceiverInstance(commandModel);
            if (receiver == null)
            {
                throw new Exception();
            }
            await receiver.Execute(commandModel);
        }

        private IReceiver GetReceiverInstance(CommandModel commandModel)
        {
            Type commandType = commandModel.GetType();
            if (_receiverInstances.ContainsKey(commandType))
            {
                return _receiverInstances[commandType]();
            }
            return null;
        }
        #endregion
    }
}
