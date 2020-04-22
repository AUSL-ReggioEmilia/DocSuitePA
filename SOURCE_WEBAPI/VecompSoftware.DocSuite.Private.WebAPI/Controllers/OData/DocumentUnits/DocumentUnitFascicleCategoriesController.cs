using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.DocumentUnits;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.DocumentUnits
{
    public class DocumentUnitFascicleCategoriesController : BaseODataController<DocumentUnitFascicleCategory, IDocumentUnitFascicleCategoryService>
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]

        public DocumentUnitFascicleCategoriesController(IDocumentUnitFascicleCategoryService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security) 
            : base(service, unitOfWork, logger, security)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region [ Methods ]

        #endregion
    }
}