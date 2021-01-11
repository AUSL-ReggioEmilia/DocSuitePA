using Microsoft.AspNet.OData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Security;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Securities
{
    [LogCategory(LogCategoryDefinition.ODATAAPI)]
    [EnableQuery]
    public class DomainGroupsController : ODataController
    {
        #region [ Fields ]

        private static IEnumerable<LogCategory> _logCategories = null;
        private readonly ISecurity _security;
        private readonly ILogger _logger;
        private readonly Guid _instanceId;

        #endregion

        #region [ Properties ]

        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(DomainGroupsController));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public DomainGroupsController(ISecurity security, ILogger logger)
        {
            _security = security;
            _logger = logger;
            _instanceId = Guid.NewGuid();
        }
        #endregion

        #region [ Dispose ]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }
        #endregion

        #region [ Methods ]
        [EnableQuery]
        public IQueryable<DomainGroupModel> Get()
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                return _security.GetGroupsCurrentUser().AsQueryable();
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GroupsFinder(string text)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                IReadOnlyCollection<DomainGroupModel> results = _security.GroupsFinder(text);
                return Ok(results);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GroupsFromUser(string username)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                IReadOnlyCollection<DomainGroupModel> results = _security.GetGroupsFromUser(username);
                return Ok(results);
            }, _logger, LogCategories);
        }

        #endregion
    }
}
