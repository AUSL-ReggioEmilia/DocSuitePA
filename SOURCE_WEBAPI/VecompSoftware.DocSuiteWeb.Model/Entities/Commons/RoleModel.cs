
using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Commons
{
    public class RoleModel
    {
        #region [ Constructors ]

        public RoleModel()
        {
            Children = new HashSet<RoleModel>();
        }

        public RoleModel(short? id) : this()
        {
            IdRole = id;
            EntityShortId = id;
        }

        #endregion

        #region [ Properties ]
        public short? EntityShortId { get; set; }
        public short? IdRole { get; set; }
        public Guid? UniqueId { get; set; }
        public string Name { get; set; }
        public string FullIncrementalPath { get; set; }
        public bool IsActive { get; set; }
        public short? IdRoleFather { get; set; }
        public string ServiceCode { get; set; }
        public RoleTypology RoleTypology { get; set; }
        public bool Collapsed { get; set; }
        public string EMailAddress { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }

        public string RoleLabel { get; set; }
        public AuthorizationRoleType AuthorizationType { get; set; }
        public Guid IdTenantAOO { get; set; }
        public bool IsRealResult { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public ICollection<RoleModel> Children { get; set; }
        #endregion
    }
}
