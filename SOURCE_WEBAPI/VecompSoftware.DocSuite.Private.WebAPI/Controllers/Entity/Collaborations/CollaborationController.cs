using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Commands.Entities.Collaborations;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Securities;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Finder.Collaborations;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Service.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Collaborations
{
    public class CollaborationController : BaseWebApiController<Collaboration, ICollaborationService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ICurrentIdentity _currentIdentity;
        private readonly IQueueService _queueService;
        private readonly ICQRSMessageMapper _cqrsMapper;
        private readonly IParameterEnvService _parameterEnvService;
        #endregion

        #region [ Constructor ]

        public CollaborationController(ICollaborationService service, IDataUnitOfWork unitOfWork, ILogger logger, ICurrentIdentity currentIdentity, 
            IQueueService queueService, ICQRSMessageMapper cqrsMapper, IParameterEnvService parameterEnvService)
            : base(service, unitOfWork, logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _queueService = queueService;
            _cqrsMapper = cqrsMapper;
            _parameterEnvService = parameterEnvService;
            _currentIdentity = currentIdentity;
            PostIsolationLevel = IsolationLevel.ReadCommitted;
        }

        #endregion

        #region [ Methods ]
        protected override void AfterSave(Collaboration entity)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Collaborations.AfterSave with entity UniqueId {entity.UniqueId}"), LogCategories);
                Collaboration collaboration = _unitOfWork.Repository<Collaboration>().GetByUniqueId(entity.UniqueId).FirstOrDefault();

                if (collaboration != null)
                {
                    IIdentityContext identity = new IdentityContext(_currentIdentity.FullUserName);
                    ICQRS command = null;

                    command = new CommandCreateCollaboration(_parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, Guid.Empty, identity, collaboration);

                    foreach (IWorkflowAction workflowAction in WorkflowActions)
                    {
                        workflowAction.IdWorkflowActivity = IdWorkflowActivity;
                        command.WorkflowActions.Add(workflowAction);
                    }
                    ServiceBusMessage message = _cqrsMapper.Map(command, new ServiceBusMessage());
                    if (message == null || string.IsNullOrEmpty(message.ChannelName))
                    {
                        throw new DSWException($"Queue name to command [{command}] is not mapped", null, DSWExceptionCode.SC_Mapper);
                    }
                    Task.Run(async () =>
                    {
                        await _queueService.SubscribeQueue(message.ChannelName).SendToQueueAsync(message);
                    }).Wait();
                }
            }
            catch (DSWException ex)
            {
                _logger.WriteError(ex, LogCategories);
            }
            base.AfterSave(entity);
        }
        #endregion
    }
}
