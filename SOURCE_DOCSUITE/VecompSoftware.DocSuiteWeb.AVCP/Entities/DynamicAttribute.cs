using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.AVCP.Entities
{
    public enum DynamicAttribute
    {
        [Description("Aggiudicatario")]
        Aggiudicatario,
        [Description("Lotti")]
        Lotti,
        [Description("Liquidato")]
        Liquidato,
        [Description("DitteInvitate")]
        DitteInvitate,
        [Description("DittePartecipanti")]
        DittePartecipanti,
        [Description("ProceduraAggiudicazione")]
        ProceduraAggiudicazione,
        [Description("ImportoComplessivo")]
        ImportoComplessivo,
        [Description("StrutturaProponente")]
        StrutturaProponente,
        [Description("DurataAl")]
        DurataAl,
        [Description("DurataDal")]
        DurataDal
    }
}
