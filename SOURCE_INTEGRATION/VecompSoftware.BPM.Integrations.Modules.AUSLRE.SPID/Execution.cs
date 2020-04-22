using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.AUSLRE.SPID.Configuration;
using VecompSoftware.BPM.Integrations.Modules.AUSLRE.SPID.Models;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Core.Command.CQRS.Commands.Models.Fascicles;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.DocSuiteWeb.Model.Workflow.Actions;
using VecompSoftware.Services.Command.CQRS.Events.Entities.Fascicles;
using VecompSoftware.Services.Command.CQRS.Events.Models.Workflows;
using VecompSoftware.Helpers.Workflow;
using Newtonsoft.Json;
using VecompSoftware.DocSuiteWeb.Model.Metadata;
using VecompSoftware.DocSuiteWeb.Model.DocumentGenerator;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using System.Security.Principal;
using VecompSoftware.Core.Command;
using VecompSoftware.BPM.Integrations.Modules.AUSLRE.SPID.Clients;
using VecompSoftware.Services.Command.CQRS.Events.Models.Fascicles;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLRE.SPID
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
        private readonly PDFGeneratorClient _pdfGeneratorClient;
        private readonly IdentityContext _identityContext = null;
        #endregion

        #region [ Const ]
        private const string SPID_ID_METADATA_LABEL = "SPID_ID";
        private const string DESCRIPTION_METADATA_LABEL = "Denominazione/Nome cognome";
        private const string MOTIVAZIONI_METADATA_LABEL = "Motivazioni";
        private const string FASCICLE_STATE_METADATA_LABEL = "Stato del fascicolo";
        private const string PUBLIC_FASCICLE_FOLDER_NAME = "Cartella pubblica SPID";
        private const string FASCICLE_CLOSED_STATE_METADATA_VALUE = "Richiesta conclusa";
        private const string FASCICLE_PROGRESS_STATE_METADATA_VALUE = "Richiesta in lavorazione";
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
                _pdfGeneratorClient = new PDFGeneratorClient(_moduleConfiguration.PDFGeneratorServiceUrl);
                string username = "anonymous";
                if (WindowsIdentity.GetCurrent() != null)
                {
                    username = WindowsIdentity.GetCurrent().Name;
                }
                _identityContext = new IdentityContext(username);
                _needInitializeModule = true;
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("AUSLRE.SPID -> Critical error in costruction module"), ex, LogCategories);
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
                _logger.WriteError(new LogMessage("AUSLRE.SPID -> Execute critical error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> AUSLRE.SPID"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventWorkflowStartRequest>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration, _moduleConfiguration.WorkflowStartSPIDSubscription, EventWorkflowStartedRequestCallbackAsync));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventUpdateFascicle>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration, _moduleConfiguration.WorkflowSPIDUpdateSubscription, EventUpdateFascicleCallbackAsync));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompleteFascicleBuild>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicBuilderEvent, _moduleConfiguration.WorkflowFascicleBuildCompleteSubscription, EventWorkflowFascicleBuildCompleteCallback));
                _needInitializeModule = false;
            }
        }

        private async Task EventUpdateFascicleCallbackAsync(IEventUpdateFascicle evt)
        {
            try
            {
                _logger.WriteDebug(new LogMessage(string.Concat("EventUpdateFascicleCallbackAsync -> evaluate event id ", evt.Id)), LogCategories);
                Fascicle fascicle = await _webAPIClient.GetFascicleAsync($"$filter=UniqueId eq {evt.ContentType.ContentTypeValue.UniqueId}&$expand=MetadataRepository,WorkflowInstances");
                if (fascicle.MetadataRepository == null || string.IsNullOrEmpty(fascicle.MetadataValues))
                {
                    throw new ArgumentNullException($"Fascicle {fascicle.UniqueId} does not have metadatas specified");
                }
                MetadataModel metadataModel = JsonConvert.DeserializeObject<MetadataModel>(fascicle.MetadataValues);
                if (fascicle.MetadataRepository.UniqueId != _moduleConfiguration.IdMetadataRepository)
                {
                    throw new ArgumentNullException("Metadatas is incorrect for definition");
                }

                _logger.WriteDebug(new LogMessage("Evaluate metadata property for fascicle state"), _logCategories);
                TextFieldModel statoFascicoloMetadata = metadataModel.TextFields.SingleOrDefault(x => x.Label.Equals("Stato del fascicolo", StringComparison.InvariantCultureIgnoreCase));
                statoFascicoloMetadata.Value = EvaluateFascicleUpdateState(fascicle, statoFascicoloMetadata);

                fascicle.MetadataValues = JsonConvert.SerializeObject(metadataModel);
                await _webAPIClient.PutAsync(fascicle);
                _logger.WriteInfo(new LogMessage($"Fascicle {fascicle.UniqueId} updated correctly"), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("EventUpdateFascicleCallbackAsync -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        private string EvaluateFascicleUpdateState(Fascicle fascicle, TextFieldModel statoFascicoloMetadata)
        {
            if (fascicle.EndDate.HasValue)
            {
                return FASCICLE_CLOSED_STATE_METADATA_VALUE;
            }

            if (fascicle.WorkflowInstances.Any(x => x.Status == DocSuiteWeb.Entity.Workflows.WorkflowStatus.Progress || 
            x.Status == DocSuiteWeb.Entity.Workflows.WorkflowStatus.Todo))
            {
                return FASCICLE_PROGRESS_STATE_METADATA_VALUE;
            }
            return statoFascicoloMetadata.Value;
        }

        private async Task EventWorkflowFascicleBuildCompleteCallback(IEventCompleteFascicleBuild evt)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"EventWorkflowFascicleBuildCompleteCallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteInfo(new LogMessage($"Notifying FascicleBuildComplete for WorkflowInstanceId {evt.CorrelationId}"), LogCategories);
                WorkflowNotify workflowNotify = null;
                WorkflowResult workflowResult = null;

                FascicleBuildModel fascicleBuildModel = evt.ContentType.ContentTypeValue;
                _logger.WriteInfo(new LogMessage($"Notifying FascicleBuildComplete for IdWorkflowActivity {fascicleBuildModel.IdWorkflowActivity}"), LogCategories);
                workflowNotify = new WorkflowNotify(fascicleBuildModel.IdWorkflowActivity.Value)
                {
                    WorkflowName = fascicleBuildModel.WorkflowName,
                    ModuleName = ModuleConfigurationHelper.MODULE_NAME
                };
                workflowResult = await _webAPIClient.WorkflowNotify(workflowNotify);
                _logger.WriteInfo(new LogMessage($"Workflow notify correctly [IsValid: {workflowResult.IsValid}] with instanceId {workflowResult.InstanceId}"), LogCategories);
                if (!workflowResult.IsValid)
                {
                    _logger.WriteError(new LogMessage("An error occured in notify workflow activity"), LogCategories);
                    throw new Exception("VecompSoftware.BPM.Integrations.Modules.AUSLRE.SPID");
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("EventWorkflowFascicleBuildCompleteCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        private async Task EventWorkflowStartedRequestCallbackAsync(IEventWorkflowStartRequest evt)
        {
            _logger.WriteDebug(new LogMessage($"EventWorkflowStartedRequestCallbackAsync -> evaluate event id {evt.Id}"), LogCategories);
            if (!evt.ContentType.ContentTypeValue.Arguments.ContainsKey(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL))
            {
                _logger.WriteError(new LogMessage($"property { WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL } not defined on event"), LogCategories);
                throw new ArgumentNullException(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, $"property {WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL} not defined on event");
            }

            try
            {
                WorkflowArgument referenceModelWorkflowArgument = evt.ContentType.ContentTypeValue.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL];
                WorkflowReferenceModel workflowReferenceModel = JsonConvert.DeserializeObject<WorkflowReferenceModel>(referenceModelWorkflowArgument.ValueString, ModuleConfigurationHelper.JsonSerializerSettings);
                MetadataModel metadataModel = JsonConvert.DeserializeObject<MetadataModel>(workflowReferenceModel.ReferenceModel, ModuleConfigurationHelper.JsonSerializerSettings);

                SPIDMetadataModelWrapper spidRequestMetadataModel = new SPIDMetadataModelWrapper(metadataModel);
                ContactModel contactModel = metadataModel.ContactFileds.FirstOrDefault();
                if (contactModel == null)
                {
                    throw new ArgumentNullException("SPID request owner is not defined");
                }

                if (string.IsNullOrEmpty(contactModel.Code))
                {
                    throw new ArgumentNullException($"SPID request owner {contactModel.Description.Replace('|', ' ')} does not have SPID code");
                }

                _logger.WriteDebug(new LogMessage("Building document model"), LogCategories);
                DocumentGeneratorModel document = BuildDocumentGeneratorModel(spidRequestMetadataModel, contactModel);
                _logger.WriteDebug(new LogMessage("Document model builded correctly"), LogCategories);
                Contact contact = await GetOrCreateContactAsync(contactModel);

                _logger.WriteDebug(new LogMessage("Generating PDF document"), LogCategories);
                byte[] documentContent = await _pdfGeneratorClient.GeneratePDFAsync(document, _moduleConfiguration.IdPDFTemplate);
                _logger.WriteDebug(new LogMessage($"PDF document generated correctly [size:{documentContent.Length}]"), LogCategories);

                Guid correlationId = Guid.NewGuid();
                if (evt.CorrelationId.HasValue)
                {
                    correlationId = evt.CorrelationId.Value;
                }
                Guid protocolUniqueId = Guid.NewGuid();                
                Guid fascicleUniqueId = Guid.NewGuid();
                Guid fascicleFolderUniqueId = Guid.NewGuid();

                _logger.WriteDebug(new LogMessage($"Preparing starting workflow with correlationId {correlationId}, fascicleUniqueId {fascicleUniqueId}, protocolUniqueId {protocolUniqueId},"), LogCategories);
                WorkflowReferenceModel fascicleWorkflowReferenceModel = CreateFascicleBuildModel(spidRequestMetadataModel, fascicleUniqueId, correlationId, contact, fascicleFolderUniqueId);
                WorkflowReferenceModel protocolWorkflowReferenceModel = await CreateProtocolBuildModelAsync(documentContent, contact, protocolUniqueId, correlationId, fascicleUniqueId, fascicleFolderUniqueId);
                WorkflowResult workflowResult = await StartWorkflowAsync(protocolWorkflowReferenceModel, fascicleWorkflowReferenceModel, contact.SearchCode, _moduleConfiguration.WorkflowRepositoryName);
                if (!workflowResult.IsValid)
                {
                    _logger.WriteError(new LogMessage("An error occured in start SPID accesso agli atti workflow"), LogCategories);
                    throw new Exception("VecompSoftware.BPM.Integrations.Modules.AUSLRE.SPID");
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("EventWorkflowStartedRequestCallbackAsync -> error complete start 'SPID Accesso agli atti' workflow"), ex, LogCategories);
                throw;
            }
        }

        private DocumentGeneratorModel BuildDocumentGeneratorModel(SPIDMetadataModelWrapper requestMetadataModel, ContactModel contact)
        {
            DocumentGeneratorBuilder documentGeneratorBuilder = new DocumentGeneratorBuilder(requestMetadataModel, contact);
            return documentGeneratorBuilder.Build();
        }

        private async Task<Contact> GetOrCreateContactAsync(ContactModel contact)
        {
            int SPIDparentID = Convert.ToInt32(_moduleConfiguration.RootContact);
            ICollection<Contact> contacts = await _webAPIClient.GetContactAsync($"$filter=SearchCode eq '{contact.Code}' and IncrementalFather eq {SPIDparentID}");
            Contact entityContact = contacts.FirstOrDefault();
            if (entityContact == null)
            {
                _logger.WriteDebug(new LogMessage($"Contact '{contact.Description}' with searchcode '{contact.Code}' not found and it's going to be creating."), LogCategories);
                entityContact = await CreateContactAsync(SPIDparentID, contact);
            }
            return entityContact;
        }

        private async Task<Contact> CreateContactAsync(int contactFatherId, ContactModel contactModel)
        {
            Guid uniqueId = Guid.NewGuid();
            Contact contact = new Contact()
            {
                IdContactType = DocSuiteWeb.Entity.Commons.ContactType.Citizen,
                IncrementalFather = contactFatherId,
                Description = contactModel.Description,
                BirthDate = contactModel.BirthDate,
                BirthPlace = contactModel.BirthPlace,
                SearchCode = contactModel.Code,
                Code = contactModel.Code,
                FiscalCode = contactModel.FiscalCode,
                Address = contactModel.Address,
                City = contactModel.City,
                CivicNumber = contactModel.CivicNumber,
                ZipCode = contactModel.CityCode,
                TelephoneNumber = contactModel.TelephoneNumber,
                CertifiedMail = contactModel.CertifiedMail,
                EmailAddress = contactModel.Email,
                IsActive = 1,
                UniqueId = uniqueId
            };
            await _webAPIClient.PostAsync(contact);
            contact = (await _webAPIClient.GetContactAsync($"$filter=UniqueId eq {uniqueId}")).Single();
            _logger.WriteInfo(new LogMessage($"Contact '{contactModel.Description}' ({contact.EntityId}) has been create succesfully"), LogCategories);
            return contact;
        }

        private WorkflowReferenceModel CreateFascicleBuildModel(SPIDMetadataModelWrapper spidRequestModel, Guid fascicleUniqueId, Guid correlationId, Contact contact, Guid fascicleFolderUniqueId)
        {
            WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel
            {
                ReferenceId = correlationId
            };

            MetadataModel fascicleMetadataModel = new MetadataModel();
            fascicleMetadataModel.TextFields.Add(new TextFieldModel()
            {
                Label = SPID_ID_METADATA_LABEL,
                Value = contact.SearchCode
            });
            fascicleMetadataModel.TextFields.Add(new TextFieldModel()
            {
                Label = DESCRIPTION_METADATA_LABEL,
                Value = contact.Description.Replace('|', ' ')
            });
            fascicleMetadataModel.TextFields.Add(new TextFieldModel()
            {
                Label = MOTIVAZIONI_METADATA_LABEL,
                Value = spidRequestModel.Motivazioni
            });
            fascicleMetadataModel.TextFields.Add(new TextFieldModel()
            {
                Label = FASCICLE_STATE_METADATA_LABEL,
                Value = "Richiesta avviata"
            });

            ICollection<ContactModel> contactModels = new List<ContactModel> {
                new ContactModel() { Id = _moduleConfiguration.FascicleResponsibleContact }
            };
            CategoryModel categoryModel = new CategoryModel() { IdCategory = _moduleConfiguration.CategoryId };

            FascicleBuildModel fascicleBuildModel = new FascicleBuildModel
            {
                WorkflowName = _moduleConfiguration.WorkflowRepositoryName,
                Fascicle = new FascicleModel
                {
                    UniqueId = fascicleUniqueId,
                    Category = categoryModel,
                    Conservation = _moduleConfiguration.ConservationPeriod,
                    FascicleObject = string.Concat("Accesso agli atti - ", contact.Description.Replace('|', ' ')),
                    MetadataValues = JsonConvert.SerializeObject(fascicleMetadataModel).Replace("'", "\'"),
                    MetadataRepository = new MetadataRepositoryModel() { Id = _moduleConfiguration.IdMetadataRepository },
                    FascicleType = DocSuiteWeb.Model.Entities.Fascicles.FascicleType.Procedure,
                    Note = spidRequestModel.Motivazioni,
                    StartDate = DateTimeOffset.UtcNow,
                    Contacts = contactModels,
                }
            };

            fascicleBuildModel.Fascicle.FascicleFolders.Add(new FascicleFolderModel()
            {
                UniqueId = fascicleFolderUniqueId,
                IdFascicle = fascicleUniqueId,
                Name = PUBLIC_FASCICLE_FOLDER_NAME,
                Typology = DocSuiteWeb.Model.Entities.Fascicles.FascicleFolderTypology.SubFascicle,
                Status = DocSuiteWeb.Model.Entities.Fascicles.FascicleFolderStatus.Internet
            });

            workflowReferenceModel.ReferenceType = DocSuiteWeb.Model.Entities.Commons.DSWEnvironmentType.Build;
            workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(fascicleBuildModel, ModuleConfigurationHelper.JsonSerializerSettings);
            return workflowReferenceModel;
        }
        private async Task<WorkflowReferenceModel> CreateProtocolBuildModelAsync(byte[] documentContent, Contact contact, Guid protocolUniqueId, Guid correlationId, Guid fascicleUniqueId, Guid fascicleFolderUniqueId)
        {
            WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel
            {
                ReferenceId = correlationId
            };

            Container container = (await _webAPIClient.GetContainerAsync(_moduleConfiguration.ContainerId)).SingleOrDefault();
            if (container == null)
            {
                throw new ArgumentException($"Container {_moduleConfiguration.ContainerId} not found");
            }
            ProtocolBuildModel protocolBuildModel = new ProtocolBuildModel
            {
                Protocol = new ProtocolModel
                {
                    UniqueId = protocolUniqueId,
                    MainDocument = new DocumentModel()
                    {
                        FileName = "richiesta_accesso_agli_atti.pdf",
                        ContentStream = documentContent
                    },
                    Category = new CategoryModel() { IdCategory = _moduleConfiguration.CategoryId },
                    Container = new ContainerModel()
                    {
                        IdContainer = container.EntityShortId,
                        Name = container.Name,
                        ProtLocation = new LocationModel()
                        {
                            IdLocation = container.ProtLocation.EntityShortId,
                            ProtocolArchive = container.ProtLocation.ProtocolArchive
                        }
                    },
                    Object = string.Concat("Accesso agli atti - ", contact.Description.Replace('|', ' ')),
                    ProtocolType = new ProtocolTypeModel(ProtocolTypology.Inbound)
                }
            };
            protocolBuildModel.Protocol.Contacts.Add(new ProtocolContactModel()
            {
                ComunicationType = ComunicationType.Sender,
                IdContact = contact.EntityId,
                Description = contact.Description
            });

            protocolBuildModel.WorkflowActions.Add(new WorkflowActionFascicleModel(
                new FascicleModel() { UniqueId = fascicleUniqueId },
                new DocumentUnitModel() { UniqueId = protocolBuildModel.Protocol.UniqueId, Environment = 1 },
                new FascicleFolderModel() { UniqueId = fascicleFolderUniqueId }));

            workflowReferenceModel.ReferenceType = DocSuiteWeb.Model.Entities.Commons.DSWEnvironmentType.Build;
            workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(protocolBuildModel, ModuleConfigurationHelper.JsonSerializerSettings);
            return workflowReferenceModel;

        }

        private async Task<WorkflowResult> StartWorkflowAsync(WorkflowReferenceModel workflowReferenceModelProtocol, WorkflowReferenceModel workflowReferenceModelFascicle, string externalIdentifer, string workflowName)
        {
            WorkflowStart workflowStart = new WorkflowStart
            {
                WorkflowName = workflowName
            };
            workflowStart.Arguments.Add(string.Concat(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, "_0"), new WorkflowArgument()
            {
                Name = string.Concat(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, "_0"),
                PropertyType = ArgumentType.Json,
                ValueString = JsonConvert.SerializeObject(workflowReferenceModelFascicle)
            });
            workflowStart.Arguments.Add(string.Concat(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, "_1"), new WorkflowArgument()
            {
                Name = string.Concat(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, "_1"),
                PropertyType = ArgumentType.Json,
                ValueString = JsonConvert.SerializeObject(workflowReferenceModelProtocol)
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
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER,
                ValueString = externalIdentifer
            });
            WorkflowResult workflowResult = await _webAPIClient.StartWorkflow(workflowStart);
            _logger.WriteInfo(new LogMessage(string.Concat("Workflow started correctly [IsValid: ", workflowResult.IsValid, "] with instanceId ", workflowResult.InstanceId)), LogCategories);
            return workflowResult;

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
        #endregion
    }
}
