
using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Workflows
{
    public enum ActivityType : short
    {
        Undefied = -1,
        [Description("Attività automatiche")]
        AutomaticActivity = 0,
        [Description("Inserisci protocollo")]
        ProtocolCreate = 1,
        [Description("Invia protocollo via PEC")]
        PecToProtocol = 2,
        [Description("Inserisci archivio")]
        UDSCreate = 3,
        [Description("Protocolla archivio")]
        UDSToProtocol = 4,
        [Description("Invia archivio via PEC")]
        UDSToPEC = 5,
        [Description("Crea collaborazione")]
        CollaborationCreate = 6,
        [Description("Firma documenti collaborazione")]
        CollaborationSign = 7,
        [Description("Inserisci protocollo da collaborazione")]
        CollaborationToProtocol = 8,
        [Description("Presa in carico del fascicolo")]
        Assignment = 9,
        [Description("Attestazione di conformità")]
        DematerialisationStatement = 11,
        [Description("Crea securizzazione documento")]
        SecureDocumentCreate = 12,
        [Description("Creazione automatica di archivio tramite servizi")]
        BuildAchive = 13,
        [Description("Creazione di protocollo tramite servizi")]
        BuildProtocol = 14,
        [Description("Creazione automatica di PEC mail tramite servizi")]
        BuildPECMail = 15,
        [Description("Creazione automatica di email tramite servizi")]
        BuildMessages = 16,
        [Description("Fascicola UD tramite servizi")]
        DocumentUnitIntoFascicle = 17,
        [Description("Collaga UD tramite servizi")]
        DocumentUnitLinks = 18,
        [Description("Attività")]
        GenericActivity = 19,
    }
}
