using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.Core.Command.CQRS.Events.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.ServiceBus.Module.CQRS.Executors;
using VecompSoftware.ServiceBus.Module.CQRS.Executors.Executors;
using VecompSoftware.ServiceBus.Module.CQRS.Executors.Executors.Collaborations;
using VecompSoftware.ServiceBus.Module.CQRS.Executors.Executors.Commons;
using VecompSoftware.ServiceBus.Module.CQRS.Executors.Executors.DocumentArchives;
using VecompSoftware.ServiceBus.Module.CQRS.Executors.Executors.Fascicles;
using VecompSoftware.ServiceBus.Module.CQRS.Executors.Executors.Messages;
using VecompSoftware.ServiceBus.Module.CQRS.Executors.Executors.PECMails;
using VecompSoftware.ServiceBus.Module.CQRS.Executors.Executors.Protocols;
using VecompSoftware.ServiceBus.Module.CQRS.Executors.Executors.Resolutions;
using VecompSoftware.ServiceBus.Module.CQRS.Executors.Executors.UDS;
using VecompSoftware.ServiceBus.Receiver.Base;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Events;
using VecompSoftware.Services.Command.CQRS.Events.Entities.DocumentUnits;

namespace VecompSoftware.ServiceBus.Module.CQRS
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class CQRSBaseExecution<TCommand> : IListenerExecution<TCommand>
        where TCommand : ICommandCQRS
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IWebAPIClient _webApiClient;
        private readonly BiblosDS.BiblosClient _biblosClient;
        protected static IEnumerable<LogCategory> _logCategories = null;
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(CQRSBaseExecution<>));
                }
                return _logCategories;
            }
        }

        public IDictionary<string, object> Properties { get; set; }
        public EvaluationModel RetryPolicyEvaluation { get; set; }

        private readonly Dictionary<Type, IBaseCommonExecutor> _commandExecutors;

        #endregion

        #region [ Constructor ]

        public CQRSBaseExecution(ILogger logger, IWebAPIClient webApiClient, BiblosDS.BiblosClient biblosClient)
        {
            _logger = logger;
            _webApiClient = webApiClient;
            _biblosClient = biblosClient;
            _commandExecutors = new Dictionary<Type, IBaseCommonExecutor>();
            IProtocolContentTypeExecutor protocolContentTypeExecutor = new ProtocolContentTypeExecutor(logger, webApiClient, biblosClient);
            IResolutionContentTypeExecutor resolutionContentTypeExecutor = new ResolutionContentTypeExecutor(logger, webApiClient, biblosClient);
            IDocumentSeriesItemContentTypeExecutor documentSeriesItemContentTypeExecutor = new DocumentSeriesItemContentTypeExecutor(logger, webApiClient, biblosClient);
            IUDSContentTypeExecutor udsContentTypeExecutor = new UDSContentTypeExecutor(logger, webApiClient, biblosClient);
            IPECMailContentTypeExecutor pecContentTypeExecutor = new PECMailContentTypeExecutor(logger, webApiClient, biblosClient);
            IFascicleContentTypeExecutor fascicleContentTypeExecutor = new FascicleContentTypeExecutor(logger, webApiClient, biblosClient);
            IMessageContentTypeExecutor messageContentTypeExecutor = new MessageContentTypeExecutor(logger, webApiClient, biblosClient);
            ICollaborationContentTypeExecutor collaborationContentTypeExecutor = new CollaborationContentTypeExecutor(logger, webApiClient, biblosClient);
            ICategoryFascicleContentTypeExecutor categoryFascicleContentTypeExecutor = new CategoryFascicleContentTypeExecutor(logger, webApiClient, biblosClient);
            IFascicleDocumentUnitContentTypeExecutor fascicleDocumentUnitContentTypeExecutor = new FascicleDocumentUnitContentTypeExecutor(logger, webApiClient, biblosClient);
            _commandExecutors.Add(typeof(Protocol), protocolContentTypeExecutor);
            _commandExecutors.Add(typeof(ResolutionModel), resolutionContentTypeExecutor);
            _commandExecutors.Add(typeof(DocumentSeriesItem), documentSeriesItemContentTypeExecutor);
            _commandExecutors.Add(typeof(UDSBuildModel), udsContentTypeExecutor);
            _commandExecutors.Add(typeof(PECMail), pecContentTypeExecutor);
            _commandExecutors.Add(typeof(Fascicle), fascicleContentTypeExecutor);
            _commandExecutors.Add(typeof(Message), messageContentTypeExecutor);
            _commandExecutors.Add(typeof(Collaboration), collaborationContentTypeExecutor);
            _commandExecutors.Add(typeof(CategoryFascicle), categoryFascicleContentTypeExecutor);
            _commandExecutors.Add(typeof(FascicleDocumentUnit), fascicleDocumentUnitContentTypeExecutor);
        }

        #endregion

        #region [ Methods ]
        public async Task ExecuteAsync(TCommand command)
        {
            _logger.WriteInfo(new LogMessage(string.Concat(command.Name, " is arrived")), LogCategories);

            IBaseCommonExecutor executor;
            IEvent evt;
            IContentBase entity;
            Tuple<DocumentUnit, DocumentUnit> evaluatedMapping = null;
            try
            {
                bool isCommandUpdate = typeof(ICommandCQRSUpdate).IsAssignableFrom(command.GetType());
                _logger.WriteDebug(new LogMessage(string.Concat(command.Name, " has been detect to be ", isCommandUpdate ? "update" : "insert", " process")), LogCategories);

                entity = command.Content.ContentValue as IContentBase;
                executor = _commandExecutors.SingleOrDefault(x => x.Key.IsAssignableFrom(entity.GetType())).Value;
                if (executor == null)
                {
                    throw new ArgumentException($"{command.GetType()} is not assignable to known type {typeof(TCommand)}");
                }
                _logger.WriteDebug(new LogMessage($"Mapping {entity.GetType()} {entity.UniqueId} created by {entity.RegistrationUser}"), LogCategories);

                evaluatedMapping = await EvaluateMappingDocumentUnitAsync(command, executor, entity, isCommandUpdate);
                evt = executor.CreateEvent(command, isCommandUpdate, documentUnit: evaluatedMapping.Item1);
                evt.CorrelatedCommands.Add(command);
                if (!await executor.PushEventAsync(evt))
                {
                    _logger.WriteError(new LogMessage("PushEventAsync Error in sending event to WebAPI"), LogCategories);
                }
                await EvaluateAuthorizationEventsAsync(executor, command, entity, evaluatedMapping);
                await EvaluateWorkflowActionsAsync(command, executor, evt);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"ExecuteAsync Error: {command.GetType()}"), ex, LogCategories);
                throw ex;
            }
        }

        private async Task EvaluateWorkflowActionsAsync(TCommand command, IBaseCommonExecutor baseCommonExecutor, IEvent evt)
        {
            if (typeof(ICQRS).IsAssignableFrom(evt.GetType()) && command.WorkflowActions != null)
            {
                ICQRS cqrsEvt = evt as ICQRS;
                _logger.WriteDebug(new LogMessage($"Evaluate {command.WorkflowActions.Count} workflow actions"), LogCategories);
                foreach (IWorkflowAction workflowAction in command.WorkflowActions)
                {
                    cqrsEvt.WorkflowActions.Add(workflowAction);
                }
                await baseCommonExecutor.CreateWorkflowActionsAsync(command, command.WorkflowActions);
            }
        }

        private async Task EvaluateAuthorizationEventsAsync(IBaseCommonExecutor executor, TCommand command, IContentBase entity,
            Tuple<DocumentUnit, DocumentUnit> evaluatedMapping)
        {
            DocumentUnit documentUnitBeforeSave = evaluatedMapping.Item2;
            DocumentUnit documentUnit = evaluatedMapping.Item1;
            if (documentUnit == null)
            {
                return;
            }

            List<DocumentUnitRole> rolesAdded = new List<DocumentUnitRole>();
            List<DocumentUnitRole> rolesDeleted = new List<DocumentUnitRole>();
            List<DocumentUnitUser> usersAdded = new List<DocumentUnitUser>();
            List<DocumentUnitUser> usersRemoved = new List<DocumentUnitUser>();
            IEventCreateRoleAuthorization createRoleUserAuthorization;
            IEventCreateUserAuthorization createUserAuthorization;
            IEventDeleteRoleAuthorization deleteRoleUserAuthorization;
            IEventDeleteUserAuthorization deleteUserAuthorization;

            if (documentUnitBeforeSave == null)
            {
                rolesAdded.AddRange(documentUnit.DocumentUnitRoles);
                usersAdded.AddRange(documentUnit.DocumentUnitUsers);
            }
            else
            {
                rolesAdded.AddRange(documentUnit.DocumentUnitRoles.Where(p => !documentUnitBeforeSave.DocumentUnitRoles.Any(l => p.UniqueId == l.UniqueId)));
                rolesDeleted.AddRange(documentUnitBeforeSave.DocumentUnitRoles.Where(p => !documentUnit.DocumentUnitRoles.Any(l => p.UniqueId == l.UniqueId)));
                usersAdded.AddRange(documentUnit.DocumentUnitUsers.Where(d => !documentUnitBeforeSave.DocumentUnitUsers.Any(l => d.UniqueId == l.UniqueId)));
                usersRemoved.AddRange(documentUnitBeforeSave.DocumentUnitUsers.Where(d => !documentUnit.DocumentUnitUsers.Any(l => d.UniqueId == l.UniqueId)));
            }

            _logger.WriteDebug(new LogMessage($"Evaluate {rolesAdded.Count} roles added"), LogCategories);
            foreach (DocumentUnitRole role in rolesAdded)
            {
                createRoleUserAuthorization = new EventCreateRoleAuthorization(Guid.NewGuid(), entity.UniqueId, command.TenantName, command.TenantId, command.Identity, documentUnit, role.UniqueIdRole, null);
                if (!await executor.PushEventAsync(createRoleUserAuthorization))
                {
                    _logger.WriteError(new LogMessage("PushEventAsync Error in sending event to WebAPI"), LogCategories);
                }
            }

            _logger.WriteDebug(new LogMessage($"Evaluate {rolesDeleted.Count} roles deleted"), LogCategories);
            foreach (DocumentUnitRole role in rolesDeleted)
            {
                deleteRoleUserAuthorization = new EventDeleteRoleAuthorization(Guid.NewGuid(), entity.UniqueId, command.TenantName, command.TenantId, command.Identity, documentUnitBeforeSave, role.UniqueIdRole, null);
                if (!await executor.PushEventAsync(deleteRoleUserAuthorization))
                {
                    _logger.WriteError(new LogMessage("PushEventAsync Error in sending event to WebAPI"), LogCategories);
                }
            }

            _logger.WriteDebug(new LogMessage($"Evaluate {usersAdded.Count} user added"), LogCategories);
            foreach (DocumentUnitUser user in usersAdded)
            {
                createUserAuthorization = new EventCreateUserAuthorization(Guid.NewGuid(), entity.UniqueId, command.TenantName, command.TenantId, command.Identity, documentUnit, user.Account, null);
                if (!await executor.PushEventAsync(createUserAuthorization))
                {
                    _logger.WriteError(new LogMessage("PushEventAsync Error in sending event to WebAPI"), LogCategories);
                }
            }

            _logger.WriteDebug(new LogMessage($"Evaluate {usersRemoved.Count} user deleted"), LogCategories);
            foreach (DocumentUnitUser user in usersRemoved)
            {
                deleteUserAuthorization = new EventDeleteUserAuthorization(Guid.NewGuid(), entity.UniqueId, command.TenantName, command.TenantId, command.Identity, documentUnitBeforeSave, user.Account, null);
                if (!await executor.PushEventAsync(deleteUserAuthorization))
                {
                    _logger.WriteError(new LogMessage("PushEventAsync Error in sending event to WebAPI"), LogCategories);
                }
            }
        }

        private async Task<Tuple<DocumentUnit, DocumentUnit>> EvaluateMappingDocumentUnitAsync(TCommand command, IBaseCommonExecutor baseCommonExecutor, IContentBase entity, bool isCommandUpdate)
        {
            DocumentUnit documentUnit = null;
            DocumentUnit existDocumentUnit = null;
            if (typeof(IDocumentUnitEntity).IsAssignableFrom(baseCommonExecutor.GetType()))
            {
                documentUnit = await baseCommonExecutor.Mapping(entity, command.Identity, isCommandUpdate);
                existDocumentUnit = await _webApiClient.GetDocumentUnitAsync(documentUnit);
                bool skipSendDocument = existDocumentUnit != null &&
                    existDocumentUnit.UniqueId == documentUnit.UniqueId && existDocumentUnit.Year == documentUnit.Year && existDocumentUnit.Number == documentUnit.Number &&
                    existDocumentUnit.Subject == existDocumentUnit.Subject && existDocumentUnit.Environment == documentUnit.Environment;
                if ((!isCommandUpdate && !skipSendDocument) || isCommandUpdate)
                {
                    await baseCommonExecutor.SendDocumentAsync(documentUnit, isCommandUpdate);
                    _logger.WriteDebug(new LogMessage($"DocumentUnit - {entity.GetType()} - {entity.UniqueId} has been successfully created."), LogCategories);
                }
                else
                {
                    _logger.WriteWarning(new LogMessage($"DocumentUnit - {entity.GetType()} - {entity.UniqueId} already exists and CQRS structures has been skipped."), LogCategories);
                }               
            }            

            return new Tuple<DocumentUnit, DocumentUnit>(documentUnit, existDocumentUnit);
        }
        #endregion
    }
}
