﻿using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Messages;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Messages
{
    public class MessageLogsController : BaseODataController<MessageLog, IMessageLogService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public MessageLogsController(IMessageLogService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
        }

        #endregion
    }
}