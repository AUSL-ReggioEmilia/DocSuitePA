using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Finder.Templates;
using VecompSoftware.DocSuiteWeb.Model.Entities.Templates;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Templates;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Templates
{
    public class TemplateCollaborationsController : BaseODataController<TemplateCollaboration, ITemplateCollaborationService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]
        public TemplateCollaborationsController(ITemplateCollaborationService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region [ Methods ]
        [HttpGet]
        public IHttpActionResult GetAuthorizedTemplates(ODataQueryOptions<TemplateCollaboration> options, string username, string domain)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<TemplateCollaborationModel> results = _unitOfWork.Repository<TemplateCollaboration>().GetAuthorized(username, domain);
                return Ok(results);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetAllParentsOfTemplate(ODataQueryOptions<TemplateCollaboration> options, Guid templateId)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<TemplateCollaborationModel> results = _unitOfWork.Repository<TemplateCollaboration>().GetAllParentsOfTemplate(templateId);
                return Ok(results);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetInvalidatingTemplatesByRoleUserAccount(ODataQueryOptions<TemplateCollaboration> options, string username, string domain, int idRole)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<TemplateCollaborationModel> result = _unitOfWork.Repository<TemplateCollaboration>().GetInvalidatingTemplatesByRoleUserAccount(string.Format(@"{0}\{1}", domain, username), idRole);
                return Ok(result);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetChildren(ODataQueryOptions<TemplateCollaboration> options, Guid idParent, short? status)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<TemplateCollaborationModel> result = _unitOfWork.Repository<TemplateCollaboration>().GetChildren(Username, Domain, idParent, status);
                return Ok(result);
            }, _logger, LogCategories);
        }
        #endregion
    }
}