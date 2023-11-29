using System;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Commands.Entities.Processes;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Securities;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Finder.Tenants;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Service.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Commands.Entities.Processes;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Processes
{
    public class ProcessController : BaseWebApiController<Process, IProcessService>
    {
        #region [ Fields ]
        private readonly IProcessService _service;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly ICurrentIdentity _currentIdentity;
        private readonly IDecryptedParameterEnvService _parameterEnvService;
        private readonly ICQRSMessageMapper _cqrsMapper;
        private readonly IQueueService _queueService;
        #endregion

        #region [ Constructor ]
        public ProcessController(IProcessService service, IDataUnitOfWork unitOfWork, ILogger logger, ICurrentIdentity currentIdentity,
            IDecryptedParameterEnvService parameterEnvService, ICQRSMessageMapper cqrsMapper, IQueueService queueService)
            : base(service, unitOfWork, logger)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentIdentity = currentIdentity;
            _parameterEnvService = parameterEnvService;
            _cqrsMapper = cqrsMapper;
            _queueService = queueService;
        }
        #endregion

        #region [ Methods ]
        protected override void AfterSave(Process entity, Process existingEntity)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Process.AfterSave with entity UniqueId {entity.UniqueId}"), LogCategories);

                if (entity != null)
                {
                    Guid id = Guid.NewGuid();
                    Guid correlationId = Guid.NewGuid();
                    IIdentityContext identity = new IdentityContext(_currentIdentity.FullUserName);
                    TenantAOO tenantAOO = _unitOfWork.Repository<Tenant>().GetByUniqueId(_parameterEnvService.CurrentTenantId).FirstOrDefault().TenantAOO;
                    IContentTypeProcess contentTypeProcess = new ContentTypeProcess(entity);
                    ServiceBusMessage message = null;
                    bool processNameUpdated = existingEntity != null && entity.Name != existingEntity.Name;

                    if (CurrentUpdateActionType.HasValue)
                    {
                        message = GetCommandUpdateProcessMessage(id, correlationId, tenantAOO.UniqueId, identity, contentTypeProcess, processNameUpdated);
                    }
                    else if (CurrentInsertActionType.HasValue)
                    {
                        message = GetCommandCreateProcessMessage(id, correlationId, tenantAOO.UniqueId, identity, contentTypeProcess);
                    }

                    if (message != null)
                    {
                        Task.Run(async () =>
                        {
                            await _queueService.SubscribeQueue(message.ChannelName).SendToQueueAsync(message);
                        }).Wait();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
            }

            base.AfterSave(entity, existingEntity);
        }

        private ServiceBusMessage GetCommandUpdateProcessMessage(Guid id, Guid correlationId, Guid tenantAOOId, IIdentityContext identity, IContentTypeProcess contentTypeProcess, bool processNameUpdated)
        {

            ICommandUpdateProcess commandUpdateProcess;
            if (processNameUpdated)
            {
                commandUpdateProcess = new CommandUpdateProcess(id, correlationId,
                    _parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, tenantAOOId, identity,
                    contentTypeProcess, null, UpdatedPropertyType.ProcessNameUpdated);
            }
            else
            {
                commandUpdateProcess = new CommandUpdateProcess(id, correlationId,
                    _parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, tenantAOOId, identity,
                    contentTypeProcess, null);
            }

            return _cqrsMapper.Map(commandUpdateProcess, new ServiceBusMessage());
        }

        private ServiceBusMessage GetCommandCreateProcessMessage(Guid id, Guid correlationId, Guid tenantAOOId, IIdentityContext identity, IContentTypeProcess contentTypeProcess)
        {
            ICommandCreateProcess commandCreateProcess = new CommandCreateProcess(id, correlationId,
                _parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, tenantAOOId, identity,
                contentTypeProcess, null);

            return _cqrsMapper.Map(commandCreateProcess, new ServiceBusMessage());
        }
        #endregion
    }
}