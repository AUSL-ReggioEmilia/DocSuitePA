using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Finder.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Repository;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.UDS;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.UDS
{
    public class UDSFieldListsController : BaseODataController<UDSFieldList, IUDSFieldListService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]
        public UDSFieldListsController(IUDSFieldListService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]
        [HttpGet]
        public IHttpActionResult GetChildrenByParent(ODataQueryOptions<UDSFieldList> options, Guid parentId)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                ICollection<UDSFieldListTableValuedModel> children = _unitOfWork.Repository<UDSFieldList>().GetChildrenByParent(parentId);
                return Ok(children);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetParent(ODataQueryOptions<UDSFieldList> options, Guid childId)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                UDSFieldListTableValuedModel parent = _unitOfWork.Repository<UDSFieldList>().GetParent(childId);
                return Ok(parent);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetAllParents(ODataQueryOptions<UDSFieldList> options, Guid childId)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                ICollection<UDSFieldListTableValuedModel> parents = _unitOfWork.Repository<UDSFieldList>().GetAllParents(childId);
                return Ok(parents);
            }, _logger, LogCategories);
        }

        #endregion
    }
}