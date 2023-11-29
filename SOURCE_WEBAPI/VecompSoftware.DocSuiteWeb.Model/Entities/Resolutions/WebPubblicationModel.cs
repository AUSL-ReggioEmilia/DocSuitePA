using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions
{
    public class WebPublicationModel
    {

        #region [ Constructor ]
        public WebPublicationModel()
        { }

        public WebPublicationModel(int idWebPublication)
        {
            Id = idWebPublication;
        }
        #endregion

        #region [ Properties ]
        public int Id { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public string LastChangedUser { get; set; }
        public string ExternalKey { get; set; }
        public int? Status { get; set; }
        public int? IDLocation { get; set; }
        public int? IDDocument { get; set; }
        public int? EnumDocument { get; set; }
        public string Descrizione { get; set; }
        public bool? IsPrivacy { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public ResolutionModel Resolution { get; set; }
        #endregion
    }
}
