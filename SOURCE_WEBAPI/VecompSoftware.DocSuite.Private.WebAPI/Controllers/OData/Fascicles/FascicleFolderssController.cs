using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Fascicles;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Fascicles
{
    public class FascicleFolderssController : BaseODataController<FascicleFolder, IFascicleFolderService>
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapperUnitOfWork _mapperUnitOfwork;
        #endregion

        #region [ Controller ]

        public FascicleFolderssController(IFascicleFolderService service, IDataUnitOfWork unitOfWork, ILogger logger, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapperUnitOfwork = mapperUnitOfWork;
        }

        #endregion

        #region [ Methods ]

        [HttpGet]
        public IHttpActionResult GetChildrenByParent(ODataQueryOptions<FascicleFolder> options, Guid idFascicleFolder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                ICollection<FascicleFolderTableValuedModel> fascicleFolders = _unitOfWork.Repository<FascicleFolder>().GetChildrenByParent(idFascicleFolder);
                ICollection<FascicleFolderModel> fascicleFoldersModel = _mapperUnitOfwork.Repository<IDomainMapper<FascicleFolderTableValuedModel, FascicleFolderModel>>().MapCollection(fascicleFolders).ToList();

                return Ok(fascicleFoldersModel);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetByCategoryAndFascicle(ODataQueryOptions<FascicleFolder> options, Guid idFascicle, short idCategory)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                IQueryable<FascicleFolder> fascicleFolders = _unitOfWork.Repository<FascicleFolder>().GetByCategoryAndFascicle(idFascicle, idCategory);
                return Ok(fascicleFolders);
            }, _logger, LogCategories);
        }
        #endregion
    }
}
