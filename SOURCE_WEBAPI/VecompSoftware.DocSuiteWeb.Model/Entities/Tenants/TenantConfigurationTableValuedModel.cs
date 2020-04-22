using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Tenants
{
    public class TenantConfigurationTableValuedModel
    {
        #region [ Constructor ]
        public TenantConfigurationTableValuedModel()
        {
        }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public TenantConfigurationType ConfigurationType { get; set; }
        public string JsonValue { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string Note { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }

        #endregion

        #region [ Methods ]

        #endregion
    }
}
