using Microsoft.AspNet.OData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using ActionHelper = VecompSoftware.DocSuite.WebAPI.Common.Helpers.ActionHelper;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.ServiceBusTopics
{
    [LogCategory(LogCategoryDefinition.ODATAAPI)]
    [EnableQuery]
    public class ServiceBusTopicsController : ODataController
    {
        #region [ Fields ]

        private readonly ITopicService _service;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly ISecurity _security;
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        private IEnumerable<LogCategory> _logCategories;


        #endregion

        #region [ Constructor ]

        public ServiceBusTopicsController(ITopicService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security,
            IMapperUnitOfWork mapperUnitOfWork)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _security = security;
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        #endregion

        #region [ Properties ]
        protected IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(ServiceBusTopicsController));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Methods ]
        public async Task<IHttpActionResult> Get(string topicName, string subscriptionName, Guid? correlationId)
        {
            return await ActionHelper.TryCatchWithLoggerGeneric(async () =>
            {
                await _service.InitializeAsync(topicName, subscriptionName);
                IEnumerable<ServiceBusMessage> messages = await _service.GetMessagesFromTopicAsync();
                if (correlationId.HasValue)
                {
                    messages = messages.Where(x => x.CorrelationId == correlationId.Value.ToString()).ToList();
                }

                IEnumerable<ServiceBusTopicMessage> result = _mapperUnitOfWork.Repository<IServiceTopicMessageMapper>().MapCollection(messages.ToList());
                return Ok(result);

            }, _logger, LogCategories);
        }

        #endregion
    }
}
