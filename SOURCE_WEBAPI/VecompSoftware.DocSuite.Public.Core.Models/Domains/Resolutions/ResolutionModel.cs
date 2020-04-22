using System;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons;

namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.Resolutions
{
    [DocumentUnit(DocumentUnitType.Resolution)]
    public class ResolutionModel : DocumentUnitModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]
        public ResolutionModel(Guid id, short year, int number, string subject,
            CategoryModel category, ContainerModel container, string location)
            : base(id, year, number, subject, category, container, location)
        { }
        #endregion

        #region [ Properties ]

        public string ServiceNumber { get; set; }

        public string InclusiveNumber { get; set; }

        public DateTime? AdoptionDate { get; set; }

        public DateTime? PublishingDate { get; set; }

        public DateTime? EffectivenessDate { get; set; }

        public ResolutionStatusType Status { get; set; }

        public ResolutionType IdType { get; set; }

        public string Proposer { get; set; }

        #endregion

    }
}
