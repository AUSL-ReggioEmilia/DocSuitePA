using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles
{
    public class FascicleFolderModel
    {
        public Guid UniqueId { get; set; }

        public string Name { get; set; }

        public FascicleFolderStatus Status { get; set; }

        public FascicleFolderTypology Typology { get; set; }

        public Guid? IdFascicle { get; set; }

        public short? IdCategory { get; set; }

        public bool HasDocuments { get; set; }

        public bool HasChildren { get; set; }

        public short FascicleFolderLevel { get; set; }
        public DocumentModel Document { get; set; }
        public ICollection<FascicleDocumentModel> FascicleDocuments { get; set; }
    }
}
