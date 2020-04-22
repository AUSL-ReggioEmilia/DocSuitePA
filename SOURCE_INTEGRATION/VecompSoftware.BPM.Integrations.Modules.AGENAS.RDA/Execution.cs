using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.AGENAS.RDA.Configuration;
using VecompSoftware.BPM.Integrations.Modules.AGENAS.RDA.Models;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Metadata;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.DocSuiteWeb.Model.Workflow.Actions;
using VecompSoftware.Helpers.UDS;
using VecompSoftware.Helpers.Workflow;
using VecompSoftware.Services.Command.CQRS.Events.Entities.Fascicles;
using VecompSoftware.Services.Command.CQRS.Events.Entities.Protocols;
using VecompSoftware.Services.Command.CQRS.Events.Entities.Workflow;
using VecompSoftware.Services.Command.CQRS.Events.Models.Fascicles;
using Category = VecompSoftware.DocSuiteWeb.Entity.Commons.Category;
using Container = VecompSoftware.DocSuiteWeb.Entity.Commons.Container;
using DSWEnvironmentType = VecompSoftware.DocSuiteWeb.Model.Entities.Commons.DSWEnvironmentType;
using FascicleType = VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles.FascicleType;

namespace VecompSoftware.BPM.Integrations.Modules.AGENAS.RDA
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
                _logger.WriteError(new LogMessage("AGENAS.RDA -> Critical error in costruction module"), ex, LogCategories);
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
                _logger.WriteError(new LogMessage("AGENAS.RDA -> Execute critical error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> AGENAS.RDA"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCreateProtocol>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration, _moduleConfiguration.WorkflowStartRDASubscription, WorkflowStartRDACallbackAsync));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventUpdateFascicle>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration, _moduleConfiguration.WorkflowStartUpdateFascicleRDASubscription, EventUpdateFascicleCallbackAsync));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompleteFascicleBuild>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicBuilderEvent, _moduleConfiguration.WorkflowRDAFascicleBuildCompleteSubscription, EventWorkflowFascicleBuildCompleteCallback));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompleteWorkflowActivity>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowActivityCompleted, _moduleConfiguration.WorkflowActivityRDAAssignmentCompleteSubscription, WorkflowActivityRDAAssignmentCompleteCallback));
                _needInitializeModule = false;
            }
        }

        private async Task EventUpdateFascicleCallbackAsync(IEventUpdateFascicle evt)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"EventUpdateFascicleCallbackAsync -> evaluate event id {evt.Id}"), LogCategories);
                Fascicle fascicle = await _webAPIClient.GetFascicleAsync($"$filter=UniqueId eq {evt.ContentType.ContentTypeValue.UniqueId}");

                //await _webAPIClient.PutAsync(fascicle);
                _logger.WriteInfo(new LogMessage($"Fascicle {fascicle.UniqueId} updated correctly"), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("EventUpdateFascicleCallbackAsync -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        private async Task WorkflowStartRDACallbackAsync(IEventCreateProtocol evt)
        {
            _logger.WriteDebug(new LogMessage($"EventCreateProtocol -> evaluate event id {evt.Id}"), LogCategories);

            try
            {
                int collaborationId = 0;
                if (!evt.CustomProperties.ContainsKey(CustomPropertyName.COLLABORATION_ID) ||
                    !int.TryParse(evt.CustomProperties[CustomPropertyName.COLLABORATION_ID].ToString(), out collaborationId))
                {
                    throw new ArgumentNullException($"Undefined {CustomPropertyName.COLLABORATION_ID} property in event properties");
                }
                Collaboration collaboration = await _webAPIClient.GetCollaborationAsync(collaborationId, expandWorkflowActivities: true);
                DocumentUnit documentUnit = null;
                if (collaboration == null || collaboration.WorkflowInstance == null || 
                    (documentUnit = collaboration.WorkflowInstance.WorkflowActivities.FirstOrDefault(f=> f.DocumentUnitReferenced != null)?.DocumentUnitReferenced) == null ||
                    documentUnit.UDSRepository == null)
                {
                    throw new ArgumentNullException($"Collaboration {CustomPropertyName.COLLABORATION_ID} are realted to not standard workflow [DocumentUnit:{documentUnit !=null},UDSRepository:{documentUnit.UDSRepository != null}]");
                }
                string controllerName = Utils.GetWebAPIControllerName(documentUnit.UDSRepository.Name);
                Dictionary<int, Guid> documents = new Dictionary<int, Guid>();
                _logger.WriteDebug(new LogMessage($"Get UDS {documentUnit.UniqueId} metadatas"), LogCategories);
                Dictionary<string, object> uds_metadatas = await _webAPIClient.GetUDS(controllerName, documentUnit.UniqueId, documents);
                if (uds_metadatas == null || !uds_metadatas.Any())
                {
                    throw new ArgumentNullException($"UDS not found");
                }
                string tag = JsonConvert.DeserializeObject<List<string>>(uds_metadatas["ProceduraDiAcquisto"] as string).FirstOrDefault();
                _logger.WriteDebug(new LogMessage($"UDS has metadata repository tag {tag} and it found in mapping {_moduleConfiguration.MetadataRepositoryMappings.ContainsKey(tag)}"), LogCategories);
                Guid metadataRepositoryId = _moduleConfiguration.MetadataRepositoryMappings[tag];
                MetadataRepository metadataRepository = (await _webAPIClient.GetMetadataRepositoryAsync($"$filter=UniqueId eq {metadataRepositoryId}")).SingleOrDefault();
                if (metadataRepository == null)
                {
                    throw new ArgumentNullException($"MetadataRepository {metadataRepositoryId} not exist");
                }
                Guid correlationId = Guid.NewGuid();
                if (evt.CorrelationId.HasValue)
                {
                    correlationId = evt.CorrelationId.Value;
                }
                Guid protocolUniqueId = evt.ContentType.ContentTypeValue.UniqueId;
                Guid fascicleUniqueId = Guid.NewGuid();

                _logger.WriteDebug(new LogMessage($"Preparing starting workflow with correlationId {correlationId}, fascicleUniqueId {fascicleUniqueId}, protocolUniqueId {protocolUniqueId} udsId {documentUnit.UniqueId}"), LogCategories);
                WorkflowReferenceModel fascicleWorkflowReferenceModel = CreateFascicleBuildModel(fascicleUniqueId, protocolUniqueId, documentUnit, uds_metadatas, metadataRepository, correlationId);
                WorkflowResult workflowResult = await StartWorkflowAsync(fascicleWorkflowReferenceModel, _moduleConfiguration.WorkflowRepositoryName);
                if (!workflowResult.IsValid)
                {
                    _logger.WriteError(new LogMessage("An error occured in start RDA workflow"), LogCategories);
                    throw new Exception("VecompSoftware.BPM.Integrations.Modules.AGENAS.RDA");
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("EventWorkflowStartedRequestCallbackAsync -> error complete start workflow"), ex, LogCategories);
                throw;
            }
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
                    throw new Exception("VecompSoftware.BPM.Integrations.Modules.AGENAS.RDA");
                }
                Dossier dossier = new Dossier();
                dossier.Container = new Container() { EntityShortId = _moduleConfiguration.ContainerDossierId };
                dossier.DossierRoles.Add(new DossierRole()
                {
                    AuthorizationRoleType = AuthorizationRoleType.Responsible,
                    IsMaster = true,
                    Status = DossierRoleStatus.Active,
                    Role =new Role() { EntityShortId = _moduleConfiguration.RoleDossierId }
                });
                dossier.Subject = evt.ContentType.ContentTypeValue.Fascicle.FascicleObject;
                dossier.StartDate = evt.ContentType.ContentTypeValue.Fascicle.StartDate.Value;
                dossier = await _webAPIClient.PostAsync(dossier);
                _logger.WriteInfo(new LogMessage($"Dossier {dossier.Subject} has been created {dossier.UniqueId}"), LogCategories);
                DossierFolder dossierFolder = new DossierFolder()
                {
                    Dossier = dossier,
                    ParentInsertId = dossier.UniqueId,
                    Name = "RDA",
                    Status = DossierFolderStatus.Folder,
                };
                dossierFolder = await _webAPIClient.PostAsync(dossierFolder);
                _logger.WriteInfo(new LogMessage($"Folder {dossierFolder.Name} has been created {dossierFolder.UniqueId}"), LogCategories);
                Guid folderRDAId = dossierFolder.UniqueId;
                dossierFolder = new DossierFolder()
                {
                    Dossier = dossier,
                    Fascicle = new Fascicle() { UniqueId = evt.ContentType.ContentTypeValue.Fascicle.UniqueId },
                    ParentInsertId = folderRDAId,
                    Name = evt.ContentType.ContentTypeValue.Fascicle.FascicleObject,
                    Category = new Category() { EntityShortId = _moduleConfiguration.RDACategoryId },
                };
                dossierFolder = await _webAPIClient.PostAsync(dossierFolder);
                _logger.WriteInfo(new LogMessage($"Fascicle {dossierFolder.Name} has been added to Dossier {dossier.UniqueId}/{dossierFolder.UniqueId}"), LogCategories);
                dossierFolder = new DossierFolder()
                {
                    Dossier = dossier,
                    ParentInsertId = dossier.UniqueId,
                    Name = "CONTRATTO",
                    Status = DossierFolderStatus.Folder,
                };
                dossierFolder = await _webAPIClient.PostAsync(dossierFolder);
                _logger.WriteInfo(new LogMessage($"Folder {dossierFolder.Name} has been created {dossierFolder.UniqueId}"), LogCategories);
                dossierFolder = new DossierFolder()
                {
                    Dossier = dossier,
                    ParentInsertId = dossier.UniqueId,
                    Name = "ESECUZIONE",
                    Status = DossierFolderStatus.Folder,
                };
                dossierFolder = await _webAPIClient.PostAsync(dossierFolder);
                
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("EventWorkflowFascicleBuildCompleteCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        private async Task WorkflowActivityRDAAssignmentCompleteCallback(IEventCompleteWorkflowActivity evt)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"WorkflowActivityRDAAssignmentCompleteCallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteInfo(new LogMessage($"Notifying workflow activity RDA assignment complete for WorkflowInstanceId {evt.CorrelationId}"), LogCategories);
                WorkflowNotify workflowNotify = null;
                WorkflowResult workflowResult = null;

                WorkflowActivity workflowActivity = evt.ContentType.ContentTypeValue;
                _logger.WriteInfo(new LogMessage($"Notifying WorkflowActivity for IdWorkflowActivity {workflowActivity.UniqueId}"), LogCategories);
                workflowNotify = new WorkflowNotify(workflowActivity.UniqueId)
                {
                    WorkflowName = workflowActivity.WorkflowInstance?.WorkflowRepository?.Name,
                    ModuleName = ModuleConfigurationHelper.MODULE_NAME
                };
                workflowResult = await _webAPIClient.WorkflowNotify(workflowNotify);
                _logger.WriteInfo(new LogMessage($"Workflow notify correctly [IsValid: {workflowResult.IsValid}] with instanceId {workflowResult.InstanceId}"), LogCategories);
                if (!workflowResult.IsValid)
                {
                    _logger.WriteError(new LogMessage("An error occured in notify workflow activity"), LogCategories);
                    throw new Exception("VecompSoftware.BPM.Integrations.Modules.AGENAS.RDA");
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("EventWorkflowFascicleBuildCompleteCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }
        
        private WorkflowReferenceModel CreateFascicleBuildModel(Guid fascicleUniqueId, Guid protocolUniqueId, DocumentUnit documentUnit, Dictionary<string, object> uds_metadatas,
            MetadataRepository metadataRepository, Guid correlationId)
        {
            WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel
            {
                ReferenceId = correlationId
            };

            ICollection<ContactModel> contactModels = new List<ContactModel> 
            {
                new ContactModel() { Id = _moduleConfiguration.FascicleResponsibleContact }
            };
            CategoryModel categoryModel = new CategoryModel() { IdCategory = _moduleConfiguration.RDACategoryId };

            FascicleBuildModel fascicleBuildModel = new FascicleBuildModel
            {
                WorkflowName = _moduleConfiguration.WorkflowRepositoryName,
                UniqueId = correlationId,
                Fascicle = new FascicleModel
                {
                    UniqueId = fascicleUniqueId,
                    Category = categoryModel,
                    Conservation = _moduleConfiguration.ConservationPeriod,
                    FascicleObject = uds_metadatas["_subject"] as string,
                    MetadataValues = metadataRepository.JsonMetadata,
                    MetadataRepository = new MetadataRepositoryModel() { Id = metadataRepository.UniqueId },
                    FascicleType = FascicleType.Procedure,
                    StartDate = DateTimeOffset.UtcNow,
                    Contacts = contactModels,
                }
            };
            fascicleBuildModel.WorkflowActions.Add(new WorkflowActionFascicleModel(
                 new FascicleModel() { UniqueId = fascicleUniqueId },
                 new DocumentUnitModel() { UniqueId = protocolUniqueId, Environment = 1 },
                 null));
            fascicleBuildModel.WorkflowActions.Add(new WorkflowActionDocumentUnitLinkModel(
                new DocumentUnitModel() { UniqueId = protocolUniqueId, Environment = (int)DSWEnvironmentType.Protocol },
                new DocumentUnitModel() { UniqueId = documentUnit.UniqueId, Environment = documentUnit.Environment, IdUDSRepository = documentUnit.UDSRepository.UniqueId }));

            workflowReferenceModel.ReferenceType = DSWEnvironmentType.Build;
            workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(fascicleBuildModel, ModuleConfigurationHelper.JsonSerializerSettings);
            return workflowReferenceModel;
        }

        private async Task<WorkflowResult> StartWorkflowAsync(WorkflowReferenceModel workflowReferenceModelFascicle, string workflowName)
        {
            WorkflowStart workflowStart = new WorkflowStart
            {
                WorkflowName = workflowName
            };
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, new WorkflowArgument()
            {
                Name = WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL,
                PropertyType = ArgumentType.Json,
                ValueString = JsonConvert.SerializeObject(workflowReferenceModelFascicle)
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
            WorkflowResult workflowResult = await _webAPIClient.StartWorkflow(workflowStart);
            _logger.WriteInfo(new LogMessage($"Workflow started correctly [IsValid: {workflowResult.IsValid}] with instanceId {workflowResult.InstanceId}"), LogCategories);
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
