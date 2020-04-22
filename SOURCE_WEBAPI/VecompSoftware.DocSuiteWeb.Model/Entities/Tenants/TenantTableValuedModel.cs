using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Tenants
{
    public class TenantTableValuedModel
    {
        #region [ Constructor ]
        public TenantTableValuedModel()
        {
        }
        #endregion

        #region [ Properties ]

        public Guid IdTenant { get; set; }
        public string TenantName { get; set; }
        public string CompanyName { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string Note { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public string LastChangedUser { get; set; }
        public byte[] Timestamp { get; set; }
        #endregion

        #region [ Methods ]

        #endregion
    }
}
