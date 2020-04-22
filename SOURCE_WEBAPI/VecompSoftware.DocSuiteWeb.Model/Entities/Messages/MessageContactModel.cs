using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Messages
{
    public class MessageContactModel
    {
        #region [ Properties ]
        public int? IdMessageContact { get; set; }
        public MessageContactType ContactType { get; set; }
        public MessageContantTypology ContactPosition { get; set; }
        public string Description { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public ICollection<MessageContactEmailModel> MessageContactEmail { get; set; }
        #endregion

        #region [ Constructor ]
        public MessageContactModel()
        {
            MessageContactEmail = new List<MessageContactEmailModel>();
        }
        #endregion

        #region [ Methods ]
        public bool HasContactEmails()
        {
            return MessageContactEmail.Count > 0;
        }
        #endregion
    }
}
