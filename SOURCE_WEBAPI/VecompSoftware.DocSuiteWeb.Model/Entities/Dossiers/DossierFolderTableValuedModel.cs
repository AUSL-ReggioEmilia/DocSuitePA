using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Entities.Processes;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers
{
    public class DossierFolderTableValuedModel
    {
        public Guid IdDossierFolder { get; set; }
        public string Name { get; set; }
        public DossierFolderStatus Status { get; set; }
        public string JsonMetadata { get; set; }
        public short DossierFolderLevel { get; set; }
        public string DossierFolderPath { get; set; }
        public Guid? Dossier_IdDossier { get; set; }
        public Guid? Fascicle_IdFascicle { get; set; }
        public short? Category_IdCategory { get; set; }
        public short? Role_IdRole { get; set; }
    }
}
