using AutoMapper;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class DossiersController : ODataController
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
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(DossiersController));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public DossiersController(IDataUnitOfWork unitOfWork, ILogger logger, IMapper mapper, Security.ISecurity security, IMapperUnitOfWork mapperUnitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        #endregion

        #region [ Methods ]
        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        public IHttpActionResult GetDossierById(Guid id)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<Dossier> dossiers = _unitOfWork.Repository<Dossier>().GetByUniqueId(id, true)
                    .ToList();
                dossiers.FirstOrDefault().DossierFolders = dossiers.FirstOrDefault().DossierFolders.Where(x => x.DossierFolderLevel == 1).ToList();
                ICollection<DossierModel> dossiersModel = new List<DossierModel>()
                {
                    _mapper.Map<Dossier, DossierModel>(dossiers.FirstOrDefault())
                };
                return Ok(dossiersModel);
            }, _logger, LogCategories);
        }

        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        public IHttpActionResult GetDossierByYearAndNumber(short year, int number)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<Dossier> dossiers = _unitOfWork.Repository<Dossier>().GetByYearAndNumber(year, number, true)
                    .ToList();
                dossiers.FirstOrDefault().DossierFolders = dossiers.FirstOrDefault().DossierFolders.Where(x => x.DossierFolderLevel == 1).ToList();
                ICollection<DossierModel> dossiersModel = new List<DossierModel>();
                foreach (Dossier dossier in dossiers)
                {
                    dossiersModel.Add(_mapper.Map<Dossier, DossierModel>(dossier));
                }
                return Ok(dossiersModel);
            }, _logger, LogCategories);
        }
        #endregion
    }
}