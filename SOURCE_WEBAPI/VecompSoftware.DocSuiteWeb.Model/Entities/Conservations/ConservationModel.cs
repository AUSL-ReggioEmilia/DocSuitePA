using System;
using VecompSoftware.DocSuiteWeb.Model.Conservations;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Conservations
{
    public class ConservationModel
    {
        #region [ Constructor ]
        #endregion

        #region [ Properties ]
        public string EntityType { get; set; }
        public ConservationStatus Status { get; set; }
        public string Message { get; set; }
        public ConservationType Type { get; set; }
        public DateTimeOffset? SendDate { get; set; }
        public string Uri { get; set; }
        public Guid UniqueId { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public string LastChangedUser { get; set; }
        #endregion
    }
}
