using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits
{
    public class DocumentUnitChainModel
    {
        #region [ Constructor ]
        public DocumentUnitChainModel()
        { }

        public DocumentUnitChainModel(Guid uniqueId)
        {
            UniqueId = uniqueId;
        }
        #endregion

        #region [ Fields ]

        #endregion

        #region [ Properties ]

        public Guid? UniqueId { get; set; }

        public string DocumentName { get; set; }

        public Guid? ArchiveChain { get; set; }

        #endregion
    }
}
