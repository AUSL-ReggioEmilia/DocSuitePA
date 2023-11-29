using System;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Commands.Entities.Dossiers;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Securities;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Finder.Tenants;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Service.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Commands.Entities.Dossiers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Dossiers
{
    public class DossierFolderController : BaseWebApiController<DossierFolder, IDossierFolderService>
    {
        #region [ Fields ]
        private readonly IDossierFolderService _service;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly ICurrentIdentity _currentIdentity;
        private readonly IDecryptedParameterEnvService _parameterEnvService;
        private readonly ICQRSMessageMapper _cqrsMapper;
        private readonly IQueueService _queueService;
        #endregion

        #region [ Constructor ]
        public DossierFolderController(IDossierFolderService service, IDataUnitOfWork unitOfWork, ILogger logger, ICurrentIdentity currentIdentity,
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
        protected override void AfterSave(DossierFolder entity, DossierFolder existingEntity)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Dossiers.AfterSave with entity UniqueId {entity.UniqueId}"), LogCategories);

                if (entity != null)
                {
                    Guid id = Guid.NewGuid();
                    Guid correlationId = Guid.NewGuid();
                    IIdentityContext identity = new IdentityContext(_currentIdentity.FullUserName);
                    TenantAOO tenantAOO = _unitOfWork.Repository<Tenant>().GetByUniqueId(_parameterEnvService.CurrentTenantId).FirstOrDefault().TenantAOO;
                    IContentTypeDossierFolder contentTypeDossierFolder = new ContentTypeDossierFolder(entity);
                    ServiceBusMessage message = null;
                    bool dossierFolderNameUpdated = existingEntity != null && entity.Name != existingEntity.Name;

                    if (CurrentUpdateActionType.HasValue)
                    {
                        message = GetCommandUpdateDossierFolderMessage(id, correlationId, tenantAOO.UniqueId, identity, contentTypeDossierFolder, dossierFolderNameUpdated);
                    }
                    else if (CurrentInsertActionType.HasValue)
                    {
                        message = GetCommandCreateDossierFolderMessage(id, correlationId, tenantAOO.UniqueId, identity, contentTypeDossierFolder);
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

        private ServiceBusMessage GetCommandUpdateDossierFolderMessage(Guid id, Guid correlationId, Guid tenantAOOId, IIdentityContext identity, IContentTypeDossierFolder contentTypeDossierFolder, bool dossierFolderNameUpdated)
        {
            ICommandUpdateDossierFolder commandUpdateDossierFolder;
            if (dossierFolderNameUpdated)
            {
                commandUpdateDossierFolder = new CommandUpdateDossierFolder(id, correlationId,
                    _parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, tenantAOOId, identity,
                    contentTypeDossierFolder, null, UpdatedPropertyType.DossierFolderNameUpdated);
            }
            else
            {
                commandUpdateDossierFolder = new CommandUpdateDossierFolder(id, correlationId,
                    _parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, tenantAOOId, identity,
                    contentTypeDossierFolder, null);
            }

            return _cqrsMapper.Map(commandUpdateDossierFolder, new ServiceBusMessage());
        }

        private ServiceBusMessage GetCommandCreateDossierFolderMessage(Guid id, Guid correlationId, Guid tenantAOOId, IIdentityContext identity, IContentTypeDossierFolder contentTypeDossierFolder)
        {
            ICommandCreateDossierFolder commandCreateDossierFolder = new CommandCreateDossierFolder(id, correlationId,
                _parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, tenantAOOId, identity,
                contentTypeDossierFolder, null);

            return _cqrsMapper.Map(commandCreateDossierFolder, new ServiceBusMessage());
        }
        #endregion
    }
}