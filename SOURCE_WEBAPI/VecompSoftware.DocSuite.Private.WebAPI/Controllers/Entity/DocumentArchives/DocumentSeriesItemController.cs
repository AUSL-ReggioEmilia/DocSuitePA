using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Commands.Entities.DocumentArchives;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuite.WebAPI.Common.Configurations;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Securities;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Finder.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Finder.Tenants;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Model.Entities.Tenants;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Service.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Commands.Entities.DocumentArchives;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.DocumentArchives
{
    public class DocumentSeriesItemController : BaseWebApiController<DocumentSeriesItem, IDocumentSeriesItemService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ICurrentIdentity _currentIdentity;
        private readonly IQueueService _service_queue;
        private readonly ICQRSMessageMapper _mapper_cqrsMessageMapper;
        private readonly IDecryptedParameterEnvService _parameterEnvService;

        #endregion

        #region [ Constructor ]
        public DocumentSeriesItemController(IDocumentSeriesItemService service, IDataUnitOfWork unitOfWork, ILogger logger, ICurrentIdentity currentIdentity, IQueueService queueService, 
            ICQRSMessageMapper cqrsSMapper, IDecryptedParameterEnvService parameterEnvService)
            : base(service, unitOfWork, logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _service_queue = queueService;
            _mapper_cqrsMessageMapper = cqrsSMapper;
            _parameterEnvService = parameterEnvService;
            _currentIdentity = currentIdentity;
        }
        #endregion

        #region [ Methods ]

        protected override void AfterSave(DocumentSeriesItem entity, DocumentSeriesItem existingEntity)
        {
            try
            {
                _logger.WriteDebug(new LogMessage(string.Concat("VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.DocumentArchives.AfterSave with entity UniqueId ", entity.UniqueId)), LogCategories);
                DocumentSeriesItem item = _unitOfWork.Repository<DocumentSeriesItem>().GetFullByUniqueId(entity.UniqueId).SingleOrDefault();
                TenantTableValuedModel result = _unitOfWork.Repository<Tenant>().GetUserTenants(_currentIdentity.Account, _currentIdentity.Domain).FirstOrDefault();
                Guid tenandId = _parameterEnvService.CurrentTenantId;
                Guid tenandAOOId = _parameterEnvService.CurrentTenantId;
                string tenantName = _parameterEnvService.CurrentTenantName;
                if (result != null)
                {
                    tenandId = result.IdTenantModel;
                    tenandAOOId = result.IdTenantAOO;
                    tenantName = result.TenantName;
                }
                if (item != null)
                {
                    IList<CategoryFascicle> categoryFascicles = item.Category.CategoryFascicles.Where(f => (f.DSWEnvironment == (int)DSWEnvironmentType.DocumentSeries || f.DSWEnvironment == 0)).ToList();
                    CategoryFascicle categoryFascicle = categoryFascicles.FirstOrDefault();
                    if (categoryFascicles != null && categoryFascicles.Count > 1)
                    {
                        categoryFascicle = categoryFascicles.FirstOrDefault(f => f.FascicleType == FascicleType.Period);
                    }
                    IIdentityContext identity = new IdentityContext(_currentIdentity.FullUserName);

                    ICommand command = new CommandUpdateDocumentSeriesItem(tenantName, tenandId, tenandAOOId, null, identity, item, categoryFascicle, null);
                    if (CurrentUpdateActionType.HasValue && CurrentUpdateActionType == UpdateActionType.ActivateDocumentSeriesItem)
                    {
                        command = new CommandCreateDocumentSeriesItem(tenantName, tenandId, tenandAOOId, null, identity, item, categoryFascicle, null);
                    }                    

                    ServiceBusMessage message = _mapper_cqrsMessageMapper.Map(command, new ServiceBusMessage());
                    if (string.IsNullOrEmpty(message.ChannelName))
                    {
                        throw new DSWException(string.Concat("Queue name to command [", command.ToString(), "] is not mapped"), null, DSWExceptionCode.SC_Mapper);
                    }

                    Task.Run(async () =>
                    {
                        await _service_queue.SubscribeQueue(message.ChannelName).SendToQueueAsync(message);
                    }).Wait();
                }
                base.AfterSave(entity, existingEntity);
            }
            catch (DSWException ex)
            {
                _logger.WriteError(ex, LogCategories);
            }
        }

        #endregion
    }
}