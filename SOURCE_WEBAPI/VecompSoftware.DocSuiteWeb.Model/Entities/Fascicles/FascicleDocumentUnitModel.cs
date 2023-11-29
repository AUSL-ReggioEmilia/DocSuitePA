using System;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles
{
    public class FascicleDocumentUnitModel
    {
        #region [ Constructor ]


        #endregion

        #region [ Properties ]
        public FascicleDocumentUnitModel() { }

        public ReferenceType ReferenceType { get; set; }
        
        public short SequenceNumber { get; set; }

        public Guid? UniqueId { get; set; }
        #endregion

        #region [ Navigation Properties ]

        public DocumentUnitModel DocumentUnit { get; set; }

        #endregion
    }
}