using System;
using VecompSoftware.DocSuiteWeb.Model.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles
{
    public class FascicleRoleModel
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public FascicleRoleModel()
        {

        }
        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }
        public bool IsMaster { get; set; }
        #endregion

        #region[ Navigation Properties ]
        public AuthorizationRoleType AuthorizationRoleType { get; set; }
        public RoleModel Role { get; set; }
        #endregion
    }
}
