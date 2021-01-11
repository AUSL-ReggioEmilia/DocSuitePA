using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.Core.Command.CQRS;
using VecompSoftware.Core.Command.CQRS.Events.Models.Workflows;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.ServiceBus.Receiver.Base.Exceptions;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Events.Models.Workflows;

namespace VecompSoftware.ServiceBus.Receiver.Base
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class ListenerMessageBase<TCommand> : IListenerMessageGeneric<TCommand>
        where TCommand : ICommand
    {
        #region [ Fields ]

        private readonly JsonSerializerSettings _serializerSettings;
        private readonly string _commandName;
        private EvaluationModel _retryPolicyEvaluation = null;
        private readonly bool _commandNameFilterEnabled;
        private readonly MessageReceiver _receiver;
        private readonly IListenerExecution<TCommand> _listenerExecution;
        private readonly ILogger _logger;
        private readonly IWebAPIClient _webApiClient;

        protected static IEnumerable<LogCategory> _logCategories = null;

        #endregion

        #region [ Properties ]

        public Type ICommandFilterType => typeof(TCommand);

        public string CommandName => _commandName;
        public EvaluationModel RetryPolicyEvaluation => _retryPolicyEvaluation;

        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(ListenerMessageBase<>));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Costructors ]
        public ListenerMessageBase(MessageReceiver receiver, ILogger logger,
            IListenerExecution<TCommand> listenerExecution, string commandName, IWebAPIClient webApiClient, bool commandNameFilterEnabled = true)
        {
            _logger = logger;
            _commandName = commandName;
            _commandNameFilterEnabled = commandNameFilterEnabled;
            _receiver = receiver;
            _listenerExecution = listenerExecution;
            _webApiClient = webApiClient;
            _serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            };
            _logger.WriteInfo(new LogMessage("Create new listener base"), LogCategories);
        }
        #endregion

        #region [ Methods ]
        internal virtual async Task MessageCallAsync(BrokeredMessage message)
        {
            TCommand command = default;
            try
            {
                _logger.WriteInfo(new LogMessage($"Message arrive is: {message.MessageId}"), LogCategories);
                _listenerExecution.Properties = ReadMessageProperties(message);
                _retryPolicyEvaluation = null;
                if (!_listenerExecution.Properties.ContainsKey(CustomPropertyName.EVALUATED_HOST_MACHINE))
                {
                    _listenerExecution.Properties.Add(CustomPropertyName.EVALUATED_HOST_MACHINE, string.Empty);
                }
                _listenerExecution.Properties[CustomPropertyName.EVALUATED_HOST_MACHINE] = Environment.MachineName;

                if (_listenerExecution.Properties.ContainsKey(CustomPropertyName.EVALUATION_EXCEPTION))
                {
                    _logger.WriteInfo(new LogMessage("Message has EvaluationException properties"), LogCategories);
                    ServiceBusEvaluationException exception = JsonConvert.DeserializeObject<ServiceBusEvaluationException>(_listenerExecution.Properties[CustomPropertyName.EVALUATION_EXCEPTION].ToString());
                    _retryPolicyEvaluation = exception != null ? exception.EvaluationModel : new EvaluationModel();
                }
                _listenerExecution.RetryPolicyEvaluation = RetryPolicyEvaluation;

                string currentCommandName = _listenerExecution.Properties[CustomPropertyName.COMMAND_NAME].ToString();
                if (!_commandNameFilterEnabled)
                {
                    currentCommandName = CommandName;
                }
                if (currentCommandName.Equals(CommandName, StringComparison.CurrentCultureIgnoreCase))
                {
                    command = ReadMessageBody(message);
                    if (ICommandFilterType.IsAssignableFrom(command.GetType()))
                    {
                        await _listenerExecution.ExecuteAsync(command);
                        _logger.WriteInfo(new LogMessage("Message has been processed correctly and it's going to be removed from queue"), LogCategories);
                        message.Complete();

                        if (_listenerExecution.Properties.ContainsKey(CustomPropertyName.WORKFLOW_NAME) && _webApiClient != null)
                        {
                            string workflowName = _listenerExecution.Properties[CustomPropertyName.WORKFLOW_NAME].ToString();
                            await SendInfoWorkflowNotificationAsync(command, $"Workflow {workflowName} has been completed successfully");
                        }

                        return;
                    }
                }
                message.Abandon();
            }
            catch (ServiceBusEvaluationException ex)
            {
                _logger.WriteError(ex, LogCategories);
                ex.EvaluationModel.CommandName = CommandName;
                ex.EvaluationModel.ModuleName = _listenerExecution.GetType().FullName;
                if (!_listenerExecution.Properties.ContainsKey(CustomPropertyName.EVALUATION_EXCEPTION))
                {
                    _listenerExecution.Properties.Add(CustomPropertyName.EVALUATION_EXCEPTION, string.Empty);
                }
                _listenerExecution.Properties[CustomPropertyName.EVALUATION_EXCEPTION] = JsonConvert.SerializeObject(ex.EvaluationModel, _serializerSettings);
                CompleteDeadletterProperties(ex, "Expected EvaluationException error");
                try
                {
                    message.DeadLetter(_listenerExecution.Properties);
                }
                catch (Exception ex_de)
                {
                    _logger.WriteWarning(new LogMessage(ex_de.Message), ex_de, LogCategories);
                    message.Abandon();
                }
                _logger.WriteWarning(new LogMessage("Message has been moved to dead letter queue with EvaluationException property setted"), LogCategories);

                if (_listenerExecution.Properties.ContainsKey(CustomPropertyName.WORKFLOW_NAME) && _webApiClient != null)
                {
                    await SendErrorWorkflowNotificationAsync(command, ex.StackTrace, ex.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                CompleteDeadletterProperties(ex, "Unexpected ListenerMessageBase error");
                message.DeadLetter(_listenerExecution.Properties);
                _logger.WriteWarning(new LogMessage("Message has been moved to dead letter queue."), LogCategories);

                if (_listenerExecution.Properties.ContainsKey(CustomPropertyName.WORKFLOW_NAME) && _webApiClient != null)
                {
                    await SendErrorWorkflowNotificationAsync(command, ex.StackTrace, ex.Message);
                }
            }
        }

        private async Task SendErrorWorkflowNotificationAsync(TCommand command, string stackTrace, string wfNotificationDescription)
        {
            WorkflowNotification errorWorkflowNotification = BuildWorkflowNotification(wfNotificationDescription, stackTrace);

            if (errorWorkflowNotification != null)
            {
                IEventWorkflowNotificationError workflowErrorEvent = new EventWorkflowNotificationError(Guid.NewGuid(),
                    command.CorrelationId,
                    command.TenantName,
                    command.TenantId,
                    command.TenantAOOId,
                    command.Identity,
                    errorWorkflowNotification, null);

                await _webApiClient.PushEventAsync(workflowErrorEvent);
            }
        }

        private async Task SendInfoWorkflowNotificationAsync(TCommand command, string wfNotificationDescription)
        {
            WorkflowNotification infoWorkflowNotification = BuildWorkflowNotification(wfNotificationDescription);

            if (infoWorkflowNotification != null)
            {
                IEventWorkflowNotificationInfo workflowInfoEvent = new EventWorkflowNotificationInfo(Guid.NewGuid(),
                    command.CorrelationId,
                    command.TenantName,
                    command.TenantId,
                    command.TenantAOOId,
                    command.Identity,
                    infoWorkflowNotification, null);

                await _webApiClient.PushEventAsync(workflowInfoEvent);
            }
        }

        private WorkflowNotification BuildWorkflowNotification(string wfNotificationDescription, string stackTrace = null)
        {
            WorkflowNotification workflowNotification = null;
            if (_listenerExecution.IdWorkflowActivity.HasValue)
            {
                Guid idWorkflowActivity = _listenerExecution.IdWorkflowActivity.Value;
                workflowNotification = new WorkflowNotification
                {
                    ModuleName = _listenerExecution.GetType().FullName,
                    LogDate = DateTimeOffset.UtcNow,
                    StackTrace = stackTrace,
                    UniqueId = Guid.NewGuid(),
                    IdWorkflowActivity = idWorkflowActivity,
                    WorkflowName = _listenerExecution.Properties[CustomPropertyName.WORKFLOW_NAME] as string,
                    Description = wfNotificationDescription
                };
            }
            return workflowNotification;
        }

        private void CompleteDeadletterProperties(Exception ex, string deadLetterReason)
        {
            if (!_listenerExecution.Properties.ContainsKey("DeadLetterReason"))
            {
                _listenerExecution.Properties.Add("DeadLetterReason", string.Empty);
            }
            if (!_listenerExecution.Properties.ContainsKey("DeadLetterErrorDescription"))
            {
                _listenerExecution.Properties.Add("DeadLetterErrorDescription", string.Empty);
            }
            _listenerExecution.Properties["DeadLetterReason"] = deadLetterReason;
            _listenerExecution.Properties["DeadLetterErrorDescription"] = GetMessageErrors(ex);
        }

        private string GetMessageErrors(Exception ex)
        {
            try
            {
                StringBuilder message = new StringBuilder(ex.Message, short.MaxValue - 1);
                if (ex.InnerException != null)
                {
                    message.Append($", {ex.InnerException.Message}");
                    if (ex.InnerException.InnerException != null)
                    {
                        message.Append($", {ex.InnerException.InnerException.Message}");
                    }
                }
                return message.ToString();
            }
            catch (ArgumentOutOfRangeException)
            {
                return ex.Message.Length >= short.MaxValue ? "Message could not be consumed after specified delivery attempts." : ex.Message;
            }
        }
        public async Task StartListeningAsync(bool? autoComplete = null, int? maxConcurrentCalls = null)
        {
            await Task.Run(async () =>
            {
                if (_receiver == null)
                {
                    _logger.WriteError(new LogMessage("Receiver is not correctly configured"), LogCategories);
                }

                _receiver.OnMessageAsync(MessageCallAsync, await SetMessageOptionAsync(autoComplete, maxConcurrentCalls));
            });
        }
        public async Task CloseListeningAsync()
        {
            _logger.WriteInfo(new LogMessage("Closing listerner"), LogCategories);
            await _receiver.CloseAsync();
        }
        private async Task<OnMessageOptions> SetMessageOptionAsync(bool? autoComplete, int? maxConcurrentCalls)
        {
            _logger.WriteInfo(new LogMessage($"Message option: {autoComplete} - {maxConcurrentCalls}"), LogCategories);
            return await Task.Run(() => new OnMessageOptions()
            {
                AutoComplete = autoComplete.HasValue ? autoComplete.Value : true,
                MaxConcurrentCalls = maxConcurrentCalls.HasValue ? maxConcurrentCalls.Value : 1
            });
        }
        private TCommand ReadMessageBody(BrokeredMessage message)
        {
            string messageContent = message.GetBody<string>();
            _logger.WriteDebug(new LogMessage($"Message content Length {messageContent?.Length}"), LogCategories);
            return JsonConvert.DeserializeObject<TCommand>(messageContent, _serializerSettings);
        }

        private IDictionary<string, object> ReadMessageProperties(BrokeredMessage message)
        {
            foreach (KeyValuePair<string, object> property in message.Properties)
            {
                _logger.WriteDebug(new LogMessage($"Message property : {property.Key} - {property.Value}"), LogCategories);
            }
            return message.Properties;
        }
        #endregion
    }
}