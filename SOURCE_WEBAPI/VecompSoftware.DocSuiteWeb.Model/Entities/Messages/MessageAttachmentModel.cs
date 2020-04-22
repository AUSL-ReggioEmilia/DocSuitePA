using System;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Messages
{
    public class MessageAttachmentModel
    {
        #region [ Properties ]
        public int? IdMessageAttachment { get; set; }
        public string Server { get; set; }
        public string Archive { get; set; }
        public int? ChainId { get; set; }
        public int? DocumentEnum { get; set; }
        public string Extension { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public DocumentModel Document { get; set; }
        #endregion

        #region [ Constructor ]
        public MessageAttachmentModel()
        {

        }
        #endregion
    }
}
