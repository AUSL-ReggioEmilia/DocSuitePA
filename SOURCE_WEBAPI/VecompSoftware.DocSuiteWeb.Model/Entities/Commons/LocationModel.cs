
using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Commons
{
    public class LocationModel
    {
        #region [ Constructors ]

        public LocationModel()
        {
        }

        public LocationModel(short? id)
        {
            IdLocation = id;
        }

        #endregion

        #region [ Properties ]

        public short? IdLocation { get; set; }

        public Guid? UniqueId { get; set; }

        public string Name { get; set; }

        public string ProtocolArchive { get; set; }

        public string DossierArchive { get; set; }

        public string ResolutionArchive { get; set; }

        public string ConservationArchive { get; set; }


        #endregion
    }
}
