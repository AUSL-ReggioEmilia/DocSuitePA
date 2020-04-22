using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.PECMails;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.PECMails
{
    public class PECMailAttachmentsController : BaseODataController<PECMailAttachment, IPECMailAttachmentService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public PECMailAttachmentsController(IPECMailAttachmentService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
        }

        #endregion
    }
}
