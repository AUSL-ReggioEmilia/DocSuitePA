using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;

namespace VecompSoftware.DocSuiteWeb.Entity.UDS
{
    public class UDSRepository : DSWBaseEntity
    {
        #region [ Constructor ]

        public UDSRepository() : this(Guid.NewGuid()) { }

        public UDSRepository(Guid uniqueId)
            : base(uniqueId)
        {

            DocumentUnits = new HashSet<DocumentUnit>();
            UDSTypologies = new HashSet<UDSTypology>();
            UDSLogs = new HashSet<UDSLog>();
            UDSRoles = new HashSet<UDSRole>();
            UDSUsers = new HashSet<UDSUser>();
            UDSContacts = new HashSet<UDSContact>();
            UDSMessages = new HashSet<UDSMessage>();
            UDSDocumentUnits = new HashSet<UDSDocumentUnit>();
            UDSPECMails = new HashSet<UDSPECMail>();
            UDSCollaborations = new HashSet<UDSCollaboration>();
        }
        #endregion

        #region [ Properties ]

        public string Name { get; set; }

        public short SequenceCurrentYear { get; set; }

        public int SequenceCurrentNumber { get; set; }

        public string ModuleXML { get; set; }

        public short Version { get; set; }

        public DateTimeOffset ActiveDate { get; set; }

        public DateTimeOffset? ExpiredDate { get; set; }

        /// <summary>
        /// Stato di una UDS
        /// 1 = Bozza
        /// 2 = Confermata
        /// </summary>
        public UDSRepositoryStatus Status { get; set; }

        public string Alias { get; set; }

        public int DSWEnvironment { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual Container Container { get; set; }

        public virtual UDSSchemaRepository SchemaRepository { get; set; }

        public virtual ICollection<DocumentUnit> DocumentUnits { get; set; }

        public virtual ICollection<PECMail> PecMails { get; set; }

        public virtual ICollection<UDSTypology> UDSTypologies { get; set; }

        public virtual ICollection<UDSLog> UDSLogs { get; set; }

        public virtual ICollection<UDSRole> UDSRoles { get; set; }

        public virtual ICollection<UDSUser> UDSUsers { get; set; }

        public virtual ICollection<UDSContact> UDSContacts { get; set; }

        public virtual ICollection<UDSMessage> UDSMessages { get; set; }

        public virtual ICollection<UDSDocumentUnit> UDSDocumentUnits { get; set; }

        public virtual ICollection<UDSPECMail> UDSPECMails { get; set; }

        public virtual ICollection<UDSCollaboration> UDSCollaborations { get; set; }

        #endregion
    }
}
