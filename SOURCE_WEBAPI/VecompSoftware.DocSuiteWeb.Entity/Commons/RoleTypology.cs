using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{
    public enum RoleTypology : short
    {
        [Description("InternalRole")]
        InternalRole = 0,
        [Description("ExternalRole")]
        ExternalRole = 1
    }
}
