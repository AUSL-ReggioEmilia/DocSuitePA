using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Entities.Processes;
using VecompSoftware.DocSuiteWeb.Model.Processes;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers
{
    public class DossierFolderModel
    {
        #region [ Constructor ]
        public DossierFolderModel()
        {

        }
        #endregion

        #region [ Proprieties ]
        public Guid UniqueId { get; set; }

        public Guid? IdProcessFascicleTemplate { get; set; }

        public string Name { get; set; }

        public DossierFolderStatus Status { get; set; }

        public string JsonMetadata { get; set; }

        public Guid? IdFascicle { get; set; }

        public Guid? IdDossier { get; set; }

        public short? IdCategory { get; set; }

        public short? IdRole { get; set; }

        public string DossierFolderPath { get; set; }

        public short DossierFolderLevel { get; set; }
        #endregion

        #region [ Navigation Proprieties ]

        public ICollection<ProcessFascicleTemplateModel> FascicleTemplates { get; set; }

        public ICollection<DossierFolderModel> DossierFolders { get; set; }

        #endregion
    }
}
