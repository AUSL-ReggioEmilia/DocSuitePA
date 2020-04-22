using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.ENAV.ERP.Contract.Configurations;
using VecompSoftware.BPM.Integrations.Modules.ENAV.ERP.Contract.Models;
using VecompSoftware.BPM.Integrations.Modules.ENAV.ERP.Data;
using VecompSoftware.BPM.Integrations.Modules.ENAV.ERP.Data.Entities;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.BiblosDS.DocumentService;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Events.Entities.Workflows;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.Metadata;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.Helpers.Workflow;
using VecompSoftware.Services.Command.CQRS.Events.Entities.Workflow;
using VecompSoftware.Services.Command.CQRS.Events.Models.ExternalSecurities;
using ERPDocumentType = VecompSoftware.BPM.Integrations.Modules.ENAV.ERP.Data.Entities.DocumentType;

namespace VecompSoftware.BPM.Integrations.Modules.ENAV.ERP.Contract
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private const string _fascicle_metadata_oda = "ODA";
        private const string _fascicle_metadata_erpFascicleId = "Identificativo ERP";
        private const string _workflow_metadata_documentType = "TipoDocumento";
        private const string _workflow_metadata_owner = "Owner";
        private const string _workflow_metadata_oda = "Oda";
        private const string _workflow_metadata_contract = "Contratto";
        private const string _workflow_metadata_cig = "CIG";
        private const string _workflow_metadata_requestId = "IDRichiesta";
        private const string _erp_verb_inserted = "Inserito";
        private const string _erp_verb_opened = "Aperto";
        private const string _erp_subject_protocol = "Protocollo";
        private const string _erp_subject_fascicle = "Fascicolo";

        private static readonly HostIdentify _hostIdentify = new HostIdentify(Environment.MachineName, ModuleConfigurationHelper.MODULE_NAME);

        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly ILogger _logger;
        private readonly IServiceBusClient _serviceBusClient;
        private readonly IWebAPIClient _webAPIClient;
        private readonly IDocumentClient _documentClient;
        private readonly IdentityContext _identityContext = null;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private bool _needInitializeModule = false;
        private ERPDbContext _dbContext;
        private readonly MetadataModel _metadataModel;
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
        public Execution(ILogger logger, IServiceBusClient serviceBusClient, IWebAPIClient webAPIClient, IDocumentClient documentClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _serviceBusClient = serviceBusClient;
                _webAPIClient = webAPIClient;
                _documentClient = documentClient;
                string username = "anonymous";
                _needInitializeModule = true;
                if (WindowsIdentity.GetCurrent() != null)
                {
                    username = WindowsIdentity.GetCurrent().Name;
                }
                _identityContext = new IdentityContext(username);
                MetadataRepository metadataRepository = _webAPIClient.GetMetadataRepositoryAsync($"$filter=UniqueId eq {_moduleConfiguration.MetadataRepositoryId}").Result.Single();
                _metadataModel = JsonConvert.DeserializeObject<MetadataModel>(metadataRepository.JsonMetadata);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("ERP.Contract -> Critical error in costruction module"), ex, LogCategories);
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

                Command currentCommandEvaluate = null;
                int successfullCount = 0;
                int totalCount = 0;
                WorkflowRoleMapping workflowRoleMapping;
                Contact contact;
                Fascicle fascicle;
                WorkflowResult workflowResult;
                ICollection<Contact> contacts;
                ICollection<Fascicle> fascicles;
                string erpFascicleId;
                Random random = new Random();
                foreach (Command command in _dbContext.Commands.Where(f => f.ProcessedTime == null))
                {
                    totalCount++;
                    try
                    {
                        currentCommandEvaluate = command;
                        erpFascicleId = string.Concat(DateTime.Today.Year, DateTime.Today.DayOfYear, random.Next(0, 100));
                        _logger.WriteDebug(new LogMessage(string.Concat("Evaluating command: RequestId ", command.RequestId, ", ODA ",
                            command.ODA, ", CIG ", command.CIG, ", Contact ", command.Contact, ", Owner ", command.Owner)), LogCategories);

                        workflowRoleMapping = FindMappingTag(command);

                        _logger.WriteDebug(new LogMessage($"Finding contact {command.Contact}"), LogCategories);
                        contacts = _webAPIClient.GetContactAsync($"$filter=Description eq '{WebUtility.UrlEncode(command.Contact.Replace("'", "''"))}' and IncrementalFather eq {_moduleConfiguration.ContactFatherId}").Result;
                        contact = contacts.FirstOrDefault();
                        if (contact == null)
                        {
                            _logger.WriteDebug(new LogMessage($"Contact '{command.Contact}' not found. Creating in progress"), LogCategories);
                            contact = CreateContactAsync(command).Result;
                        }

                        _logger.WriteDebug(new LogMessage($"Finding fascicle by ODA {command.ODA} metadata"), LogCategories);
                        fascicles = _webAPIClient.GetFasciclesAsync($"$filter=contains(MetadataValues,'{command.ODA}')").Result;
                        fascicle = fascicles.FirstOrDefault();
                        if (fascicle == null &&
                            (command.DocumentType.Equals(ERPDocumentType.LOA_Numero_1, StringComparison.InvariantCultureIgnoreCase) ||
                             command.DocumentType.Equals(ERPDocumentType.Contratto_Principale, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            _logger.WriteDebug(new LogMessage($"Fascicle '{command.ODA}' not found"), LogCategories);
                            fascicle = CreateFascicle(workflowRoleMapping, erpFascicleId, command);
                        }

                        _logger.WriteDebug(new LogMessage($"Starting workflow for RequestId {command.RequestId}"), LogCategories);
                        workflowResult = StartWorkflow(command, contact, fascicle, workflowRoleMapping);

                        command.ProcessedTime = DateTime.Now;
                        _dbContext.SaveChanges();
                        _logger.WriteInfo(new LogMessage($"RequestId {command.RequestId} has been completed"), LogCategories);
                        successfullCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteError(new LogMessage($"ERP.Contract -> Error on evaluating command id: {currentCommandEvaluate.RequestId}"), ex, LogCategories);
                    }

                }
                _logger.WriteInfo(new LogMessage($"Contract {successfullCount}/{totalCount} has been successfully evaluated"), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("ENAV.ERP.Contract -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _dbContext.Dispose();
            _logger.WriteInfo(new LogMessage("OnStop -> ENAV.ERP.Contract"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _dbContext = new ERPDbContext(_logger, ModuleConfigurationHelper.JsonSerializerSettings, _moduleConfiguration.ConnectionString);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventTokenSecurity>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.WorkflowIntegrationTopic,
                    _moduleConfiguration.SecurityTopicSubscription, EventTokenSecurityCallbackAsync));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompleteWorkflowInstance>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.WorkflowCompletedTopic, 
                    _moduleConfiguration.WorkflowERPContrattiSubscription, EventCompleteWorkflowInstanceCallbackAsync));

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

        private WorkflowRoleMapping FindMappingTag(Command command)
        {
            ICollection<WorkflowRoleMapping> workflowRoleMappings = _webAPIClient.GetWorkflowRoleMappingAsync(string.Concat("$filter=MappingTag eq '", command.Owner,
                "' and WorkflowRepository/UniqueId eq ", _moduleConfiguration.WorkflowRepositoryId, "&$expand=Role,WorkflowRepository")).Result;

            if (!workflowRoleMappings.Any() || workflowRoleMappings.Count > 1)
            {
                throw new ArgumentException($"ERP Owner [{command.Owner}] not correctly configurated in SkyDoc Role Mapping. The ERPCommand has ID: {command.RequestId}");
            }
            WorkflowRoleMapping workflowRoleMapping = workflowRoleMappings.Single();
            _logger.WriteDebug(new LogMessage($"Found mapping tag {workflowRoleMapping.Role.UniqueId} - {workflowRoleMapping.Role.Name}"), LogCategories);
            return workflowRoleMapping;
        }

        private async Task<Contact> CreateContactAsync(Command command)
        {
            Guid uniqueId = Guid.NewGuid();
            Contact contact = new Contact()
            {
                IdContactType = DocSuiteWeb.Entity.Commons.ContactType.AOO,
                IncrementalFather = _moduleConfiguration.ContactFatherId,
                Description = command.Contact,
                IsActive = 1,
                IsLocked = 0,
                IsChanged = 0,
                CertifiedMail = command.ContactPEC,
                EmailAddress = command.ContactPEC,
                UniqueId = uniqueId
            };
            contact = await _webAPIClient.PostAsync(contact);
            contact = (await _webAPIClient.GetContactAsync($"$filter=UniqueId eq {uniqueId}")).Single();
            _logger.WriteInfo(new LogMessage($"Contact '{command.Contact}' ({contact.EntityId}) has been create succesfully"), LogCategories);
            return contact;
        }

        private Fascicle CreateFascicle(WorkflowRoleMapping workflowRoleMapping, string erpFascicleId, Command command)
        {
            _metadataModel.TextFields.Single(f => f.Label.Equals(_fascicle_metadata_oda, StringComparison.InvariantCultureIgnoreCase)).Value = command.ODA;
            _metadataModel.TextFields.Single(f => f.Label.Equals(_fascicle_metadata_erpFascicleId, StringComparison.InvariantCultureIgnoreCase)).Value = erpFascicleId;
            Fascicle fascicle = new Fascicle()
            {
                Category = new Category() { EntityShortId = _moduleConfiguration.CategoryId },
                FascicleType = FascicleType.Procedure,
                MetadataValues = JsonConvert.SerializeObject(_metadataModel),
                VisibilityType = VisibilityType.Confidential,
                FascicleObject = $"Contratto {command.Contact}[{command.ODA}]",
                Conservation = _moduleConfiguration.FascicleConservation

            };
            fascicle.FascicleRoles.Add(new FascicleRole() { Role = workflowRoleMapping.Role, IsMaster = true, AuthorizationRoleType = AuthorizationRoleType.Responsible });
            fascicle.Contacts.Add(new Contact() { EntityId = _moduleConfiguration.FascicleContactId });
            fascicle = _webAPIClient.PostAsync(fascicle).Result;

            _logger.WriteInfo(new LogMessage($"Fascicle '{command.ODA}' ({fascicle.UniqueId}) has been create succesfully"), LogCategories);
            Event @event = new Event()
            {
                EventId = _dbContext.NextEventId(),
                CorrelationId = command.RequestId,
                Subject = _erp_subject_fascicle,
                Verb = _erp_verb_opened,
                Year = DateTime.Now.Year,
                Number = int.Parse(erpFascicleId),
                DigitalSigners = string.Empty,
                InsertTime = DateTime.Now,
                ProcessedTime = null,
            };
            _dbContext.Events.Add(@event);
            _dbContext.SaveChanges();
            return fascicle;
        }

        private WorkflowResult StartWorkflow(Command command, Contact contact, Fascicle fascicle, WorkflowRoleMapping workflowRoleMapping)
        {
            try
            {
                WorkflowStart workflowStart = new WorkflowStart
                {
                    WorkflowName = workflowRoleMapping.WorkflowRepository.Name
                };

                workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_ROLE_ID, new WorkflowArgument()
                {
                    PropertyType = ArgumentType.PropertyInt,
                    Name = WorkflowPropertyHelper.DSW_PROPERTY_ROLE_ID,
                    ValueInt = workflowRoleMapping.Role.EntityShortId
                });

                workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME, new WorkflowArgument()
                {
                    PropertyType = ArgumentType.PropertyString,
                    Name = WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME,
                    ValueString = $"Attività - Crea Contratto '{command.Contact}({command.Contract})'"
                });

                workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_FIELD_UDS_NAME, new WorkflowArgument()
                {
                    PropertyType = ArgumentType.PropertyString,
                    Name = WorkflowPropertyHelper.DSW_FIELD_UDS_NAME,
                    ValueString = _moduleConfiguration.UDSRepositoryName
                });

                workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, new WorkflowArgument()
                {
                    PropertyType = ArgumentType.RelationGuid,
                    Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID,
                    ValueGuid = _moduleConfiguration.TenantId
                });

                workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, new WorkflowArgument()
                {
                    PropertyType = ArgumentType.PropertyString,
                    Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME,
                    ValueString = _moduleConfiguration.TenantName
                });

                UDSWorkflowModel uDSWorkflowModel = new UDSWorkflowModel
                {
                    Contact = new ContactModel() { Id = contact.EntityId },
                    DynamicDatas = new Dictionary<string, string>()
                };
                uDSWorkflowModel.DynamicDatas.Add(_workflow_metadata_documentType, command.DocumentType);
                uDSWorkflowModel.DynamicDatas.Add(_workflow_metadata_owner, command.Owner);
                uDSWorkflowModel.DynamicDatas.Add(_workflow_metadata_oda, command.ODA);
                uDSWorkflowModel.DynamicDatas.Add(_workflow_metadata_contract, command.Contract);
                uDSWorkflowModel.DynamicDatas.Add(_workflow_metadata_cig, command.CIG);
                uDSWorkflowModel.DynamicDatas.Add(_workflow_metadata_requestId, command.RequestId.ToString());
                //uDSWorkflowModel.DynamicDatas.Add(workflowRoleMapping.Role.EntityId.ToString()
                if (command.DocumentType.Equals(ERPDocumentType.Accordo_Quadro, StringComparison.InvariantCultureIgnoreCase) ||
                    command.DocumentType.Equals(ERPDocumentType.Attivazione_Opzione, StringComparison.InvariantCultureIgnoreCase) ||
                    command.DocumentType.Equals(ERPDocumentType.Contratto_Principale, StringComparison.InvariantCultureIgnoreCase) ||
                    command.DocumentType.Equals(ERPDocumentType.LOA_Lettera_Ordine_Applicativa, StringComparison.InvariantCultureIgnoreCase) ||
                    command.DocumentType.Equals(ERPDocumentType.LOA_Numero_1, StringComparison.InvariantCultureIgnoreCase))
                {
                    workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_SIGNED_DOC_REQUIRED, new WorkflowArgument()
                    {
                        PropertyType = ArgumentType.PropertyBoolean,
                        Name = WorkflowPropertyHelper.DSW_PROPERTY_SIGNED_DOC_REQUIRED,
                        ValueBoolean = true
                    });
                }

                if (fascicle != null)
                {
                    workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_ACTION_TO_FASCICLE, new WorkflowArgument()
                    {
                        PropertyType = ArgumentType.PropertyGuid,
                        Name = WorkflowPropertyHelper.DSW_ACTION_TO_FASCICLE,
                        ValueGuid = fascicle.UniqueId
                    });
                }
                workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_MODEL, new WorkflowArgument()
                {
                    PropertyType = ArgumentType.Json,
                    Name = WorkflowPropertyHelper.DSW_PROPERTY_MODEL,
                    ValueString = JsonConvert.SerializeObject(uDSWorkflowModel, ModuleConfigurationHelper.JsonSerializerSettings)
                });
                WorkflowResult workflowResult = _webAPIClient.StartWorkflow(workflowStart).Result;
                _logger.WriteInfo(new LogMessage(string.Concat("Workflow started correctly [IsValid: ", workflowResult.IsValid, "] with instanceId ", workflowResult.InstanceId)), LogCategories);
                return workflowResult;
            }
            catch (Exception ex)
            {
                _logger.WriteWarning(ex, LogCategories);
                throw;
            }
        }

        private async Task EventTokenSecurityCallbackAsync(IEventTokenSecurity eventTokenSecurity)
        {
            if (Cancel)
            {
                return;
            }

            _logger.WriteDebug(new LogMessage(string.Concat("EventTokenSecurityCallbackAsync -> received callback with event id ", eventTokenSecurity.Id)), LogCategories);

            try
            {
                using (ERPDbContext dbContext = new ERPDbContext(_logger, ModuleConfigurationHelper.JsonSerializerSettings, _moduleConfiguration.ConnectionString))
                {
                    TokenSecurityModel tokenSecurityModel = eventTokenSecurity.ContentType.ContentTypeValue;
                    _dbContext.Claims.Remove(_dbContext.Claims.First());
                    _dbContext.Claims.Add(new Claim() { Token = tokenSecurityModel.Token.ToString() });
                    await _dbContext.SaveChangesAsync();
                    _logger.WriteInfo(new LogMessage("EventTokenSecurityCallbackAsync -> Claim token has been saved"), LogCategories);
                }
            }
            catch (Exception ex)
            {
                //non mandare via email
                _logger.WriteError(new LogMessage("EventTokenSecurityCallbackAsync -> error occured saving claim token in oracle database"), ex, LogCategories);
                throw;
            }
        }

        private async Task EventCompleteWorkflowInstanceCallbackAsync(IEventCompleteWorkflowInstance eventCompleteWorkflowInstance)
        {
            if (Cancel)
            {
                return;
            }

            _logger.WriteDebug(new LogMessage(string.Concat("EventCompleteWorkflowInstanceCallbackAsync -> received callback with event id ", eventCompleteWorkflowInstance.Id)), LogCategories);

            try
            {
                using (ERPDbContext dbContext = new ERPDbContext(_logger, ModuleConfigurationHelper.JsonSerializerSettings, _moduleConfiguration.ConnectionString))
                {
                    Event @event;
                    WorkflowProperty dsw_p_Model;
                    WorkflowProperty dsw_e_ProtocolNumber;
                    WorkflowProperty dsw_e_ProtocolYear;
                    WorkflowProperty dsw_e_MainChainId;
                    UDSWorkflowModel udsWorkflowModel;
                    IList<DocumentSignInfo> documentSignInfos;
                    string digitalSigners = string.Empty;
                    long? year = null;
                    long? number = null;
                    decimal correlationId;
                    WorkflowInstance workflowInstance = eventCompleteWorkflowInstance.ContentType.ContentTypeValue;
                    _logger.WriteDebug(new LogMessage(string.Concat("WorkflowInstance -> ", workflowInstance.InstanceId, " ctype -> ", eventCompleteWorkflowInstance.ContentType.Id)), LogCategories);
                    dsw_p_Model = workflowInstance.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_MODEL);
                    dsw_e_ProtocolNumber = workflowInstance.WorkflowActivities.SelectMany(f => f.WorkflowProperties).First(p => p.Name == WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_NUMBER);
                    _logger.WriteDebug(new LogMessage(string.Concat(WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_NUMBER, " -> ", dsw_e_ProtocolNumber.ValueInt)), LogCategories);
                    dsw_e_ProtocolYear = workflowInstance.WorkflowActivities.SelectMany(f => f.WorkflowProperties).First(p => p.Name == WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_YEAR);
                    _logger.WriteDebug(new LogMessage(string.Concat(WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_YEAR, " -> ", dsw_e_ProtocolYear.ValueInt)), LogCategories);
                    dsw_e_MainChainId = workflowInstance.WorkflowActivities.SelectMany(f => f.WorkflowProperties).First(p => p.Name == WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_CHAINID_MAIN);
                    _logger.WriteDebug(new LogMessage(string.Concat(WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_CHAINID_MAIN, " -> ", dsw_e_MainChainId.ValueGuid)), LogCategories);

                    udsWorkflowModel = JsonConvert.DeserializeObject<UDSWorkflowModel>(dsw_p_Model.ValueString, ModuleConfigurationHelper.JsonSerializerSettings);
                    documentSignInfos = await _documentClient.GetDocumentSignInfoAsync(dsw_e_MainChainId.ValueGuid.Value);
                    _logger.WriteDebug(new LogMessage(string.Concat("Found signers ", documentSignInfos != null || documentSignInfos.Any())), LogCategories);

                    if (documentSignInfos != null || documentSignInfos.Any())
                    {
                        digitalSigners = string.Join(",", documentSignInfos.Select(f => f.SignUser).ToArray());
                        _logger.WriteDebug(new LogMessage(string.Concat("Digital signers are ", digitalSigners)), LogCategories);
                    }
                    year = dsw_e_ProtocolYear.ValueInt.Value;
                    number = dsw_e_ProtocolNumber.ValueInt.Value;
                    correlationId = decimal.Parse(udsWorkflowModel.DynamicDatas[_workflow_metadata_requestId]);
                    if (year.HasValue && year.Value > 0 && number.HasValue && number.Value > 0)
                    {
                        @event = new Event
                        {
                            EventId = _dbContext.NextEventId(),
                            Subject = _erp_subject_protocol,
                            Verb = _erp_verb_inserted,
                            DigitalSigners = digitalSigners,
                            CorrelationId = correlationId,
                            Year = year.Value,
                            Number = number.Value,
                            InsertTime = DateTime.Now,
                            ProcessedTime = null
                        };
                        _dbContext.Events.Add(@event);
                        _dbContext.SaveChanges();

                        _logger.WriteInfo(new LogMessage(string.Concat("Complete workflow instance ", workflowInstance.InstanceId, " successfull")), LogCategories);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("EventCompleteWorkflowInstanceCallbackAsync -> error occured saving event protocollo in oracle database"), ex, LogCategories);
                throw;
            }
        }

        #endregion

    }
}
