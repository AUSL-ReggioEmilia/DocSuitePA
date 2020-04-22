using System;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Entity.UDS
{
    public class UDSDocumentUnit : DSWUDSRelationBaseEntity<DocumentUnit>
    {
        #region [ Constructor ]

        public UDSDocumentUnit() : this(Guid.NewGuid()) { }

        public UDSDocumentUnit(Guid uniqueId)
            : base(uniqueId)
        {

        }
        #endregion

        #region [ Properties ]


        #endregion

        #region [ Navigation Properties ]

        #endregion
    }
}
