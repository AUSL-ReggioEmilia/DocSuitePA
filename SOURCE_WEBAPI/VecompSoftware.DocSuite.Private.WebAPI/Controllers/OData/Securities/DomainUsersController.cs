using Microsoft.AspNet.OData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Security;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Securities
{
    [LogCategory(LogCategoryDefinition.ODATAAPI)]
    [EnableQuery]
    public class DomainUsersController : ODataController
    {
        #region [ Fields ]

        private static IEnumerable<LogCategory> _logCategories = null;
        private readonly ISecurity _security;
        private readonly ILogger _logger;
        private readonly Guid _instanceId;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IDecryptedParameterEnvService _parameterEnvService;

        #endregion

        #region [ Properties ]

        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(DomainUsersController));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public DomainUsersController(ISecurity security, ILogger logger, IDataUnitOfWork unitOfWork, IDecryptedParameterEnvService parameterEnvService)
        {
            _security = security;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _parameterEnvService = parameterEnvService;
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
        public IQueryable<DomainUserModel> Get()
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                return new List<DomainUserModel>(1) { _security.GetCurrentUser() }.AsQueryable();
            }, _logger, LogCategories);
        }

        public IHttpActionResult GetCurrentRights()
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                DomainUserModel domainUserModel = _security.GetCurrentUser();
                if (!domainUserModel.Rights.Any())
                {
                    domainUserModel.Rights = _unitOfWork.Repository<RoleUser>().GetUserRights(domainUserModel.Name, domainUserModel.Domain, 
                        _parameterEnvService.RoleGroupPECRightEnabled);
                }
                return Ok(domainUserModel);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetMembers(string groupName)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                IReadOnlyCollection<DomainUserModel> results = _security.GetMembers(groupName);
                return Ok(results);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult UsersFinder(string text)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                IReadOnlyCollection<DomainUserModel> results = _security.UsersFinder(text);
                return Ok(results);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetUser(string username, string domain)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                string account = string.Concat(domain, "\\", username);
                DomainUserModel results = _security.GetUser(account);
                return Ok(results);
            }, _logger, LogCategories);
        }

        #endregion
    }
}
