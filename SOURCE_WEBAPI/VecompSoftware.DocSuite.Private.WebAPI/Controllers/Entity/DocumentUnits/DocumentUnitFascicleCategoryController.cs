using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Service.Entity.DocumentUnits;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.DocumentUnits
{
    public class DocumentUnitFascicleCategoryController : BaseWebApiController<DocumentUnitFascicleCategory, IDocumentUnitFascicleCategoryService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public DocumentUnitFascicleCategoryController(IDocumentUnitFascicleCategoryService service, IDataUnitOfWork unitOfWork, ILogger logger) 
            : base(service, unitOfWork, logger)
        {
        }

        #endregion

        #region [ Methods ]

        #endregion
    }
}