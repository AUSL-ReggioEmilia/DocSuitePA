using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Service.Entity.PECMails;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.PECMails
{
    public class PECMailAttachmentController : BaseWebApiController<PECMailAttachment, IPECMailAttachmentService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public PECMailAttachmentController(IPECMailAttachmentService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion

    }
}