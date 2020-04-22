using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.AUSLRE.RequestOpinon.Configuration;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Core.Command.CQRS;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.Helpers.Workflow;
using VecompSoftware.Services.Command.CQRS.Events.Entities.DocumentUnits;
using ModuleConfigurationModel = VecompSoftware.BPM.Integrations.Modules.AUSLRE.RequestOpinon.Models.ModuleConfigurationModel;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLRE.RequestOpinon
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IWebAPIClient _webAPIClient;
        private readonly IServiceBusClient _serviceBusClient;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private bool _needInitializeModule = false;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(Execution));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        [ImportingConstructor]
        public Execution(ILogger logger, IServiceBusClient serviceBusClient, IWebAPIClient webAPIClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _webAPIClient = webAPIClient;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _serviceBusClient = serviceBusClient;
                _needInitializeModule = true;
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("AUSLRE.RequestOpinion -> Critical error in costruction module"), ex, LogCategories);
                throw;
            }
        }
        #endregion

        #region [ Methods ]
        protected override void Execute()
        {
            if (Cancel)
            {
                return;
            }

            try
            {
                InitializeModule();
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("AUSLRE.RequestOpinion -> Execute critical error"), ex, LogCategories);
                throw;
            }
        }
        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> AUSLRE.RequestOpinion"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCreateUserAuthorization>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration,
                    _moduleConfiguration.WorkflowStartProtocolUserAuthorizationSubscription, EventCreateUserAuthorizationCallback));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventDeleteUserAuthorization>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration, 
                    _moduleConfiguration.WorkflowStartProtocolRemoveUserAuthorizationSubscription, EventDeleteUserAuthorizationCallback));

                _needInitializeModule = false;
            }
        }

        internal void CleanSubscriptions()
        {
            foreach (Guid item in _subscriptions)
            {
                _serviceBusClient.CloseListeningAsync(item).Wait();
            }
            _subscriptions.Clear();
            _needInitializeModule = true;
        }

        #region [ ServiceBus Callbacks ]
        private async Task EventCreateUserAuthorizationCallback(IEventCreateUserAuthorization evt)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"CreateUserAuthorizationCallback -> evaluate event id {evt.Id}"), LogCategories);

                if (!evt.CustomProperties.ContainsKey(CustomPropertyName.AUTHORIZATION_USER))
                {
                    throw new ArgumentException($"unsupport authorization event: {CustomPropertyName.AUTHORIZATION_USER} not specified");
                }
                if (!evt.CustomProperties.ContainsKey(CustomPropertyName.DOCUMENT_UNIT_ID))
                {
                    throw new ArgumentException($"unsupport authorization event: {CustomPropertyName.DOCUMENT_UNIT_ID} not specified");
                }
                if (!evt.CustomProperties.ContainsKey(CustomPropertyName.DSW_ENVIRONMENT) || ((long)evt.CustomProperties[CustomPropertyName.DSW_ENVIRONMENT]) != (int)DSWEnvironmentType.Protocol)
                {
                    throw new ArgumentException($"unsupport authorization event: {CustomPropertyName.DOCUMENT_UNIT_ID} not specified or invalid");
                }
                _logger.WriteInfo(new LogMessage($"Authorization properties {evt.CustomProperties[CustomPropertyName.AUTHORIZATION_USER]} - {evt.CustomProperties[CustomPropertyName.DOCUMENT_UNIT_ID]} - {evt.CustomProperties[CustomPropertyName.DSW_ENVIRONMENT]}"), LogCategories);

                string account = evt.CustomProperties[CustomPropertyName.AUTHORIZATION_USER].ToString();

                DocumentUnit documentUnit = evt.ContentType.ContentTypeValue;
                DocumentUnitUser documentUnitUser = documentUnit.DocumentUnitUsers.SingleOrDefault(f => f.Account.Equals(account, StringComparison.InvariantCultureIgnoreCase));
                Guid documentUnitId = Guid.Parse(evt.CustomProperties[CustomPropertyName.DOCUMENT_UNIT_ID].ToString());

                ICollection<WorkflowActivity> activedWorkflows = await _webAPIClient.GetWorkflowAuthorizedActivitiesByDocumentUnitAsync(documentUnitId, account, _moduleConfiguration.WorkflowRepositoryName);
                if (activedWorkflows != null && activedWorkflows.Count > 0)
                {
                    _logger.WriteWarning(new LogMessage("The message will be ignored"), LogCategories);
                    return;
                }
                //WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel
                //{
                //    ReferenceType = DSWEnvironmentType.Protocol,
                //    ReferenceId = documentUnitId
                //};

                DocumentUnitChain main = documentUnit.DocumentUnitChains.Single(f => f.ChainType == ChainType.MainChain);
                DocumentUnitChain attachments = documentUnit.DocumentUnitChains.SingleOrDefault(f => f.ChainType == ChainType.AttachmentsChain);
                WorkflowDocumentModel workflowDocumentModel = new WorkflowDocumentModel();
                workflowDocumentModel.Documents.Add(new KeyValuePair<DocSuiteWeb.Model.Entities.DocumentUnits.ChainType, DocumentModel>(DocSuiteWeb.Model.Entities.DocumentUnits.ChainType.MainChain, new DocumentModel()
                {
                    ChainType = DocSuiteWeb.Model.Entities.DocumentUnits.ChainType.MainChain,
                    ChainId = main.IdArchiveChain,
                }));
                if (attachments != null)
                {
                    workflowDocumentModel.Documents.Add(new KeyValuePair<DocSuiteWeb.Model.Entities.DocumentUnits.ChainType, DocumentModel>(DocSuiteWeb.Model.Entities.DocumentUnits.ChainType.AttachmentsChain, new DocumentModel()
                    {
                        ChainType = DocSuiteWeb.Model.Entities.DocumentUnits.ChainType.AttachmentsChain,
                        ChainId = attachments.IdArchiveChain,
                    }));
                }
                WorkflowResult workflowResult = await StartWorkflowAsync(workflowDocumentModel, account, _moduleConfiguration.WorkflowRepositoryName, documentUnit, documentUnitUser);

                if (!workflowResult.IsValid)
                {
                    _logger.WriteError(new LogMessage("An error occured in start request opinion workflow"), LogCategories);
                    throw new Exception("VecompSoftware.BPM.Integrations.Modules.AUSLRE.RequestOpinion");
                }
                _logger.WriteInfo(new LogMessage(string.Concat("Workflow started correctly [IsValid: ", workflowResult.IsValid, "] with instanceId ", workflowResult.InstanceId)), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("CreateUserAuthorizationCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        private async Task EventDeleteUserAuthorizationCallback(IEventDeleteUserAuthorization evt)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"DeleteUserAuthorizationCallback -> evaluate event id {evt.Id}"), LogCategories);
                if (!evt.CustomProperties.ContainsKey(CustomPropertyName.AUTHORIZATION_USER))
                {
                    throw new ArgumentException($"unsupport authorization event: {CustomPropertyName.AUTHORIZATION_USER} not specified");
                }
                if (!evt.CustomProperties.ContainsKey(CustomPropertyName.DSW_ENVIRONMENT) || ((long)evt.CustomProperties[CustomPropertyName.DSW_ENVIRONMENT]) != (int)DSWEnvironmentType.Protocol)
                {
                    throw new ArgumentException($"unsupport authorization event: {CustomPropertyName.DOCUMENT_UNIT_ID} not specified or invalid");
                }

                string account = evt.CustomProperties[CustomPropertyName.AUTHORIZATION_USER].ToString();
                DocumentUnit documentUnit = evt.ContentType.ContentTypeValue;
                ICollection<WorkflowActivity> activedWorkflows = await _webAPIClient.GetWorkflowAuthorizedActivitiesByDocumentUnitAsync(documentUnit.UniqueId, account, _moduleConfiguration.WorkflowRepositoryName);
                if (activedWorkflows == null || activedWorkflows.Count == 0)
                {
                    _logger.WriteWarning(new LogMessage($"Authorized activities not found for document unit {documentUnit.UniqueId}. The message will be ignored"), LogCategories);
                    return;
                }

                if (activedWorkflows.Count > 1)
                {
                    throw new Exception($"Found more than 1 authorized activities for document unit {documentUnit.UniqueId}.");
                }

                WorkflowActivity activity = activedWorkflows.Single();
                activity.Status = WorkflowStatus.LogicalDelete;
                await _webAPIClient.PutAsync(activity);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("DeleteUserAuthorizationCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        private async Task<WorkflowResult> StartWorkflowAsync(WorkflowDocumentModel workflowDocumentModel, string destinationAccount,
        string workflowName, DocumentUnit documentUnit, DocumentUnitUser documentUnitUser)
        {
            WorkflowStart workflowStart = new WorkflowStart()
            {
                WorkflowName = workflowName
            };

            //workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, new WorkflowArgument()
            //{
            //    Name = WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL,
            //    PropertyType = ArgumentType.Json,
            //    ValueString = JsonConvert.SerializeObject(workflowReferenceModel)
            //});

            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, new WorkflowArgument()
            {
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID,
                PropertyType = ArgumentType.PropertyGuid,
                ValueGuid = _moduleConfiguration.TenantId
            });

            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, new WorkflowArgument()
            {
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME,
                PropertyType = ArgumentType.PropertyString,
                ValueString = _moduleConfiguration.TenantName​
            });

            DomainUserModel account = await _webAPIClient.GetUserAsync(documentUnitUser.RegistrationUser) ?? new DomainUserModel() { Name = documentUnitUser.RegistrationUser, DisplayName = documentUnitUser.RegistrationUser };
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_USER, new WorkflowArgument()
            {
                Name = WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_USER,
                PropertyType = ArgumentType.Json,
                ValueString = JsonConvert.SerializeObject(new WorkflowAccount()
                {
                    AccountName = account.Account,
                    DisplayName = account.DisplayName,
                    EmailAddress = account.EmailAddress
                })
            });
            account = await _webAPIClient.GetUserAsync(destinationAccount) ?? new DomainUserModel() { Name = destinationAccount, DisplayName = destinationAccount };
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS, new WorkflowArgument()
            {
                Name = WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS,
                PropertyType = ArgumentType.Json,
                ValueString = JsonConvert.SerializeObject(new List<WorkflowAccount>() { new WorkflowAccount()
                {
                    AccountName = account.Account,
                    DisplayName = account.DisplayName,
                    EmailAddress = account.EmailAddress
                }})
            });

            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT, new WorkflowArgument()
            {
                Name = WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT,
                PropertyType = ArgumentType.PropertyString,
                ValueString = documentUnit.Subject
            });
            
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_REFERENCE_MODEL, new WorkflowArgument()
            {
                Name = WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_REFERENCE_MODEL,
                PropertyType = ArgumentType.Json,
                ValueString = JsonConvert.SerializeObject(new WorkflowReferenceModel()
                {
                    ReferenceType = DSWEnvironmentType.Desk,
                    ReferenceModel = JsonConvert.SerializeObject(workflowDocumentModel, ModuleConfigurationHelper.JsonSerializerSettings)
                }, ModuleConfigurationHelper.JsonSerializerSettings)
            });

            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_MOTIVATION, new WorkflowArgument()
            {
                Name = WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_MOTIVATION,
                PropertyType = ArgumentType.PropertyString,
                ValueString = $"{documentUnit.Title}-{documentUnit.Subject}"
            });

            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_ACTION_SET_AUDITABLE_PROPERTIES, new WorkflowArgument()
            {
                Name = WorkflowPropertyHelper.DSW_ACTION_SET_AUDITABLE_PROPERTIES,
                PropertyType = ArgumentType.Json,
                ValueString = JsonConvert.SerializeObject(new WorkflowReferenceModel()
                {
                    ReferenceType = DSWEnvironmentType.Workflow,
                    Title = WorkflowPropertyHelper.DSW_ACTION_SET_AUDITABLE_PROPERTIES,
                    ReferenceModel = JsonConvert.SerializeObject(new DocumentUnit { RegistrationUser = documentUnitUser.RegistrationUser, RegistrationDate = documentUnitUser.RegistrationDate }, ModuleConfigurationHelper.JsonSerializerSettings)
                }, ModuleConfigurationHelper.JsonSerializerSettings)
            });
            WorkflowResult workflowResult = await _webAPIClient.StartWorkflow(workflowStart);
            return workflowResult;
        }
    }
    #endregion


    #endregion

}
