using Newtonsoft.Json;
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
using VecompSoftware.Services.Command.CQRS.Commands;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.ServiceBus
{
    [LogCategory(LogCategoryDefinition.WEBAPISERVICEBUS)]
    public class QueueController : ApiController
    {
        #region [ Fields ]
        private static IEnumerable<LogCategory> _logCategories;
        private readonly IQueueService _service;
        private readonly IValidatorService _validationService;
        private readonly IQueueRuleset _ruleset;
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
        public QueueController(IQueueService service, IValidatorService validationService, IQueueRuleset ruleset, ILogger logger, ICQRSMessageMapper mapper)
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
        public async Task<IHttpActionResult> PostAsync([FromBody]ICommand command)
        {
            return await ActionHelper.TryCatchWithLoggerAsync(async () =>
            {
                _logger.WriteDebug(new LogMessage(string.Concat("VecompSoftware.DocSuiteWeb.WebAPI.Controllers.ServiceBus.QueueController.PostAsync(", command != null, ")")), LogCategories);

                if (command == null)
                {
                    _logger.WriteWarning(new LogMessage(string.Concat("Command received is invalid (json deserialization set null value) : ",
                        GetType().ToString())), LogCategories);

                    return BadRequest("Command received is invalid (json deserialization set null value)");
                }

                _logger.WriteInfo(new LogMessage(string.Concat("Command received ", command.ToString())), LogCategories);
                if (!_validationService.Validate(command, _ruleset.INSERT))
                {
                    _logger.WriteWarning(new LogMessage(string.Concat("Service Bus validation error: ", GetType().ToString())), LogCategories);
                    return BadRequest("Service Bus validation error.");
                }

                ServiceBusMessage message = _mapper.Map(command, new ServiceBusMessage());
                if (string.IsNullOrEmpty(message.ChannelName))
                {
                    throw new DSWException(string.Concat("Queue name to command [", command.ToString(), "] is not mapped"), null, DSWExceptionCode.SC_Mapper);
                }
                ServiceBusMessage response = await _service.SubscribeQueue(message.ChannelName).SendToQueueAsync(message);
                return Ok(response);

            }, BadRequest, Content, InternalServerError, _logger, LogCategories);
        }
        #endregion
    }
}