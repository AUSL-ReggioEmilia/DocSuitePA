using VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Messages;

namespace VecompSoftware.DocSuiteWeb.WebAPI.Controllers.OData.Messages
{
    public class MessageEmailsController : BaseODataController<MessageEmail, IMessageEmailService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public MessageEmailsController(IMessageEmailService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
        }

        #endregion
    }
}
