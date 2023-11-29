using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.JeepServiceHosts
{
    public class JeepServiceHostModel
    {
        #region [ Constructor ]
        public JeepServiceHostModel()
        {
        }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public string Hostname { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        #endregion

        #region [ Methods ]

        #endregion
    }
}
