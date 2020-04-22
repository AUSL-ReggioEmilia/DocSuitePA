using System;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles
{
    public class FascicleDocumentModel
    {
        #region [ Constructor ]


        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public ChainType ChainType { get; set; }
        public Guid IdArchiveChain { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset? RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public FascicleFolderModel FascicleFolder { get; set; }
        public DocumentModel Document { get; set; }

        #endregion

        #region [ Navigation Properties ]



        #endregion
    }
}