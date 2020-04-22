using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Finder.Protocols;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Protocols;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Protocols
{
    public class ProtocolsController : BaseODataController<Protocol, IProtocolService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IProtocolTableValuedModelMapper _mapperTableValued;
        private readonly ISecurity _security;
        private readonly DomainUserModel _currentUser;

        #endregion

        #region [ Constructor ]

        public ProtocolsController(IProtocolService service, IDataUnitOfWork unitOfWork, ILogger logger, IProtocolTableValuedModelMapper mapperTableValued,
            ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
            _mapperTableValued = mapperTableValued;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _security = security;
            _currentUser = security.GetCurrentUser();
        }

        #endregion

        #region [ Methods ]

        [HttpGet]
        public IHttpActionResult GetAuthorizedProtocols(string username, string domain, string dateFrom, string dateTo)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<ProtocolTableValuedModel> results = _unitOfWork.Repository<Protocol>().GetAuthorized(username, domain,
                    DateTimeOffset.ParseExact(dateFrom, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                    DateTimeOffset.ParseExact(dateTo, "yyyyMMddHHmmss", CultureInfo.InvariantCulture));
                return Ok(_mapperTableValued.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        [EnableQuery(MaxExpansionDepth = 6)]
        public IHttpActionResult GetProtocolSummary(Guid id)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                IList<Protocol> protocol = _unitOfWork.Repository<Protocol>()
                                                      .GetUserAuthorizedByUniqueId(id, _security.GetCurrentUser().Account).ToList();

                if (protocol == null)
                {
                    throw new ArgumentNullException("Protocol not found");
                }

                return Ok(protocol);
            }, _logger, LogCategories);
        }

        [HttpGet]
        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        public IHttpActionResult GetUserAuthorizedProtocols(int skip, int top, string subject, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, string contact)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                IList<Protocol> protocols = _unitOfWork.Repository<Protocol>().GetUserAuthorized(skip, top, _currentUser.Account, subject, dateFrom, dateTo, contact).ToList();
                if (protocols == null)
                {
                    throw new ArgumentNullException("Protocols not found");
                }

                return Ok(protocols.AsQueryable());
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetUserAuthorizedProtocolsCount(string subject, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, string contact)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                long protocolCount = _unitOfWork.Repository<Protocol>().CountUserAuthorized(_currentUser.Account, subject, dateFrom, dateTo, contact);
                return Ok(protocolCount);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult CountProtocolToRead()
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                long protocolsToReadCount = _unitOfWork.Repository<Protocol>().CountProtocolToRead(_currentUser.Account);
                return Ok(protocolsToReadCount);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetProtocolToRead()
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                IList<Protocol> protocolsToRead = new List<Protocol>();
                return Ok(protocolsToRead);
            }, _logger, LogCategories);
        }


    }
    #endregion
}
