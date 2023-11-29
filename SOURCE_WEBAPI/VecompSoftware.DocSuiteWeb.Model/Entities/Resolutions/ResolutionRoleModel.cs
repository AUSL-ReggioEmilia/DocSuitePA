using System;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions
{
    public class ResolutionRoleModel
    {
        #region [ Constructor ]
        public ResolutionRoleModel()
        {

        }

        public ResolutionRoleModel(Guid uniqueId)
        {
            UniqueId = uniqueId;
        }
        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }

        public int? IdResolutionRoleType { get; set; }

        public RoleModel Role { get; set; }
        public ResolutionModel Resolution { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string RegistrationUser { get; set; }

        #endregion
    }
}
