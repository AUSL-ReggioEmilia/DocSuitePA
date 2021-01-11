using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits
{
    public class DocumentUnitRoleModel
    {
        #region [ Constructor ]

        public DocumentUnitRoleModel()
        { }

        public DocumentUnitRoleModel(Guid uniqueId)
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
