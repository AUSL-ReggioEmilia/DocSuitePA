using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.DGROOVE.ImportCedolini.Models;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Commands.Models.UDS;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.Helpers.UDS;
using VecompSoftware.Helpers.Workflow;
using VecompSoftware.Services.Command.CQRS.Events.Models.UDS;

namespace VecompSoftware.BPM.Integrations.Modules.DGROOVE.ImportCedolini
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
        private readonly IdentityContext _identityContext = null;
        #endregion

        #region [ UDS Metadata ]
        private const string CEDOLINI_MATRICOLA_FIELD = "CodiceDipendente";
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
        public Execution(ILogger logger, IWebAPIClient webAPIClient, IServiceBusClient serviceBusClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _webAPIClient = webAPIClient;
                _serviceBusClient = serviceBusClient;
                string username = "anonymous";
                _needInitializeModule = true;
                if (WindowsIdentity.GetCurrent() != null)
                {
                    username = WindowsIdentity.GetCurrent().Name;
                }
                _identityContext = new IdentityContext(username);

                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("VecompSoftware.BPM.Integrations.Modules.DGROOVE.ImportCedolini -> Critical error in costruction module"), ex, LogCategories);
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
                _logger.WriteError(new LogMessage("VecompSoftware.BPM.Integrations.Modules.DGROOVE.ImportCedolini -> Execute critical error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> DGROOVE.ImportCedolini"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCQRSCreateUDSData>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration,
                    _moduleConfiguration.WorkflowUDSCedoliniCompleteSubscription, UDSCompletedCallbackAsync));

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

        private async Task UDSCompletedCallbackAsync(IEventCQRSCreateUDSData eventMessage, IDictionary<string, object> properties)
        {
            if (Cancel)
            {
                return;
            }

            _logger.WriteInfo(new LogMessage(string.Format("UDSCompletedCallbackAsync -> received callback with eventId {0}", eventMessage.Id)), LogCategories);

            try
            {
                DocumentUnit documentUnit = eventMessage.DocumentUnit;
                UDSRepository udsRepository = (await _webAPIClient.GetUDSRepository(documentUnit.UDSRepository.UniqueId)).SingleOrDefault();

                _logger.WriteInfo(new LogMessage("Reading matricola from UDS"), LogCategories);
                string controllerName = Utils.GetWebAPIControllerName(udsRepository.Name);
                Dictionary<int, Guid> uds_documents = new Dictionary<int, Guid>();
                IDictionary<string, object> uds_metadatas = await _webAPIClient.GetUDS(controllerName, documentUnit.UniqueId, uds_documents);
                string matricola = GetMatricolaMetadata(uds_metadatas);

                _logger.WriteInfo(new LogMessage($"Evaluate user by matricola {matricola}"), LogCategories);
                DomainUserModel account = await GetEvaluatedUserbyMatricolaAsync(matricola);

                _logger.WriteInfo(new LogMessage($"Found account {account.Account} for matricola {matricola}"), LogCategories);
                UDSBuildModel toUpdateModel = await BuildAuthorizedUDSModelAsync(documentUnit, udsRepository, account, uds_metadatas, uds_documents);

                _logger.WriteInfo(new LogMessage($"Authorize account {account.Account} for UDS {documentUnit.UniqueId}"), LogCategories);
                CommandUpdateUDSData commandUpdateUDS = new CommandUpdateUDSData(_moduleConfiguration.TenantName, _moduleConfiguration.TenantId, _moduleConfiguration.TenantAOOId, _identityContext, toUpdateModel);
                await _webAPIClient.SendCommandAsync(commandUpdateUDS);

                _logger.WriteDebug(new LogMessage($"Preparing starting workflow with udsUniqueId {documentUnit.UniqueId},"), LogCategories);
                WorkflowReferenceModel referenceModel = new WorkflowReferenceModel()
                {
                    ReferenceModel = JsonConvert.SerializeObject(documentUnit, ModuleConfigurationHelper.JsonSerializerSettings),
                    ReferenceType = DocSuiteWeb.Model.Entities.Commons.DSWEnvironmentType.UDS
                };
                WorkflowResult workflowResult = await StartWorkflowAsync(referenceModel, account, _moduleConfiguration.WorkflowRepositoryName);
                if (!workflowResult.IsValid || !workflowResult.InstanceId.HasValue)
                {
                    _logger.WriteError(new LogMessage($"An error occured in start {_moduleConfiguration.WorkflowRepositoryName} workflow"), LogCategories);
                    throw new Exception(string.Join(", ", workflowResult.Errors));
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("UDSCompletedCallbackAsync -> error on complete activity"), ex, LogCategories);
                throw;
            }
        }

        private string GetMatricolaMetadata(IDictionary<string, object> uds_metadatas)
        {
            KeyValuePair<string, object> matricolaField = uds_metadatas.SingleOrDefault(x => x.Key == CEDOLINI_MATRICOLA_FIELD);
            if (matricolaField.Value == null)
            {
                throw new ArgumentException($"Field {CEDOLINI_MATRICOLA_FIELD} not found");
            }
            return matricolaField.Value.ToString();
        }

        private async Task<DomainUserModel> GetEvaluatedUserbyMatricolaAsync(string matricola)
        {
            if (!_moduleConfiguration.MatricolaUtenti.ContainsKey(matricola))
            {
                throw new ArgumentException($"Matricola {matricola} not found. Check module configuration file.");
            }

            string user = _moduleConfiguration.MatricolaUtenti[matricola];
            DomainUserModel result = await _webAPIClient.GetUserAsync(user);
            if (result == null)
            {
                throw new ArgumentException($"User {user} not found for matricola {matricola}.");
            }
            return result;
        }

        private async Task<UDSBuildModel> BuildAuthorizedUDSModelAsync(DocumentUnit documentUnit, UDSRepository udsRepository, DomainUserModel account, IDictionary<string, object> uds_metadatas,
            Dictionary<int, Guid> uds_documents)
        {
            ICollection<UDSRole> udsRoles = await _webAPIClient.GetUDSRoles(documentUnit.UniqueId);
            ICollection<UDSContact> udsContacts = await _webAPIClient.GetUDSContacts(documentUnit.UniqueId);
            ICollection<UDSMessage> udsMessages = await _webAPIClient.GetUDSMessages(documentUnit.UniqueId);
            ICollection<UDSPECMail> udsPECMails = await _webAPIClient.GetUDSPECMails(documentUnit.UniqueId);
            ICollection<UDSDocumentUnit> udsDocumentUnits = await _webAPIClient.GetUDSDocumentUnits(documentUnit.UniqueId, false, false);

            return PrepareUpdateUDSBuildModel(udsRepository, documentUnit.UniqueId, uds_metadatas, uds_documents, udsRoles, udsContacts, udsMessages, udsPECMails, udsDocumentUnits, account);
        }

        private UDSBuildModel PrepareUpdateUDSBuildModel(UDSRepository udsRepository, Guid IdUDS, IDictionary<string, object> uds_metadatas, Dictionary<int, Guid> documents,
            ICollection<UDSRole> udsRoles, ICollection<UDSContact> udsContacts, ICollection<UDSMessage> udsMessages, ICollection<UDSPECMail> udsPECMails,
            ICollection<UDSDocumentUnit> udsDocumentUnits, DomainUserModel account)
        {
            UDSModel model = UDSModel.LoadXml(udsRepository.ModuleXML);

            foreach (Section metadata in model.Model.Metadata)
            {
                foreach (FieldBaseType item in metadata.Items)
                {
                    UDSModelField udsField = new UDSModelField(item)
                    {
                        Value = uds_metadatas.Single(f => f.Key == item.ColumnName).Value
                    };
                }
            }

            model.Model.Title = udsRepository?.Name;
            model.Model.Subject.Value = uds_metadatas["_subject"].ToString();
            model.Model.Category.IdCategory = uds_metadatas["IdCategory"].ToString();

            ICollection<Guid> mainDocuments = documents.Where(x => x.Key == (int)Helpers.UDS.UDSDocumentType.Main).Select(s => s.Value).ToList();
            ICollection<Guid> attachmentsDocuments = documents.Where(x => x.Key == (int)Helpers.UDS.UDSDocumentType.Attachment).Select(s => s.Value).ToList();
            ICollection<Guid> annexedDocuments = documents.Where(x => x.Key == (int)Helpers.UDS.UDSDocumentType.Annexed).Select(s => s.Value).ToList();
            ICollection<Guid> dematerialisationDocuments = documents.Where(x => x.Key == (int)Helpers.UDS.UDSDocumentType.Dematerialisation).Select(s => s.Value).ToList();

            model.FillDocuments(mainDocuments);
            model.FillDocumentAttachments(attachmentsDocuments);
            model.FillDocumentAnnexed(annexedDocuments);
            model.FillDocumentDematerialisation(dematerialisationDocuments);

            UDSContact authContact = new UDSContact()
            {
                ContactLabel = model.Model.Contacts.First(x => x.ContactType == Helpers.UDS.ContactType.AccountAuthorization).Label,
                ContactManual = JsonConvert.SerializeObject(new
                {
                    Contact = new
                    {
                        Description = $"{account.DisplayName}",
                        Code = account.Account,
                        EmailAddress = account.EmailAddress,
                        ContactType = new
                        {
                            ContactTypeId = DocSuiteWeb.Entity.Commons.ContactType.Citizen
                        }
                    },
                    Type = 0
                })
            };

            udsContacts.Add(authContact);

            IList<UDSContact> contacts;
            foreach (Contacts modelContacts in model.Model.Contacts)
            {
                contacts = udsContacts.Where(x => x.ContactLabel == modelContacts.Label).ToList();
                foreach (UDSContact contact in contacts)
                {
                    if (contact.ContactType.HasValue && contact.ContactType.Value == (short)Helpers.UDS.UDSContactType.Contact)
                    {
                        if (contact.Relation != null)
                        {
                            modelContacts.ContactInstances = (modelContacts.ContactInstances ?? Enumerable.Empty<ContactInstance>()).Concat(new ContactInstance[] { new ContactInstance() { IdContact = contact.Relation.EntityId } }).ToArray();
                        }
                    }
                    else
                    {
                        modelContacts.ContactManualInstances = (modelContacts.ContactManualInstances ?? Enumerable.Empty<ContactManualInstance>()).Concat(new ContactManualInstance[] { new ContactManualInstance() { ContactDescription = contact.ContactManual } }).ToArray();
                    }
                }
            }

            IEnumerable<ReferenceModel> referenceModels = null;
            if (model.Model.Authorizations != null)
            {
                referenceModels = udsRoles.Select(s => new ReferenceModel() { EntityId = s.Relation.EntityShortId, UniqueId = s.UniqueId, AuthorizationType = AuthorizationType.Accounted });
                model.FillAuthorizations(referenceModels, model.Model.Authorizations.Label);
            }

            referenceModels = udsDocumentUnits.Select(s => new ReferenceModel() { UniqueId = s.Relation.UniqueId });
            model.FillProtocols(referenceModels);

            referenceModels = udsMessages.Select(s => new ReferenceModel() { EntityId = s.Relation.EntityId, UniqueId = s.UniqueId });
            model.FillMessages(referenceModels);

            referenceModels = udsPECMails.Select(s => new ReferenceModel() { EntityId = s.Relation.EntityId, UniqueId = s.UniqueId });
            model.FillPECMails(referenceModels);

            UDSBuildModel udsBuildModel = new UDSBuildModel(model.SerializeToXml())
            {
                UDSRepository = new UDSRepositoryModel(udsRepository.UniqueId)
                {
                    ActiveDate = udsRepository.ActiveDate,
                    ExpiredDate = udsRepository.ExpiredDate,
                    ModuleXML = udsRepository.ModuleXML,
                    Name = udsRepository.Name,
                    Status = DocSuiteWeb.Model.Entities.UDS.UDSRepositoryStatus.Confirmed,
                    Version = udsRepository.Version,
                    DSWEnvironment = udsRepository.DSWEnvironment,
                    Alias = udsRepository.Alias
                },
                UniqueId = IdUDS,
                RegistrationUser = _identityContext.User
            };
            return udsBuildModel;
        }

        private async Task<WorkflowResult> StartWorkflowAsync(WorkflowReferenceModel workflowReferenceModelUDS, DomainUserModel account, string workflowName)
        {
            WorkflowStart workflowStart = new WorkflowStart
            {
                WorkflowName = workflowName
            };
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
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, new WorkflowArgument()
            {
                Name = WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL,
                PropertyType = ArgumentType.Json,
                ValueString = JsonConvert.SerializeObject(workflowReferenceModelUDS)
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.RelationGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID,
                ValueGuid = _moduleConfiguration.TenantId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.RelationGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID,
                ValueGuid = _moduleConfiguration.TenantAOOId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME,
                ValueString = _moduleConfiguration.TenantName
            });
            WorkflowResult workflowResult = await _webAPIClient.StartWorkflow(workflowStart);
            _logger.WriteInfo(new LogMessage(string.Concat("Workflow started correctly [IsValid: ", workflowResult.IsValid, "] with instanceId ", workflowResult.InstanceId)), LogCategories);
            return workflowResult;
        }
        #endregion        
    }
}
