using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Model.Workflow
{
    public enum WorkflowAuthorizationType : short
    {
        [Description("Nessuna")]
        None = 0,
        [Description("Tutti gli utenti di settore")]
        AllRoleUser = 1,
        [Description("Tutte le segreterie")]
        AllSecretary = 2,
        [Description("Tutti i firmatari")]
        AllSigner = AllSecretary * 2,
        [Description("Tutti i responsabili")]
        AllManager = AllSigner * 2,
        [Description("Tutti gli settori configurati in organigramma")]
        AllOChartRoleUser = AllManager * 2,
        [Description("Tutti i responsabili configurati in organigramma")]
        AllOChartManager = AllOChartRoleUser * 2,
        [Description("Tutti i responsabili gerarchici in organigramma")]
        AllOChartHierarchyManager = AllOChartManager * 2,
        [Description("Nome utente")]
        UserName = AllOChartHierarchyManager * 2,
        [Description("Gruppo AD")]
        ADGroup = UserName * 2,
        [Description("Tags")]
        MappingTags = ADGroup * 2,
        [Description("Tutti i responsabili della dematerializzazione")]
        AllDematerialisationManager = MappingTags * 2,
        [Description("Tutti gli utenti del gruppo di sicurezza di protocollo")]
        AllProtocolSecurityUsers = AllDematerialisationManager * 2,
        [Description("Tutti gli utenti del gruppo di sicurezza delle UDS")]
        AllUDSSecurityUsers = AllProtocolSecurityUsers * 2,
        [Description("Tutti gli utenti abilitati alla gestione PEC")]
        AllPECMailBoxRoleUser = AllUDSSecurityUsers * 2,
    }
}
