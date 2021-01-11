using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Messages
{
    public class MessageModel
    {
        #region [ Properties ]
        public int? IdMessage { get; set; }
        public MessageStatus Status { get; set; }
        public MessageType MessageType { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public ICollection<MessageEmailModel> MessageEmails { get; set; }
        public ICollection<MessageAttachmentModel> MessageAttachments { get; set; }
        public ICollection<MessageContactModel> MessageContacts { get; set; }
        #endregion

        #region [ Constructor ]
        public MessageModel()
        {
            MessageEmails = new List<MessageEmailModel>();
            MessageAttachments = new List<MessageAttachmentModel>();
            MessageContacts = new List<MessageContactModel>();
        }
        #endregion

        #region [ Methods ]
        public bool HasAttachments()
        {
            return MessageAttachments.Count > 0;
        }

        public bool HasContacts()
        {
            return MessageContacts.Count > 0;
        }
        #endregion
    }
}
