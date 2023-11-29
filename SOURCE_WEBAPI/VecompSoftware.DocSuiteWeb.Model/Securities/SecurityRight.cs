using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Securities
{
    public class SecurityRight
    {
        public DSWEnvironmentType Environment{ get; set; }
        public bool? HasInsertable { get; set; }
        public bool? HasViewable { get; set; }
        public bool? HasSignerRole { get; set; }
        public bool? HasSecretaryRole { get; set; }
        public bool? HasFascicleResponsibleRole { get; set; }
        public bool? HasFascicleSecretaryRole { get; set; }
        public bool? HasManagerRole { get; set; }
        public bool? HasPECSendableRight { get; set; }
    }
}
