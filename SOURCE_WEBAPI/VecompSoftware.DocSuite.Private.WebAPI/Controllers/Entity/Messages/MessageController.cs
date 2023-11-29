using System;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Commands.Entities.Messages;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Securities;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Service.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Messages
{
    public class MessageController : BaseWebApiController<Message, IMessageService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ICurrentIdentity _currentIdentity;
        private readonly IQueueService _queueService;
        private readonly ICQRSMessageMapper _cqrsMapper;
        private readonly IDecryptedParameterEnvService _parameterEnvService;
        #endregion

        #region [ Constructor ]

        public MessageController(IMessageService service, IDataUnitOfWork unitOfWork, ILogger logger, ICurrentIdentity currentIdentity, IQueueService queueService, 
            ICQRSMessageMapper cqrsMapper, IDecryptedParameterEnvService parameterEnvService)
            : base(service, unitOfWork, logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _currentIdentity = currentIdentity;
            _cqrsMapper = cqrsMapper;
            _currentIdentity = currentIdentity;
            _queueService = queueService;
            _parameterEnvService = parameterEnvService;
        }
        #endregion

        #region [ Methods ]
        protected override void AfterSave(Message entity, Message existingEntity)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Messages.AfterSave with entity UniqueId {entity.UniqueId}"), LogCategories);
                Message message = _unitOfWork.Repository<Message>().Find(entity.EntityId);
                if (message != null)
                {
                    IIdentityContext identity = new IdentityContext(_currentIdentity.FullUserName);
                    ICQRS command = null;
                    if (entity.Status == MessageStatus.Active)
                    {
                        command = new CommandCreateMessage(_parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, Guid.Empty, identity, message);
                    }
                    if (command != null)
                    {
                        foreach (IWorkflowAction workflowAction in WorkflowActions)
                        {
                            workflowAction.IdWorkflowActivity = IdWorkflowActivity;
                            command.WorkflowActions.Add(workflowAction);
                        }
                        ServiceBusMessage serviceBusMessage = _cqrsMapper.Map(command, new ServiceBusMessage());
                        if (serviceBusMessage == null || string.IsNullOrEmpty(serviceBusMessage.ChannelName))
                        {
                            throw new DSWException($"Queue name to command [{command}] is not mapped", null, DSWExceptionCode.SC_Mapper);
                        }
                        Task.Run(async () =>
                        {
                            await _queueService.SubscribeQueue(serviceBusMessage.ChannelName).SendToQueueAsync(serviceBusMessage);
                        }).Wait();
                    }
                }
            }
            catch (DSWException ex)
            {
                _logger.WriteError(ex, LogCategories);
            }
            base.AfterSave(entity, existingEntity);
        }
        #endregion

    }
}