using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.FALCK.Data.Entities;
using VecompSoftware.BPM.Integrations.Modules.FALCK.Workflows.Models;
using VecompSoftware.BPM.Integrations.Modules.FALCK.Workflows.Persistance.Repositories;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Core.Command.CQRS.Commands.Models.UDS;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.Helpers.UDS;
using VecompSoftware.Helpers.Workflow;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Events;
using VecompSoftware.Services.Command.CQRS.Events.Entities.Workflow;
using VecompSoftware.Services.Command.CQRS.Events.Models.UDS;

namespace VecompSoftware.BPM.Integrations.Modules.FALCK.Workflows
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IWebAPIClient _webApiClient;
        private readonly IServiceBusClient _serviceBusClient;
        private readonly IDocumentClient _documentClient;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly IReadOnlyCollection<SpecialCategoryModel> _specialCategories;
        private readonly IReadOnlyCollection<ProcurementRightModel> _procurementRight;
        private readonly ConcurrentDictionary<int, Guid> _pendingCommands;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private bool _needInitializeModule = false;
        private readonly UDSService _udsService;
        private readonly WorkflowService _workflowService;
        private readonly FascicleService _fascicleService;
        private const string EXTERNAL_VIEWER_FASCICLE_LINK = "#/Fascicolo/identificativo/{0}/Sommario";
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
        public Execution(ILogger logger, IWebAPIClient webApiClient, IServiceBusClient serviceBusClient, IDocumentClient documentClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _webApiClient = webApiClient;
                _documentClient = documentClient;
                _udsService = new UDSService(logger, webApiClient);
                _workflowService = new WorkflowService(logger, webApiClient);
                _fascicleService = new FascicleService(logger, webApiClient);
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _specialCategories = ModuleConfigurationHelper.GetSpecialCategories();
                _procurementRight = ModuleConfigurationHelper.GetProcurementRights();
                _serviceBusClient = serviceBusClient;
                _needInitializeModule = true;

                _pendingCommands = new ConcurrentDictionary<int, Guid>();
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("FALCK.Workflows -> Critical error in costruction module"), ex, LogCategories);
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
            InitializeModule();

            TransporterModel transporterModel = new TransporterModel();
            using (WorkflowMetadataRepository workflowMetadataRepository = new WorkflowMetadataRepository(_logger))
            {
                transporterModel.WorkflowMetadata = workflowMetadataRepository.GetManageableMetadata();
                if (transporterModel.WorkflowMetadata == null)
                {
                    _logger.WriteDebug(new LogMessage("Execute -> no data found"), LogCategories);
                    return;
                }
            }

            _logger.WriteInfo(new LogMessage(string.Format("Execute -> found workflow metadataId {0}",
                transporterModel.WorkflowMetadata.MetadataId)), LogCategories);

            if (_pendingCommands.ContainsKey(transporterModel.WorkflowMetadata.MetadataId))
            {
                return;
            }

            bool needChangeMetadataStatus = false;
            if (!ValidateMetadata(transporterModel, out needChangeMetadataStatus))
            {
                if (needChangeMetadataStatus)
                {
                    UpdateMetadataStatus(WorkflowStatusType.ValidationError, transporterModel.WorkflowMetadata.MetadataId);
                }
                return;
            }

            CreateUDS(transporterModel);
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> FALCK.Workflows"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventInsertUDSData>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.UDSTopic, 
                    _moduleConfiguration.UDSSubscriptionSuccess, UDSCompletedCallbackAsync));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventError>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.UDSTopic, 
                    _moduleConfiguration.UDSSubscriptionError, UDSErrorCallbackAsync));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompleteWorkflowInstance>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.WorkflowTopic,
                    _moduleConfiguration.WorkflowSubscriptionSuccess, WorkflowSuccessCallbackAsync));

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

        private async Task UDSCompletedCallbackAsync(IEventInsertUDSData eventMessage)
        {
            if (Cancel)
            {
                return;
            }

            _logger.WriteInfo(new LogMessage(string.Format("UDSCompletedCallbackAsync -> received callback with eventId {0}", eventMessage.Id)), LogCategories);
            CommandInsertUDSData correlatedCommand = eventMessage.CorrelatedCommands.Cast<CommandInsertUDSData>().FirstOrDefault();
            if (correlatedCommand == null || !_pendingCommands.Any(x => x.Value == correlatedCommand.Id))
            {
                _logger.WriteInfo(new LogMessage(string.Format("UDSCompletedCallbackAsync -> no pending command found with commandId {0}", correlatedCommand.Id)), LogCategories);
                return;
            }

            int metadataId = _pendingCommands.First(x => x.Value == correlatedCommand.Id).Key;
            TransporterModel transporterModel = new TransporterModel();
            using (WorkflowMetadataRepository workflowMetadataRepository = new WorkflowMetadataRepository(_logger))
            {
                transporterModel.WorkflowMetadata = workflowMetadataRepository.GetById(metadataId);

                transporterModel = DetectIsWorkflowAutoapproval(transporterModel);
            }

            try
            {
                transporterModel.DomainUser = _webApiClient.GetUserAsync(transporterModel.WorkflowMetadata.CreationUser).Result;

                transporterModel.UDSRepository = _udsService.GetRepository(correlatedCommand.ContentType.ContentTypeValue.UDSRepository.Id);
                transporterModel.UDS = _udsService.GetUDS(transporterModel.UDSRepository, correlatedCommand.ContentType.ContentTypeValue.UniqueId);
                if (string.IsNullOrEmpty(transporterModel.WorkflowMetadata.ReferenceDocNumber))
                {
                    transporterModel.Fascicle = await _fascicleService.GetByUDSIdAsync(transporterModel.UDS.UDSId);
                    if (transporterModel.Fascicle == null)
                    {
                        _logger.WriteInfo(new LogMessage(string.Format("UDSCompletedCallbackAsync -> create new fascicle for UDSId {0}",
                            transporterModel.UDS.UDSId)), LogCategories);

                        transporterModel.Fascicle = await CreateNewFascicleAsync(transporterModel);
                        LinkFascicle(transporterModel.UDS, transporterModel.Fascicle);
                    }
                    else
                    {
                        _logger.WriteInfo(new LogMessage(string.Format("UDSCompletedCallbackAsync -> fascicle already exist with Id {0}",
                            transporterModel.Fascicle.UniqueId)), LogCategories);
                    }
                    UpdateMedatadaDocSuiteURL(transporterModel.WorkflowMetadata, transporterModel.Fascicle);
                }
                else
                {
                    //TODO: RIVEDERE CON REPO CORRETTO
                    UDSBaseEntity referencedUDS = _udsService.GetUDSByNumber(new UDSRepository(Guid.NewGuid()) { Name = "RDA" }, transporterModel.WorkflowMetadata.ReferenceDocNumber);
                    if (referencedUDS != null)
                    {
                        transporterModel.Fascicle = await _fascicleService.GetByUDSIdAsync(referencedUDS.UDSId);
                        if (transporterModel.Fascicle != null)
                        {
                            LinkFascicle(transporterModel.UDS, transporterModel.Fascicle);
                            UpdateMedatadaDocSuiteURL(transporterModel.WorkflowMetadata, transporterModel.Fascicle);
                        }
                    }

                    if (transporterModel.Fascicle == null)
                    {
                        _logger.WriteWarning(new LogMessage(string.Format("UDSCompletedCallbackAsync -> fascicle not defined for UDSId {0}", referencedUDS.UDSId)), LogCategories);
                    }
                }

                transporterModel = PopulateSpecialCategoryResponsabile(transporterModel);
                transporterModel = PopulateProcurementRight(transporterModel);
                transporterModel = await CreateCollaborationModel(transporterModel);

                StartWorkflow(transporterModel, _moduleConfiguration.WorkflowStarterName);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("UDSCompletedCallbackAsync -> error start workflow"), ex, LogCategories);
                UpdateMetadataStatus(WorkflowStatusType.UnexpectedError, metadataId);
                throw;
            }
            finally
            {
                Guid id;
                if (!_pendingCommands.TryRemove(metadataId, out id))
                {
                    _logger.WriteError(new LogMessage(string.Concat("UDSCompletedCallbackAsync -> pendingCommands ", metadataId, " not removed")), null, LogCategories);
                };
            }
        }

        private async Task<TransporterModel> CreateCollaborationModel(TransporterModel transporterModel)
        {
            try
            {
                _logger.WriteDebug(new LogMessage("CreateCollaborationModel -> Geneare Model"), LogCategories);
                transporterModel.CollaborationModel = new CollaborationModel
                {
                    RegistrationName = transporterModel.WorkflowMetadata.CreationUser,
                    RegistrationUser = transporterModel.DomainUser.DisplayName
                };
                WorkflowAttachment mainDocument = transporterModel.WorkflowMetadata.WorkflowAttachments.First(f => Convert.ToBoolean(f.IsMainDocument));
                ArchiveDocument archiveDocument = await StoreArchiveDocument(mainDocument);
                short collaborationIncremental = 0;
                transporterModel.CollaborationModel.CollaborationVersionings.Add(new CollaborationVersioningModel()
                {
                    Incremental = 1,
                    CollaborationIncremental = collaborationIncremental++,
                    DocumentName = archiveDocument.Name,
                    IdDocument = archiveDocument.IdLegacyChain,
                    DocumentGroup = "MainDocument",
                    IsActive = true,
                });
                _logger.WriteDebug(new LogMessage(string.Concat("CreateCollaborationModel -> Add MainDocument ", archiveDocument.Name)), LogCategories);
                foreach (WorkflowAttachment workflowAttachment in transporterModel.WorkflowMetadata.WorkflowAttachments.Where(f => !Convert.ToBoolean(f.IsMainDocument)))
                {
                    archiveDocument = await StoreArchiveDocument(workflowAttachment);
                    _logger.WriteDebug(new LogMessage(string.Concat("CreateCollaborationModel -> Add Attachment ", archiveDocument.Name)), LogCategories);
                    transporterModel.CollaborationModel.CollaborationVersionings.Add(new CollaborationVersioningModel()
                    {
                        Incremental = 1,
                        CollaborationIncremental = collaborationIncremental++,
                        DocumentName = archiveDocument.Name,
                        IdDocument = archiveDocument.IdLegacyChain,
                        DocumentGroup = "Attachment",
                        IsActive = true,
                    });
                }

                if (transporterModel.WorkflowMetadata.DocumentType == DocumentType.ODA && File.Exists(_moduleConfiguration.GeneralConditionFileName))
                {
                    _logger.WriteDebug(new LogMessage(string.Concat("CreateCollaborationModel -> Add GeneralConditionFileName {0}", archiveDocument.Name)), LogCategories);
                    FileInfo fileInfo = new FileInfo(_moduleConfiguration.GeneralConditionFileName);

                    WorkflowAttachment generalCondition = new WorkflowAttachment()
                    {
                        InternalFileName = fileInfo.Name,
                        IsMainDocument = 0,
                        OriginalFileName = fileInfo.Name,
                    };
                    archiveDocument = await StoreArchiveDocument(generalCondition);
                    transporterModel.CollaborationModel.CollaborationVersionings.Add(new CollaborationVersioningModel()
                    {
                        Incremental = 1,
                        CollaborationIncremental = collaborationIncremental++,
                        DocumentName = archiveDocument.Name,
                        IdDocument = archiveDocument.IdLegacyChain,
                        DocumentGroup = "Attachment",
                        IsActive = true,
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("ArchiveDocument -> error start workflow"), ex, LogCategories);
                UpdateMetadataStatus(WorkflowStatusType.UnexpectedError, transporterModel.WorkflowMetadata.MetadataId);
                throw;
            }
            return transporterModel;
        }

        private async Task<ArchiveDocument> StoreArchiveDocument(WorkflowAttachment workflowAttachment)
        {
            string filePath = Path.Combine(_moduleConfiguration.AttachmentsPath, workflowAttachment.InternalFileName);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("StoreArchiveDocument: document not found", filePath);
            }
            byte[] docContent = File.ReadAllBytes(filePath);
            ArchiveDocument archiveDocument = new ArchiveDocument()
            {
                Archive = _moduleConfiguration.CollaborationBiblosArchiveName,
                ContentStream = docContent,
                Name = workflowAttachment.OriginalFileName
            };
            return await _documentClient.InsertDocumentAsync(archiveDocument);
        }

        private async Task<ICollection<ArchiveDocument>> StoreArchiveDocument(ICollection<WorkflowAttachment> workflowAttachments)
        {
            List<ArchiveDocument> archiveDocuments = new List<ArchiveDocument>();
            byte[] docContent;
            foreach (WorkflowAttachment workflowAttachment in workflowAttachments)
            {
                string filePath = Path.Combine(_moduleConfiguration.AttachmentsPath, workflowAttachment.InternalFileName);
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("StoreArchiveDocument: document not found", filePath);
                }
                docContent = File.ReadAllBytes(filePath);
                archiveDocuments.Add(new ArchiveDocument()
                {
                    Archive = _moduleConfiguration.CollaborationBiblosArchiveName,
                    ContentStream = docContent,
                    Name = workflowAttachment.OriginalFileName
                });
            }
            return await _documentClient.InsertDocumentsAsync(archiveDocuments);
        }

        private async Task UDSErrorCallbackAsync(IEventError eventMessage)
        {
            await Task.Run(() =>
            {
                if (Cancel)
                {
                    return;
                }

                _logger.WriteInfo(new LogMessage(string.Format("UDSErrorCallbackAsync -> received callback with event id {0}", eventMessage.Id)), LogCategories);
                ICommand correlatedCommand = eventMessage.CorrelatedCommands.FirstOrDefault();
                if (correlatedCommand == null || !_pendingCommands.Any(x => x.Value == correlatedCommand.Id))
                {
                    _logger.WriteInfo(new LogMessage(string.Format("UDSErrorCallbackAsync -> no pending command found with command id {0}", correlatedCommand.Id)), LogCategories);
                    return;
                }

                int metadataId = _pendingCommands.First(x => x.Value == correlatedCommand.Id).Key;
                UpdateMetadataStatus(WorkflowStatusType.UnexpectedError, metadataId);
                Guid id;
                if (!_pendingCommands.TryRemove(metadataId, out id))
                {
                    _logger.WriteError(new LogMessage(string.Concat("UDSErrorCallbackAsync -> pendingCommands ", metadataId, " not removed")), null, LogCategories);
                };
            });
        }

        private async Task WorkflowSuccessCallbackAsync(IEventCompleteWorkflowInstance eventMessage)
        {
            await Task.Run(() =>
            {
                if (Cancel)
                {
                    return;
                }

                _logger.WriteInfo(new LogMessage(string.Format("WorkflowSuccessCallbackAsync -> received callback with event id {0}", eventMessage.Id)), LogCategories);

                TransporterModel transporterModel = new TransporterModel();

                WorkflowInstance instance = eventMessage.ContentType.ContentTypeValue;
                if (!instance.InstanceId.HasValue)
                {
                    _logger.WriteError(new LogMessage(string.Format("WorkflowSuccessCallbackAsync -> WorkflowInstance {0} not no value ", instance.UniqueId)), LogCategories);
                    throw new ArgumentException(string.Format("InstanceId {0} not found", instance.InstanceId.Value));
                }

                using (WorkflowMetadataRepository workflowMetadataRepository = new WorkflowMetadataRepository(_logger))
                {
                    transporterModel.WorkflowMetadata = workflowMetadataRepository.GetByInstance(instance.InstanceId.Value);
                    if (transporterModel.WorkflowMetadata == null)
                    {
                        _logger.WriteError(new LogMessage(string.Format("WorkflowSuccessCallbackAsync -> InstanceId {0} not found", instance.InstanceId.Value)), LogCategories);
                        throw new ArgumentException(string.Format("InstanceId {0} not found", instance.InstanceId.Value));
                    }
                    _logger.WriteInfo(new LogMessage(string.Format("WorkflowSuccessCallbackAsync -> process metadata id {0}", transporterModel.WorkflowMetadata.MetadataId)), LogCategories);
                    transporterModel.WorkflowMetadata.WorkflowUpdateDate = DateTime.UtcNow;
                    transporterModel.WorkflowMetadata.WorkflowCompletedID = eventMessage.Id.ToString();
                    workflowMetadataRepository.Update(transporterModel.WorkflowMetadata);
                }

                try
                {
                    transporterModel.DomainUser = _webApiClient.GetUserAsync(transporterModel.WorkflowMetadata.CreationUser).Result;
                    transporterModel = PopulateSpecialCategoryResponsabile(transporterModel);
                    transporterModel = PopulateProcurementRight(transporterModel);

                    Guid udsId = instance.WorkflowProperties.Single(x => x.Name == WorkflowPropertyHelper.DSW_FIELD_UDS_ID).ValueGuid.Value;
                    _logger.WriteDebug(new LogMessage(string.Format("WorkflowSuccessCallbackAsync -> found DSW_FIELD_UDS_ID {0}", udsId)), LogCategories);

                    Guid udsRepositoryId = instance.WorkflowProperties.Single(x => x.Name == WorkflowPropertyHelper.DSW_FIELD_UDS_REPOSITORY_ID).ValueGuid.Value;
                    _logger.WriteDebug(new LogMessage(string.Format("WorkflowSuccessCallbackAsync -> found DSW_FIELD_UDS_REPOSITORY_ID {0}", udsRepositoryId)), LogCategories);

                    WorkflowProperty startWorkflowNameProperty = instance.WorkflowProperties.SingleOrDefault(x => x.Name == WorkflowService.START_WORKFLOW_NAME);
                    if (startWorkflowNameProperty != null)
                    {
                        _logger.WriteDebug(new LogMessage("WorkflowSuccessCallbackAsync -> found startWorkflowNameProperty"), LogCategories);
                    }

                    WorkflowProperty collaborationModelProperty = instance.WorkflowProperties.SingleOrDefault(x => x.Name == WorkflowPropertyHelper.DSW_PROPERTY_MODEL);
                    if (collaborationModelProperty != null)
                    {
                        transporterModel.CollaborationModel = JsonConvert.DeserializeObject<CollaborationModel>(collaborationModelProperty.ValueString, ModuleConfigurationHelper.JsonSerializerSettings);
                        _logger.WriteDebug(new LogMessage("WorkflowSuccessCallbackAsync -> found CollaborationModel"), LogCategories);
                    }

                    if (instance.WorkflowRepository.Name == _moduleConfiguration.WorkflowStarterName)
                    {
                        _logger.WriteInfo(new LogMessage(string.Format("WorkflowSuccessCallbackAsync -> starter workflow completed for instance id {0}", instance.InstanceId)), LogCategories);

                        string startWorkflowName = startWorkflowNameProperty.ValueString;
                        bool startWorkflowStatus = startWorkflowNameProperty.WorkflowInstance.WorkflowProperties.Single(x => x.Name == WorkflowService.START_WORKFLOW_STATUS).ValueBoolean.GetValueOrDefault(false);
                        if (!startWorkflowStatus)
                        {
                            _logger.WriteError(new LogMessage(string.Format("WorkflowSuccessCallbackAsync -> workflow status is false for instance id {0}", instance.InstanceId)), LogCategories);
                            UpdateMetadataStatus(WorkflowStatusType.UnexpectedError, transporterModel.WorkflowMetadata.MetadataId);
                            return;
                        }
                        transporterModel.UDSRepository = new UDSRepository(udsRepositoryId);
                        transporterModel.UDS = new UDSBaseEntity()
                        {
                            UDSId = udsId,
                            IdUDSRepository = udsRepositoryId
                        };

                        StartWorkflow(transporterModel, startWorkflowName);
                    }
                    else
                    {
                        _logger.WriteInfo(new LogMessage(string.Format("WorkflowSuccessCallbackAsync -> workflow completed for instance id {0}", instance.InstanceId)), LogCategories);
                        WorkflowProperty workflowApprovalProperty = instance.WorkflowProperties.SingleOrDefault(x => x.Name == WorkflowService.WORKFLOW_AUTOAPPROVAL);
                        bool? isWorkflowAutoApproval = null;
                        bool isWorkflowApproved = false;
                        if (workflowApprovalProperty != null)
                        {
                            isWorkflowAutoApproval = workflowApprovalProperty.ValueBoolean.Value;
                            isWorkflowApproved = workflowApprovalProperty.WorkflowInstance.WorkflowProperties.Single(x => x.Name == WorkflowService.WORKFLOW_APPROVED).ValueBoolean.Value;
                        }
                        else
                        {
                            isWorkflowApproved = instance.WorkflowProperties.Single(x => x.Name == WorkflowService.WORKFLOW_APPROVED).ValueBoolean.Value;
                        }

                        using (WorkflowMetadataRepository workflowMetadataRepository = new WorkflowMetadataRepository(_logger))
                        {
                            transporterModel.WorkflowMetadata.WorkflowStatus = WorkflowStatusType.WorkflowCompleted;
                            transporterModel.WorkflowMetadata.IsWorkflowApproved = Convert.ToByte(isWorkflowApproved);
                            if (isWorkflowAutoApproval.HasValue)
                            {
                                transporterModel.WorkflowMetadata.IsWorkflowAutoapproval = Convert.ToByte(isWorkflowAutoApproval);
                            }
                            transporterModel.WorkflowMetadata.WorkflowUpdateDate = DateTime.UtcNow;
                            transporterModel.WorkflowMetadata.WorkflowCompletedID = eventMessage.Id.ToString();
                            _logger.WriteInfo(new LogMessage(string.Format("WorkflowSuccessCallbackAsync -> workflow set WorkflowCompletedID {0}", eventMessage.Id.ToString())), LogCategories);
                            workflowMetadataRepository.Update(transporterModel.WorkflowMetadata);
                        }
                        try
                        {
                            Array.ForEach(Path.GetInvalidFileNameChars(), c => transporterModel.WorkflowMetadata.DocumentNumber = transporterModel.WorkflowMetadata.DocumentNumber.Replace(c.ToString(), string.Empty));
                            string drop_folder = Path.Combine(_moduleConfiguration.DropsPath, transporterModel.WorkflowMetadata.DocumentNumber);
                            Directory.CreateDirectory(drop_folder);
                            string filePath = string.Empty;
                            foreach (WorkflowAttachment workflowAttachment in transporterModel.WorkflowMetadata.WorkflowAttachments)
                            {
                                filePath = Path.Combine(_moduleConfiguration.AttachmentsPath, workflowAttachment.InternalFileName);
                                _logger.WriteDebug(new LogMessage(string.Format("origin -> successful clean to drops {0}", drop_folder)), LogCategories);
                                File.Move(filePath, Path.Combine(drop_folder, workflowAttachment.InternalFileName));
                            }
                            _logger.WriteInfo(new LogMessage(string.Format("WorkflowSuccessCallbackAsync -> successful clean to drops {0}", drop_folder)), LogCategories);
                        }
                        catch (Exception ex)
                        {
                            _logger.WriteWarning(new LogMessage("WorkflowSuccessCallbackAsync -> move to drop folder error. Ignored!"), ex, LogCategories);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.WriteError(new LogMessage("WorkflowSuccessCallbackAsync -> error complete workflow"), ex, LogCategories);
                    UpdateMetadataStatus(WorkflowStatusType.UnexpectedError,
                        transporterModel == null || transporterModel.WorkflowMetadata == null ? -1 : transporterModel.WorkflowMetadata.MetadataId);
                    throw;
                }
            });
        }

        private bool ValidateMetadata(TransporterModel transporterModel, out bool needChangeMetadataStatus)
        {
            needChangeMetadataStatus = true;

            if (string.IsNullOrEmpty(transporterModel.WorkflowMetadata.CompanyCode))
            {
                _logger.WriteWarning(new LogMessage(string.Format("ValidateMetadata -> entityId {0} - Workflow Metadata CompanyCode {1} empty",
                     transporterModel.WorkflowMetadata.MetadataId, transporterModel.WorkflowMetadata.CompanyCode)), LogCategories);
                return false;
            }

            if (string.IsNullOrEmpty(transporterModel.WorkflowMetadata.CompanyName))
            {
                _logger.WriteWarning(new LogMessage(string.Format("ValidateMetadata -> entityId {0} - Workflow Metadata CompanyName {1} empty",
                     transporterModel.WorkflowMetadata.MetadataId, transporterModel.WorkflowMetadata.CompanyName)), LogCategories);
                return false;
            }

            if (string.IsNullOrEmpty(transporterModel.WorkflowMetadata.DocumentNumber))
            {
                _logger.WriteWarning(new LogMessage(string.Format("ValidateMetadata -> entityId {0} - Workflow Metadata DocumentNumber {1} empty",
                     transporterModel.WorkflowMetadata.MetadataId, transporterModel.WorkflowMetadata.DocumentNumber)), LogCategories);
                return false;
            }

            if (string.IsNullOrEmpty(transporterModel.WorkflowMetadata.DocumentDescription))
            {
                _logger.WriteWarning(new LogMessage(string.Format("ValidateMetadata -> entityId {0} - Workflow Metadata DocumentDescription {1} empty",
                     transporterModel.WorkflowMetadata.MetadataId, transporterModel.WorkflowMetadata.DocumentDescription)), LogCategories);
                return false;
            }

            if (string.IsNullOrEmpty(transporterModel.WorkflowMetadata.JobCode))
            {
                _logger.WriteWarning(new LogMessage(string.Format("ValidateMetadata -> entityId {0} - Workflow Metadata JobCode {1} empty",
                     transporterModel.WorkflowMetadata.MetadataId, transporterModel.WorkflowMetadata.JobCode)), LogCategories);
                return false;
            }

            if (string.IsNullOrEmpty(transporterModel.WorkflowMetadata.JobDescription))
            {
                _logger.WriteWarning(new LogMessage(string.Format("ValidateMetadata -> entityId {0} - Workflow Metadata JobDescription {1} empty",
                     transporterModel.WorkflowMetadata.MetadataId, transporterModel.WorkflowMetadata.JobDescription)), LogCategories);
                return false;
            }

            if (string.IsNullOrEmpty(transporterModel.WorkflowMetadata.JobType))
            {
                _logger.WriteWarning(new LogMessage(string.Format("ValidateMetadata -> entityId {0} - Workflow Metadata JobType {1} empty",
                     transporterModel.WorkflowMetadata.MetadataId, transporterModel.WorkflowMetadata.JobType)), LogCategories);
                return false;
            }

            if (transporterModel.WorkflowMetadata.AmountLCY < 0)
            {
                _logger.WriteWarning(new LogMessage(string.Format("ValidateMetadata -> entityId {0} - Workflow Metadata AmountLCY {1} is not valid",
                     transporterModel.WorkflowMetadata.MetadataId, transporterModel.WorkflowMetadata.AmountLCY)), LogCategories);
                return false;
            }

            if (string.IsNullOrEmpty(transporterModel.WorkflowMetadata.CurrencyCode))
            {
                _logger.WriteWarning(new LogMessage(string.Format("ValidateMetadata -> entityId {0} - Workflow Metadata CurrencyCode {1} empty",
                     transporterModel.WorkflowMetadata.MetadataId, transporterModel.WorkflowMetadata.CurrencyCode)), LogCategories);
                return false;
            }
            if (Convert.ToBoolean(transporterModel.WorkflowMetadata.IsItemSpecialCategory) &&
                (string.IsNullOrEmpty(transporterModel.WorkflowMetadata.SpecialCategoryCode) || string.IsNullOrEmpty(transporterModel.WorkflowMetadata.SpecialCategoryDescription)))
            {
                _logger.WriteWarning(new LogMessage(string.Format("ValidateMetadata -> entityId {0} - Workflow Metadata IsItemSpecialCategory {1}-{2} empty",
                     transporterModel.WorkflowMetadata.MetadataId, transporterModel.WorkflowMetadata.SpecialCategoryCode, transporterModel.WorkflowMetadata.SpecialCategoryDescription)),
                     LogCategories);
                return false;
            }

            if (Convert.ToBoolean(transporterModel.WorkflowMetadata.IsItemSpecialCategory) &&
                !_specialCategories.Any(f => f.SpecialCategoryCode.ToString() == transporterModel.WorkflowMetadata.SpecialCategoryCode &&
                    f.Country == _moduleConfiguration.Country && f.CompanyCode == transporterModel.WorkflowMetadata.CompanyCode))
            {
                _logger.WriteWarning(new LogMessage(string.Format("ValidateMetadata -> entityId {0} - Workflow Metadata IsItemSpecialCategory {1} is not configured to CompanyCode {2}",
                     transporterModel.WorkflowMetadata.MetadataId, transporterModel.WorkflowMetadata.SpecialCategoryCode, transporterModel.WorkflowMetadata.CompanyCode)),
                     LogCategories);
                return false;
            }

            if (!_procurementRight.Any(f => f.Country == _moduleConfiguration.Country && f.CompanyCode == transporterModel.WorkflowMetadata.CompanyCode))
            {
                _logger.WriteWarning(new LogMessage(string.Format("ValidateMetadata -> entityId {0} - Workflow Metadata Procurement Right is not configured to CompanyCode {1}",
                     transporterModel.WorkflowMetadata.MetadataId, transporterModel.WorkflowMetadata.CompanyCode)),
                     LogCategories);
                return false;
            }

            try
            {
                transporterModel.DomainUser = _webApiClient.GetUserAsync(transporterModel.WorkflowMetadata.CreationUser).Result;
                if (string.IsNullOrEmpty(transporterModel.DomainUser.EmailAddress))
                {
                    _logger.WriteWarning(new LogMessage(string.Format("ValidateMetadata -> entityId {0} - Email Address {1} not found",
                        transporterModel.WorkflowMetadata.MetadataId, transporterModel.DomainUser.DistinguishedName)), LogCategories);
                    return false;
                }
                _logger.WriteDebug(new LogMessage(string.Format("ValidateMetadata -> entityId {0} - Email Address {1} valid",
                        transporterModel.WorkflowMetadata.MetadataId, transporterModel.DomainUser.EmailAddress)), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("ValidateMetadata -> error FindUserAsync"), ex, LogCategories);
                return false;
            }

            try
            {
                string repositoryName = GetRepositoryName(transporterModel.WorkflowMetadata);
                transporterModel.UDSRepository = _udsService.GetRepository(repositoryName);
                if (transporterModel.UDSRepository == null)
                {
                    _logger.WriteWarning(new LogMessage(string.Format("ValidateMetadata ->  entityId {0} - Archive Repository {1} not found",
                        transporterModel.WorkflowMetadata.MetadataId, repositoryName)), LogCategories);
                    UpdateMetadataStatus(WorkflowStatusType.UnexpectedError, transporterModel.WorkflowMetadata.MetadataId);
                    needChangeMetadataStatus = false;
                    return false;
                }
                _logger.WriteDebug(new LogMessage(string.Format("ValidateMetadata -> entityId {0} - UDSRepository {1} valid",
                            transporterModel.WorkflowMetadata.MetadataId, transporterModel.UDSRepository.Name)), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("ValidateMetadata -> error FindUserAsync"), ex, LogCategories);
                return false;
            }

            try
            {
                ICollection<WorkflowRoleMapping> mappings = _webApiClient.GetWorkflowRoleMappingAsync(string.Concat("$filter=MappingTag eq '",
                    transporterModel.WorkflowMetadata.GetMappingTag(), "'&$expand=WorkflowRepository")).Result;
                if (!mappings.Any())
                {
                    _logger.WriteWarning(new LogMessage(string.Format("ValidateMetadata -> entityId {0} - Mapping Tag {1} not found",
                        transporterModel.WorkflowMetadata.MetadataId, transporterModel.WorkflowMetadata.GetMappingTag())), LogCategories);
                    return false;
                }

                IEnumerable<string> workflowNames = mappings
                .Where(f => f.WorkflowRepository.Status == WorkflowStatus.Todo)
                .Select(f => f.WorkflowRepository.Name);
                foreach (string workflowName in _moduleConfiguration.ValidWorkflows)
                {
                    if (!workflowNames.Any(m => m.Equals(workflowName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        _logger.WriteWarning(new LogMessage(string.Format("ValidateMetadata -> entityId {0} - Mapping Tag {1} not defined in Workflow {2}",
                            transporterModel.WorkflowMetadata.MetadataId, transporterModel.WorkflowMetadata.GetMappingTag(), workflowName)), LogCategories);
                        return false;
                    }
                }

                _logger.WriteDebug(new LogMessage(string.Format("ValidateMetadata -> entityId {0} - Mapping Tag  {1} is valid",
                       transporterModel.WorkflowMetadata.MetadataId, transporterModel.WorkflowMetadata.GetMappingTag())), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("ValidateMetadata -> error WorkflowRoleMapping"), ex, LogCategories);
                return false;
            }

            if (transporterModel.WorkflowMetadata.WorkflowAttachments == null || transporterModel.WorkflowMetadata.WorkflowAttachments.Count == 0)
            {
                _logger.WriteWarning(new LogMessage(string.Format("ValidateMetadata -> entityId {0} - attachments empty",
                    transporterModel.WorkflowMetadata.MetadataId)), LogCategories);
                return false;
            }

            if (!transporterModel.WorkflowMetadata.WorkflowAttachments.Any(f => Convert.ToBoolean(f.IsMainDocument)))
            {
                _logger.WriteWarning(new LogMessage(string.Format("ValidateMetadata -> entityId {0} - main document not defined",
                    transporterModel.WorkflowMetadata.MetadataId)), LogCategories);
                return false;
            }
            _logger.WriteDebug(new LogMessage(string.Format("ValidateMetadata -> entityId {0} - main document is defined",
                    transporterModel.WorkflowMetadata.MetadataId)), LogCategories);

            foreach (WorkflowAttachment attachment in transporterModel.WorkflowMetadata.WorkflowAttachments.Where(f => !Convert.ToBoolean(f.IsMainDocument)))
            {
                string filePath = Path.Combine(_moduleConfiguration.AttachmentsPath, attachment.InternalFileName);
                if (!File.Exists(filePath))
                {
                    _logger.WriteWarning(new LogMessage(string.Format("ValidateMetadata -> {0} not found", filePath)), LogCategories);
                    UpdateMetadataStatus(WorkflowStatusType.FileNotFound, transporterModel.WorkflowMetadata.MetadataId);
                    needChangeMetadataStatus = false;
                    return false;
                }
            }
            if (transporterModel.WorkflowMetadata.WorkflowAttachments.Any(f => !Convert.ToBoolean(f.IsMainDocument)))
            {
                _logger.WriteDebug(new LogMessage(string.Format("ValidateMetadata -> entityId {0} - attachments are defined",
                  transporterModel.WorkflowMetadata.MetadataId)), LogCategories);
            }

            return true;
        }

        private TransporterModel PopulateSpecialCategoryResponsabile(TransporterModel transporterModel)
        {
            if (Convert.ToBoolean(transporterModel.WorkflowMetadata.IsItemSpecialCategory))
            {
                transporterModel.SpecialCategory = _specialCategories.Single(f => f.SpecialCategoryCode.ToString() == transporterModel.WorkflowMetadata.SpecialCategoryCode &&
                    f.Country == _moduleConfiguration.Country && f.CompanyCode == transporterModel.WorkflowMetadata.CompanyCode);
            }
            return transporterModel;
        }

        private TransporterModel PopulateProcurementRight(TransporterModel transporterModel)
        {
            transporterModel.ProcurementRight = _procurementRight.Single(f => f.Country == _moduleConfiguration.Country && f.CompanyCode == transporterModel.WorkflowMetadata.CompanyCode);
            return transporterModel;
        }

        private void CreateUDS(TransporterModel transporterModel)
        {
            if (Cancel)
            {
                return;
            }

            try
            {
                transporterModel.UDS = _udsService.GetUDSByNumber(transporterModel.UDSRepository, transporterModel.WorkflowMetadata.DocumentNumber);
                if (transporterModel.UDS != null)
                {
                    try
                    {
                        _logger.WriteInfo(new LogMessage(string.Format("CreateUDS -> found UDS with Id {0}.", transporterModel.UDS.UDSId)), LogCategories);

                        if (string.IsNullOrEmpty(transporterModel.WorkflowMetadata.ReferenceDocNumber))
                        {
                            transporterModel.Fascicle = _fascicleService.GetByUDSIdAsync(transporterModel.UDS.UDSId).Result;
                            if (transporterModel.Fascicle == null)
                            {
                                _logger.WriteInfo(new LogMessage(string.Format("CreateUDS -> create new fascicle for UDSId {0}", transporterModel.UDS.UDSId)), LogCategories);
                                transporterModel.Fascicle = CreateNewFascicleAsync(transporterModel).Result;
                                LinkFascicle(transporterModel.UDS, transporterModel.Fascicle);
                            }
                            else
                            {
                                _logger.WriteInfo(new LogMessage(string.Format("CreateUDS -> fascicle already exist with Id {0}", transporterModel.Fascicle.UniqueId)), LogCategories);
                            }
                        }
                        else
                        {
                            //TODO: RIVEDER E PULIRE CON FACADE COMUNE
                            UDSBaseEntity referencedUDS = _udsService.GetUDSByNumber(new UDSRepository(Guid.NewGuid()) { Name = "RDA" }, transporterModel.WorkflowMetadata.ReferenceDocNumber);
                            transporterModel.Fascicle = _fascicleService.GetByUDSIdAsync(referencedUDS.UDSId).Result;
                            if (transporterModel.Fascicle != null)
                            {
                                LinkFascicle(transporterModel.UDS, transporterModel.Fascicle);
                            }
                            else
                            {
                                _logger.WriteWarning(new LogMessage(string.Format("CreateUDS -> fascicle not defined for UDSId {0}", referencedUDS.UDSId)), LogCategories);
                            }
                        }

                        transporterModel = DetectIsWorkflowAutoapproval(transporterModel);
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteError(new LogMessage("CreateUDS -> error start workflow"), ex, LogCategories);
                        UpdateMetadataStatus(WorkflowStatusType.UnexpectedError, transporterModel.WorkflowMetadata.MetadataId);
                        throw;
                    }

                    transporterModel = CreateCollaborationModel(transporterModel).Result;
                    transporterModel = PopulateSpecialCategoryResponsabile(transporterModel);
                    transporterModel = PopulateProcurementRight(transporterModel);

                    StartWorkflow(transporterModel, _moduleConfiguration.WorkflowStarterName);
                    return;
                }

                _logger.WriteInfo(new LogMessage(string.Format("CreateUDS -> create UDSModel for Id {0}", transporterModel.WorkflowMetadata.MetadataId)), LogCategories);
                Contact contact = CreateContact(transporterModel);
                UDSModel model = _udsService.FillUDSModel(transporterModel.WorkflowMetadata, transporterModel.UDSRepository, contact);
                Guid commandId = _udsService.SendCommandInsertData(transporterModel.UDSRepository.UniqueId, model, transporterModel.UDSRepository);
                _logger.WriteInfo(new LogMessage(string.Format("CreateUDS -> command sended with Id {0}", commandId)), LogCategories);
                if (!_pendingCommands.TryAdd(transporterModel.WorkflowMetadata.MetadataId, commandId))
                {
                    _logger.WriteError(new LogMessage(string.Concat("CreateUDS -> pendingCommands ", transporterModel.WorkflowMetadata.MetadataId, " not added")), null, LogCategories);
                };
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Format("CreateUDS -> error command for Id {0}", transporterModel.WorkflowMetadata.MetadataId)), ex, LogCategories);
                UpdateMetadataStatus(WorkflowStatusType.UnexpectedError, transporterModel.WorkflowMetadata.MetadataId);
                throw;
            }
        }

        private Contact CreateContact(TransporterModel transporterModel)
        {
            Contact contact = _udsService.GetContact(transporterModel.WorkflowMetadata.SourceFiscalCode);
            if (contact == null)
            {
                contact = new Contact()
                {
                    SearchCode = transporterModel.WorkflowMetadata.SourceNumber,
                    Description = transporterModel.WorkflowMetadata.SourceName,
                    EmailAddress = transporterModel.WorkflowMetadata.SourceEmail,
                    CertifiedMail = transporterModel.WorkflowMetadata.SourceCertifiedEmail,
                    FiscalCode = transporterModel.WorkflowMetadata.SourceFiscalCode,
                    IncrementalFather = _moduleConfiguration.Contact_RootId,
                    IdContactType = "A",
                    Note = string.Concat(transporterModel.WorkflowMetadata.SourceLegalForm, " - ", transporterModel.WorkflowMetadata.SourceVatRegNumber),
                    IsActive = 1,
                    IsLocked = 0,
                    IsChanged = 0
                };
                contact.EntityId = _udsService.GetLastContactId(transporterModel.WorkflowMetadata.SourceFiscalCode) + 1;
                contact.FullIncrementalPath = string.Concat(_moduleConfiguration.Contact_RootId, "|", contact.EntityId);
                contact = _udsService.CreateContact(contact);
            }
            else
            {
                contact.SearchCode = transporterModel.WorkflowMetadata.SourceNumber;
                contact.Description = transporterModel.WorkflowMetadata.SourceName;
                contact.EmailAddress = transporterModel.WorkflowMetadata.SourceEmail;
                contact.CertifiedMail = transporterModel.WorkflowMetadata.SourceCertifiedEmail;
                contact.FiscalCode = transporterModel.WorkflowMetadata.SourceFiscalCode;
                contact.Note = string.Concat(transporterModel.WorkflowMetadata.SourceLegalForm, " - ", transporterModel.WorkflowMetadata.SourceVatRegNumber);
                contact = _udsService.UpdateContact(contact);
            }

            return contact;
        }

        private TransporterModel DetectIsWorkflowAutoapproval(TransporterModel transporterModel)
        {
            _logger.WriteDebug(new LogMessage(string.Format("DetectIsWorkflowAutoapproval -> Finding IsWorkflowAutoapproval ReferenceDocNumber {0}",
                transporterModel.WorkflowMetadata.ReferenceDocNumber)), LogCategories);

            if (transporterModel.WorkflowMetadata.ReferenceDocType == DocumentType.RDA && !string.IsNullOrEmpty(transporterModel.WorkflowMetadata.ReferenceDocNumber))
            {
                using (WorkflowMetadataRepository workflowMetadataRepository = new WorkflowMetadataRepository(_logger))
                {
                    WorkflowMetadata referencedWorkflowMetadata = workflowMetadataRepository.GetReferenceDocType(transporterModel.WorkflowMetadata.ReferenceDocNumber);
                    if (referencedWorkflowMetadata != null)
                    {
                        _logger.WriteInfo(new LogMessage(string.Format("DetectIsWorkflowAutoapproval -> Found {0} DocumentNumber {1} IsWorkflowAutoapproval {2}",
                           referencedWorkflowMetadata.MetadataId, referencedWorkflowMetadata.DocumentNumber, referencedWorkflowMetadata.IsWorkflowAutoapproval)),
                           LogCategories);

                        transporterModel.WorkflowMetadata.IsWorkflowAutoapproval = referencedWorkflowMetadata.IsWorkflowAutoapproval;
                    }
                }

            }
            return transporterModel;
        }

        private void StartWorkflow(TransporterModel transporterModel, string workflowName)
        {
            try
            {
                _logger.WriteInfo(new LogMessage(string.Format("StartWorkflow -> starting workflow {0}, UDSId {1} - UDSRepositoryId {2}",
                    workflowName, transporterModel.UDS.UDSId, transporterModel.UDSRepository.UniqueId)), LogCategories);

                Guid instanceId = _workflowService.StartWorkflow(transporterModel, workflowName);
                using (WorkflowMetadataRepository workflowMetadataRepository = new WorkflowMetadataRepository(_logger))
                {
                    try
                    {
                        _logger.WriteInfo(new LogMessage("StartWorkflow -> complete metadata workflow"), LogCategories);
                        transporterModel.WorkflowMetadata.WorkflowStatus = WorkflowStatusType.WorkflowStarted;
                        transporterModel.WorkflowMetadata.WorkflowInstanceID = instanceId.ToString();
                        transporterModel.WorkflowMetadata.WorkflowUpdateDate = DateTime.UtcNow;
                        workflowMetadataRepository.Update(transporterModel.WorkflowMetadata);
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteError(new LogMessage("SetMetadataStatus -> error update status"), ex, LogCategories);
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("StartWorkflow -> error start workflow"), ex, LogCategories);
                UpdateMetadataStatus(WorkflowStatusType.UnexpectedError, transporterModel.WorkflowMetadata.MetadataId);
                throw;
            }
        }

        private void UpdateMetadataStatus(WorkflowStatusType status, int metadataId)
        {
            using (WorkflowMetadataRepository workflowMetadataRepository = new WorkflowMetadataRepository(_logger))
            {
                try
                {
                    _logger.WriteInfo(new LogMessage(string.Format("SetMetadataStatus -> setting status, id: {0} - status: {1}", metadataId, status.ToString())), LogCategories);

                    WorkflowMetadata entity = workflowMetadataRepository.GetById(metadataId);
                    entity.WorkflowStatus = status;
                    entity.WorkflowUpdateDate = DateTime.UtcNow;
                    workflowMetadataRepository.Update(entity);
                }
                catch (Exception ex)
                {
                    _logger.WriteError(new LogMessage("SetMetadataStatus -> error update status"), ex, LogCategories);
                    throw;
                }
            }
        }

        private void UpdateMedatadaDocSuiteURL(WorkflowMetadata entity, Fascicle fascicle)
        {
            using (WorkflowMetadataRepository workflowMetadataRepository = new WorkflowMetadataRepository(_logger))
            {
                try
                {
                    _logger.WriteInfo(new LogMessage(string.Format("SetFascicleLink -> setting DocSuiteURL, id: {0} - fascicle id: {1}", entity.MetadataId, fascicle.UniqueId)), LogCategories);
                    string url = new Uri(new Uri(_moduleConfiguration.ExternalViewerBaseUrl), string.Format(EXTERNAL_VIEWER_FASCICLE_LINK, fascicle.UniqueId)).ToString();
                    entity.DocSuiteURL = url;
                    entity.WorkflowUpdateDate = DateTime.UtcNow;
                    workflowMetadataRepository.Update(entity);
                }
                catch (Exception ex)
                {
                    _logger.WriteError(new LogMessage("SetFascicleLink -> error update DocSuiteURL"), ex, LogCategories);
                    throw;
                }
            }
        }

        private string GetRepositoryName(WorkflowMetadata entity)
        {
            string result = string.Empty;
            switch (entity.DocumentType)
            {
                case DocumentType.FatturaAttiva:
                    result = _moduleConfiguration.ArchiveName_CustomerInvoice;
                    break;
                case DocumentType.FatturaPassiva:
                    result = _moduleConfiguration.ArchiveName_VendorInvoice;
                    break;
                case DocumentType.NotaAccreditoAttiva:
                    result = _moduleConfiguration.ArchiveName_CustomerCreditMemo;
                    break;
                case DocumentType.NotaAccreditoPassiva:
                    result = _moduleConfiguration.ArchiveName_VendorCreditMemo;
                    break;
                case DocumentType.ODA:
                    result = _moduleConfiguration.ArchiveName_ODA;
                    break;
                case DocumentType.RDA:
                    result = _moduleConfiguration.ArchiveName_RDA;
                    break;
            }
            return result;
        }

        private async Task<DocSuiteWeb.Entity.Commons.Category> GetCategoryAsync(short idCategory)
        {
            return (await _webApiClient.GetCategoryAsync(string.Format("$filter=EntityShortId eq {0}&$expand=CategoryFascicles($expand=Manager)", idCategory))).SingleOrDefault();
        }

        private async Task<Fascicle> CreateNewFascicleAsync(TransporterModel transporterModel)
        {
            DocSuiteWeb.Entity.Commons.Category category = await GetCategoryAsync(transporterModel.UDS.IdCategory);
            CategoryFascicle categoryFascicle = category.CategoryFascicles.FirstOrDefault(f => f.DSWEnvironment == transporterModel.UDSRepository.DSWEnvironment);
            Contact manager = null;
            if (categoryFascicle != null)
            {
                manager = categoryFascicle.Manager;
            }
            _logger.WriteInfo(new LogMessage("CreateNewFascicle -> create new fascicle"), LogCategories);
            Fascicle fascicle = _fascicleService.CreateFascicle(category, manager, transporterModel.WorkflowMetadata);
            return fascicle;
        }

        private void LinkFascicle(UDSBaseEntity udsEntity, Fascicle fascicle)
        {
            _logger.WriteInfo(new LogMessage(string.Format("LinkFascicle -> link UDS {0} to Fascicle {1}", udsEntity.UDSId, fascicle.UniqueId)), LogCategories);
            _fascicleService.CreateFascicleDocumentUnit(udsEntity.UDSId, fascicle);
        }
        #endregion
    }
}
