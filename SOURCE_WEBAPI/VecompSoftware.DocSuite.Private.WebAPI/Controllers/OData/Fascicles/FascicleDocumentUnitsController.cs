using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Web.Http;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Fascicles;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Fascicles
{
    public class FascicleDocumentUnitsController : BaseODataController<FascicleDocumentUnit, IFascicleDocumentUnitService>
    {
        #region [ Fields ]
        private readonly IFascicleDocumentUnitService _service;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IDecryptedParameterEnvService _parameterEnvService;
        private readonly IFascicleDocumentUnitTableValuedModelMapper _mapperTableValue;
        #endregion

        #region [ Constructor ]

        public FascicleDocumentUnitsController(IFascicleDocumentUnitService service, IDataUnitOfWork unitOfWork, ILogger logger,
            ISecurity security, IFascicleDocumentUnitTableValuedModelMapper mapperTableValue, IDecryptedParameterEnvService parameterEnvService) 
            : base(service, unitOfWork, logger, security)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapperTableValue = mapperTableValue;
            _parameterEnvService = parameterEnvService;
        }

        #endregion

        #region [ Methods ]
        [HttpGet]
        public IHttpActionResult FascicleDocumentUnits(ODataQueryOptions<FascicleDocumentUnitModel> options, [FromODataUri] Guid idFascicle, [FromODataUri] Guid? idFascicleFolder, [FromODataUri] Guid idTenantAOO)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                Guid? tmpidTenantAOO = idTenantAOO;
                if (_parameterEnvService.MultiAOOFascicleEnabled)
                {
                    tmpidTenantAOO = null;
                }

                ICollection<FascicleDocumentUnitTableValuedModel> documentUnits = _unitOfWork.Repository<FascicleDocumentUnit>().GetFascicleDocumentUnitsPrivate(new Fascicle(idFascicle), idFascicleFolder, tmpidTenantAOO);
                ICollection<FascicleDocumentUnitModel> mappedModels = _mapperTableValue.MapCollection(documentUnits);

                return Ok(mappedModels);
            }, _logger, LogCategories);
        }
        #endregion
    }
}