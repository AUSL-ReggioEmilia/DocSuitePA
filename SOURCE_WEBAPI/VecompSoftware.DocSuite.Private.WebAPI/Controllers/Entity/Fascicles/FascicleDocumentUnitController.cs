using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Commands.Entities.DocumentArchives;
using VecompSoftware.Core.Command.CQRS.Commands.Entities.Fascicles;
using VecompSoftware.Core.Command.CQRS.Commands.Entities.Protocols;
using VecompSoftware.Core.Command.CQRS.Commands.Models.Resolutions;
using VecompSoftware.Core.Command.CQRS.Commands.Models.UDS;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Securities;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Finder.Collaborations;
using VecompSoftware.DocSuiteWeb.Finder.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Finder.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Finder.Protocols;
using VecompSoftware.DocSuiteWeb.Finder.Resolutions;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Resolutions;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Service.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Commands;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Fascicles
{
    public class FascicleDocumentUnitController : BaseWebApiController<FascicleDocumentUnit, IFascicleDocumentUnitService>
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ICurrentIdentity _currentIdentity;
        private readonly IQueueService _queueService;
        private readonly ICQRSMessageMapper _cqrsMapper;
        private readonly IResolutionModelMapper _mapper;
        private readonly ILogger _logger;
        private readonly IMapperUnitOfWork _mapperUnitOfwork;
        private readonly IParameterEnvService _parameterEnvService;
        #endregion

        #region [ Constructor ]

        public FascicleDocumentUnitController(IFascicleDocumentUnitService service, IDataUnitOfWork unitOfWork, ILogger logger,
            ICurrentIdentity currentIdentity, IQueueService queueService, ICQRSMessageMapper CQRSMapper, IResolutionModelMapper mapper,
            IMapperUnitOfWork mapperUnitOfWork, IParameterEnvService parameterEnvService)
            : base(service, unitOfWork, logger)
        {
            _unitOfWork = unitOfWork;
            _currentIdentity = currentIdentity;
            _cqrsMapper = CQRSMapper;
            _queueService = queueService;
            _logger = logger;
            _mapper = mapper;
            _mapperUnitOfwork = mapperUnitOfWork;
            _parameterEnvService = parameterEnvService;
        }
        #endregion

        #region [ Methods ]


        private ServiceBusMessage GenerateMessage(Category category, int environmentType, Func<CategoryFascicle, ICommand> func)
        {
            IList<CategoryFascicle> categoryFascicles = category.CategoryFascicles.Where(f => (f.DSWEnvironment == environmentType || f.DSWEnvironment == 0)).ToList();
            CategoryFascicle categoryFascicle = categoryFascicles.FirstOrDefault();
            if (categoryFascicles != null && categoryFascicles.Count > 1)
            {
                categoryFascicle = categoryFascicles.FirstOrDefault(f => f.FascicleType == FascicleType.Period);
            }
            IIdentityContext identity = new IdentityContext(_currentIdentity.FullUserName);

            ServiceBusMessage message = null;
            ICommand command = func(categoryFascicle);
            message = _cqrsMapper.Map(command, new ServiceBusMessage());
            if (string.IsNullOrEmpty(message.ChannelName))
            {
                throw new DSWException(string.Concat("Queue name to command [", command.ToString(), "] is not mapped"), null, DSWExceptionCode.SC_Mapper);
            }
            return message;
        }
        protected override IQueryFluent<FascicleDocumentUnit> SetEntityIncludeOnDelete(IQueryFluent<FascicleDocumentUnit> query)
        {
            return query
                .Include(c => c.DocumentUnit.Category)
                .Include(c => c.DocumentUnit.Container)
                .Include(c => c.Fascicle)
                .Include(c => c.Fascicle.Category)
                .Include(c => c.DocumentUnit.UDSRepository);
        }

        protected override void AfterSave(FascicleDocumentUnit entity)
        {
            try
            {
                _logger.WriteDebug(new LogMessage(string.Concat("VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Fascicles.FascicleDocumentUnit.AfterSave with entity UniqueId ", entity.UniqueId)), LogCategories);
                ServiceBusMessage message = null;

                if (entity.ReferenceType == ReferenceType.Fascicle)
                {
                    switch (entity.DocumentUnit.Environment)
                    {
                        case 1:
                            {
                                Collaboration collaboration = null;
                                Guid? collaborationUniqueId = null;
                                int? collaborationId = null;
                                string collaborationTemplateName = string.Empty;
                                Protocol protocol = _unitOfWork.Repository<Protocol>().GetByUniqueIdWithRole(entity.DocumentUnit.UniqueId).SingleOrDefault();
                                if (protocol != null)
                                {
                                    collaboration = _unitOfWork.Repository<Collaboration>().GetByProtocol(protocol.Year, protocol.Number).SingleOrDefault();
                                    if (collaboration != null)
                                    {
                                        collaborationId = collaboration.EntityId;
                                        collaborationUniqueId = collaboration.UniqueId;
                                        collaborationTemplateName = collaboration.TemplateName;
                                    }
                                }
                                message = GenerateMessage(entity.DocumentUnit.Category, (int)DSWEnvironmentType.Protocol,
                                    (categoryFascicle) => new CommandUpdateProtocol(_parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, protocol.TenantAOO.UniqueId,
                                    collaborationUniqueId, collaborationId, collaborationTemplateName, new IdentityContext(_currentIdentity.FullUserName), protocol, categoryFascicle, null));
                                Task.Run(async () =>
                                {
                                    await _queueService.SubscribeQueue(message.ChannelName).SendToQueueAsync(message);
                                }).Wait();
                                break;
                            }
                        case 2:
                            {
                                Resolution resolution = _unitOfWork.Repository<Resolution>().GetFullByUniqueId(entity.DocumentUnit.UniqueId).SingleOrDefault();
                                if (resolution.AdoptionDate.HasValue)
                                {
                                    ResolutionModel resolutionModel = _mapper.Map(resolution, new ResolutionModel());
                                    _mapper.FileResolution = _unitOfWork.Repository<FileResolution>().GetByResolution(resolution.EntityId).SingleOrDefault();
                                    _mapper.ResolutionRoles = _unitOfWork.Repository<ResolutionRole>().GetByResolution(resolution.EntityId);
                                    message = GenerateMessage(entity.DocumentUnit.Category, (int)DSWEnvironmentType.Resolution,
                                        (categoryFascicle) => new CommandUpdateResolution(_parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, Guid.Empty,
                                        new IdentityContext(_currentIdentity.FullUserName), resolutionModel, categoryFascicle, null));
                                    Task.Run(async () =>
                                    {
                                        await _queueService.SubscribeQueue(message.ChannelName).SendToQueueAsync(message);
                                    }).Wait();
                                }
                                break;
                            }
                        case 4:
                            {
                                DocumentSeriesItem documentSeriesItem = _unitOfWork.Repository<DocumentSeriesItem>().GetFullByUniqueId(entity.DocumentUnit.UniqueId).SingleOrDefault();
                                message = GenerateMessage(entity.DocumentUnit.Category, (int)DSWEnvironmentType.DocumentSeries,
                                    (categoryFascicle) => new CommandUpdateDocumentSeriesItem(_parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, Guid.Empty, null,
                                    new IdentityContext(_currentIdentity.FullUserName), documentSeriesItem, categoryFascicle, null));
                                Task.Run(async () =>
                                {
                                    await _queueService.SubscribeQueue(message.ChannelName).SendToQueueAsync(message);
                                }).Wait();
                                break;
                            }
                        default:
                            {
                                if (entity.DocumentUnit.Environment >= 100)
                                {
                                    UDSBuildModel commandModel = _mapperUnitOfwork.Repository<IDomainMapper<DocumentUnit, UDSBuildModel>>().Map(entity.DocumentUnit, new UDSBuildModel());
                                    commandModel.UniqueId = entity.DocumentUnit.UniqueId;
                                    commandModel.UDSId = entity.DocumentUnit.UniqueId;
                                    commandModel.RegistrationDate = entity.DocumentUnit.RegistrationDate;
                                    commandModel.RegistrationUser = entity.DocumentUnit.RegistrationUser;
                                    message = GenerateMessage(entity.DocumentUnit.Category, entity.DocumentUnit.Environment,
                                        (categoryFascicle) => new CommandCQRSUpdateUDSData(_parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, entity.DocumentUnit.TenantAOO?.UniqueId ?? Guid.Empty,
                                        new IdentityContext(_currentIdentity.FullUserName), commandModel, categoryFascicle, entity.DocumentUnit, null, null, null));
                                    Task.Run(async () =>
                                    {
                                        await _queueService.SubscribeQueue(message.ChannelName).SendToQueueAsync(message);
                                    }).Wait();
                                }
                                break;
                            }
                    }
                }

                if (CurrentInsertActionType.HasValue)
                {
                    message = GenerateMessage(entity.DocumentUnit.Category, (int)DSWEnvironmentType.Fascicle,
                        (categoryFascicle) =>
                        {
                            return new CommandCreateFascicleDocumentUnit(_parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, Guid.Empty,
                                new IdentityContext(_currentIdentity.FullUserName), entity);
                        });
                    Task.Run(async () =>
                    {
                        await _queueService.SubscribeQueue(message.ChannelName).SendToQueueAsync(message);
                    }).Wait();
                }

                if (CurrentDeleteActionType.HasValue)
                {
                    message = GenerateMessage(entity.DocumentUnit.Category, (int)DSWEnvironmentType.Fascicle,
                        (categoryFascicle) =>
                        {
                            return new CommandDeleteFascicleDocumentUnit(_parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, Guid.Empty,
                                new IdentityContext(_currentIdentity.FullUserName), entity);
                        });
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