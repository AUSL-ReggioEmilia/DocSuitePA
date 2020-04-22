using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.Core.Command.CQRS;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.BPM.Integrations.Services.ServiceBus
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class ServiceBusClient : IServiceBusClient
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly JsonSerializerSettings _serializerSettings;
        private static readonly ConcurrentDictionary<Guid, MessageClientEntity> _clients = new ConcurrentDictionary<Guid, MessageClientEntity>();
        private static readonly OnMessageOptions messageOptions = new OnMessageOptions() { AutoComplete = false };
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(ServiceBusClient));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public ServiceBusClient(ILogger logger)
        {
            _logger = logger;
            _serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            };
        }
        #endregion

        #region [ Methods ]
        private async Task OnMessage<T>(string moduleName, BrokeredMessage message, Func<T, Task> callbackAsync)
        {
            IDictionary<string, object> properties = new Dictionary<string, object>();
            try
            {
                properties = ReadMessageProperties(message);
                if (!properties.ContainsKey(CustomPropertyName.EVALUATED_HOST_MACHINE))
                {
                    properties.Add(CustomPropertyName.EVALUATED_HOST_MACHINE, string.Empty);
                }
                properties[CustomPropertyName.EVALUATED_HOST_MACHINE] = Environment.MachineName;
                if (!properties.ContainsKey("ModuleName"))
                {
                    properties.Add("ModuleName", string.Empty);
                }
                properties["ModuleName"] = moduleName;
                T content = ReadMessageBody<T>(message);
                await callbackAsync(content);
                await message.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                properties = CompleteDeadletterProperties(ex, properties, "Expected Exception error");
                await message.DeadLetterAsync(properties);
            }
        }
        private IDictionary<string, object> ReadMessageProperties(BrokeredMessage message)
        {
            foreach (KeyValuePair<string, object> property in message.Properties)
            {
                _logger.WriteDebug(new LogMessage($"Message property : {property.Key} - {property.Value}"), LogCategories);
            }
            return message.Properties;
        }

        private IDictionary<string, object> CompleteDeadletterProperties(Exception ex, IDictionary<string, object> properties, string deadLetterReason)
        {
            if (!properties.ContainsKey("DeadLetterReason"))
            {
                properties.Add("DeadLetterReason", string.Empty);
            }
            if (!properties.ContainsKey("DeadLetterErrorDescription"))
            {
                properties.Add("DeadLetterErrorDescription", string.Empty);
            }
            properties["DeadLetterReason"] = deadLetterReason;
            properties["DeadLetterErrorDescription"] = GetMessageErrors(ex);
            return properties;
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

        public Guid StartListening<TEvent>(string moduleName, string topicName, string subscriptionName, Func<TEvent, Task> callbackAsync)
            where TEvent : IEvent
        {
            Guid id = Guid.NewGuid();
            try
            {
                _logger.WriteInfo(new LogMessage($"Connect topic {topicName} with subscription name {subscriptionName} from module {moduleName}"), LogCategories);
                SubscriptionClient client = SubscriptionClient.Create(topicName, subscriptionName, ReceiveMode.PeekLock);

                client.OnMessageAsync(async (message) => await OnMessage(moduleName, message, callbackAsync), messageOptions);
                _clients.TryAdd(id, client);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"Error occured during start listening in topic {topicName} on subscription name {subscriptionName}"), ex, LogCategories);
                throw ex;
            }
            return id;
        }

        public Guid StartListening<TCommand>(string moduleName, string queueName, Func<TCommand, Task> callbackAsync)
            where TCommand : ICommand
        {
            Guid id = Guid.NewGuid();
            try
            {
                _logger.WriteInfo(new LogMessage($"Connect queue {queueName}from module {moduleName}"), LogCategories);
                QueueClient client = QueueClient.Create(queueName, ReceiveMode.PeekLock);

                client.OnMessageAsync(async (message) => await OnMessage(moduleName, message, callbackAsync), messageOptions);
                _clients.TryAdd(id, client);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
            return id;
        }

        public async Task<string> CreateSubscriptionAsync(string topicName, string filter)
        {
            try
            {
                _logger.WriteInfo(new LogMessage("Creating subscription..."), LogCategories);
                NamespaceManager manager = NamespaceManager.Create();
                SqlFilter sqlFilter;
                string subscriptionName = Guid.NewGuid().ToString("N");

                if (!string.IsNullOrEmpty(filter))
                {
                    sqlFilter = new SqlFilter(filter);
                    _logger.WriteInfo(new LogMessage(string.Format("Create subscription name {0} with filters {1}", subscriptionName, sqlFilter.SqlExpression)), LogCategories);
                    await manager.CreateSubscriptionAsync(topicName, subscriptionName, sqlFilter);
                }
                else
                {
                    _logger.WriteInfo(new LogMessage(string.Format("Create subscription name {0}", subscriptionName)), LogCategories);
                    await manager.CreateSubscriptionAsync(topicName, subscriptionName);
                }
                return subscriptionName;
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        public async Task DeleteSubscriptionAsync(string topicName, string subscriptionName)
        {
            try
            {
                _logger.WriteInfo(new LogMessage("Deleting subscription..."), LogCategories);
                NamespaceManager manager = NamespaceManager.Create();

                if (string.IsNullOrEmpty(subscriptionName))
                {
                    _logger.WriteWarning(new LogMessage("Cannot delete a subscription without specify subscription name"), LogCategories);
                    return;
                }

                _logger.WriteInfo(new LogMessage(string.Concat("Delete subscription name ", subscriptionName)), LogCategories);
                await manager.DeleteSubscriptionAsync(topicName, subscriptionName);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        public async Task CloseListeningAsync(Guid clientId)
        {
            try
            {
                if (!_clients.ContainsKey(clientId))
                {
                    return;
                }

                MessageClientEntity client = _clients[clientId];
                await client.CloseAsync();
                _clients.TryRemove(clientId, out client);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        public async Task SendCommandAsync<TCommand>(TCommand command, string queueName, DateTime? scheduleEnqueueTimeUtc)
            where TCommand : ICommand
        {
            try
            {
                string serializedContent = JsonConvert.SerializeObject(command, _serializerSettings);
                BrokeredMessage message = new BrokeredMessage(serializedContent);
                if (scheduleEnqueueTimeUtc.HasValue)
                {
                    message.ScheduledEnqueueTimeUtc = scheduleEnqueueTimeUtc.Value;
                }
                foreach (KeyValuePair<string, object> property in command.CustomProperties)
                {
                    message.Properties.Add(property.Key, property.Value);
                }

                QueueClient client = QueueClient.Create(queueName);
                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        public async Task SendEventAsync<TEvent>(TEvent @event, string topicName, DateTime? scheduleEnqueueTimeUtc)
            where TEvent : IEvent
        {
            try
            {
                string serializedContent = JsonConvert.SerializeObject(@event, _serializerSettings);
                BrokeredMessage message = new BrokeredMessage(serializedContent);
                if (scheduleEnqueueTimeUtc.HasValue)
                {
                    message.ScheduledEnqueueTimeUtc = scheduleEnqueueTimeUtc.Value;
                }
                foreach (KeyValuePair<string, object> property in @event.CustomProperties)
                {
                    message.Properties.Add(property.Key, property.Value);
                }
                if (@event.CorrelationId.HasValue)
                {
                    message.CorrelationId = @event.CorrelationId.Value.ToString();
                }

                TopicClient client = TopicClient.Create(topicName);
                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        private T ReadMessageBody<T>(BrokeredMessage message)
        {
            string messageContent = message.GetBody<string>();
            _logger.WriteInfo(new LogMessage($"Message {message.MessageId} arrived"), LogCategories);
            _logger.WriteDebug(new LogMessage($"Message content {string.Join(string.Empty, messageContent.Take(256))}"), LogCategories);
            return JsonConvert.DeserializeObject<T>(messageContent, _serializerSettings);
        }
        #endregion
    }
}
