using System;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Entity.Fascicles
{
    public class FascicleDocumentUnit : DSWBaseEntity, IContentBase
    {

        #region [ Constructor ]

        public FascicleDocumentUnit() : this(Guid.NewGuid()) { }

        public FascicleDocumentUnit(Guid uniqueId) : base(uniqueId)
        {
        }


        #endregion

        #region  [ Properties ]

        /// <summary>
        /// Get or set ReferenceType
        /// </summary>
        public ReferenceType ReferenceType { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual Fascicle Fascicle { get; set; }
        public virtual FascicleFolder FascicleFolder { get; set; }
        public virtual DocumentUnit DocumentUnit { get; set; }

        #endregion
    }
}
