
using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers
{
    public class DossierModel
    {
        #region [ Constructor ]
        public DossierModel()
        {
            Roles = new HashSet<DossierRoleModel>();
            Contacts = new HashSet<ContactModel>();
            Documents = new HashSet<DossierDocumentModel>();
            DossierFolders = new HashSet<DossierFolderModel>();
        }
        #endregion

        #region [ Proprieties ]
        public Guid UniqueId { get; set; }

        public short Year { get; set; }

        public int Number { get; set; }

        public string Title { get; set; }

        public string Subject { get; set; }

        public string Note { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public short ContainerId { get; set; }

        public string ContainerName { get; set; }

        public string JsonMetadata { get; set; }
        #endregion

        #region [ Navigation Proprieties ]
        public ICollection<DossierRoleModel> Roles { get; set; }

        public ICollection<ContactModel> Contacts { get; set; }

        public ICollection<DossierDocumentModel> Documents { get; set; }

        public ICollection<DossierFolderModel> DossierFolders { get; set; }
        #endregion
    }
}
