using Microsoft.AspNet.OData.Query;
using System;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Finder.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Mapper.Model.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.MassimariScarto;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.MassimariScarto
{
    public class MassimariScartoController : BaseODataController<MassimarioScarto, IMassimarioScartoService>
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMassimarioScartoModelMapper _mapper;
        private static readonly char[] separator = { ',', '.' };

        #endregion

        #region [ Constructor ]

        public MassimariScartoController(IMassimarioScartoService service, IDataUnitOfWork unitOfWork, ILogger logger,
            IMassimarioScartoModelMapper mapper, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region [ Methods ]
        [HttpGet]
        public IHttpActionResult GetAllChildren(ODataQueryOptions<MassimarioScarto> options, Guid? parentId, bool includeLogicalDelete)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                IQueryable<MassimarioScarto> query = null;
                if (parentId.HasValue)
                {
                    query = _unitOfWork.Repository<MassimarioScarto>().GetAllChildrenByParent(parentId.Value, includeLogicalDelete);
                }
                else
                {
                    query = _unitOfWork.Repository<MassimarioScarto>().GetRootChildren(includeLogicalDelete);
                }
                IQueryable<MassimarioScarto> results = options.ApplyTo(query) as IQueryable<MassimarioScarto>;
                return Ok(_mapper.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetMassimari(ODataQueryOptions<MassimarioScarto> options, string name, bool includeLogicalDelete)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                IQueryable<MassimarioScarto> query = _unitOfWork.Repository<MassimarioScarto>().Get(name ?? string.Empty, includeLogicalDelete);
                IQueryable<MassimarioScarto> results = options.ApplyTo(query) as IQueryable<MassimarioScarto>;
                return Ok(_mapper.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetMassimari(ODataQueryOptions<MassimarioScarto> options, string name, string fullCode, bool includeLogicalDelete)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                string[] codes = fullCode.Split(separator);
                IQueryable<MassimarioScarto> query = _unitOfWork.Repository<MassimarioScarto>().Get(name ?? string.Empty, string.Join("|", codes), includeLogicalDelete);
                IQueryable<MassimarioScarto> results = options.ApplyTo(query) as IQueryable<MassimarioScarto>;
                return Ok(_mapper.MapCollection(results));
            }, _logger, LogCategories);
        }
        #endregion
    }
}
