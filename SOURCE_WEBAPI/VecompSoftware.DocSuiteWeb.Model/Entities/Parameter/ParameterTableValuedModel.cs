using System;
using VecompSoftware.DocSuiteWeb.Model.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Parameter
{
    public class ParameterTableValuedModel : ITenantAOOTableValuedModel
    {
        #region [ Constructor ]
        public ParameterTableValuedModel()
        {
        }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public int Incremental { get; set; }
        public short LastUsedYear { get; set; }
        public int LastUsedNumber { get; set; }
        public bool Locked { get; set; }
        public short LastUsedidCategory { get; set; }
        public short LastUsedidContainer { get; set; }
        public short LastUsedidRole { get; set; }
        public short LastUsedIdRoleUser { get; set; }
        public int? LastUsedidResolution { get; set; }
        public short LastUsedResolutionYear { get; set; }
        public short LastUsedResolutionNumber { get; set; }
        public short LastUsedBillNumber { get; set; }

        #region [ TenantAOO ]
        public Guid? TenantAOO_IdTenantAOO { get; set; }
        public string TenantAOO_Name { get; set; }
        #endregion

        #endregion

        #region [ Methods ]

        #endregion
    }
}
