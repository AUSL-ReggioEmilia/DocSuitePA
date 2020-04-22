using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Entity.UDS
{
    public enum UDSUserStatus : short
    {
        [Description("Non Attivo")]
        Inactive = 0,

        [Description("Attivo")]
        Active = 1
    }
}
