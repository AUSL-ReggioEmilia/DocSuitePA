using System;
using System.Collections.Generic;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.Tenants;
using VecompSoftware.DocSuiteWeb.Model.Parameters;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits
{
    public class DocumentUnitModel : IContentBase
    {
        #region [ Constructor ]
        public DocumentUnitModel()
        {
            UniqueId = Guid.NewGuid();
        }
        #endregion

        #region [ Fields ]

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }

        public int? EntityId { get; set; }

        public string DocumentUnitName { get; set; }

        public short Year { get; set; }

        public string Number { get; set; }

        public string Title { get; set; }

        public ReferenceType? ReferenceType { get; set; }

        public DateTimeOffset? RegistrationDate { get; set; }

        public string RegistrationUser { get; set; }

        public string Subject { get; set; }

        public Guid? IdFascicle { get; set; }

        public bool? IsFascicolable { get; set; }

        public CategoryModel Category { get; set; }

        public ContainerModel Container { get; set; }

        public int Environment { get; set; }

        public string MainDocumentName { get; set; }

        public Guid? IdUDSRepository { get; set; }

        public TenantAOOModel TenantAOO { get; set; }

        public DocumentUnitChainModel DocumentUnitChain { get; set; }

        public ICollection<DocumentUnitRoleModel> DocumentUnitRoles { get; set; }

        public FascicleFolderModel FascicleFolder { get; set; }

        #endregion
    }
}
