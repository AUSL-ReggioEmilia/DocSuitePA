using System;
using System.Collections.Generic;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Archives.Commons;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;

namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.Fascicles
{
    //attributo?

    public class FascicleModel : DomainModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]
        public FascicleModel(Guid id)
            : base(id)
        {
            Contacts = new HashSet<FascicleContactModel>();
            DocumentUnits = new HashSet<GenericDocumentUnitModel>();
            Documents = new HashSet<DocumentModel>();
            FascicleFolders = new HashSet<FascicleFolderModel>();
        }
        #endregion

        #region [ Properties ]

        public short Year { get; set; }

        public long Number { get; set; }

        public string Title { get; set; }

        public string Subject { get; set; }

        public CategoryModel Category { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public string FascicleName { get; set; }

        public string Note { get; set; }

        public FascicleType FascicleType { get; set; }

        public VisibilityType VisibilityType { get; set; }

        public string MetadataValues { get; set; }

        public ICollection<FascicleContactModel> Contacts { get; set; }

        public ICollection<FascicleFolderModel> FascicleFolders { get; set; }

        public ICollection<GenericDocumentUnitModel> DocumentUnits { get; set; }

        public ICollection<DocumentModel> Documents { get; set; }
        #endregion

    }
}
