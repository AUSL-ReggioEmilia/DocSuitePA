using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Messages
{
    public class MessageContactEmailModel
    {
        #region [ Properties ]
        public int? IdMessageContactEmail { get; set; }
        public string User { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        #endregion

        #region [ Constructor ]
        public MessageContactEmailModel()
        {

        }
        #endregion
    }
}
