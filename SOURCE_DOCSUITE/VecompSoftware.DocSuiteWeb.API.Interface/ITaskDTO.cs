using System;

namespace VecompSoftware.DocSuiteWeb.API
{
    public interface ITaskDTO : IAPIArgument
    {
        #region [ Properties ]

        int? Id { get; set; }

        string Code { get; set; }

        string Title { get; set; }

        string Description { get; set; }

        int? TaskType { get; set; }

        int? Status { get; set; }

        DateTime? TaskDate { get; set; }

        IProtocolDTO[] Protocols { get; set; }

        IMailDTO[] PECMails { get; set; }

        IMailDTO[] POLMails { get; set; }

        #endregion
    }
}
