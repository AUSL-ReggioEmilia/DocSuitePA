using System;
using VecompSoftware.DocSuiteWeb.Repository.Entity;

namespace VecompSoftware.DocSuiteWeb.Entity.DocumentUnits
{
    public class DocumentUnitChain : DSWBaseEntity, IUnauditableEntity
    {
        #region [ Constructor ]

        public DocumentUnitChain() : this(Guid.NewGuid()) { }
        public DocumentUnitChain(Guid uniqueId)
            : base(uniqueId) { }
        #endregion

        #region [ Properties ]

        public string DocumentName { get; set; }

        public Guid IdArchiveChain { get; set; }

        public string ArchiveName { get; set; }

        public ChainType ChainType { get; set; }

        public string DocumentLabel { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual DocumentUnit DocumentUnit { get; set; }

        #endregion
    }
}
