using System;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Entity.DocumentUnits
{
    public class DocumentUnitFascicleHistoricizedCategory : DSWBaseEntity
    {
        #region [ Constructor ]

        public DocumentUnitFascicleHistoricizedCategory(Guid uniqueId) : base(uniqueId)
        {
        }
        public DocumentUnitFascicleHistoricizedCategory() : this(Guid.NewGuid()) { }
        #endregion

        #region [ Properties ]

        public DateTimeOffset UnfascicolatedDate { get; set; }
        public string ReferencedFascicle { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual DocumentUnit DocumentUnit { get; set; }
        public virtual Category Category { get; set; }

        #endregion
    }
}