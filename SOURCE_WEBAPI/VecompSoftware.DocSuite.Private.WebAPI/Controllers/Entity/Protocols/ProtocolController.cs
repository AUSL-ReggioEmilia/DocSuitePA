using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Commands.Entities.Protocols;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Securities;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Finder.Protocols;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Model.Workflow.Actions;
using VecompSoftware.DocSuiteWeb.Service.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Protocols
{
    public class ProtocolController : BaseWebApiController<Protocol, IProtocolService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ICurrentIdentity _currentIdentity;
        private readonly IQueueService _queueService;
        private readonly ICQRSMessageMapper _cqrsMapper;
        private readonly IDecryptedParameterEnvService _parameterEnvService;
        #endregion

        #region [ Constructor ]

        public ProtocolController(IProtocolService service, IDataUnitOfWork unitOfWork, ILogger logger, ICurrentIdentity currentIdentity, IQueueService queueService, 
            ICQRSMessageMapper cqrsMapper, IDecryptedParameterEnvService parameterEnvService)
            : base(service, unitOfWork, logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _queueService = queueService;
            _cqrsMapper = cqrsMapper;
            _parameterEnvService = parameterEnvService;
            _currentIdentity = currentIdentity;
            PostIsolationLevel = IsolationLevel.Serializable;
        }
        #endregion

        #region [ Methods ]

        protected override void AfterSave(Protocol entity, Protocol existingEntity)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Protocols.AfterSave with entity UniqueId {entity.UniqueId}"), LogCategories);
                Protocol protocol = _unitOfWork.Repository<Protocol>().GetByUniqueIdWithRoleAndContact(entity.UniqueId).FirstOrDefault();
                if (protocol != null)
                {
                    IList<CategoryFascicle> categoryFascicles = protocol.Category.CategoryFascicles.Where(f => f.DSWEnvironment == (int)DSWEnvironmentType.Protocol || f.DSWEnvironment == 0).ToList();
                    CategoryFascicle categoryFascicle = categoryFascicles.FirstOrDefault();
                    if (categoryFascicles != null && categoryFascicles.Count > 1)
                    {
                        categoryFascicle = categoryFascicles.FirstOrDefault(f => f.FascicleType == FascicleType.Period);
                    }
                    IIdentityContext identity = new IdentityContext(protocol.LastChangedUser);
                    ICQRS command = null;

                    if (CurrentUpdateActionType.HasValue && CurrentUpdateActionType == UpdateActionType.ActivateProtocol)
                    {
                        command = new CommandCreateProtocol(_parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, protocol.TenantAOO.UniqueId, null, null, null, identity, protocol, categoryFascicle, null);
                    }

                    if (CurrentDeleteActionType.HasValue && CurrentDeleteActionType == DeleteActionType.DeleteProtocol)
                    {
                        command = new CommandUpdateProtocol(_parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, protocol.TenantAOO.UniqueId, null, null, null, identity, protocol, categoryFascicle, null);
                    }

                    if (HttpContext.Current.Request.HttpMethod == HttpMethod.Put.Method &&
                        CurrentUpdateActionType.HasValue && CurrentUpdateActionType != UpdateActionType.ActivateProtocol)
                    {
                        command = new CommandUpdateProtocol(_parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, protocol.TenantAOO.UniqueId, null, null, null, identity, protocol, categoryFascicle, null);
                    }

                    if (command != null)
                    {
                        WorkflowActionDocumentUnitLinkModel workflowActionDocumentUnitLinkModel;
                        DocumentUnitModel documentUnitModel;
                        foreach (IWorkflowAction workflowAction in WorkflowActions)
                        {
                            workflowAction.IdWorkflowActivity = IdWorkflowActivity;
                            if (workflowAction is IWorkflowActionDocumentUnitLink)
                            {
                                workflowActionDocumentUnitLinkModel = (WorkflowActionDocumentUnitLinkModel)workflowAction;
                                documentUnitModel = workflowActionDocumentUnitLinkModel.GetReferenced();
                                if (documentUnitModel.UniqueId != protocol.UniqueId)
                                {
                                    documentUnitModel = workflowActionDocumentUnitLinkModel.GetDestinationLink();
                                }
                                documentUnitModel.Year = protocol.Year;
                                documentUnitModel.Number = protocol.Number.ToString();
                            }
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
            }
            catch (DSWException ex)
            {
                _logger.WriteError(ex, LogCategories);
            }
            base.AfterSave(entity, existingEntity);
        }

        #endregion

    }
}