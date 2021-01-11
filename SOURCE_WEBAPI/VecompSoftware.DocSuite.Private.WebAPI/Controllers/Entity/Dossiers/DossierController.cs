using System;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Commands.Entities.Dossiers;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Securities;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Finder.Dossiers;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Service.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Dossiers
{
    public class DossierController : BaseWebApiController<Dossier, IDossierService>
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ICurrentIdentity _currentIdentity;
        private readonly IParameterEnvService _parameterEnvService;
        private readonly ICQRSMessageMapper _cqrsMapper;
        private readonly IQueueService _queueService;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]

        public DossierController(IDossierService service, IDataUnitOfWork unitOfWork, 
            ILogger logger, 
            ICurrentIdentity currentIdentity,
            IParameterEnvService parameterEnvService, 
            ICQRSMessageMapper CQRSMapper,
            IQueueService queueService)
            : base(service, unitOfWork, logger)
        {
            _unitOfWork = unitOfWork;
            _currentIdentity = currentIdentity;
            _parameterEnvService = parameterEnvService;
            _cqrsMapper = CQRSMapper;
            _queueService = queueService;
            _logger = logger;
        }
        #endregion

        #region [ Methods ]
        protected override void AfterSave(Dossier entity)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Dossiers.AfterSave with entity UniqueId {entity.UniqueId}"), LogCategories);

                if (CurrentInsertActionType.HasValue || CurrentUpdateActionType.HasValue)
                {
                    IIdentityContext identity = new IdentityContext(_currentIdentity.FullUserName);
                    Dossier dossier = _unitOfWork.Repository<Dossier>().GetByUniqueId(entity.UniqueId).SingleOrDefault();

                    ICQRS command = new CommandCreateDossier(_parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, Guid.Empty, identity, dossier);
                    if (CurrentUpdateActionType.HasValue)
                    {
                        command = new CommandUpdateDossier(_parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, Guid.Empty, identity, dossier);
                    }

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