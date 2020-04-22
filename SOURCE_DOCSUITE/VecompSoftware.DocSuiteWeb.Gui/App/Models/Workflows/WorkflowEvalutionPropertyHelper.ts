import WorkflowEvaluationPropertyType = require('App/Models/Workflows/WorkflowEvaluationPropertyType');

class WorkflowEvalutionPropertyHelper {
    static _dsw_p_WorkflowStartDocument = { Name: "Richiedi documento di avvio", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_p_WorkflowStartMetadata = { Name: "Richiedi metadato di avvio", Type: WorkflowEvaluationPropertyType.Json };
    static _dsw_p_WorkflowStartProposer = { Name: "Proponente di avvio [0:Settore, 1:Utente]", Type: WorkflowEvaluationPropertyType.Integer };
    static _dsw_p_WorkflowStartRecipient = { Name: "Destinatario di avvio [0:Settore, 1:Utente]", Type: WorkflowEvaluationPropertyType.Integer };
    static _dsw_p_WorkflowStartInstructions = { Name: "Istruzioni di avvio", Type: WorkflowEvaluationPropertyType.String };
    static _dsw_p_WorkflowStartMotivationRequired = { Name: "Motivo di avvio obbligatorio", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_p_WorkflowProposerAuthorizationReadOnly = { Name: "Proponente in sola lettura", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_p_WorkflowRecipientAuthorizationReadOnly = { Name: "Destinatario in sola lettura", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_p_WorkflowEndMotivationRequired = { Name: "Commento di conclusione obbligatorio", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_p_ActivityAutoHandling = { Name: "Presa in carico automatica", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_a_SetRecipientResponsible = { Name: "Rendi i settori destinatari responsabili", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_a_Metadata_LabelMotivation = { Name: "Metadato da aggiornare", Type: WorkflowEvaluationPropertyType.String };
    static _dsw_p_Subject = { Name: "Oggetto dell'attività", Type: WorkflowEvaluationPropertyType.String };
    static _dsw_p_ProposerRole = { Name: "Settore proponente", Type: WorkflowEvaluationPropertyType.Json };
    static _dsw_p_WorkflowDefaultProposer = { Name: "Proponente di default", Type: WorkflowEvaluationPropertyType.Json };
    static _dsw_p_WorkflowDefaultRecipient = { Name: "Destinatario di default", Type: WorkflowEvaluationPropertyType.Json };
    static _dsw_p_Roles = { Name: "Settori", Type: WorkflowEvaluationPropertyType.String };
    static _dsw_p_Accounts = { Name: "Utenti", Type: WorkflowEvaluationPropertyType.String };
    static _dsw_p_WorkflowRecipientMultiple = { Name: "Selezione destinatario multiplo", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_p_WorkflowStartTemplateCollaborationRequired = { Name: "Abilita selezione template di collaborazione", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_p_WorkflowDefaultTemplateCollaboration = { Name: "Template di collaborazione di default", Type: WorkflowEvaluationPropertyType.Guid };
    static _dsw_p_TemplateCollaboration = { Name: "Seleziona remplate di collaborazione", Type: WorkflowEvaluationPropertyType.Guid };
    static _dsw_p_DocumentChainTypeSelection = { Name: "Seleziona tipologia di catena documentale", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_p_CollaborationToManageModel = { Name: "Modello contenente le proprietà di default nella gestione di collaborazione", Type: WorkflowEvaluationPropertyType.Json };
    static _dsw_a_CollaborationSignSummaryTemplateId = { Name: "Identificativo (vedi sezione deposito documentale) del documento di riepilogo firmatari", Type: WorkflowEvaluationPropertyType.Guid };
    static _dsw_p_DocumentOriginalTypeSelection = { Name: "Abilita scelta copia conforme/originale per invio documenti", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_p_MultiTenantId = { Name: "Identificativo del tenant di destinazione", Type: WorkflowEvaluationPropertyType.Guid };
    static _dsw_p_TenantName = { Name: "Nome del tenant di destinazione", Type: WorkflowEvaluationPropertyType.String };

    static _dsw_a_Generate_WordTemplate = { Name: "Generazione dinamica del documento di fase avvio (OpenXML)", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_a_Generate_PDFTemplate = { Name: "Generazione dinamica del documento di fase avvio (Itext7)", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_a_Generate_TemplateId = { Name: "Identificativo (vedi sezione deposito documentale) del documento da generare", Type: WorkflowEvaluationPropertyType.Guid };
    static _dsw_a_Collaboration_AddChains = { Name: "Includi documenti originali dell'unità documentaria in collaborazione", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_a_Collaboration_AddProposerHierarchySigner = { Name: "Aggiungi come primo firmatario di collaborazione il proponente del workflow", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_a_Collaboration_GenerateSignSummary = { Name: "Genera allegato durante la gestione col riepilogo dei firmatari", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_a_Fascicle_RemoveProposerRoles = { Name: "Rimuovi settore proponente dal fascicolo", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_a_Fascicle_PublicEnforcement = { Name: "Rendi fascicolo pubblico permanentemente", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_a_Fascicle_PublicTemporaryEnforcement = { Name: "Rendi fascicolo pubblico temporaneamente", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_a_Fascicle_RemoveRoles = { Name: "Rimuovi settori dal fascicolo ad attività conclusa", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_a_Dossier_RemoveProposerRoles = { Name: "Rimuovi settore proponente dal dossier", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_a_Dossier_RemoveRoles = { Name: "Rimuovi settori dal dossier ad attività conclusa", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_a_Parallel_Activity = { Name: "Avvio attività multipla", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_a_RedirectToCollaboration = { Name: "Porta nella pagina di collaborazione", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_a_RedirectToProtocol = { Name: "Porta nella pagina di protocollazione", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_a_RedirectToFascicleSignDocument = { Name: "Porta nella pagina di firma inserto", Type: WorkflowEvaluationPropertyType.Boolean };

    static _dsw_v_Fascicle_DocumentRequired = { Name: "Selezione inserto di fascicolo obbligatorio", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_v_Fascicle_DocumentUnitRequired = { Name: "Selezione documento di fascicolo obbligatorio", Type: WorkflowEvaluationPropertyType.Boolean };
    static _dsw_v_Workflow_NoInstanceRequired = { Name: "Workflow non richiede instanze persistite", Type: WorkflowEvaluationPropertyType.Boolean };
}

export = WorkflowEvalutionPropertyHelper;