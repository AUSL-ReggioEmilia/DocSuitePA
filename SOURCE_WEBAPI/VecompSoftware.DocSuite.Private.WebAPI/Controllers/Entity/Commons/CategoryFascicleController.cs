using System;
using System.Threading.Tasks;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Commands.Entities.Commons;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Securities;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Commands.Entities.Commons;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Commons
{
    public class CategoryFascicleController : BaseWebApiController<CategoryFascicle, ICategoryFascicleService>
    {
        #region [ Fields ]
        private readonly ICurrentIdentity _currentIdentity;
        private readonly IQueueService _queueService;
        private readonly ICQRSMessageMapper _cqrsMapper;
        private readonly IDecryptedParameterEnvService _parameterEnvService;
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]
        public CategoryFascicleController(ICategoryFascicleService service, IDataUnitOfWork unitOfWork, ILogger logger, ICurrentIdentity currentIdentity, IQueueService queueService, 
            ICQRSMessageMapper cqrsMapper, IDecryptedParameterEnvService parameterEnvService)
            : base(service, unitOfWork, logger)
        {
            _currentIdentity = currentIdentity;
            _queueService = queueService;
            _cqrsMapper = cqrsMapper;
            _parameterEnvService = parameterEnvService;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]
        protected override IQueryFluent<CategoryFascicle> SetEntityIncludeOnDelete(IQueryFluent<CategoryFascicle> query)
        {
            return query.Include(c => c.Category);
        }

        protected override void AfterSave(CategoryFascicle entity, CategoryFascicle existingEntity)
        {
            if (CurrentDeleteActionType.HasValue && CurrentDeleteActionType == DeleteActionType.DeleteCategoryFascicle)
            {
                IIdentityContext identity = new IdentityContext(_currentIdentity.FullUserName);

                ICommandDeleteCategoryFascicle command = new CommandDeleteCategoryFascicle(Guid.Empty, _parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, Guid.Empty, identity, entity);
                ServiceBusMessage message = _cqrsMapper.Map(command, new ServiceBusMessage());
                if (message == null || string.IsNullOrEmpty(message.ChannelName))
                {
                    throw new DSWException(string.Concat("Queue name to command [", command.ToString(), "] is not mapped"), null, DSWExceptionCode.SC_Mapper);
                }
                Task.Run(async () =>
                {
                    await _queueService.SubscribeQueue(message.ChannelName).SendToQueueAsync(message);
                }).Wait();
            }
            base.AfterSave(entity, existingEntity);
        }
        #endregion
    }
}