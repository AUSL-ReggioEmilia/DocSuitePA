using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
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
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Finder.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Finder.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Protocols;
using VecompSoftware.DocSuiteWeb.Finder.Resolutions;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Resolutions;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Service.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS;
using VecompSoftware.Services.Command.CQRS.Commands;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Fascicles
{
    public class FascicleController : BaseWebApiController<Fascicle, IFascicleService>
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ICurrentIdentity _currentIdentity;
        private readonly IQueueService _queueService;
        private readonly ICQRSMessageMapper _cqrsMapper;
        private readonly ILogger _logger;
        private readonly IResolutionModelMapper _mapper;
        private readonly IMapperUnitOfWork _mapperUnitOfwork;
        private readonly IParameterEnvService _parameterEnvService;
        #endregion

        #region [ Constructor ]

        public FascicleController(IFascicleService service, IDataUnitOfWork unitOfWork, ILogger logger, ICurrentIdentity currentIdentity, IQueueService queueService, ICQRSMessageMapper CQRSMapper, 
            IResolutionModelMapper mapper, IMapperUnitOfWork mapperUnitOfWork, IParameterEnvService parameterEnvService)
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

        protected override void AfterSave(Fascicle entity)
        {
            try
            {
                _logger.WriteDebug(new LogMessage(string.Concat("VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Fascicles.AfterSave with entity UniqueId ", entity.UniqueId)), LogCategories);

                if (CurrentDeleteActionType.HasValue && CurrentDeleteActionType == DeleteActionType.CancelFascicle)
                {
                    IQueryable<FascicleDocumentUnit> documentUnits = _unitOfWork.Repository<FascicleDocumentUnit>().GetByFascicle(entity.UniqueId);

                    ServiceBusMessage message;
                    foreach (FascicleDocumentUnit item in documentUnits)
                    {
                        if (item.DocumentUnit.Environment == (int)DSWEnvironmentType.Protocol)
                        {
                            Protocol protocol = _unitOfWork.Repository<Protocol>().GetByUniqueId(entity.UniqueId).Single();
                            message = GenerateMessage(item.DocumentUnit.Category, (int)DSWEnvironmentType.Protocol,
                                 (categoryFascicle) => new CommandUpdateProtocol(_parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, null, null, null,
                                 new IdentityContext(_currentIdentity.FullUserName), protocol, categoryFascicle, null));
                            Task.Run(async () =>
                            {
                                await _queueService.SubscribeQueue(message.ChannelName).SendToQueueAsync(message);
                            }).Wait();
                        }

                        if (item.DocumentUnit.Environment == (int)DSWEnvironmentType.Resolution)
                        {
                            Resolution resolution = _unitOfWork.Repository<Resolution>().GetByUniqueId(entity.UniqueId).Single();
                            ResolutionModel resolutionModel = _mapper.Map(resolution, new ResolutionModel());
                            _mapper.FileResolution = _unitOfWork.Repository<FileResolution>().GetByResolution(item.DocumentUnit.EntityId).SingleOrDefault();
                            _mapper.ResolutionRoles = _unitOfWork.Repository<ResolutionRole>().GetByResolution(item.DocumentUnit.EntityId);
                            message = GenerateMessage(item.DocumentUnit.Category, (int)DSWEnvironmentType.Resolution,
                             (categoryFascicle) => new CommandUpdateResolution(_parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId,
                             new IdentityContext(_currentIdentity.FullUserName), resolutionModel, categoryFascicle, null));
                            Task.Run(async () =>
                            {
                                await _queueService.SubscribeQueue(message.ChannelName).SendToQueueAsync(message);
                            }).Wait();
                        }

                        if (item.DocumentUnit.Environment == (int)DSWEnvironmentType.DocumentSeries)
                        {
                            DocumentSeriesItem documentSeriesItem = _unitOfWork.Repository<DocumentSeriesItem>().GetFullByUniqueId(entity.UniqueId).SingleOrDefault();
                            message = GenerateMessage(item.DocumentUnit.Category, (int)DSWEnvironmentType.DocumentSeries,
                                (categoryFascicle) => new CommandUpdateDocumentSeriesItem(_parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, null,
                                new IdentityContext(_currentIdentity.FullUserName), documentSeriesItem, categoryFascicle, null));
                            Task.Run(async () =>
                            {
                                await _queueService.SubscribeQueue(message.ChannelName).SendToQueueAsync(message);
                            }).Wait();
                        }

                        if (item.DocumentUnit.Environment >= 100)
                        {
                            if (item.DocumentUnit != null)
                            {
                                UDSBuildModel commandModel = _mapperUnitOfwork.Repository<IDomainMapper<DocumentUnit, UDSBuildModel>>().Map(item.DocumentUnit, new UDSBuildModel());
                                commandModel.UniqueId = item.DocumentUnit.UniqueId;
                                commandModel.RegistrationDate = item.DocumentUnit.RegistrationDate;
                                commandModel.RegistrationUser = item.DocumentUnit.RegistrationUser;
                                message = GenerateMessage(item.DocumentUnit.Category, item.DocumentUnit.Environment,
                                 (categoryFascicle) => new CommandCQRSUpdateUDSData(_parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId,
                                 new IdentityContext(_currentIdentity.FullUserName), commandModel, categoryFascicle, null, null, null, null));
                                Task.Run(async () =>
                                {
                                    await _queueService.SubscribeQueue(message.ChannelName).SendToQueueAsync(message);
                                }).Wait();
                            }
                        }
                    }
                }

                if (CurrentInsertActionType.HasValue || CurrentUpdateActionType.HasValue)
                {
                    IIdentityContext identity = new IdentityContext(_currentIdentity.FullUserName);
                    Fascicle fascicle = _unitOfWork.Repository<Fascicle>().GetByUniqueId(entity.UniqueId);
                    ICQRS command = new CommandCreateFascicle(_parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, identity, fascicle);
                    if (CurrentUpdateActionType.HasValue)
                    {
                        command = new CommandUpdateFascicle(_parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, identity, fascicle);
                    }

                    foreach (IWorkflowAction workflowAction in WorkflowActions)
                    {
                        if (IdWorkflowActivity.HasValue)
                        {
                            workflowAction.IdWorkflowActivity = IdWorkflowActivity.Value;
                        }
                        command.WorkflowActions.Add(workflowAction);
                    }
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