using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.VSW.DocumentUnitInterop.Configurations;
using VecompSoftware.BPM.Integrations.Modules.VSW.DocumentUnitInterop.Models;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.StampaConforme;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.Helpers.Workflow;
using VecompSoftware.Services.Command.CQRS.Events.Entities.DocumentUnits;
using VecompSoftware.Services.Command.CQRS.Events.Entities.Workflow;
using TenantConfiguration = VecompSoftware.BPM.Integrations.Modules.VSW.DocumentUnitInterop.Models.TenantConfiguration;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.DocumentUnitInterop
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region  [ Fields ]
        private readonly ILogger _logger;
        private readonly IWebAPIClient _webAPIClient;
        private readonly IServiceBusClient _serviceBusClient;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private bool _needInitializeModule = false;
        private Tenant _currentTenant = null;
        private string _signatureTemplate;
        private readonly IDocumentClient _documentClient;
        private readonly IStampaConformeClient _stampaConformeClient;
        private readonly IDictionary<ChainType, Action<ProtocolModel, ICollection<ArchiveDocument>>> _protocolDocumentModelMappingActions = new Dictionary<ChainType, Action<ProtocolModel, ICollection<ArchiveDocument>>>()
        {
            {
                ChainType.MainChain, (protocolModel, documents) => protocolModel.MainDocument = new DocumentModel()
                {
                    FileName = documents.First().Name,
                    ChainId = documents.First().IdChain,
                    DocumentId = documents.First().IdDocument
                }
            },
            {
                ChainType.AttachmentsChain, (protocolModel, documents) =>
                {
                    foreach (ArchiveDocument item in documents)
                    {
                        protocolModel.Attachments.Add(new DocumentModel() { FileName = item.Name, ChainId = item.IdChain, DocumentId = item.IdDocument });
                    }
                }
            },
            {
                ChainType.AnnexedChain, (protocolModel, documents) =>
                {
                    foreach (ArchiveDocument item in documents)
                    {
                        protocolModel.Annexes.Add(new DocumentModel() { FileName = item.Name, ChainId = item.IdChain, DocumentId = item.IdDocument });
                    }
                }
            }
        };
        #endregion 

        #region  [ Properties ]
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

        #region  [ Constructor ]
        [ImportingConstructor]
        public Execution(ILogger logger, IServiceBusClient serviceBusClient, IWebAPIClient webAPIClient, IDocumentClient documentClient,
            IStampaConformeClient stampaConformeClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _webAPIClient = webAPIClient;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _serviceBusClient = serviceBusClient;
                _documentClient = documentClient;
                _stampaConformeClient = stampaConformeClient;
                _needInitializeModule = true;
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("VSW.DocumentUnitInterop -> Critical error in costruction module"), ex, LogCategories);
                throw;
            }
        }
        #endregion 

        #region  [ Methods ]        
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
                _logger.WriteError(new LogMessage("VSW.DocumentUnitInterop -> Execute critical error"), ex, LogCategories);
                throw;
            }
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);

                _currentTenant = _webAPIClient.GetTenantAsync(_moduleConfiguration.TenantId).Result;
                if (_currentTenant == null)
                {
                    throw new ArgumentNullException($"Tenant {_moduleConfiguration.TenantId} not found");
                }

                _signatureTemplate = _webAPIClient.GetParameterSignatureTemplate().Result;
                _subscriptions.Add(_serviceBusClient.StartListening<IEventShareDocumentUnit>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration,
                    _moduleConfiguration.WorkflowStartInteropShareDocumentUnitSubscription, EventShareDocumentUnitRequestCallback));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompleteWorkflowActivity>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowActivityCompleted,
                    _moduleConfiguration.WorkflowActivityInteropShareDocumentUnitCompleteSubscription, WorkflowActivityInteropShareDocumentUnitCompleteCallback));
                _needInitializeModule = false;
            }
        }

        #region [ EventShareDocumentUnitRequestCallback ]
        private async Task EventShareDocumentUnitRequestCallback(IEventShareDocumentUnit evt, IDictionary<string, object> properties)
        {
            _logger.WriteInfo(new LogMessage($"EventShareDocumentUnitRequestCallback -> received callback with event id {evt.Id}"), LogCategories);

            try
            {
                IWebAPIClient proxyWebAPIClient = null;
                IDocumentClient proxyDocumentClient = null;
                if (!(evt.ContentType.ContentValue is DocumentUnit documentUnit))
                {
                    _logger.WriteError(new LogMessage("EventShareDocumentUnitRequestCallback -> DocumentUnit is null"), LogCategories);
                    throw new ArgumentNullException("DocumentUnit", "DocumentUnit is null.");
                }

                Guid documentUnitId = documentUnit.UniqueId;
                Guid? idWorkflowActivity = documentUnit.IdWorkflowActivity;

                if (!idWorkflowActivity.HasValue)
                {
                    _logger.WriteError(new LogMessage($"Property IdWorkflowActivity is not defined for DocumentUnit {documentUnit.UniqueId}"), LogCategories);
                    throw new ArgumentNullException("IdWorkflowActivity", $"Property IdWorkflowActivity is not defined for DocumentUnit {documentUnit.UniqueId}");
                }
                if (evt.TenantAOOId == Guid.Empty)
                {
                    _logger.WriteError(new LogMessage($"Property TenantAOOId is not defined for EventShareDocumentUnit {documentUnit.UniqueId}"), LogCategories);
                    throw new ArgumentNullException("TenantAOOId", $"Property TenantAOOId is not defined for EventShareDocumentUnit {documentUnit.UniqueId}");
                }

                TenantAOO tenantAOO = await _webAPIClient.GetTenantAOOAsync(evt.TenantAOOId);
                if (tenantAOO == null)
                {
                    _logger.WriteError(new LogMessage($"TenantAOO {evt.TenantAOOId} not exists"), LogCategories);
                    throw new ArgumentNullException("TenantAOO", $"TenantAOO {evt.TenantAOOId} not exists");
                }
                ExternalIdentifierModel externalIdentifierModel = new ExternalIdentifierModel()
                {
                    TenantAOOId = tenantAOO.UniqueId,
                    TenantAOOName = tenantAOO.Name,
                    TenantId = _currentTenant.UniqueId,
                    TenantName = _currentTenant.TenantName
                };

                _logger.WriteInfo(new LogMessage($"Reading parameter TenantName from workflow activity {idWorkflowActivity}"), LogCategories);
                ICollection<WorkflowProperty> workflowActivityProperties = await _webAPIClient.GetWorkflowPropertiesAsync(idWorkflowActivity.Value);
                string externalTenantName = GetExternalTenantNameProperty(idWorkflowActivity.Value, workflowActivityProperties);
                if (string.IsNullOrEmpty(externalTenantName))
                {
                    throw new ArgumentNullException("TenantName", $"Parameter TenantName is not defined for workflow activity {idWorkflowActivity}");
                }

                _logger.WriteInfo(new LogMessage($"Readed TenantName parameter with value {externalTenantName}"), LogCategories);
                proxyWebAPIClient = BuildExternalWebAPIClient(externalTenantName);
                proxyDocumentClient = BuildExternalBiblosClient(externalTenantName);

                Tenant externalTenant = (await proxyWebAPIClient.GetTenantsAsync($"$filter=TenantName eq '{externalTenantName}'&$expand=TenantAOO")).SingleOrDefault();
                if (externalTenant == null)
                {
                    throw new ArgumentNullException($"Tenant {externalTenant} not found");
                }

                int? externalWorkflowLocationId = await proxyWebAPIClient.GetParameterWorkflowLocationIdAsync();
                if (!externalWorkflowLocationId.HasValue)
                {
                    throw new ArgumentNullException($"Parameter WorkflowLocationId is not defined for Tenant {externalTenantName}");
                }
                Location externalWorkflowLocation = (await proxyWebAPIClient.GetLocationAsync(externalWorkflowLocationId.Value)).Single();

                _logger.WriteInfo(new LogMessage($"Coping document unit documents to external location {externalWorkflowLocation.Name}"), LogCategories);
                bool documentOriginalTypeSelection = GetDocumentOriginalTypeSelectionProperty(idWorkflowActivity.Value, workflowActivityProperties);
                string subject = GetSubjectProperty(workflowActivityProperties);
                ICollection<DocumentUnitChain> documentUnitChains = await _webAPIClient.GetDocumentUnitChainsAsync(documentUnitId);
                IDictionary<ChainType, ICollection<ArchiveDocument>> toArchiveChains = await DuplicateDocumentUnitChainsInMemoryAsync(documentUnitId, externalWorkflowLocation.ProtocolArchive, documentOriginalTypeSelection);

                IDictionary<ChainType, ICollection<ArchiveDocument>> chainDocumentsInserted = new Dictionary<ChainType, ICollection<ArchiveDocument>>();
                ICollection<ArchiveDocument> tmpDocuments;
                foreach (KeyValuePair<ChainType, ICollection<ArchiveDocument>> toArchiveChainDocuments in toArchiveChains)
                {
                    tmpDocuments = await proxyDocumentClient.InsertDocumentsAsync(toArchiveChainDocuments.Value);
                    chainDocumentsInserted.Add(toArchiveChainDocuments.Key, tmpDocuments);
                }
                _logger.WriteInfo(new LogMessage($"Documents copied correctly"), LogCategories);

                _logger.WriteDebug(new LogMessage($"Preparing starting workflow"), LogCategories);
                ProtocolModel protocolModel = BuildWorkflowProtocolModel(documentUnit, subject, chainDocumentsInserted);
                WorkflowResult workflowResult = await StartWorkflowAsync(proxyWebAPIClient, externalTenant, protocolModel, externalIdentifierModel, idWorkflowActivity);
                if (!workflowResult.IsValid || !workflowResult.InstanceId.HasValue)
                {
                    _logger.WriteError(new LogMessage("An error occured in start workflow"), LogCategories);
                    throw new Exception(string.Join(", ", workflowResult.Errors));
                }
                _logger.WriteInfo(new LogMessage($"Workflow started correctly with id: {workflowResult.InstanceId.Value}"), LogCategories);

            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"EventShareDocumentUnitRequestCallback -> error occouring when calling DocumentUnitInterop service for event id {evt.Id}"), ex, LogCategories);
                throw;
            }
        }

        private IWebAPIClient BuildExternalWebAPIClient(string externalTenantName)
        {
            if (!_moduleConfiguration.TenantProxies.ContainsKey(externalTenantName))
            {
                throw new ArgumentException($"The external tenant {externalTenantName} is not recognized");
            }
            string baseUrl = _moduleConfiguration.TenantProxies[externalTenantName].WebAPIUrl;
            return new WebAPIClient(_logger, baseUrl);
        }

        private IDocumentClient BuildExternalBiblosClient(string externalTenantName)
        {
            if (!_moduleConfiguration.TenantProxies.ContainsKey(externalTenantName))
            {
                throw new ArgumentException($"The external tenant {externalTenantName} is not recognized");
            }
            TenantConfiguration tenantConfiguration = _moduleConfiguration.TenantProxies[externalTenantName];
            return new DocumentClient(_logger, new DocumentClientConfiguration()
            {
                EndPointConfigurationName = tenantConfiguration.BiblosDSBindingConfiguration,
                RemoteAddress = tenantConfiguration.BiblosDSAddresses
            });
        }

        private string GetExternalTenantNameProperty(Guid idWorkflowActivity, ICollection<WorkflowProperty> workflowActivityProperties)
        {
            WorkflowProperty dsw_p_TenantName = workflowActivityProperties.SingleOrDefault(x => x.Name.Equals(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME) && !string.IsNullOrEmpty(x.ValueString));
            if (dsw_p_TenantName == null)
            {
                throw new ArgumentException($"The property '_dsw_p_TenantName' for workflow activity {idWorkflowActivity} is not specified.");
            }

            return dsw_p_TenantName.ValueString;
        }

        private bool GetDocumentOriginalTypeSelectionProperty(Guid idWorkflowActivity, ICollection<WorkflowProperty> workflowActivityProperties)
        {
            WorkflowProperty dsw_p_DocumentOriginalTypeSelection = workflowActivityProperties.SingleOrDefault(x => x.Name.Equals(WorkflowPropertyHelper.DSW_PROPERTY_DOCUMENT_ORIGINAL_TYPE_SELECTION));
            if (dsw_p_DocumentOriginalTypeSelection == null)
            {
                throw new ArgumentException($"The property '_dsw_p_DocumentOriginalTypeSelection' for workflow activity {idWorkflowActivity} is not specified.");
            }

            return dsw_p_DocumentOriginalTypeSelection.ValueBoolean.GetValueOrDefault(false);
        }

        private string GetSubjectProperty(ICollection<WorkflowProperty> workflowActivityProperties)
        {
            WorkflowProperty dsw_p_Subject = workflowActivityProperties.SingleOrDefault(x => x.Name.Equals(WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT));
            return dsw_p_Subject?.ValueString;
        }

        private async Task<IDictionary<ChainType, ICollection<ArchiveDocument>>> DuplicateDocumentUnitChainsInMemoryAsync(Guid documentUnitId, string externalWorkflowArchiveName, bool documentOriginalTypeSelection)
        {
            ICollection<DocumentUnitChain> documentUnitChains = await _webAPIClient.GetDocumentUnitChainsAsync(documentUnitId);
            IDictionary<ChainType, ICollection<ArchiveDocument>> results = new Dictionary<ChainType, ICollection<ArchiveDocument>>();
            byte[] documentContent;
            ICollection<ArchiveDocument> documents;
            ICollection<ArchiveDocument> chainDocumentResults;
            string documentName;
            string signature;
            foreach (DocumentUnitChain documentUnitChain in documentUnitChains)
            {
                documents = await _documentClient.GetChildrenAsync(documentUnitChain.IdArchiveChain);
                chainDocumentResults = new List<ArchiveDocument>();
                foreach (ArchiveDocument document in documents)
                {
                    documentContent = await _documentClient.GetDocumentStreamAsync(document.IdDocument);
                    documentName = document.Name;
                    if (!documentOriginalTypeSelection)
                    {
                        signature = StampaConformeClient.GetSignature(_signatureTemplate, document.Metadata[_documentClient.ATTRIBUTE_SIGNATURE] as string);
                        documentContent = await _stampaConformeClient.ConvertToPDFAAsync(documentContent, Path.GetExtension(document.Name), signature);
                        if (!documentName.ToLower().Contains(".p7m"))
                        {
                            documentName = Path.GetFileNameWithoutExtension(documentName);
                        }
                        documentName = $"{documentName}.pdf";
                        _logger.WriteInfo(new LogMessage($"Generated {documentName} PDF/A with signature {signature}"), LogCategories);
                    }

                    chainDocumentResults.Add(new ArchiveDocument()
                    {
                        Archive = externalWorkflowArchiveName,
                        ContentStream = documentContent,
                        Name = documentName
                    });
                }
                results.Add(documentUnitChain.ChainType, chainDocumentResults);
            }
            return results;
        }

        private ProtocolModel BuildWorkflowProtocolModel(DocumentUnit documentUnit, string subject, IDictionary<ChainType, ICollection<ArchiveDocument>> chainDocuments)
        {
            ProtocolModel protocolModel = new ProtocolModel
            {
                Object = documentUnit.Subject,
                ProtocolType = new ProtocolTypeModel() { EntityShortId = -1 },
                DocumentProtocol = documentUnit.Title,
                DocumentDate = documentUnit.RegistrationDate.ToLocalTime().DateTime,
                Note = subject
            };

            protocolModel.ContactManuals.Add(new ProtocolContactManualModel()
            {
                Description = _currentTenant.CompanyName,
                ComunicationType = ComunicationType.Sender
            });

            if (chainDocuments.Count > 0)
            {
                foreach (KeyValuePair<ChainType, ICollection<ArchiveDocument>> chainId in chainDocuments)
                {
                    _protocolDocumentModelMappingActions[chainId.Key](protocolModel, chainId.Value);
                }
            }
            return protocolModel;
        }

        private async Task<WorkflowResult> StartWorkflowAsync(IWebAPIClient webAPIClient, Tenant externalTenant,
            ProtocolModel protocolModel, ExternalIdentifierModel externalIdentifierModel, Guid? idWorkflowActivity)
        {
            WorkflowStart start = new WorkflowStart
            {
                WorkflowName = _moduleConfiguration.WorkflowName
            };

            start.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME, new WorkflowArgument()
            {
                Name = WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME,
                PropertyType = ArgumentType.PropertyString,
                ValueString = $"Richiesta di protollazione ricevuta da {_currentTenant.CompanyName}"
            });

            if (!string.IsNullOrEmpty(protocolModel.Note))
            {
                start.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT, new WorkflowArgument()
                {
                    Name = WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT,
                    PropertyType = ArgumentType.PropertyString,
                    ValueString = protocolModel.Note
                });
            }

            start.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, new WorkflowArgument()
            {
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID,
                PropertyType = ArgumentType.RelationGuid,
                ValueGuid = externalTenant.UniqueId
            });

            start.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, new WorkflowArgument()
            {
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID,
                PropertyType = ArgumentType.RelationGuid,
                ValueGuid = externalTenant.TenantAOO.UniqueId
            });

            start.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, new WorkflowArgument()
            {
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME,
                PropertyType = ArgumentType.PropertyString,
                ValueString = externalTenant.TenantName
            });

            start.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_ROLE_ID, new WorkflowArgument()
            {
                Name = WorkflowPropertyHelper.DSW_PROPERTY_ROLE_ID,
                PropertyType = ArgumentType.PropertyInt,
                ValueInt = _moduleConfiguration.TenantProxies[externalTenant.TenantName].AuthorizedRole
            });

            start.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_MODEL, new WorkflowArgument()
            {
                Name = WorkflowPropertyHelper.DSW_PROPERTY_MODEL,
                PropertyType = ArgumentType.Json,
                ValueString = JsonConvert.SerializeObject(protocolModel, ModuleConfigurationHelper.JsonSerializerSettings)
            });

            start.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER,
                ValueGuid = idWorkflowActivity
            });

            start.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER_MODEL, new WorkflowArgument()
            {
                PropertyType = ArgumentType.Json,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER_MODEL,
                ValueString = JsonConvert.SerializeObject(externalIdentifierModel, ModuleConfigurationHelper.JsonSerializerSettings)
            });

            WorkflowResult workflowResult = await webAPIClient.StartWorkflow(start);
            return workflowResult;
        }
        #endregion

        private async Task WorkflowActivityInteropShareDocumentUnitCompleteCallback(IEventCompleteWorkflowActivity evt, IDictionary<string, object> properties)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"WorkflowActivityInteropShareDocumentUnitCompleteCallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteInfo(new LogMessage($"Notifying workflow activity complete for WorkflowInstanceId {evt.CorrelationId}"), LogCategories);
                WorkflowNotify workflowNotify = null;
                WorkflowResult workflowResult = null;
                WorkflowActivity workflowActivity = evt.ContentType.ContentTypeValue;
                if (workflowActivity == null)
                {
                    _logger.WriteError(new LogMessage("WorkflowActivityInteropShareDocumentUnitCompleteCallback -> WorkflowActivity is null"), LogCategories);
                    throw new ArgumentNullException("WorkflowActivity", "WorkflowActivity is null.");
                }

                WorkflowProperty dsw_e_ProtocolNumber = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_NUMBER);
                WorkflowProperty dsw_e_ProtocolYear = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_YEAR);
                WorkflowProperty dsw_p_TenantName = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME);
                WorkflowProperty dsw_p_ExternalIdentifier = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER);
                WorkflowProperty dsw_p_ExternalIdentifierModel = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER_MODEL);
                if (dsw_e_ProtocolNumber == null || !dsw_e_ProtocolNumber.ValueInt.HasValue)
                {
                    _logger.WriteError(new LogMessage("WorkflowActivityInteropShareDocumentUnitCompleteCallback -> DSW_FIELD_PROTOCOL_NUMBER is null"), LogCategories);
                    throw new ArgumentNullException("WorkflowProperties", "DSW_FIELD_PROTOCOL_NUMBER is null.");
                }
                if (dsw_e_ProtocolYear == null || !dsw_e_ProtocolYear.ValueInt.HasValue)
                {
                    _logger.WriteError(new LogMessage("WorkflowActivityInteropShareDocumentUnitCompleteCallback -> DSW_FIELD_PROTOCOL_YEAR is null"), LogCategories);
                    throw new ArgumentNullException("WorkflowProperties", "DSW_FIELD_PROTOCOL_YEAR is null.");
                }
                if (dsw_p_ExternalIdentifier == null || !dsw_p_ExternalIdentifier.ValueGuid.HasValue)
                {
                    _logger.WriteError(new LogMessage("WorkflowActivityInteropShareDocumentUnitCompleteCallback -> DSW_PROPERTY_EXTERNAL_IDENTIFIER is null"), LogCategories);
                    throw new ArgumentNullException("WorkflowProperties", "DSW_PROPERTY_EXTERNAL_IDENTIFIER is null.");
                }
                ExternalIdentifierModel externalIdentifierModel = null;
                if (dsw_p_ExternalIdentifierModel == null || string.IsNullOrEmpty(dsw_p_ExternalIdentifierModel.ValueString) ||
                    (externalIdentifierModel = JsonConvert.DeserializeObject<ExternalIdentifierModel>(dsw_p_ExternalIdentifierModel.ValueString, ModuleConfigurationHelper.JsonSerializerSettings)) == null)
                {
                    _logger.WriteError(new LogMessage("WorkflowActivityInteropShareDocumentUnitCompleteCallback -> DSW_FIELD_EXTERNAL_IDENTIFIER_MODEL is null"), LogCategories);
                    throw new ArgumentNullException("WorkflowProperties", "DSW_FIELD_EXTERNAL_IDENTIFIER_MODEL is null.");
                }
                _logger.WriteInfo(new LogMessage($"Notifying Protocol {dsw_e_ProtocolYear.ValueInt.Value}/{dsw_e_ProtocolNumber.ValueInt.Value:0000000} from WorkflowActivityId {workflowActivity.UniqueId} to {dsw_p_ExternalIdentifier.ValueGuid.Value}/{externalIdentifierModel.TenantAOOName}"), LogCategories);

                IWebAPIClient proxyWebAPIClient = BuildExternalWebAPIClient(externalIdentifierModel.TenantAOOName);

                workflowNotify = new WorkflowNotify(dsw_p_ExternalIdentifier.ValueGuid.Value)
                {
                    ModuleName = ModuleConfigurationHelper.MODULE_NAME
                };
                workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_NUMBER, new WorkflowArgument()
                {
                    PropertyType = ArgumentType.PropertyInt,
                    Name = WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_NUMBER,
                    ValueInt = dsw_e_ProtocolNumber.ValueInt
                });
                workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_YEAR, new WorkflowArgument()
                {
                    PropertyType = ArgumentType.PropertyInt,
                    Name = WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_YEAR,
                    ValueInt = dsw_e_ProtocolYear.ValueInt
                });
                workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT, new WorkflowArgument()
                {
                    PropertyType = ArgumentType.PropertyString,
                    Name = WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT,
                    ValueString = $"Invio interoperabile protocollato da {evt.Identity?.User} con riferimento {dsw_e_ProtocolYear.ValueInt.Value}/{dsw_e_ProtocolNumber.ValueInt.Value:0000000} nella AOO {dsw_p_TenantName?.ValueString}"
                });

                workflowResult = await proxyWebAPIClient.WorkflowNotify(workflowNotify);
                _logger.WriteInfo(new LogMessage($"Workflow notify correctly [IsValid: {workflowResult.IsValid}] with instanceId {workflowResult.InstanceId}"), LogCategories);
                if(!workflowResult.IsValid || !workflowResult.InstanceId.HasValue)
                {
                    _logger.WriteError(new LogMessage("An error occured in notify workflow activity"), LogCategories);
                    throw new Exception(string.Join(", ", workflowResult.Errors));
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("EventWorkflowFascicleBuildCompleteCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> VSW.DocumentUnitInterop"), LogCategories);
        }

        private void CleanSubscriptions()
        {
            foreach (Guid item in _subscriptions)
            {
                _serviceBusClient.CloseListeningAsync(item).Wait();
            }
            _subscriptions.Clear();
            _needInitializeModule = true;
        }

        #endregion 
    }
}