using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Xml.Linq;
using VecompSoftware.BPM.Integrations.Modules.TECMARKET.EventViewerAnalyzer.Configuration;
using VecompSoftware.BPM.Integrations.Modules.TECMARKET.EventViewerAnalyzer.Models;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Events.Models.Workflows;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.Helpers.Workflow;

namespace VecompSoftware.BPM.Integrations.Modules.TECMARKET.EventViewerAnalyzer
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [Fields]

        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly ILogger _logger;
        private readonly IServiceBusClient _serviceBusClient;
        private readonly IDocumentClient _documentClient;
        private readonly IWebAPIClient _webAPIClient;
        private bool _needInitializeModule = false;
        private readonly IdentityContext _identityContext = null;
        private List<EventLogModel> _eventLogs;
        private readonly string _queryString;

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

        #region [Constructor]
        [ImportingConstructor]
        public Execution(ILogger logger, IServiceBusClient serviceBusClient, IWebAPIClient webAPIClient, IDocumentClient client)
                : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _serviceBusClient = serviceBusClient;
                _webAPIClient = webAPIClient;
                _documentClient = client;
                string username = "anonymous";
                _needInitializeModule = true;

                if (WindowsIdentity.GetCurrent() != null)
                {
                    username = WindowsIdentity.GetCurrent().Name;
                }
                _identityContext = new IdentityContext(username);

                _logger.WriteDebug(new LogMessage("Initialize query string file for event logs."), LogCategories);
                _queryString = File.ReadAllText(ModuleConfigurationHelper.QueryStringPath);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("TECMARKET.EventViewerAnalyzer -> Critical error in costruction module"), ex, LogCategories);
                throw;
            }
        }
        #endregion

        #region [Methods]   
        protected override void Execute()
        {
            if (Cancel)
            {
                return;
            }
            try
            {
                InitializeModule();

                _eventLogs = ReadEventLogsFromEventViewer();
                WriteEventLogsInServiceBus().Wait();
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("TECMARKET.EventViewerAnalyzer -> Execute critical error"), ex, LogCategories);
                throw;
            }
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);

                _needInitializeModule = false;
            }
        }

        private List<EventLogModel> ReadEventLogsFromEventViewer()
        {
            List<EventLogModel> eventLogs = new List<EventLogModel>();

            List<string> devices = new List<string>()
            {
                EventAttributeName.Processor.ToString(),
                EventAttributeName.Memory.ToString(),
                EventAttributeName.PhysicalDisk.ToString()
            };

            int eventLogCount = 0;
            string pathValue;

            XDocument doc = XDocument.Load(ModuleConfigurationHelper.QueryStringPath);
            pathValue = doc.Descendants("Select").Select(rec => rec.Attribute("Path").Value).FirstOrDefault();

            EventLogQuery query = new EventLogQuery(pathValue, PathType.LogName, _queryString);

            EventLogReader logReader = new EventLogReader(query);
            _logger.WriteDebug(new LogMessage("Read all event logs from QueryList"), LogCategories);

            for (EventRecord eventRecord = logReader.ReadEvent(); eventRecord != null; eventRecord = logReader.ReadEvent())
            {
                _logger.WriteDebug(new LogMessage("Read event log and create EventLogModel"), LogCategories);

                eventLogCount++;
                EventLogModel eventLog = new EventLogModel
                {
                    LogSource = eventRecord.ProviderName,
                    LogType = eventRecord.LevelDisplayName,
                    LogDate = eventRecord.TimeCreated.ToString(),
                    LogDescription = eventRecord.FormatDescription()
                };
                if (eventRecord.Properties.Count() == 2 && eventRecord.Properties.Any(x => devices.Any(d => d.Equals(x.Value))))
                {
                    eventLog.SourceDeviceName = eventRecord.Properties[1].Value.ToString();
                }

                eventLogs.Add(eventLog);

            }
            _logger.WriteInfo(new LogMessage("Event logs were read from Event Viewer"), LogCategories);
            return eventLogs;
        }

        private async Task WriteEventLogsInServiceBus()
        {
            try
            {
                WorkflowReferenceModel workflowReferenceModel = CreateEventLogsReferenceModel(_eventLogs);

                WorkflowStart workflowStart = BuildWorkflowStart(workflowReferenceModel);

                _logger.WriteDebug(new LogMessage("Create EventWorkflowStartRequest for ServiceBus"), LogCategories);
                EventWorkflowStartRequest eventWorkflowStartRequest = new EventWorkflowStartRequest(_moduleConfiguration.TenantName, _moduleConfiguration.TenantId,
                    _moduleConfiguration.TenantAOOId, _identityContext, workflowStart);

                _logger.WriteDebug(new LogMessage($"Write EventWorkflowStartRequest with id = {eventWorkflowStartRequest.Id} in Service Bus"), LogCategories);
                ServiceBusMessage message = await _webAPIClient.SendEventAsync(eventWorkflowStartRequest);
                _logger.WriteInfo(new LogMessage($"EventWorkflowStartRequest with id = {eventWorkflowStartRequest.Id} was successfully written in Service Bus"), LogCategories);

            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WriteEventLogsInServiceBus -> error complete start EventViewerAnalyzer workflow"), ex, LogCategories);
                throw;
            }

        }
        private WorkflowReferenceModel CreateEventLogsReferenceModel(List<EventLogModel> eventLogs)
        {
            _logger.WriteDebug(new LogMessage("Create WorflowReferenceModel for event logs"), LogCategories);
            WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel
            {
                ReferenceId = new Guid(),
                ReferenceType = DocSuiteWeb.Model.Entities.Commons.DSWEnvironmentType.Build
            };

            _logger.WriteDebug(new LogMessage("Create Event Model for ServiceBus"), LogCategories);
            EventModel eventModel = new EventModel
            {
                EventDate = DateTime.Now.ToString(),
                ServerHost = Dns.GetHostName(),
                ServerIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString(),
                EventLogs = eventLogs
            };
            workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(eventModel, ModuleConfigurationHelper.JsonSerializerSettings);

            _logger.WriteInfo(new LogMessage($"WorflowReferenceModel with referenceId = {workflowReferenceModel.ReferenceId} was created"), LogCategories);
            return workflowReferenceModel;
        }

        private WorkflowStart BuildWorkflowStart(WorkflowReferenceModel workflowReferenceModelEventViewer)
        {
            _logger.WriteDebug(new LogMessage("Create WorflowStart for event logs"), LogCategories);
            WorkflowStart workflowStart = new WorkflowStart
            {
                WorkflowName = _moduleConfiguration.WorkflowName
            };

            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, new WorkflowArgument()
            {
                Name = WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL,
                PropertyType = ArgumentType.Json,
                ValueString = JsonConvert.SerializeObject(workflowReferenceModelEventViewer)
            });

            _logger.WriteInfo(new LogMessage($"WorflowStart for WorflowReferenceModel with referenceId = {workflowReferenceModelEventViewer.ReferenceId} was created"), LogCategories);
            return workflowStart;
        }
        protected override void OnStop()
        {
            _logger.WriteInfo(new LogMessage("OnStop -> TECMARKET.EventViewerAnalyzer"), LogCategories);
        }

        #endregion
    }
}
