using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.UDS
{
    public class UDSDocumentUnitModel
    {
        public Guid UniqueId { get; set; }
        public UDSRelationType RelationType { get; set; }
        public Guid IdUDS { get; set; }
        public Guid IdDocumentUnit { get; set; }
        public int Environment { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
    }
}
