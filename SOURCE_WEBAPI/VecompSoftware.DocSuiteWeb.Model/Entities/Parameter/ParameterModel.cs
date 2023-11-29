using System;
using VecompSoftware.DocSuiteWeb.Model.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Parameter
{
    public class ParameterModel
    {
        #region [ Constructor ]
        public ParameterModel()
        {
        }
        #endregion

        #region [ Proprieties ]
        public Guid UniqueId { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
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
        public TenantAOOModel TenantAOO { get; set; }
        #endregion

        #region [ Methods ]

        #endregion
    }
}
