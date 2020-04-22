using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.DocSuiteWeb.Service.Workflow;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Workflows;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Workflow
{
    [LogCategory(LogCategoryDefinition.WEBAPIWORKFLOW)]
    public class BaseWorkflowController<TEntity, TService> : ApiController
        where TEntity : class
        where TService : IWorkflowBaseService<TEntity>
    {
        #region [ Fields ]
        private static IEnumerable<LogCategory> _logCategories;
        private readonly IValidatorService _validationService;
        private readonly IWorkflowRuleset _ruleset;
        private readonly ILogger _logger;
        private readonly IWorkflowBaseService<TEntity> _service;

        #endregion

        #region [ Constructor ]
        public BaseWorkflowController(IWorkflowBaseService<TEntity> service, IValidatorService validationService, IWorkflowRuleset ruleset, ILogger logger)
            : base()
        {
            _service = service;
            _validationService = validationService;
            _ruleset = ruleset;
            _logger = logger;
        }
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(BaseWorkflowController<,>));
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
        public async Task<IHttpActionResult> PostAsync([FromBody]TEntity workflowEntity)
        {
            return await ActionHelper.TryCatchWithLoggerAsync(async () =>
            {
                if (workflowEntity == null)
                {
                    _logger.WriteWarning(new LogMessage(string.Concat("workflow model received is invalid (json deserialization set null value) : ", GetType().ToString())), LogCategories);
                    return BadRequest("workflow model received is invalid (json deserialization set null value)");
                }

                //if (!this._validationService.Validate(command, this._ruleset.INSERT))
                //{
                //    return BadRequest("Errore nella validazione del comando.");
                //}
                _logger.WriteInfo(new LogMessage(string.Concat("Workflow controller receive message ", workflowEntity.GetType())), LogCategories);
                WorkflowResult response = await _service.CreateAsync(workflowEntity);
                return Ok(response);
            }, BadRequest, Content, InternalServerError, _logger, LogCategories, logValidationException: true);
        }
        #endregion

        #region [ NotImplemented ]
        public Task<IQueryable<TEntity>> GetAsync(ODataQueryOptions opts)
        {
            throw new NotImplementedException();
        }

        public Task<IHttpActionResult> PutAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<IHttpActionResult> DeleteAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}