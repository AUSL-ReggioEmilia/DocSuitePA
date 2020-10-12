using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Commands.Models.Resolutions;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Securities;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Finder.Resolutions;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Resolutions;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Service.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Commands.Models.Resolutions;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Resolutions
{
    public class ResolutionController : BaseWebApiController<Resolution, IResolutionService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ICurrentIdentity _currentIdentity;
        private readonly IQueueService _queueService;
        private readonly ICQRSMessageMapper _cqrsMapper;
        private readonly IResolutionModelMapper _mapper;
        private readonly IParameterEnvService _parameterEnvService;
        #endregion

        #region [ Constructor ]

        public ResolutionController(IResolutionService service, IDataUnitOfWork unitOfWork, ILogger logger, ICurrentIdentity currentIdentity, IQueueService queueService, 
            ICQRSMessageMapper cqrsMapper, IResolutionModelMapper mapper, IParameterEnvService parameterEnvService)
            : base(service, unitOfWork, logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _queueService = queueService;
            _cqrsMapper = cqrsMapper;
            _currentIdentity = currentIdentity;
            _mapper = mapper;
            _parameterEnvService = parameterEnvService;
        }
        #endregion

        #region [ Methods ]

        protected override void AfterSave(Resolution entity)
        {
            try
            {
                _logger.WriteDebug(new LogMessage(string.Concat("VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Resolutions.AfterSave with entity UniqueId ", entity.UniqueId)), LogCategories);
                Resolution resolution = _unitOfWork.Repository<Resolution>().GetFullByUniqueId(entity.UniqueId).SingleOrDefault();
                if (resolution != null)
                {
                    IList<CategoryFascicle> categoryFascicles = resolution.Category.CategoryFascicles.Where(f => (f.DSWEnvironment == (int)DSWEnvironmentType.Resolution || f.DSWEnvironment == 0)).ToList();
                    CategoryFascicle categoryFascicle = categoryFascicles.FirstOrDefault();
                    if (categoryFascicles != null && categoryFascicles.Count > 1)
                    {
                        categoryFascicle = categoryFascicles.FirstOrDefault(f => f.FascicleType == FascicleType.Period);
                    }
                    _mapper.FileResolution = _unitOfWork.Repository<FileResolution>().GetByResolution(resolution.EntityId).SingleOrDefault();
                    _mapper.ResolutionRoles = _unitOfWork.Repository<ResolutionRole>().GetByResolution(resolution.EntityId);
                    IIdentityContext identity = new IdentityContext(_currentIdentity.FullUserName);
                    ResolutionModel resolutionModel = _mapper.Map(resolution, new ResolutionModel());

                    ICommandUpdateResolution commandUpdate = new CommandUpdateResolution(_parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, Guid.Empty, identity, resolutionModel, categoryFascicle, null);

                    ServiceBusMessage message = _cqrsMapper.Map(commandUpdate, new ServiceBusMessage());
                    if (string.IsNullOrEmpty(message.ChannelName))
                    {
                        throw new DSWException(string.Concat("Queue name to command [", commandUpdate.ToString(), "] is not mapped"), null, DSWExceptionCode.SC_Mapper);
                    }

                    Task.Run(async () =>
                    {
                        await _queueService.SubscribeQueue(message.ChannelName).SendToQueueAsync(message);
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