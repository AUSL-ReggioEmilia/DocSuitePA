using System;

namespace VecompSoftware.DocSuiteWeb.DTO.Desks
{
    public class DeskDocumentResult
    {
        #region [ Properties ]

        public Guid? IdDesk { get; set; }

        public Guid? IdDeskDocument { get; set; }

        public string Name { get; set; }

        public long? Size { get; set; }

        public string BiblosSerializeKey { get; set; }

        public bool IsSavedToBiblos { get; set; }

        public bool IsSigned { get; set; }

        public string LastComment { get; set; }

        public DateTimeOffset? RegistrationDate { get; set; }

        public decimal? LastVersion { get; set; }

        public short IsActive { get; set; }
        /// <summary>
        /// Proprietà che identifica il nome server Biblos.
        /// </summary>
        public string DocumentServer { get; set; }

        public Guid? IdDocumentBiblos { get; set; }

        public Guid? IdChainBiblos { get; set; }

        public bool? IsJustInCollaboration { get; set; }

        #endregion
    }
}
