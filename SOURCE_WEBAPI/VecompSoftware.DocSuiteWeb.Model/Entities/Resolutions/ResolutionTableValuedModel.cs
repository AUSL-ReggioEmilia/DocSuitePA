using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions
{
    public class ResolutionTableValuedModel
    {
        #region [ Constructor ]
        public ResolutionTableValuedModel()
        {
            UniqueId = Guid.NewGuid();
        }
        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }

        public int EntityId { get; set; }

        public short Year { get; set; }

        public int Number { get; set; }

        public string Object { get; set; }

        public string ServiceNumber { get; set; }

        public string InclusiveNumber { get; set; }

        public DateTime? AdoptionDate { get; set; }

        public DateTime? PublishingDate { get; set; }

        public DateTime? EffectivenessDate { get; set; }

        public short Status { get; set; }

        public byte IdType { get; set; }

        public string ProposerCode { get; set; }

        public string ProposerDescription { get; set; }


        #endregion

    }
}
