using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Monitors
{
    public class TransparentAdministrationMonitorLogModel
    {
        #region [ Constructor ]
        public TransparentAdministrationMonitorLogModel()
        {
        }
        #endregion

        #region [ Proprieties ]
        public Guid? UniqueId { get; set; }
        public DateTimeOffset? Date { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string Note { get; set; }
        public string Rating { get; set; }
        public Guid IdDocumentUnit { get; set; }
        public string DocumentUnitName { get; set; }
        public string DocumentUnitTitle { get; set; }
        public short? IdRole { get; set; }
        public string RoleName { get; set; }

        #endregion

        #region [ Methods ]

        #endregion
    }
}
