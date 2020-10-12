using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.VSW.WorkflowAutoNotify.Configuration;
using VecompSoftware.BPM.Integrations.Modules.VSW.WorkflowAutoNotify.Models;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.Helpers.Workflow;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Events;
using VecompSoftware.Services.Command.CQRS.Events.Entities.Workflow;
using VecompSoftware.Services.Command.CQRS.Events.Models.Fascicles;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.WorkflowAutoNotify
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly ILogger _logger;
        private readonly IServiceBusClient _serviceBusClient;
        private readonly IWebAPIClient _webAPIClient;
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
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _serviceBusClient = serviceBusClient;
                _webAPIClient = webAPIClient;
                _needInitializeModule = true;
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("VSW.WorkflowAutoNotify -> Critical error in costruction module"), ex, LogCategories);
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
                _logger.WriteError(new LogMessage("WorkflowAutoNotify -> critical error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> VSW.WorkflowAutoNotify"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEvent>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicBuilderEvent,
                    _moduleConfiguration.WorkflowAutoCompleteBuildCompleteSubscription, WorkflowAutoCompleteBuildCompleteCallback));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompleteWorkflowActivity>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowActivityCompleted,
                    _moduleConfiguration.WorkflowActivityAutoCompleteSubscription, WorkflowActivityAutoCompleteCallback));
                _needInitializeModule = false;
            }
        }

        public IWorkflowContentBase EvaluateContentValue(IContent content)
        {
            IWorkflowContentBase workflowContentBase;
            IEnumerable<PropertyInfo> propertyInfos = content.GetType().GetRuntimeProperties();
            foreach (PropertyInfo item in propertyInfos.Where(f => f.Name.Equals("ContentTypeValue")))
            {
                workflowContentBase = item.GetValue(content, null) as IWorkflowContentBase;
                return workflowContentBase;
            }

            return null;
        }

        private async Task WorkflowAutoCompleteBuildCompleteCallback(IEvent evt, IDictionary<string, object> properties)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"WorkflowAutoCompleteBuildCompleteCallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteDebug(new LogMessage($"WorkflowAutoCompleteBuildCompleteCallback -> evaluate event type {evt.GetType().FullName}"), LogCategories);
                IWorkflowContentBase contentType = EvaluateContentValue(evt.Content);
                _logger.WriteDebug(new LogMessage($"WorkflowAutoCompleteBuildCompleteCallback -> evaluate content type {contentType?.GetType().FullName}"), LogCategories);
                _logger.WriteInfo(new LogMessage($"Notifying BuildComplete for WorkflowInstanceId {evt.CorrelationId}"), LogCategories);
                WorkflowNotify workflowNotify = null;
                WorkflowResult workflowResult = null;

                _logger.WriteInfo(new LogMessage($"Notifying {contentType.GetType().FullName} for IdWorkflowActivity {contentType.IdWorkflowActivity}"), LogCategories);
                workflowNotify = new WorkflowNotify(contentType.IdWorkflowActivity.Value)
                {
                    WorkflowName = contentType.WorkflowName,
                    ModuleName = ModuleConfigurationHelper.MODULE_NAME
                };

                if (evt is IEventCompleteFascicleBuild)
                {
                    IEventCompleteFascicleBuild @event = evt as IEventCompleteFascicleBuild;

                    workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_FIELD_FASCICLE_FASCICLEMODEL, new WorkflowArgument
                    {
                        Name = WorkflowPropertyHelper.DSW_FIELD_FASCICLE_FASCICLEMODEL,
                        PropertyType = ArgumentType.Json,
                        ValueString = JsonConvert.SerializeObject(@event.ContentType.ContentTypeValue.Fascicle)
                    });
                }

                workflowResult = await _webAPIClient.WorkflowNotify(workflowNotify);
                _logger.WriteInfo(new LogMessage($"Workflow notify correctly [IsValid: {workflowResult.IsValid}] with instanceId {workflowResult.InstanceId}"), LogCategories);
                if (!workflowResult.IsValid || !workflowResult.InstanceId.HasValue)
                {
                    _logger.WriteError(new LogMessage("An error occured in notify workflow activity"), LogCategories);
                    throw new Exception(string.Join(", ", workflowResult.Errors));
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WorkflowAutoCompleteBuildCompleteCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        private async Task WorkflowActivityAutoCompleteCallback(IEventCompleteWorkflowActivity evt, IDictionary<string, object> properties)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"WorkflowActivityAutoCompleteCallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteDebug(new LogMessage($"WorkflowActivityAutoCompleteCallback -> evaluate event type {evt.GetType().FullName}"), LogCategories);
                _logger.WriteDebug(new LogMessage($"WorkflowActivityAutoCompleteCallback -> evaluate content type {evt.ContentType?.GetType().FullName}"), LogCategories);
                _logger.WriteDebug(new LogMessage($"Notifying WorkflowActivityId {evt.ContentType?.ContentTypeValue?.UniqueId} for WorkflowInstanceId {evt.CorrelationId}"), LogCategories);

                WorkflowNotify workflowNotify = null;
                WorkflowResult workflowResult = null;

                WorkflowActivity workflowActivity = evt.ContentType.ContentTypeValue;
                _logger.WriteInfo(new LogMessage($"Notifying WorkflowActivity {workflowActivity.Subject} for IdWorkflowActivity {workflowActivity.UniqueId}"), LogCategories);
                workflowNotify = new WorkflowNotify(workflowActivity.UniqueId)
                {
                    WorkflowName = workflowActivity.WorkflowInstance?.WorkflowRepository?.Name,
                    ModuleName = ModuleConfigurationHelper.MODULE_NAME
                };
                workflowResult = await _webAPIClient.WorkflowNotify(workflowNotify);
                _logger.WriteInfo(new LogMessage($"Workflow notify correctly [IsValid: {workflowResult.IsValid}] with instanceId {workflowResult.InstanceId}"), LogCategories);
                if (!workflowResult.IsValid || !workflowResult.InstanceId.HasValue)
                {
                    _logger.WriteError(new LogMessage("An error occured in notify workflow activity"), LogCategories);
                    throw new Exception(string.Join(", ", workflowResult.Errors));
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WorkflowActivityAutoCompleteCallback -> Critical Error"), ex, LogCategories);
                throw;
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

        #endregion
    }
}
