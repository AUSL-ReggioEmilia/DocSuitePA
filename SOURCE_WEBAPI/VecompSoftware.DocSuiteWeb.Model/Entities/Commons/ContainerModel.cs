
using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Commons
{
    public class ContainerModel
    {
        #region [ Constructors ]

        public ContainerModel()
        {
        }

        public ContainerModel(int? id)
        {
            IdContainer = id;
        }

        #endregion

        #region [ Properties ]

        public int? IdContainer { get; set; }

        public string Name { get; set; }

        public string Note { get; set; }

        public Guid? UniqueId { get; set; }

        public LocationModel DocumentSeriesAnnexedLocation { get; set; }

        public LocationModel DocumentSeriesLocation { get; set; }

        public LocationModel DocumentSeriesUnpublishedAnnexedLocation { get; set; }

        public LocationModel ProtLocation { get; set; }

        public LocationModel ProtocolAttachmentLocation { get; set; }

        public LocationModel ReslLocation { get; set; }

        public LocationModel DocmLocation { get; set; }

        public LocationModel UDSLocation { get; set; }

        #endregion

    }
}
