using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.Workflow.Actions;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.ServiceBus.Module.CQRS.Executors
{
    public abstract class BaseWorkflowActionExecutor<TWFAction> : IBaseWorkflowActionExecutor<TWFAction>
        where TWFAction : WorkflowActionModel
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IWebAPIClient _webAPIClient;
        protected static IEnumerable<LogCategory> _logCategories = null;
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(BaseWorkflowActionExecutor<>));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public BaseWorkflowActionExecutor(ILogger logger, IWebAPIClient webAPIClient)
        {
            _logger = logger;
            _webAPIClient = webAPIClient;
        }
        #endregion

        #region [ Methods ]
        IEvent IWorkflowActionExecutor.BuildEvent(ICommandCQRS command, IWorkflowAction workflowAction)
        {
            return BuildEvent(command, (TWFAction)workflowAction);
        }

        public abstract IEvent BuildEvent(ICommandCQRS command, TWFAction workflowAction);

        async Task IWorkflowActionExecutor.CreateActionEventAsync(ICommandCQRS command, IWorkflowAction workflowAction)
        {
            await CreateActionEventAsync(command, (TWFAction)workflowAction);
        }

        public async Task CreateActionEventAsync(ICommandCQRS command, TWFAction workflowAction)
        {
            try
            {
                IEvent @event = BuildEvent(command, workflowAction);
                if (workflowAction.CorrelationId != Guid.Empty)
                {
                    @event.ReplyTo = ServiceBus.ServiceBusClient.WorkflowAggregationTopicName;
                }
                _logger.WriteInfo(new LogMessage(string.Concat("CreateActionEventAsync -> create action event ", @event.GetType(), " for command ", command.Name)), LogCategories);
                bool res = await _webAPIClient.PushEventAsync(@event);
                if (!res)
                {
                    _logger.WriteError(new LogMessage(string.Concat("CreateActionEventAsync -> error in sending event ", @event.GetType(), " to WebAPI")), LogCategories);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Concat("CreateActionEventAsync -> error on creating workflow event for model ", workflowAction.GetType(), ". Command: ", command.Name)), ex, LogCategories);
            }
        }
        #endregion
    }
}
