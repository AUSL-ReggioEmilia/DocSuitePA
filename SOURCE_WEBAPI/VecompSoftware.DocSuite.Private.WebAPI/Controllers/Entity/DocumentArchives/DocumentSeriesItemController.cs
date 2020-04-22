using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Commands.Entities.DocumentArchives;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuite.WebAPI.Common.Configurations;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Securities;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Service.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.Services.Command;
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
        private readonly IParameterEnvService _parameterEnvService;

        #endregion

        #region [ Constructor ]
        public DocumentSeriesItemController(IDocumentSeriesItemService service, IDataUnitOfWork unitOfWork, ILogger logger, ICurrentIdentity currentIdentity, IQueueService queueService, 
            ICQRSMessageMapper cqrsSMapper, IParameterEnvService parameterEnvService)
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

        protected override void AfterSave(DocumentSeriesItem entity)
        {
            try
            {
                _logger.WriteDebug(new LogMessage(string.Concat("VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.DocumentArchives.AfterSave with entity UniqueId ", entity.UniqueId)), LogCategories);
                DocumentSeriesItem item = _unitOfWork.Repository<DocumentSeriesItem>().GetFullByUniqueId(entity.UniqueId).SingleOrDefault();
                if (item != null)
                {
                    IList<CategoryFascicle> categoryFascicles = item.Category.CategoryFascicles.Where(f => (f.DSWEnvironment == (int)DSWEnvironmentType.DocumentSeries || f.DSWEnvironment == 0)).ToList();
                    CategoryFascicle categoryFascicle = categoryFascicles.FirstOrDefault();
                    if (categoryFascicles != null && categoryFascicles.Count > 1)
                    {
                        categoryFascicle = categoryFascicles.FirstOrDefault(f => f.FascicleType == FascicleType.Period);
                    }
                    IIdentityContext identity = new IdentityContext(_currentIdentity.FullUserName);
                    ICommandUpdateDocumentSeriesItem commandUpdate = new CommandUpdateDocumentSeriesItem(_parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, null, identity, item, categoryFascicle, null);

                    ServiceBusMessage message = _mapper_cqrsMessageMapper.Map(commandUpdate, new ServiceBusMessage());
                    if (string.IsNullOrEmpty(message.ChannelName))
                    {
                        throw new DSWException(string.Concat("Queue name to command [", commandUpdate.ToString(), "] is not mapped"), null, DSWExceptionCode.SC_Mapper);
                    }

                    Task.Run(async () =>
                    {
                        await _service_queue.SubscribeQueue(message.ChannelName).SendToQueueAsync(message);
                    }).Wait();
                }
                base.AfterSave(entity);
            }
            catch (DSWException ex)
            {
                _logger.WriteError(ex, LogCategories);
            }
        }

        #endregion
    }
}