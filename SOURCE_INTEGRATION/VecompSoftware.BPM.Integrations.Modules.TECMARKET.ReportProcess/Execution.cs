using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.TECMARKET.ReportProcess.Configuration;
using VecompSoftware.BPM.Integrations.Modules.TECMARKET.ReportProcess.Models;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.StampaConforme;
using VecompSoftware.BPM.Integrations.Services.StampaConforme.Service;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.Core.Command.CQRS.Events.Models.Workflows;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.Helpers.UDS;
using VecompSoftware.Helpers.Workflow;
using DSWEnvironmentType = VecompSoftware.DocSuiteWeb.Model.Entities.Commons.DSWEnvironmentType;

namespace VecompSoftware.BPM.Integrations.Modules.TECMARKET.ReportProcess
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]

        private bool _needInitializeModule = false;
        private UDSRepository _udsRepository;
        private Location _workflowLocation;

        private readonly ILogger _logger;
        private readonly IWebAPIClient _webAPIClient;
        private readonly IStampaConformeClient _stampaConformeClient;
        private readonly IDocumentClient _documentClient;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly DirectoryInfo _workflowEventsPath;
        private readonly DirectoryInfo _backuPath;
        private readonly Dictionary<EventAttributeName, string> _eventDataType;

        public const string UDSMetadata_DataInizioControlli = "DataInizioControlli";
        public const string UDSMetadata_DataFineControlli = "DataFineControlli";
        public const string UDSMetadata_PresenzaSegnalazioniDaAnalizzare = "PresenzaSegnalazioniDaAnalizzare";
        public const string UDSMetadata_DataRisoluzione = "DataRisoluzione";
        public const string UDSMetadata_Stato = "Stato";
        public const string UDSMetadata_Stato_Value_Good = "Nessuna segnalazione critica";
        public const string UDSMetadata_Stato_Value_Error = "Segnalazione critica da Analizzare";

        private const string CHECK_START_DATE = "DataInizioControlli";
        private const string CHECK_END_DATE = "DataFineControlli";
        private const string GENERAL_OUTCOME = "EsitoGenerale";
        private const string CRITICAL_REPORT = "SONO PRESENTI SEGNALAZIONI CRITICHE";
        private const string NON_CRITICAL_REPORT = "NON SONO PRESENTI SEGNALAZIONI CRITICHE";
        private const string DATE_ROW = "DATARow";
        private const string SERVER_NAME = "NOME DEL SERVER";
        private const string RAM_CHECK = "CONTROLLO RAM";
        private const string CPU_CHECK = "CONTROLLO CPU";
        private const string DISK_CHECK = "CONTROLLO DISCO";
        private const string WINDOWS_EVENTS = "EVENTI DI WINDOWS";
        private const string OK_CHECK_RESULT = "CONTROLLO POSITIVO";
        private const string ERROR_CHECK_RESULT = "CONTROLLO IN ERRORE";
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
        public Execution(ILogger logger, IStampaConformeClient stampaConformeClient, IWebAPIClient webAPIClient, IDocumentClient documentClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            _logger = logger;
            _webAPIClient = webAPIClient;
            _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
            _documentClient = documentClient;
            _stampaConformeClient = stampaConformeClient;
            _workflowEventsPath = new DirectoryInfo(_moduleConfiguration.WorkflowEventsPath);
            _backuPath = new DirectoryInfo(_moduleConfiguration.BackupPath);
            if (!_workflowEventsPath.Exists)
            {
                throw new ArgumentException($"Drop directory {_moduleConfiguration.WorkflowEventsPath} not exists");
            }
            _eventDataType = new Dictionary<EventAttributeName, string>
                {
                    { EventAttributeName.Processor,"Processor"},
                    { EventAttributeName.Memory,"Memory"},
                    { EventAttributeName.PhysicalDisk,"PhysicalDisk"}
                };
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
                ProcessEventsAsync().Wait();
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("TECMARKET.ReportProcess -> Execute critical error"), ex, LogCategories);
                throw;
            }
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);

                int? workflowLocationId = _webAPIClient.GetParameterWorkflowLocationIdAsync().Result;
                if (!workflowLocationId.HasValue)
                {
                    throw new ArgumentNullException("Parameter WorkflowLocationId is not defined");
                }
                _workflowLocation = _webAPIClient.GetLocationAsync(workflowLocationId.Value).Result.Single();

                _udsRepository = _webAPIClient.GetUDSRepository(_moduleConfiguration.UDSName).Result.Last(f => f.Status == DocSuiteWeb.Entity.UDS.UDSRepositoryStatus.Confirmed);
                if (_udsRepository == null)
                {
                    throw new ArgumentException($"UDSRepository {_moduleConfiguration.UDSName} not found");
                }

                _needInitializeModule = false;
            }
        }

        private async Task ProcessEventsAsync()
        {
            _logger.WriteInfo(new LogMessage("Processing ..."), LogCategories);
            List<EventWorkflowStartRequest> events = new List<EventWorkflowStartRequest>();

            WorkflowReferenceModel workflowReferenceModelUDS = new WorkflowReferenceModel();
            WorkflowArgument referenceModelWorkflowArgument = new WorkflowArgument();
            WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel();
            EventModel eventModel;
            FileInfo[] files;
            string backupPath;
            string sessionId;
            try
            {
                string workflowName = string.Empty;
                byte[] templatePDF = await GetPDFTemplateAsync();
                Guid correlationId;
                EventWorkflowStartRequest evt;
                byte[] pdfContent = null;
                byte[] excelContent = null;
                WorkflowResult workflowResult;
                IDictionary<string, object> metadatas;
                List<EventModel> eventModels = new List<EventModel>();
                foreach (DirectoryInfo directoryPath in _workflowEventsPath.GetDirectories())
                {
                    _logger.WriteDebug(new LogMessage($"Looking {directoryPath.FullName} ..."), LogCategories);
                    files = directoryPath.GetFiles();
                    if (!files.Any())
                    {
                        _logger.WriteDebug(new LogMessage($"There are no events in {directoryPath.FullName} to be processed. Directory are going to be deleting..."), LogCategories);
                        directoryPath.Delete();
                        _logger.WriteDebug(new LogMessage($"Directory {directoryPath.FullName} deleted"), LogCategories);
                        continue;
                    }

                    eventModels.Clear();
                    metadatas = new Dictionary<string, object>();
                    foreach (FileInfo fileInfo in files)
                    {
                        _logger.WriteDebug(new LogMessage($"reading {fileInfo.FullName} ..."), LogCategories);
                        evt = JsonConvert.DeserializeObject<EventWorkflowStartRequest>(File.ReadAllText(fileInfo.FullName), ModuleConfigurationHelper.JsonSerializerSettings);
                        events.Add(evt);
                        referenceModelWorkflowArgument = evt.ContentType.ContentTypeValue.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL];
                        workflowReferenceModel = JsonConvert.DeserializeObject<WorkflowReferenceModel>(referenceModelWorkflowArgument.ValueString, ModuleConfigurationHelper.JsonSerializerSettings);
                        eventModel = JsonConvert.DeserializeObject<EventModel>(workflowReferenceModel.ReferenceModel);
                        eventModels.Add(eventModel);
                    }

                    correlationId = Guid.NewGuid();
                    _logger.WriteDebug(new LogMessage("Generating PDF document"), LogCategories);
                    pdfContent = await BuildPDF(eventModels, templatePDF, metadatas);
                    _logger.WriteDebug(new LogMessage("PDF document successfully generated"), LogCategories);

                    excelContent = ExcelHelper.BuildExcel(eventModels, "Teckmarket Report");
                    _logger.WriteDebug(new LogMessage("Excel document successfully generated"), LogCategories);

                    _logger.WriteDebug(new LogMessage("Building document model"), LogCategories);
                    workflowName = _moduleConfiguration.WorkflowRepositoryNormalName;
                    if (eventModels.Any(x => x.EventLogs.Any(y => y.LogType == nameof(EventLogEntryType.Error))))
                    {
                        workflowName = _moduleConfiguration.WorkflowRepositoryErrorName;
                    }
                    workflowReferenceModelUDS = await CreateUDSBuildModelAsync(correlationId, workflowName, pdfContent, excelContent, metadatas);

                    _logger.WriteDebug(new LogMessage($"Workflow {workflowName} initialize model correctly"), LogCategories);
                    _logger.WriteDebug(new LogMessage($"Preparing starting workflow with correlationId {correlationId}"), LogCategories);
                    workflowResult = await StartWorkflowAsync(workflowReferenceModelUDS, workflowName);
                    if (!workflowResult.IsValid || !workflowResult.InstanceId.HasValue)
                    {
                        _logger.WriteError(new LogMessage("An error occured in StartWorkflowAsync"), LogCategories);
                        throw new Exception(string.Join(", ", workflowResult.Errors));
                    }
                    sessionId = string.Format("{0:yyyyMMdd_HHmmss}", DateTime.Now);
                    backupPath = string.Empty;
                    if (_backuPath.Exists)
                    {
                        backupPath = Directory.CreateDirectory(Path.Combine(_backuPath.FullName, sessionId)).FullName;
                    }
                    foreach (FileInfo fileInfo in directoryPath.GetFiles())
                    {
                        if (!_backuPath.Exists)
                        {
                            fileInfo.Delete();
                            _logger.WriteDebug(new LogMessage($"File {fileInfo.FullName} deleted"), LogCategories);
                        }
                        else
                        {

                            backupPath = Path.Combine(backupPath, fileInfo.Name);
                            File.Move(fileInfo.FullName, backupPath);
                            _logger.WriteDebug(new LogMessage($"File {fileInfo.FullName} has moved to {backupPath}"), LogCategories);
                        }
                    }
                    directoryPath.Delete();
                    _logger.WriteDebug(new LogMessage($"Directory {directoryPath.FullName} deleted"), LogCategories);
                }
                _logger.WriteInfo(new LogMessage($"File {_moduleConfiguration.WorkflowEventsPath} contains {events.Count} events"), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("ProcessEventsAsync -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        private async Task<byte[]> GetPDFTemplateAsync()
        {
            TemplateDocumentRepository templateDocumentRepository = await _webAPIClient.GetTemplateDocumentRepositoryAsync(_moduleConfiguration.IdTemplateDocumentRepository);
            List<Services.BiblosDS.DocumentService.Document> documents = await _documentClient.GetDocumentChildrenAsync(templateDocumentRepository.IdArchiveChain);

            byte[] template = await _documentClient.GetDocumentStreamAsync(documents.FirstOrDefault().IdDocument);

            return template;
        }

        private async Task<byte[]> BuildPDF(List<EventModel> eventModels, byte[] templatePDF, IDictionary<string, object> metadatas)
        {
            List<BuildValueModel> buildValueModels = new List<BuildValueModel>();

            bool anyServerWithError = eventModels.Any(y => y.EventLogs.Any(x => x.LogType == nameof(EventLogEntryType.Error)));

            metadatas.Add(UDSMetadata_DataInizioControlli, eventModels.Max(x => Convert.ToDateTime(x.EventDate)));
            buildValueModels.Add(new BuildValueModel()
            {
                Name = CHECK_START_DATE,
                Value = eventModels.Min(x => Convert.ToDateTime(x.EventDate)).ToShortDateString()
            });
            metadatas.Add(UDSMetadata_DataFineControlli, eventModels.Max(x => Convert.ToDateTime(x.EventDate)));
            buildValueModels.Add(new BuildValueModel()
            {
                Name = CHECK_END_DATE,
                Value = eventModels.Max(x => Convert.ToDateTime(x.EventDate)).ToShortDateString()
            });
            metadatas.Add(UDSMetadata_Stato, anyServerWithError ? UDSMetadata_Stato_Value_Error : UDSMetadata_Stato_Value_Good);
            metadatas.Add(UDSMetadata_PresenzaSegnalazioniDaAnalizzare, anyServerWithError);

            buildValueModels.Add(new BuildValueModel()
            {
                Name = GENERAL_OUTCOME,
                Value = anyServerWithError ? CRITICAL_REPORT : NON_CRITICAL_REPORT
            });
            for (int i = 1; i <= eventModels.Count; i++)
            {
                PopulatePDFColumns(buildValueModels, eventModels[i - 1], i, i == 1);
            }
            return await _stampaConformeClient.BuildPDFAsync(templatePDF, buildValueModels.ToArray(), string.Empty);
        }

        private void PopulatePDFColumns(List<BuildValueModel> buildValueModels, EventModel eventModel, int currentDirIndex, bool IsFirstTable)
        {
            EventLogModel ram = eventModel.EventLogs.Where(x => !string.IsNullOrEmpty(x.SourceDeviceName) && x.SourceDeviceName.Equals(_eventDataType[EventAttributeName.Memory])).FirstOrDefault();
            EventLogModel proc = eventModel.EventLogs.Where(x => !string.IsNullOrEmpty(x.SourceDeviceName) && x.SourceDeviceName.Equals(_eventDataType[EventAttributeName.Processor])).FirstOrDefault();
            EventLogModel disk = eventModel.EventLogs.Where(x => !string.IsNullOrEmpty(x.SourceDeviceName) && x.SourceDeviceName.Equals(_eventDataType[EventAttributeName.PhysicalDisk])).FirstOrDefault();

            for (int tableIndex = 1; tableIndex < 5; tableIndex++)
            {
                buildValueModels.Add(new BuildValueModel()
                {
                    Name = IsFirstTable ? $"{DATE_ROW}{tableIndex}" : $"{DATE_ROW}{tableIndex}_{currentDirIndex}",
                    Value = Convert.ToDateTime(eventModel.EventDate).ToShortDateString()
                });
            }
            buildValueModels.Add(new BuildValueModel()
            {
                Name = IsFirstTable ? SERVER_NAME : $"{SERVER_NAME}_{currentDirIndex}",
                Value = $"{eventModel.ServerHost} - {eventModel.ServerIP}"
            });

            buildValueModels.Add(new BuildValueModel()
            {
                Name = IsFirstTable ? RAM_CHECK : $"{RAM_CHECK}_{currentDirIndex}",
                Value = ram != null ? ERROR_CHECK_RESULT : OK_CHECK_RESULT
            });

            buildValueModels.Add(new BuildValueModel()
            {
                Name = IsFirstTable ? CPU_CHECK : $"{CPU_CHECK}_{currentDirIndex}",
                Value = proc != null ? ERROR_CHECK_RESULT : OK_CHECK_RESULT
            });

            buildValueModels.Add(new BuildValueModel()
            {
                Name = IsFirstTable ? DISK_CHECK : $"{DISK_CHECK}_{currentDirIndex}",
                Value = disk != null ? ERROR_CHECK_RESULT : OK_CHECK_RESULT
            });

            buildValueModels.Add(new BuildValueModel()
            {
                Name = IsFirstTable ? WINDOWS_EVENTS : $"{WINDOWS_EVENTS}_{currentDirIndex}",
                Value = eventModel.EventLogs.Any(y => y.LogType == nameof(EventLogEntryType.Error)) ? ERROR_CHECK_RESULT : OK_CHECK_RESULT
            });
        }

        private async Task<DocumentInstance[]> FillDocumentInstancesAsync(List<FileModel> files)
        {
            if (files.Count == 0)
            {
                return new DocumentInstance[] { };
            }

            IList<DocumentInstance> instances = new List<DocumentInstance>();
            ArchiveDocument archiveDocument;
            foreach (FileModel file in files)
            {
                archiveDocument = await _documentClient.InsertDocumentAsync(new ArchiveDocument()
                {
                    Archive = _workflowLocation.ProtocolArchive,
                    ContentStream = file.Content,
                    Name = file.Filename,
                });
                instances.Add(new DocumentInstance()
                {
                    IdDocumentToStore = archiveDocument.IdDocument.ToString(),
                    DocumentName = file.Filename
                });
            }
            return instances.ToArray();
        }

        private async Task<WorkflowReferenceModel> CreateUDSBuildModelAsync(Guid correlationId, string workflowName, byte[] pdfContent, byte[] excelContent, IDictionary<string, object> metadatas)
        {
            WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel
            {
                ReferenceId = correlationId
            };

            UDSModel model = UDSModel.LoadXml(_udsRepository.ModuleXML);

            model.Model.UDSId = Guid.NewGuid().ToString();
            model.Model.Subject.Value = $"{_moduleConfiguration.ReportSubject} del {DateTime.Today.ToShortDateString()}";

            IDictionary<string, object> uds_metadatas = MappingMetadatas(model.Model.Metadata, metadatas);

            model.FillMetaData(uds_metadatas);

            List<FileModel> fileModels = new List<FileModel>
            {
                new FileModel()
                {
                    Content = pdfContent,
                    Filename = _moduleConfiguration.ReportPDFFileName
                }
            };

            List<FileModel> attachment = new List<FileModel>
            {
                new FileModel()
                {
                    Content = excelContent,
                    Filename = _moduleConfiguration.ReportExcelFileName
                }
            };

            model.Model.Documents.Document.Instances = await FillDocumentInstancesAsync(fileModels);
            model.Model.Documents.DocumentAttachment.Instances = await FillDocumentInstancesAsync(attachment);

            UDSBuildModel udsBuildModel = new UDSBuildModel(model.SerializeToXml())
            {
                WorkflowName = workflowName,
                WorkflowActions = new List<IWorkflowAction>(),
                Documents = new List<UDSDocumentModel>(),
                Roles = new List<RoleModel>(),
                Users = new List<UserModel>(),
                UniqueId = correlationId,
                UDSRepository = new UDSRepositoryModel(_udsRepository.UniqueId)
                {
                    DSWEnvironment = _udsRepository.DSWEnvironment,
                    Name = _udsRepository.Name
                },
                Subject = model.Model.Subject.Value,
                RegistrationUser = WindowsIdentity.GetCurrent().Name
            };
            workflowReferenceModel.ReferenceType = DSWEnvironmentType.Build;
            workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(udsBuildModel, ModuleConfigurationHelper.JsonSerializerSettings);

            return workflowReferenceModel;
        }

        private async Task<WorkflowResult> StartWorkflowAsync(WorkflowReferenceModel workflowReferenceModel, string workflowName)
        {
            WorkflowStart workflowStart = new WorkflowStart
            {
                WorkflowName = workflowName
            };

            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, new WorkflowArgument()
            {
                Name = WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL,
                PropertyType = ArgumentType.Json,
                ValueString = JsonConvert.SerializeObject(workflowReferenceModel)
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

        public static IDictionary<string, object> MappingMetadatas(Section[] sections, IDictionary<string, object> metadatas)
        {
            IDictionary<string, object> uds_metadatas = new Dictionary<string, object>();
            uds_metadatas = SetUDSNullableMetadata(metadatas, UDSMetadata_DataFineControlli, uds_metadatas, UDSMetadata_DataFineControlli);
            uds_metadatas = SetUDSNullableMetadata(metadatas, UDSMetadata_DataInizioControlli, uds_metadatas, UDSMetadata_DataInizioControlli);
            uds_metadatas = SetUDSNullableMetadata(metadatas, UDSMetadata_DataRisoluzione, uds_metadatas, UDSMetadata_DataRisoluzione);
            uds_metadatas = SetUDSNullableMetadata(metadatas, UDSMetadata_PresenzaSegnalazioniDaAnalizzare, uds_metadatas, UDSMetadata_PresenzaSegnalazioniDaAnalizzare);
            uds_metadatas = SetUDSNullableMetadata(metadatas, UDSMetadata_Stato, uds_metadatas, UDSMetadata_Stato);
            return uds_metadatas;
        }

        public static IDictionary<string, object> SetUDSNullableMetadata(IDictionary<string, object> metadatas, string metadataName,
           IDictionary<string, object> uds_metadatas, string udsMetadataName)
        {
            object currentMetadata = null;
            if (metadatas.ContainsKey(metadataName))
            {
                currentMetadata = metadatas[metadataName];
            }
            if (!uds_metadatas.ContainsKey(udsMetadataName))
            {
                uds_metadatas.Add(udsMetadataName, currentMetadata);
            }
            uds_metadatas[udsMetadataName] = currentMetadata;
            return uds_metadatas;
        }

        protected override void OnStop()
        {
            _logger.WriteInfo(new LogMessage("OnStop -> TECMARKET.ReportProcess"), LogCategories);
        }

        #endregion
    }
}
