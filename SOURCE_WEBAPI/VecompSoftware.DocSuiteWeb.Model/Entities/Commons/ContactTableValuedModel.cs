using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Commons
{
    public class ContactTableValuedModel
    {

        #region [ Constructors ]

        public ContactTableValuedModel()
        {
        }

        #endregion

        #region [ Properties ]
        public int? Index { get; set; }

        public int? IdContact { get; set; }

        public int? IncrementalFather { get; set; }

        public Guid UniqueId { get; set; }

        public string Description { get; set; }

        public string ContactType { get; set; }

        public string ComunicationType { get; set; }

        public string Type { get; set; }

        public bool IsSelected { get; set; }

        public string Code { get; set; }

        public string Email { get; set; }

        public string CertifiedMail { get; set; }

        public string Note { get; set; }

        public string FiscalCode { get; set; }

        public DateTimeOffset? RegistrationDate { get; set; }

        #endregion
    }
}
