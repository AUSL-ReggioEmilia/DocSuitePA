using System;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Entity.DocumentUnits
{
    public class DocumentUnitFascicleCategory : DSWBaseEntity
    {
        #region [ Constructor ]

        public DocumentUnitFascicleCategory(Guid uniqueId) : base(uniqueId)
        {
        }

        public DocumentUnitFascicleCategory() : this(Guid.NewGuid()) { }

        #endregion

        #region [ Navigation Properties ]

        public virtual DocumentUnit DocumentUnit { get; set; }
        public virtual Category Category { get; set; }
        public virtual Fascicle Fascicle { get; set; }

        #endregion
    }
}
