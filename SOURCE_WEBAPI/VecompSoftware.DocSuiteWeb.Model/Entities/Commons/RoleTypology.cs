using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Commons
{
    public enum RoleTypology : short
    {
        [Description("InternalRole")]
        InternalRole = 0,
        [Description("ExternalRole")]
        ExternalRole = 1
    }
}
