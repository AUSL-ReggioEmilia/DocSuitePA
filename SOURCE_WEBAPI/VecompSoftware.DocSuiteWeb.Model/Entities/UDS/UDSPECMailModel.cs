using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.UDS
{
    public class UDSPECMailModel
    {
        public Guid UniqueId { get; set; }
        public UDSRelationType RelationType { get; set; }
        public Guid IdUDS { get; set; }
        public int IdPECMail { get; set; }
        public Guid UniqueIdPECMail { get; set; }
        public int Environment { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
    }
}
