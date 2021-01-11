using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.UDS
{
    public class UDSContactModel
    {
        public Guid UniqueId { get; set; }
        public UDSRelationType RelationType { get; set; }
        public Guid IdUDS { get; set; }
        public int IdContact { get; set; }
        public string ContactManual { get; set; }
        public string ContactLabel { get; set; }
        public int Environment { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
    }
}
