using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Monitors
{
    public class TransparentAdministrationMonitorLogTableValuedModel
    {
        #region [ Constructor ]
        public TransparentAdministrationMonitorLogTableValuedModel()
        {
        }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public DateTimeOffset Date { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string Note { get; set; }
        public string Rating { get; set; }
        #endregion

        #region [ Methods ]

        #endregion
    }
}
