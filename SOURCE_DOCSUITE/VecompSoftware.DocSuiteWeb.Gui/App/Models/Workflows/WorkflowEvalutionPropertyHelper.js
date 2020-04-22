define(["require", "exports", "App/Models/Workflows/WorkflowEvaluationPropertyType"], function (require, exports, WorkflowEvaluationPropertyType) {
    var WorkflowEvalutionPropertyHelper = /** @class */ (function () {
        function WorkflowEvalutionPropertyHelper() {
        }
        WorkflowEvalutionPropertyHelper._dsw_p_WorkflowStartDocument = { Name: "Richiedi documento di avvio", Type: WorkflowEvaluationPropertyType.Boolean };
        WorkflowEvalutionPropertyHelper._dsw_p_WorkflowStartMetadata = { Name: "Richiedi metadato di avvio", Type: WorkflowEvaluationPropertyType.Json };
        WorkflowEvalutionPropertyHelper._dsw_p_WorkflowStartProposer = { Name: "Proponente di avvio [0:Settore, 1:Utente]", Type: WorkflowEvaluationPropertyType.Integer };
        WorkflowEvalutionPropertyHelper._dsw_p_WorkflowStartRecipient = { Name: "Destinatario di avvio [0:Settore, 1:Utente]", Type: WorkflowEvaluationPropertyType.Integer };
        WorkflowEvalutionPropertyHelper._dsw_p_WorkflowStartInstructions = { Name: "Istruzioni di avvio", Type: WorkflowEvaluationPropertyType.String };
        WorkflowEvalutionPropertyHelper._dsw_p_WorkflowStartMotivationRequired = { Name: "Motivo di avvio obbligatorio", Type: WorkflowEvaluationPropertyType.Boolean };
        WorkflowEvalutionPropertyHelper._dsw_p_WorkflowProposerAuthorizationReadOnly = { Name: "Proponente in sola lettura", Type: WorkflowEvaluationPropertyType.Boolean };
        WorkflowEvalutionPropertyHelper._dsw_p_WorkflowRecipientAuthorizationReadOnly = { Name: "Destinatario in sola lettura", Type: WorkflowEvaluationPropertyType.Boolean };
        WorkflowEvalutionPropertyHelper._dsw_p_WorkflowEndMotivationRequired = { Name: "Commento di conclusione obbligatorio", Type: WorkflowEvaluationPropertyType.Boolean };
        WorkflowEvalutionPropertyHelper._dsw_p_ActivityAutoHandling = { Name: "Presa in carico automatica", Type: WorkflowEvaluationPropertyType.Boolean };
        WorkflowEvalutionPropertyHelper._dsw_a_Metadata_LabelMotivation = { Name: "Metadato da aggiornare", Type: WorkflowEvaluationPropertyType.String };
        WorkflowEvalutionPropertyHelper._dsw_p_Subject = { Name: "Oggetto dell'attività", Type: WorkflowEvaluationPropertyType.String };
        WorkflowEvalutionPropertyHelper._dsw_p_ProposerRole = { Name: "Settore proponente", Type: WorkflowEvaluationPropertyType.Json };
        WorkflowEvalutionPropertyHelper._dsw_p_WorkflowDefaultProposer = { Name: "Proponente di default", Type: WorkflowEvaluationPropertyType.Json };
        WorkflowEvalutionPropertyHelper._dsw_p_WorkflowDefaultRecipient = { Name: "Destinatario di default", Type: WorkflowEvaluationPropertyType.Json };
        WorkflowEvalutionPropertyHelper._dsw_p_Roles = { Name: "Settori", Type: WorkflowEvaluationPropertyType.String };
        WorkflowEvalutionPropertyHelper._dsw_p_Accounts = { Name: "Utenti", Type: WorkflowEvaluationPropertyType.String };
        WorkflowEvalutionPropertyHelper._dsw_p_WorkflowRecipientMultiple = { Name: "Selezione destinatario multiplo", Type: WorkflowEvaluationPropertyType.Boolean };
        WorkflowEvalutionPropertyHelper._dsw_p_WorkflowStartTemplateCollaborationRequired = { Name: "Abilita selezione template di collaborazione", Type: WorkflowEvaluationPropertyType.Boolean };
        WorkflowEvalutionPropertyHelper._dsw_p_WorkflowDefaultTemplateCollaboration = { Name: "Template di collaborazione di default", Type: WorkflowEvaluationPropertyType.Guid };
        WorkflowEvalutionPropertyHelper._dsw_p_TemplateCollaboration = { Name: "Seleziona remplate di collaborazione", Type: WorkflowEvaluationPropertyType.Guid };
        WorkflowEvalutionPropertyHelper._dsw_p_DocumentChainTypeSelection = { Name: "Seleziona tipologia di catena documentale", Type: WorkflowEvaluationPropertyType.Boolean };
        WorkflowEvalutionPropertyHelper._dsw_a_Generate_WordTemplate = { Name: "Genera documento di avvio", Type: WorkflowEvaluationPropertyType.Boolean };
        WorkflowEvalutionPropertyHelper._dsw_a_Generate_WordTemplateId = { Name: "Id del documenti (sezione deposito documenatale) da generare", Type: WorkflowEvaluationPropertyType.String };
        WorkflowEvalutionPropertyHelper._dsw_a_Fascicle_RemoveProposerRoles = { Name: "Rimuovi settore proponente dal fascicolo", Type: WorkflowEvaluationPropertyType.Boolean };
        WorkflowEvalutionPropertyHelper._dsw_a_Fascicle_PublicEnforcement = { Name: "Rendi fascicolo pubblico permanentemente", Type: WorkflowEvaluationPropertyType.Boolean };
        WorkflowEvalutionPropertyHelper._dsw_a_Fascicle_PublicTemporaryEnforcement = { Name: "Rendi fascicolo pubblico temporaneamente", Type: WorkflowEvaluationPropertyType.Boolean };
        WorkflowEvalutionPropertyHelper._dsw_a_Fascicle_RemoveRoles = { Name: "Rimuovi settori dal fascicolo ad attività conclusa", Type: WorkflowEvaluationPropertyType.Boolean };
        WorkflowEvalutionPropertyHelper._dsw_a_Dossier_RemoveProposerRoles = { Name: "Rimuovi settore proponente dal dossier", Type: WorkflowEvaluationPropertyType.Boolean };
        WorkflowEvalutionPropertyHelper._dsw_a_Dossier_RemoveRoles = { Name: "Rimuovi settori dal dossier ad attività conclusa", Type: WorkflowEvaluationPropertyType.Boolean };
        WorkflowEvalutionPropertyHelper._dsw_a_Parallel_Activity = { Name: "Avvio attività multipla", Type: WorkflowEvaluationPropertyType.Boolean };
        WorkflowEvalutionPropertyHelper._dsw_a_RedirectToCollaboration = { Name: "Porta nella pagina di collaborazione", Type: WorkflowEvaluationPropertyType.Boolean };
        WorkflowEvalutionPropertyHelper._dsw_a_RedirectToProtocol = { Name: "Porta nella pagina di protocollazione", Type: WorkflowEvaluationPropertyType.Boolean };
        WorkflowEvalutionPropertyHelper._dsw_v_Fascicle_DocumentRequired = { Name: "Selezione inserto di fascicolo obbligatorio", Type: WorkflowEvaluationPropertyType.Boolean };
        WorkflowEvalutionPropertyHelper._dsw_v_Fascicle_DocumentUnitRequired = { Name: "Selezione documento di fascicolo obbligatorio", Type: WorkflowEvaluationPropertyType.Boolean };
        return WorkflowEvalutionPropertyHelper;
    }());
    return WorkflowEvalutionPropertyHelper;
});
//# sourceMappingURL=WorkflowEvalutionPropertyHelper.js.map