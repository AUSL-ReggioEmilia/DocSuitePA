﻿using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Service.Entity.Resolutions;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Resolutions
{
    public class ResolutionDocumentSeriesItemController : BaseWebApiController<ResolutionDocumentSeriesItem, IResolutionDocumentSeriesItemService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public ResolutionDocumentSeriesItemController(IResolutionDocumentSeriesItemService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {

        }
        #endregion

        #region [ Methods ]

        #endregion

    }
}