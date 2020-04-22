using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Messages
{
    public class MessageEmailModel
    {
        #region [ Properties ]
        public int? IdMessageEmail { get; set; }
        public DateTime? SentDate { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Priority { get; set; }
        public Guid? EmlDocumentId { get; set; }
        public bool? IsDispositionNotification { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        #endregion

        #region [ Constructor ]
        public MessageEmailModel()
        {

        }
        #endregion
    }
}
