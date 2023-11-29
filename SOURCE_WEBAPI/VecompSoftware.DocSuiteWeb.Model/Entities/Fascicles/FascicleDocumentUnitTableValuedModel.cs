using System;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles
{
    public class FascicleDocumentUnitTableValuedModel : ICategoryTableValuedModel, IContainerTableValuedModel, ITenantAOOTableValuedModel
    {
        #region [ Constructor ]

        public FascicleDocumentUnitTableValuedModel()
        {
            UniqueId = Guid.NewGuid();
        }

        #endregion

        #region [ Properties ]

        public ReferenceType ReferenceType { get; set; }

        public short SequenceNumber { get; set; }

        public Guid UniqueId { get; set; }
        public Guid? DocumentUnit_IdDocumentUnit { get; set; }

        #endregion

        #region DocumentUnit

        public Guid? DocumentUnit_IdFascicle { get; set; }
        public int? DocumentUnit_EntityId { get; set; }
        public short DocumentUnit_Year { get; set; }
        public string DocumentUnit_Number { get; set; }
        public string DocumentUnit_Title { get; set; }
        public string DocumentUnit_Subject { get; set; }
        public string DocumentUnit_DocumentUnitName { get; set; }
        public int DocumentUnit_Environment { get; set; }
        public string DocumentUnit_RegistrationUser { get; set; }
        public DateTimeOffset? DocumentUnit_RegistrationDate { get; set; }
        public Guid? DocumentUnit_IdUDSRepository { get; set; }

        #region [ Category ]

        public short? Category_IdCategory { get; set; }

        public string Category_Name { get; set; }

        #endregion

        #region [ Container ]

        public short? Container_IdContainer { get; set; }

        public string Container_Name { get; set; }

        #endregion

        #region [ TenantAOO ]
        public Guid? TenantAOO_IdTenantAOO { get; set; }
        public string TenantAOO_Name { get; set; }
        #endregion
        #endregion
    }
}
