using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Commons
{
    public class RoleTableValuedModel
    {

        #region [ Constructors ]

        public RoleTableValuedModel()
        {
        }

        #endregion

        #region [ Properties ]

        public short IdRole { get; set; }

        public Guid? UniqueIdFather { get; set; }

        public Guid UniqueId { get; set; }

        public string Name { get; set; }

        public string DistributionType { get; set; }

        public string Type { get; set; }

        public bool IsAuthorized { get; set; }

        #endregion
    }
}
