using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Entity.Dossiers
{
    public enum DossierType : short
    {
        [Description("Persona fisica o giuridica")]
        Person = 0,

        [Description("Oggetto fisico")]
        PhysicalObject = 1,

        [Description("Procedimenti")]
        Procedure = 2,

        [Description("Serie di archivistica")]
        Process = 3
    }
}
