using System;

namespace VecompSoftware.DocSuiteWeb.DTO.Commons
{
    public class MiscellaneaDocumentModel : DocumentModel
    {
        #region [ Fields ]       

        public int? Version { get; set; }

        public string Note { get; set; }

        public MiscellaneaDocumentStatus Status { get; set; }

        public string Serialized { get; set; }

        public DateTime RegistrationDate { get; set; }

        public string RegistrationUser { get; set; }

        public int LocationId { get; set; }

        public bool EditEnabled { get; set; }
        #endregion
    }
}
