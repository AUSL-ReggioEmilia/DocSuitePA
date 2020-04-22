using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Finder.Collaborations;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Collaborations;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Collaborations
{
    public class CollaborationsController : BaseODataController<Collaboration, ICollaborationService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ICollaborationModelMapper _mapper;
        private readonly ICollaborationTableValuedModelMapper _mapperTableValue;

        #endregion

        #region [ Constructor ]

        public CollaborationsController(ICollaborationService service, IDataUnitOfWork unitOfWork, ILogger logger,
            ICollaborationModelMapper mapper, ICollaborationTableValuedModelMapper mapperTableValue, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _mapperTableValue = mapperTableValue;
        }

        #endregion

        #region [ Methods ]

        [HttpGet]
        public IHttpActionResult GetAtVisionSignCollaborations(ODataQueryOptions<Collaboration> options, string username, string domain)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetProposed(string.Format(@"{0}\{1}", domain, username));
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetToVisionSignCollaborations(ODataQueryOptions<Collaboration> options, string username, string domain, bool? isRequired)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetSigning(string.Format(@"{0}\{1}", domain, username), isRequired);
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetAtProtocolAdmissionCollaborations(ODataQueryOptions<Collaboration> options, string username, string domain)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetProtocolAdmissions(string.Format(@"{0}\{1}", domain, username));
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetCurrentActivitiesAllCollaborations(ODataQueryOptions<Collaboration> options, string username, string domain)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetAllUsers(string.Format(@"{0}\{1}", domain, username));
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetCurrentActivitiesActiveCollaborations(ODataQueryOptions<Collaboration> options, string username, string domain)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<string> signers = _unitOfWork.Repository<RoleUser>().GetAccounts(string.Format(@"{0}\{1}", domain, username)).Select(s => s.Account).Distinct().ToList();
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetActiveUsers(string.Format(@"{0}\{1}", domain, username), signers);
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetCurrentActivitiesPastCollaborations(ODataQueryOptions<Collaboration> options, string username, string domain)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetAlreadySigned(string.Format(@"{0}\{1}", domain, username));
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetToManageCollaborations(ODataQueryOptions<Collaboration> options, string username, string domain)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetManagings(string.Format(@"{0}\{1}", domain, username));
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        [EnableQuery]
        public IHttpActionResult GetRegisteredCollaborations(ODataQueryOptions<Collaboration> options, string username, string domain, string dateFrom, string dateTo)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetRegistered(string.Format(@"{0}\{1}", domain, username),
                    DateTimeOffset.ParseExact(dateFrom, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                    DateTimeOffset.ParseExact(dateTo, "yyyyMMddHHmmss", CultureInfo.InvariantCulture));
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetMyCheckedOutCollaborations(ODataQueryOptions<Collaboration> options, string username, string domain)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetCheckedOuts(string.Format(@"{0}\{1}", domain, username));
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        #endregion
    }
}
