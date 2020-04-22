using System;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits
{
    public class DocumentUnitTableValuedModel : ICategoryTableValuedModel, IContainerTableValuedModel
    {
        #region [ Constructor ]

        public DocumentUnitTableValuedModel()
        {
            UniqueId = Guid.NewGuid();
        }

        #endregion

        #region [ Properties ]

        public int? EntityId { get; set; }

        public Guid UniqueId { get; set; }

        public string DocumentUnitName { get; set; }

        public short Year { get; set; }

        public string Number { get; set; }

        public string Title { get; set; }

        public ReferenceType? ReferenceType { get; set; }

        public DateTimeOffset? RegistrationDate { get; set; }

        public string RegistrationUser { get; set; }

        public string Subject { get; set; }

        public Guid? IdFascicle { get; set; }

        public int Environment { get; set; }

        public Guid? IdUDSRepository { get; set; }

        public bool? IsFascicolable { get; set; }

        #region [ Category ]

        public short? Category_IdCategory { get; set; }

        public string Category_Name { get; set; }

        #endregion

        #region [ Container ]

        public short? Container_IdContainer { get; set; }

        public string Container_Name { get; set; }

        #endregion

        #region [ DocumentUnitChain ]

        public Guid? DocumentUnitChain_IdDocumentUnitChain { get; set; }

        public string DocumentUnitChain_DocumentName { get; set; }

        #endregion

        #region [ DocumentUnitRole ]

        public Guid? DocumentUnitRole_IdDocumentUnitRole { get; set; }

        public Guid? DocumentUnitRole_UniqueIdRole { get; set; }

        #endregion

        #endregion
    }
}
