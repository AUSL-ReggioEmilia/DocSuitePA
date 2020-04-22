using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
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
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.Helpers.UDS;
using VecompSoftware.Helpers.Workflow;

namespace VecompSoftware.BPM.Integrations.Modules.TECMARKET.ReportProcess
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IWebAPIClient _webAPIClient;
        private readonly IStampaConformeClient _stampaConformeClient;
        private readonly IDocumentClient _documentClient;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly UDSRepository _udsRepository;
        private readonly DirectoryInfo _rootDirectory;

        public const string UDSMetadata_DataInizioControlli = "DataInizioControlli";
        public const string UDSMetadata_DataFineControlli = "DataFineControlli";
        public const string UDSMetadata_PresenzaSegnalazioniDaAnalizzare = "PresenzaSegnalazioniDaAnalizzare";
        public const string UDSMetadata_DataRisoluzione = "DataRisoluzione";
        public const string UDSMetadata_Stato = "Stato";
        public const string UDSMetadata_Stato_Value_Good = "Nessuna segnalazione critica";
        public const string UDSMetadata_Stato_Value_Error = "Segnalazione critica da Analizzare";
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
            _udsRepository = _webAPIClient.GetUDSRepository(_moduleConfiguration.UDSName).Result.Last(f => f.Status == DocSuiteWeb.Entity.UDS.UDSRepositoryStatus.Confirmed);
            if (_udsRepository == null)
            {
                throw new ArgumentException($"UDSRepository {_moduleConfiguration.UDSName} not found");
            }
            _rootDirectory = new DirectoryInfo(_moduleConfiguration.WorkflowEventsPath);
            if (!_rootDirectory.Exists)
            {
                throw new ArgumentException($"Drop directory {_moduleConfiguration.WorkflowEventsPath} not exists");
            }
        }

        #endregion

        #region [ Methods ]        

        private async Task ProcessEventsAsync()
        {
            _logger.WriteInfo(new LogMessage("Processing ..."), LogCategories);
            List<EventWorkflowStartRequest> events = new List<EventWorkflowStartRequest>();

            WorkflowReferenceModel workflowReferenceModelUDS = new WorkflowReferenceModel();
            WorkflowArgument referenceModelWorkflowArgument = new WorkflowArgument();
            WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel();
            EventModel eventModel;
            FileInfo[] files;
            try
            {
                string workflowName = string.Empty;
                byte[] templatePDF = await GetPDFTemplateAsync();
                Guid correlationId;
                EventWorkflowStartRequest evt;
                byte[] pdfContent;
                WorkflowResult workflowResult;
                IDictionary<string, object> metadatas;
                List<EventModel> eventModels = new List<EventModel>();
                foreach (DirectoryInfo directoryPath in _rootDirectory.GetDirectories())
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

                    _logger.WriteDebug(new LogMessage("Building document model"), LogCategories);
                    workflowName = _moduleConfiguration.WorkflowRepositoryNormalName;
                    if (eventModels.Any(x => x.EventLogs.Any(y => y.LogType == nameof(EventLogEntryType.Error))))
                    {
                        workflowName = _moduleConfiguration.WorkflowRepositoryErrorName;
                    }
                    workflowReferenceModelUDS = CreateUDSBuildModelAsync(correlationId, workflowName, pdfContent, metadatas);

                    _logger.WriteDebug(new LogMessage($"Workflow {workflowName} initialize model correctly"), LogCategories);
                    _logger.WriteDebug(new LogMessage($"Preparing starting workflow with correlationId {correlationId}"), LogCategories);
                    workflowResult = await StartWorkflowAsync(workflowReferenceModelUDS, workflowName);
                    if (!workflowResult.IsValid)
                    {
                        _logger.WriteError(new LogMessage("An error occured in StartWorkflowAsync"), LogCategories);
                        throw new Exception("VecompSoftware.BPM.Integrations.Modules.TECMARKET.ReportProcess");
                    }
                    else
                    {
                        foreach(var fileInfo in directoryPath.GetFiles())
                        {
                            fileInfo.Delete();
                            _logger.WriteDebug(new LogMessage($"File {fileInfo.FullName} deleted"), LogCategories);
                        }
                    }
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

            bool anyServerWithError= eventModels.Any(y => y.EventLogs.Any(x=>x.LogType == nameof(EventLogEntryType.Error)));

            metadatas.Add(UDSMetadata_DataInizioControlli, eventModels.Max(x => Convert.ToDateTime(x.EventDate)));
            buildValueModels.Add(new BuildValueModel()
            {
                Name = "Data inizio controlli",
                Value = eventModels.Min(x => Convert.ToDateTime(x.EventDate)).ToShortDateString()
            });
            metadatas.Add(UDSMetadata_DataFineControlli, eventModels.Max(x => Convert.ToDateTime(x.EventDate)));
            buildValueModels.Add(new BuildValueModel()
            {
                Name = "Data fine controlli",
                Value = eventModels.Max(x => Convert.ToDateTime(x.EventDate)).ToShortDateString()
            });
            metadatas.Add(UDSMetadata_Stato, anyServerWithError ? UDSMetadata_Stato_Value_Error : UDSMetadata_Stato_Value_Good);
            metadatas.Add(UDSMetadata_PresenzaSegnalazioniDaAnalizzare, anyServerWithError);

            buildValueModels.Add(new BuildValueModel()
            {
                Name = "Esito",
                Value = anyServerWithError
                ? "SONO PRESENTI SEGNALAZIONI CRITICHE"
                : "NON SONO PRESENTI SEGNALAZIONI CRITICHE"
            });
            for (int i = 1; i <= eventModels.Count; i++)
            {
                EventModel eventModel = eventModels[i - 1];

                var hasError = eventModel.EventLogs.Any(y => y.LogType == nameof(EventLogEntryType.Error));
                var hasWarning = eventModel.EventLogs.Any(y => y.LogType == nameof(EventLogEntryType.Warning));
                var logEventType = hasError ? "Errore" : hasWarning ? "Avviso" : "Successo";

                buildValueModels.Add(new BuildValueModel()
                {
                    Name = $"DATA INIZIORow{i}",
                    Value = Convert.ToDateTime(eventModel.EventDate).ToShortDateString()
                }); ;
                buildValueModels.Add(new BuildValueModel()
                {
                    Name = $"NOME DEL SERVERRow{i}",
                    Value = $"{eventModel.ServerHost} - {eventModel.ServerIP}"
                });
                buildValueModels.Add(new BuildValueModel()
                {
                    Name = $"ESITORow{i}",
                    Value = logEventType
                });
                buildValueModels.Add(new BuildValueModel()
                {
                    Name = $"NOME CONTROLLORow{i}",
                    Value = hasError ? eventModel.EventLogs.First(f => f.LogType == "Error").LogSource : eventModel.EventLogs.FirstOrDefault()?.LogSource
                }) ;
                
            }
            byte[] doc = await _stampaConformeClient.BuildPDFAsync(templatePDF, buildValueModels.ToArray(), string.Empty);
            return doc;
        }

        private static DocumentInstance[] FillDocumentInstances(List<FileModel> files)
        {
            if (files.Count == 0)
            {
                return new DocumentInstance[] { };
            }

            IList<DocumentInstance> instances = new List<DocumentInstance>();
            foreach (FileModel file in files)
            {
                instances.Add(new DocumentInstance()
                {
                    DocumentContent = Convert.ToBase64String(file.Content),
                    DocumentName = file.Filename
                });
            }
            return instances.ToArray();
        }

        private WorkflowReferenceModel CreateUDSBuildModelAsync(Guid correlationId, string workflowName, byte[] pdfContent, IDictionary<string, object> metadatas)
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
                    Filename = _moduleConfiguration.ReportFileName
                }
            };

            model.Model.Documents.Document.Instances = FillDocumentInstances(fileModels);

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

        protected override void Execute()
        {
            try
            {
                ProcessEventsAsync().Wait();
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("TECMARKET.ReportProcess -> Execute critical error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            _logger.WriteInfo(new LogMessage("OnStop -> TECMARKET.ReportProcess"), LogCategories);
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

        #endregion
    }
}
