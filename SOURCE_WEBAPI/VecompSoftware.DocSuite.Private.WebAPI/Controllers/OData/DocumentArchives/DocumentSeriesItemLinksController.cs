using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.DocumentArchives;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.DocumentArchives
{
    public class DocumentSeriesItemLinksController : BaseODataController<DocumentSeriesItemLink, IDocumentSeriesItemLinkService>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]

        public DocumentSeriesItemLinksController(IDocumentSeriesItemLinkService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {

        }

        #endregion

        #region [ Methods ]
        #endregion
    }
}