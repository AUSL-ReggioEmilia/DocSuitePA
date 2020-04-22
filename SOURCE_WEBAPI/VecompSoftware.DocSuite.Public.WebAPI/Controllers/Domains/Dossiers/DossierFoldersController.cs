using AutoMapper;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Web.Http;
using VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Finder.Dossiers;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI.Controllers.Domains.Dossiers
{
    [LogCategory(LogCategoryDefinition.ODATAAPI)]
    [EnableQuery]
    public class DossierFoldersController : ODataController
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
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(DossierFoldersController));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public DossierFoldersController(IDataUnitOfWork unitOfWork, ILogger logger, IMapper mapper, Security.ISecurity security, IMapperUnitOfWork mapperUnitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        #endregion

        #region [ Methods ]
        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        public IHttpActionResult GetNextDossierFolders(Guid id)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<DossierFolderTableValuedModel> dossierFolders = _unitOfWork.Repository<DossierFolder>()
                    .GetChildrenByParent(id, 0);
                return Ok(dossierFolders);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult HasChildren(Guid id)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                int childrenCount = _unitOfWork.Repository<DossierFolder>().CountChildren(id);
                return Ok(childrenCount > 0);
            }, _logger, LogCategories);
        }
        #endregion
    }
}