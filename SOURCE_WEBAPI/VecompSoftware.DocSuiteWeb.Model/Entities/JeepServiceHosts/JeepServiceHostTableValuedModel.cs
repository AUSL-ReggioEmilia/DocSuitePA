using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.JeepServiceHosts
{
    public class JeepServiceHostTableValuedModel
    {
        #region [ Constructor ]
        public JeepServiceHostTableValuedModel()
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
        #endregion

        #region [ Methods ]

        #endregion
    }
}
