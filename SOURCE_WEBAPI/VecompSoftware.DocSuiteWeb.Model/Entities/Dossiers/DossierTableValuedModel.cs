using System;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers
{
    public class DossierTableValuedModel : IRoleTableValuedModel, IContactTableValuedModel
    {
        #region [ Constructor ]

        public DossierTableValuedModel()
        {
        }

        #endregion

        #region [ Properties ]

        public Guid IdDossier { get; set; }

        public short Year { get; set; }

        public int Number { get; set; }

        public string Subject { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public short Container_Id { get; set; }

        public string Container_Name { get; set; }

        public short? Role_IdRole { get; set; }

        public string Role_Name { get; set; }

        public int? Contact_Incremental { get; set; }

        public string Contact_Description { get; set; }
        public string MetadataDesigner { get; set; }
        public string MetadataValues { get; set; }

        #endregion

        #region [ Methods ]

        #endregion
    }
}
