using System;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles
{
    public class FascicleFolderTableValuedModel
    {
        public Guid IdFascicleFolder { get; set; }

        public string Name { get; set; }

        public FascicleFolderStatus Status { get; set; }

        public FascicleFolderTypology Typology { get; set; }

        public Guid? Fascicle_IdFascicle { get; set; }

        public short? Category_IdCategory { get; set; }

        public Guid? Document_IdFascicleDocument { get; set; }

        public Guid? Document_IdArchiveChain { get; set; }

        public ChainType? Document_ChainType { get; set; }

        public bool HasDocuments { get; set; }

        public bool HasChildren { get; set; }

        public short FascicleFolderLevel { get; set; }
    }
}
