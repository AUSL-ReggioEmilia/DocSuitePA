using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits
{
    public class DocumentUnitContactModel
    {
        #region [ Constructor ]
        public DocumentUnitContactModel()
        { }
        public DocumentUnitContactModel(Guid uniqueId)
        {
            UniqueId = uniqueId;
        }
        #endregion

        #region [ Fields ]

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }

        public string ContactManual { get; set; }

        public short ContactType { get; set; }

        public string ContactLabel { get; set; }
        #endregion
    }
}
