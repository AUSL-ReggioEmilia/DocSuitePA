using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles
{
    public class FascicleModel : IContentBase
    {
        #region [ Constructors ]

        public FascicleModel()
        {
            Contacts = new List<ContactModel>();
            FascicleDocuments = new List<FascicleDocumentModel>();
            FascicleDocumentUnits = new List<FascicleDocumentUnitModel>();
            Roles = new List<ProtocolRoleModel>();
            DocumentUnits = new List<DocumentUnitModel>();
            FascicleFolders = new List<FascicleFolderModel>();
        }

        #endregion

        #region [ Properties ]
        public short? Conservation { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public string FascicleObject { get; set; }

        public FascicleType? FascicleType { get; set; }

        public VisibilityType? VisibilityType { get; set; }

        public string Manager { get; set; }

        public string Name { get; set; }

        public string Note { get; set; }

        public int? Number { get; set; }

        public string Rack { get; set; }

        public DateTimeOffset? StartDate { get; set; }

        public string Title { get; set; }

        public short? Year { get; set; }
        
        public Guid UniqueId { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset? RegistrationDate { get; set; }

        public CategoryModel Category { get; set; }

        public string MetadataValues { get; set; }

        public MetadataRepositoryModel MetadataRepository { get; set; }

        public ContactModel ManagerC { get; set; }

        public ICollection<ContactModel> Contacts { get; set; }

        public ICollection<FascicleDocumentUnitModel> FascicleDocumentUnits { get; set; }

        public ICollection<FascicleDocumentModel> FascicleDocuments { get; set; }

        public ICollection<ProtocolRoleModel> Roles { get; set; }

        public ICollection<DocumentUnitModel> DocumentUnits { get; set; }

        public ContainerModel Container { get; set; }

        public ICollection<FascicleFolderModel> FascicleFolders { get; set; }

        #endregion

        #region [ Methods ]

        public bool HasContacts()
        {
            return Contacts.Any();
        }


        #endregion
    }
}
