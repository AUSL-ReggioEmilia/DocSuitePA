using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.UDS
{
    public class UDSMessageModel
    {
        public Guid UniqueId { get; set; }
        public UDSRelationType RelationType { get; set; }
        public Guid IdUDS { get; set; }
        public int IdMessage { get; set; }
        public Guid UniqueIdMessage { get; set; }
        public int Environment { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
    }
}
