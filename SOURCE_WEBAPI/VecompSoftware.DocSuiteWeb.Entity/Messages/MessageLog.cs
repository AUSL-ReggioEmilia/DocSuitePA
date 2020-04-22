using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Messages
{
    public class MessageLog : DSWBaseLogEntity<Message, MessageLogType>
    {
        #region [ Constructor ]
        public MessageLog() : this(Guid.NewGuid()) { }

        public MessageLog(Guid uniqueId)
            : base(uniqueId) { }
        #endregion

        #region [ Properties ]

        public DateTime LogDate { get; set; }
        #endregion

        #region [ Navigation Properties ]

        #endregion
    }
}
