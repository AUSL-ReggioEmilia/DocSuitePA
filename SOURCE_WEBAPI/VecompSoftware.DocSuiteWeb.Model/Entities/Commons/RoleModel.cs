
using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Commons
{
    public class RoleModel
    {
        #region [ Constructors ]

        public RoleModel()
        {
            Children = new HashSet<RoleModel>();
        }

        public RoleModel(short? id)
        {
            IdRole = id;
        }

        #endregion

        #region [ Properties ]
        public short? IdRole { get; set; }
        public Guid? UniqueId { get; set; }
        public Guid? TenantId { get; set; }
        public string Name { get; set; }
        public string FullIncrementalPath { get; set; }
        public short? IdRoleTenant { get; set; }
        public byte? IsActive { get; set; }
        public short? IdRoleFather { get; set; }
        public DateTime? ActiveFrom { get; set; }
        public string ServiceCode { get; set; }

        public string RoleLabel { get; set; }
        public AuthorizationRoleType AuthorizationType { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public ICollection<RoleModel> Children { get; set; }
        #endregion
    }
}
