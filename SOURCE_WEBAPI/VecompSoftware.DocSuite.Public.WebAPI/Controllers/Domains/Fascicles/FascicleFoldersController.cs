using AutoMapper;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Archives.Commons;
using VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI.Controllers.Domains.Fascicles
{
    [LogCategory(LogCategoryDefinition.ODATAAPI)]
    [EnableQuery]
    public class FascicleFoldersController : ODataController
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private static IEnumerable<LogCategory> _logCategories = null;
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(FascicleFoldersController));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public FascicleFoldersController(IDataUnitOfWork unitOfWork, ILogger logger, IMapper mapper, Security.ISecurity security, IMapperUnitOfWork mapperUnitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        #endregion

        #region [ Methods ]
        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        public IHttpActionResult GetNextFascicleFolders(Guid id)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<FascicleFolderTableValuedModel> fascicleFolders = _unitOfWork.Repository<FascicleFolder>()
                    .GetChildrenByParent(id);
                return Ok(fascicleFolders);
            }, _logger, LogCategories);
        }

        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        public IHttpActionResult GetFascicleDocumentUnitFromFolder(Guid id)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<FascicleDocumentUnit> fascicleDocumentUnits = _unitOfWork.Repository<FascicleFolder>()
                    .GetByIdFascicleFolder(id).ToList();
                ICollection<GenericDocumentUnitModel> fascicleDocumentUnitModels = _mapper.Map<ICollection<FascicleDocumentUnit>, ICollection<GenericDocumentUnitModel>>(fascicleDocumentUnits).ToList();
                return Ok(fascicleDocumentUnitModels);
            }, _logger, LogCategories);
        }

        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        public IHttpActionResult GetFascicleDocumentFromFolder(Guid id)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<FascicleDocument> fascicleDocuments = _unitOfWork.Repository<FascicleFolder>()
                    .GetDocumentsByIdFascicleFolder(id).ToList();
                ICollection<GenericDocumentUnitModel> fascicleDocumentModels = _mapper.Map<ICollection<FascicleDocument>, ICollection<GenericDocumentUnitModel>>(fascicleDocuments).ToList();
                return Ok(fascicleDocumentModels);
            }, _logger, LogCategories);
        }
        #endregion
    }
}