using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Events.Entities.Workflows;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Mapper.Workflow;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Workflows;
using VecompSoftware.Helpers.Workflow;
using VecompSoftware.Services.Command.CQRS.Events.Entities.Workflow;
using CustomHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Workflow
{
    [LogCategory(LogCategoryDefinition.WEBAPIWORKFLOW)]
    public class WorkflowCompleteController : ApiController
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly IValidatorService _validationService;
        private readonly IWorkflowRuleset _ruleset;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ITopicService _topicServiceBus;
        private readonly IdentityContext _identityContext;
        private readonly IWorkflowInstanceService _workflowInstanceService;
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        #endregion

        #region [ Constructor ]
        public WorkflowCompleteController(IValidatorService validationService, IWorkflowRuleset ruleset, ILogger logger,
            ITopicService topicServiceBus, IDataUnitOfWork unitOfWork, ISecurity security,
            IWorkflowInstanceService workflowInstanceService, IMapperUnitOfWork mapperUnitOfWork)
        {
            _validationService = validationService;
            _ruleset = ruleset;
            _logger = logger;
            _topicServiceBus = topicServiceBus;
            _unitOfWork = unitOfWork;
            _workflowInstanceService = workflowInstanceService;
            _mapperUnitOfWork = mapperUnitOfWork;
            _identityContext = new IdentityContext(security.GetCurrentUser().Name);
        }

        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(WorkflowCompleteController));
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


        [HttpPost]
        public async Task<IHttpActionResult> PostAsync([FromBody]WorkflowComplete model)
        {
            return await CustomHelpers.ActionHelper.TryCatchWithLoggerAsync(async () =>
            {
                if (model == null)
                {
                    _logger.WriteWarning(new LogMessage(string.Concat("workflow model received is invalid (json deserialization set null value) : ", GetType().ToString())), LogCategories);
                    return BadRequest("workflow model received is invalid (json deserialization set null value)");
                }

                _logger.WriteInfo(new LogMessage(string.Concat("Workflow complete controller receive instanceId ", model.InstanceId,
                    " with output ", model.OutputArguments.Count(), " arguments")), LogCategories);

                WorkflowInstance workflowInstance = _unitOfWork.Repository<WorkflowInstance>().GetByInstanceId(model.InstanceId);
                if (workflowInstance == null)
                {
                    _logger.WriteWarning(new LogMessage(string.Concat("WorkflowInstance '", model.InstanceId, "' not found")), LogCategories);
                    return BadRequest(string.Concat("WorkflowInstance '", model.InstanceId, "' not found"));
                }
                workflowInstance.Status = WorkflowStatus.Done;
                WorkflowProperty prop;
                foreach (KeyValuePair<string, WorkflowArgument> item in model.OutputArguments.Where(x => !workflowInstance.WorkflowProperties.Any(f => f.Name == x.Key)))
                {
                    _logger.WriteDebug(new LogMessage(string.Concat("Add OutputArgument ", item.Key)), LogCategories);
                    prop = _mapperUnitOfWork.Repository<IWorkflowArgumentMapper>().Map(item.Value, new WorkflowProperty());
                    prop.WorkflowType = WorkflowType.Workflow;
                    workflowInstance.WorkflowProperties.Add(prop);
                }

                _unitOfWork.BeginTransaction();
                WorkflowProperty dsw_p_TenantId = workflowInstance.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID);
                WorkflowProperty dsw_p_TenantName = workflowInstance.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME);
                WorkflowInstance updated = await _workflowInstanceService.UpdateAsync(workflowInstance);

                _logger.WriteInfo(new LogMessage(string.Concat("WorkflowInstance ", workflowInstance.WorkflowRepository.Name,
                    " [", workflowInstance.InstanceId, "] in Tenant [",
                    dsw_p_TenantName.ValueString, "/", dsw_p_TenantId.ValueGuid.Value, "] completed.")),
                    LogCategories);

                IEventCompleteWorkflowInstance evt = new EventCompleteWorkflowInstance(dsw_p_TenantName.ValueString, dsw_p_TenantId.ValueGuid.Value,
                   _identityContext, workflowInstance);
                ServiceBusMessage message = _mapperUnitOfWork.Repository<ICQRSMessageMapper>().Map(evt, new ServiceBusMessage());
                ServiceBusMessage response = await _topicServiceBus.SendToTopicAsync(message);

                bool result = await _unitOfWork.SaveAsync();

                _logger.WriteInfo(new LogMessage(string.Concat("WorkflowInstance ", workflowInstance.WorkflowRepository.Name,
                   " [", workflowInstance.InstanceId, "] in Tenant [", dsw_p_TenantName.ValueString, "/", dsw_p_TenantId.ValueGuid.Value, "] sended notification [", response.MessageId, "].")),
                   LogCategories);

                return Ok();
            }, BadRequest, Content, InternalServerError, _logger, LogCategories);
        }
        #endregion
    }
}