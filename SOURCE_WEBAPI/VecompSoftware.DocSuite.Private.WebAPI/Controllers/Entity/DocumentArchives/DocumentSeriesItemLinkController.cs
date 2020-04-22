using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Securities;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Service.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.DocumentArchives
{
    public class DocumentSeriesItemLinkController : BaseWebApiController<DocumentSeriesItemLink, IDocumentSeriesItemLinkService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;

        #endregion

        #region [ Constructor ]
        public DocumentSeriesItemLinkController(IDocumentSeriesItemLinkService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        #endregion

        #region [ Methods ]
        
        #endregion
    }
}