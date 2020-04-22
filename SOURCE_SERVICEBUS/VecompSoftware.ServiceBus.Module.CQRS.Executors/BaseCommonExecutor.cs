using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Workflow.Actions;
using VecompSoftware.ServiceBus.Module.CQRS.Executors.Executors.Workflows;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.ServiceBus.Module.CQRS.Executors
{
    public abstract class BaseCommonExecutor : IBaseCommonExecutor
    {
        #region [ Fields ]
        private readonly BiblosDS.BiblosClient _biblosClient;
        private readonly IWebAPIClient _webApiClient;
        private readonly ILogger _logger;
        protected static IEnumerable<LogCategory> _logCategories = null;
        private readonly IDictionary<Type, IWorkflowActionExecutor> _workflowActionExecutors;
        #endregion

        #region [ Constructor ]
        public BaseCommonExecutor(ILogger logger, IWebAPIClient webApiClient, BiblosDS.BiblosClient biblosClient)
        {
            _webApiClient = webApiClient;
            _biblosClient = biblosClient;
            _logger = logger;
            _workflowActionExecutors = new Dictionary<Type, IWorkflowActionExecutor>();
            IWorkflowActionFascicleExecutor workflowActionFascicleExecutor = new WorkflowActionFascicleExecutor(logger, webApiClient);
            IWorkflowActionDocumentUnitLinkExecutor workflowActionDocumentUnitLinkExecutor = new WorkflowActionDocumentUnitLinkExecutor(logger, webApiClient);
            _workflowActionExecutors.Add(typeof(WorkflowActionFascicleModel), workflowActionFascicleExecutor);
            _workflowActionExecutors.Add(typeof(WorkflowActionDocumentUnitLinkModel), workflowActionDocumentUnitLinkExecutor);
        }
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(BaseCommonExecutor));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Methods ]

        internal abstract IEvent CreateInsertEvent(ICommandCQRSCreate command, DocumentUnit documentUnit = null);

        internal abstract IEvent CreateUpdateEvent(ICommandCQRSUpdate command, DocumentUnit documentUnit = null);

        public IEvent CreateEvent(ICommandCQRS command, bool isUpdate, DocumentUnit documentUnit = null)
        {
            IEvent evt = null;
            if (!isUpdate)
            {
                ICommandCQRSCreate cQRSCreate = (ICommandCQRSCreate)command;
                evt = CreateInsertEvent(cQRSCreate, documentUnit);
            }
            if (isUpdate)
            {
                ICommandCQRSUpdate cQRSUpdate = (ICommandCQRSUpdate)command;
                evt = CreateUpdateEvent(cQRSUpdate, documentUnit);
            }
            return evt;
        }

        public async Task CreateWorkflowActionsAsync(ICommandCQRS command, ICollection<IWorkflowAction> workflowActions)
        {
            foreach (IWorkflowAction workflowAction in workflowActions)
            {
                foreach (IWorkflowActionExecutor workflowExecutor in _workflowActionExecutors.Where(x => workflowAction.GetType() == x.Key).Select(s => s.Value))
                {
                    await workflowExecutor.CreateActionEventAsync(command, workflowAction);
                }
            }
        }

        public async Task<DocumentUnit> Mapping(IContentBase entity, IIdentityContext identity, bool isUpdate)
        {
            DocumentUnit doc = null;
            DocumentUnit currentDocumentUnit;
            if (!isUpdate)
            {
                doc = await MappingInsertAsync(entity, identity);
            }
            if (isUpdate)
            {
                currentDocumentUnit = await _webApiClient.GetDocumentUnitAsync(new DocumentUnit(entity.UniqueId));
                if (currentDocumentUnit == null)
                {
                    string message = string.Concat("DoumentUnit ", entity.GetType().Name, " not found with UniqueId ", entity.UniqueId);
                    _logger.WriteError(new LogMessage(message), LogCategories);
                    throw new NullReferenceException(message);
                }
                doc = await MappingUpdateAsync(entity, currentDocumentUnit, identity);
            }
            return doc;
        }
        internal abstract Task<DocumentUnit> MappingInsertAsync(IContentBase entity, IIdentityContext identity);

        internal abstract Task<DocumentUnit> MappingUpdateAsync(IContentBase entity, DocumentUnit documentUnit, IIdentityContext identity);

        internal async Task<DocumentUnit> PostDocumentUnitAsync(DocumentUnit documentUnit)
        {
            try
            {
                return await _webApiClient.PostDocumentUnitAsync(documentUnit);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("PostDocumentUnitAsync Error: DocumentUnit not sended to WebAPI"), ex, LogCategories);
                throw ex;
            }
        }

        internal async Task<DocumentUnitFascicleCategory> PostDocumentUnitFascicleCategory(DocumentUnitFascicleCategory documentUnitFascicleCategory)
        {
            try
            {
                return await _webApiClient.PostEntityAsync(documentUnitFascicleCategory);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("PostDocumentUnitFascicleCategory Error: DocumentUnitFascicleCategory not sended to WebAPI"), ex, LogCategories);
                throw ex;
            }
        }

        internal async Task<DocumentUnitFascicleCategory> DeleteDocumentUnitFascicleCategory(DocumentUnitFascicleCategory documentUnitFascicleCategory)
        {
            try
            {
                return await _webApiClient.DeleteEntityAsync(documentUnitFascicleCategory);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("DeleteDocumentUnitFascicleCategory Error: DocumentUnitFascicleCategory not sended to WebAPI"), ex, LogCategories);
                throw ex;
            }
        }

        internal async Task<DocumentUnitFascicleCategory> GetDocumentUnitFascicleCategory(FascicleDocumentUnit fascicleDocumentUnit)
        {
            try
            {
                return await _webApiClient.GetDocumentUnitFascicleCategoryAsync(fascicleDocumentUnit.DocumentUnit.UniqueId, 
                    fascicleDocumentUnit.Fascicle.Category.EntityShortId, 
                    fascicleDocumentUnit.Fascicle.UniqueId);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("GetDocumentUnitFascicleCategory Error: FascicleDocumentUnit not sended to WebAPI"), ex, LogCategories);
                throw ex;
            }
        }

        internal async Task<DocumentUnitFascicleHistoricizedCategory> PostDocumentUnitFascicleHistoricizedCategory(DocumentUnitFascicleHistoricizedCategory documentUnitFascicleHistoricizedCategory)
        {
            try
            {
                return await _webApiClient.PostEntityAsync(documentUnitFascicleHistoricizedCategory);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("PostDocumentUnitFascicleHistoricizedCategory Error: DocumentUnitFascicleHistoricizedCategory not sended to WebAPI"), ex, LogCategories);
                throw ex;
            }
        }

        internal async Task<DocumentUnit> PutDocumentUnitAsync(DocumentUnit documentUnit)
        {
            try
            {
                return await _webApiClient.PutDocumentUnitAsync(documentUnit);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("PutDocumentUnitAsync Error: DocumentUnit not sended to WebAPI"), ex, LogCategories);
                throw ex;
            }
        }
        public void AddDocumentUnitChain(DocumentUnit documentUnit, Guid chainId, ChainType chainType, IIdentityContext identity, string archiveName, string documentName = null, string label = null)
        {
            documentUnit.DocumentUnitChains.Add(new DocumentUnitChain()
            {
                ArchiveName = archiveName,
                ChainType = chainType,
                DocumentName = chainType == ChainType.MainChain ? documentName : null,
                IdArchiveChain = chainId,
                RegistrationDate = DateTimeOffset.UtcNow,
                RegistrationUser = identity.User,
                DocumentLabel = label
            });
        }

        protected AuthorizationRoleType GetRoleType(string roleType)
        {
            switch (roleType)
            {
                case "P":
                    {
                        return AuthorizationRoleType.Responsible;
                    }
                case "CC":
                    {
                        return AuthorizationRoleType.Informed;
                    }

                default:
                    {
                        return AuthorizationRoleType.Accounted;
                    }
            }
        }


        public async Task<DocumentUnit> SendDocumentAsync(DocumentUnit documentUnit, bool isUpdate)
        {
            DocumentUnit doc = null;

            if (!isUpdate)
            {
                doc = await PostDocumentUnitAsync(documentUnit);
            }

            if (isUpdate)
            {
                doc = await PutDocumentUnitAsync(documentUnit);
            }

            return doc;
        }
        public async Task<bool> PushEventAsync(IEvent evt)
        {
            try
            {
                return await _webApiClient.PushEventAsync(evt);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("PushEventAsync Error: not sended to WebAPI"), ex, LogCategories);
                throw ex;
            }

        }

    }


    #endregion
}
