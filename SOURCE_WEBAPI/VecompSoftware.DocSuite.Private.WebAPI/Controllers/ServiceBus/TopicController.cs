using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.ServiceBus;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.ServiceBus
{
    [LogCategory(LogCategoryDefinition.WEBAPISERVICEBUS)]
    public class TopicController : ApiController
    {
        #region [ Fields ]
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ITopicService _service;
        private readonly IValidatorService _validationService;
        private readonly ITopicRuleset _ruleset;
        private readonly ILogger _logger;
        private readonly ICQRSMessageMapper _mapper;

        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };

        #endregion

        #region [ Constructor ]
        public TopicController(ITopicService service, IValidatorService validationService, ITopicRuleset ruleset,
            ILogger logger, ICQRSMessageMapper mapper)
            : base()
        {
            _service = service;
            _validationService = validationService;
            _ruleset = ruleset;
            _logger = logger;
            _mapper = mapper;
        }
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(QueueController));
                }
                return _logCategories;
            }
        }

        #endregion

        #region [ Methods ]

        [AcceptVerbs("OPTIONS")]
        [AllowAnonymous]
        public IHttpActionResult Options()
        {
            return Ok();
        }

        public IHttpActionResult Get()
        {
            return Ok("Web API is alive.");
        }

        [HttpPost]
        public async Task<IHttpActionResult> PostAsync([FromBody]IEvent @event)
        {
            return await ActionHelper.TryCatchWithLoggerAsync(async () =>
            {
                _logger.WriteDebug(new LogMessage(string.Concat("VecompSoftware.DocSuiteWeb.WebAPI.Controllers.ServiceBus.TopicController.PostAsync(", @event != null, ")")), LogCategories);
                if (@event == null)
                {
                    _logger.WriteWarning(new LogMessage(string.Concat("Event received is invalid (json deserialization set null value) : ",
                        GetType().ToString())), LogCategories);

                    return BadRequest("Event received is invalid (json deserialization set null value)");
                }

                _logger.WriteInfo(new LogMessage(string.Concat("Event received ", @event.ToString())), LogCategories);

                if (!_validationService.Validate(@event, _ruleset.INSERT))
                {
                    _logger.WriteWarning(new LogMessage(string.Concat("Service Bus validation error: ", GetType().ToString())), LogCategories);
                    return BadRequest("Service Bus validation error.");
                }

                ServiceBusMessage message = _mapper.Map(@event, new ServiceBusMessage());
                if (string.IsNullOrEmpty(message.ChannelName))
                {
                    throw new DSWException(string.Concat("Topic name to event [", @event.ToString(), "] is not mapped"), null, DSWExceptionCode.SC_Mapper);
                }
                ServiceBusMessage response = await _service.SendToTopicAsync(message);
                return Ok(response);

            }, BadRequest, Content, InternalServerError, _logger, LogCategories);
        }

        [HttpDelete]
        public async Task<IHttpActionResult> DeleteAsync(Guid? messageId)
        {
            return await ActionHelper.TryCatchWithLoggerAsync(async () =>
            {
                if (!messageId.HasValue || messageId == Guid.Empty)
                {
                    _logger.WriteWarning(new LogMessage(string.Concat("MessageId received is invalid (json deserialization set default value) : ", GetType().ToString())), LogCategories);

                    return BadRequest("MessageId received is invalid (json deserialization set default value)");
                }

                _logger.WriteInfo(new LogMessage(string.Concat("MessageId received ", messageId.ToString())), LogCategories);

                ServiceBusMessage message = await _service.GetMessageByIdFromTopicAsync(messageId.Value);
                if (message == null)
                {
                    throw new DSWException($"Message not found in topics", null, DSWExceptionCode.SC_Mapper);
                }
                BrokeredMessage brokeredMessage = new BrokeredMessage(message.Content);
                await brokeredMessage.CompleteAsync();

                return Ok();

            }, BadRequest, Content, InternalServerError, _logger, LogCategories);

        }
        #endregion
    }
}
