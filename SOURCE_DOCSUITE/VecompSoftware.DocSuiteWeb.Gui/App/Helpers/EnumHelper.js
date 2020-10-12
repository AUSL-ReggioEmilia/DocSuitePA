define(["require", "exports", "App/Models/Dossiers/DossierLogType", "App/Models/UDS/UDSLogType", "App/Models/Workflows/WorkflowStatus", "App/Models/UDS/UDSTypologyStatus", "App/Models/Commons/LocationTypeEnum", "App/Models/Workflows/WorkflowInstanceLogType", "App/Models/PECMails/InvoiceTypeEnum", "App/Models/PECMails/InvoiceStatusEnum", "App/Models/PECMails/PECMailDirection", "App/Models/Workflows/ActivityType", "App/Models/Tenants/TenantConfigurationTypeEnum", "App/Models/Tenants/TenantWorkflowRepositoryTypeEnum", "App/Models/Workflows/WorkflowPropertyType", "App/Models/Workflows/ArgumentType", "App/Models/Fascicles/FascicleType", "App/Models/Fascicles/FascicleLogType", "App/Models/Workflows/ActivityAction", "App/Models/SignDocuments/ProviderSignType", "App/Models/DocumentUnits/ChainType", "App/Models/Workflows/ActivityArea", "App/Models/Commons/AUSSubjectType", "App/Models/Workflows/WorkflowAuthorizationType", "App/Models/Dossiers/DossierType", "App/Models/Dossiers/DossierStatus", "App/Models/Commons/WorkflowValidationRulesType"], function (require, exports, DossierLogType, UDSLogType, WorkflowStatus, UDSTypologyStatus, LocationTypeEnum, WorkflowInstanceLogType, InvoiceTypeEnum, InvoiceStatusEnum, PECMailDirection, ActivityType, TenantConfigurationTypeEnum, TenantWorkflowRepositoryTypeEnum, WorkflowPropertyType, ArgumentType, FascicleType, FascicleLogType, ActivityAction, ProviderSignType, ChainType, ActivityArea, AUSSubjectType, WorkflowAuthorizationType, DossierType, DossierStatus, WorkflowValidationRulesType) {
    var EnumHelper = /** @class */ (function () {
        function EnumHelper() {
        }
        EnumHelper.prototype.getLogTypeDescription = function (type) {
            switch (DossierLogType[type.toString()]) {
                case DossierLogType.Insert: {
                    return "Inserimento dossier";
                }
                case DossierLogType.Modify: {
                    return "Modifica dossier";
                }
                case DossierLogType.View: {
                    return "Visualizzazione dossier";
                }
                case DossierLogType.Authorize: {
                    return "Autorizzazione dossier";
                }
                case DossierLogType.Delete: {
                    return "Eliminazione dossier";
                }
                case DossierLogType.Close: {
                    return "Chiusura dossier";
                }
                case DossierLogType.Workflow: {
                    return "Flusso di lavoro";
                }
                case DossierLogType.FolderInsert: {
                    return "Creazione cartella";
                }
                case DossierLogType.FolderModify: {
                    return "Modifica cartella";
                }
                case DossierLogType.FolderAuthorize: {
                    return "Autorizzazione cartella";
                }
                case DossierLogType.FolderFascicleRemove: {
                    return "Rimozione del fascicolo dalla cartella";
                }
                case DossierLogType.FolderClose: {
                    return "Chiusura cartella";
                }
                case DossierLogType.FolderHystory: {
                    return "Storicizzazione della cartella";
                }
                case DossierLogType.FolderResponsibleChange: {
                    return "Presa in carico temporanea della cartella";
                }
                case DossierLogType.ResponsibleChange: {
                    return "Presa in carico temporanea del dossier";
                }
                case DossierLogType.FascicleInsert: {
                    return "Inserimento fascicolo nel dossier";
                }
                case DossierLogType.FascicleView: {
                    return "Visualizzazione fascicolo";
                }
                case DossierLogType.DocumentInsert: {
                    return "Inserimento inserto in dossier";
                }
                case DossierLogType.DocumentView: {
                    return "Visualizzazione documenti";
                }
                case DossierLogType.DocumentDelete: {
                    return "Eliminazione inserto da dossier";
                }
                case DossierLogType.Error: {
                    return "Errore generico";
                }
                default: {
                    return "";
                }
            }
        };
        EnumHelper.prototype.getFascicleLogTypeDescription = function (type) {
            switch (FascicleLogType[type.toString()]) {
                case FascicleLogType.Insert: {
                    return "Inserimento fascicolo";
                }
                case FascicleLogType.Modify: {
                    return "Modifica fascicolo";
                }
                case FascicleLogType.View: {
                    return "Visualizzazione fascicolo";
                }
                case FascicleLogType.Delete: {
                    return "Eliminazione fascicolo";
                }
                case FascicleLogType.Close: {
                    return "Chiusura fascicolo";
                }
                case FascicleLogType.UDInsert: {
                    return "Inserimento unità documentaria nel fascicolo";
                }
                case FascicleLogType.UDReferenceInsert: {
                    return "Inserimento unità documentaria nel fascicolo per riferimento";
                }
                case FascicleLogType.DocumentView: {
                    return "Visualizzazione dei documenti";
                }
                case FascicleLogType.UDDelete: {
                    return "Eliminazione unità documentaria del fascicolo";
                }
                case FascicleLogType.Error: {
                    return "Errore";
                }
                case FascicleLogType.DocumentInsert: {
                    return "Inserimento inserto nel fascicolo";
                }
                case FascicleLogType.DocumentDelete: {
                    return "Eliminazione inserto dal fascicolo";
                }
                case FascicleLogType.Workflow: {
                    return "Workflow";
                }
                case FascicleLogType.Authorize: {
                    return "Autorizzazione fascicolo";
                }
                case FascicleLogType.FolderInsert: {
                    return "Inserimento cartella";
                }
                case FascicleLogType.FolderUpdate: {
                    return "Modifica cartella";
                }
                case FascicleLogType.FolderDelete: {
                    return "Eliminazione cartella";
                }
                default: {
                    return "";
                }
            }
        };
        EnumHelper.prototype.getWorkflowStatusDescription = function (type) {
            switch (WorkflowStatus[type.toString()]) {
                case WorkflowStatus.Todo: {
                    return "Attiva";
                }
                case WorkflowStatus.Progress: {
                    return "In lavorazione";
                }
                case WorkflowStatus.Done: {
                    return "Completata";
                }
                case WorkflowStatus.Error: {
                    return "In errore";
                }
                case WorkflowStatus.Suspended: {
                    return "Sospesa";
                }
                default: {
                    return "";
                }
            }
        };
        EnumHelper.prototype.getActivityTypeDescription = function (type) {
            switch (ActivityType[type.toString()]) {
                case ActivityType.AutomaticActivity: {
                    return "Attività automatica";
                }
                case ActivityType.CollaborationCreate: {
                    return "Crea collaborazione";
                }
                case ActivityType.CollaborationSign: {
                    return "Firma documenti collaborazione";
                }
                case ActivityType.CollaborationToProtocol: {
                    return "Protocolla da collaborazione";
                }
                case ActivityType.DematerialisationStatement: {
                    return "Attestazione di conformità";
                }
                case ActivityType.Assignment: {
                    return "Presa in carico";
                }
                case ActivityType.PecToProtocol: {
                    return "Protocolla da PEC";
                }
                case ActivityType.ProtocolCreate: {
                    return "Crea protocollo";
                }
                case ActivityType.UDSCreate: {
                    return "Inserisci archivio";
                }
                case ActivityType.UDSToPEC: {
                    return "Invia PEC da archivio";
                }
                case ActivityType.UDSToProtocol: {
                    return "Protocolla da archivio";
                }
                case ActivityType.SecureDocumentCreate: {
                    return "Crea documento con glifo sicuro";
                }
                case ActivityType.BuildAchive: {
                    return "Creazione automatica di archivio tramite servizi";
                }
                case ActivityType.BuildProtocol: {
                    return "Creazione automatica di protocollo tramite servizi";
                }
                case ActivityType.BuildPECMail: {
                    return "Creazione automatica di PEC mail tramite servizi";
                }
                case ActivityType.BuildMessages: {
                    return "Creazione automatica di email tramite servizi";
                }
                case ActivityType.DocumentUnitIntoFascicle: {
                    return "Fascicola documento";
                }
                case ActivityType.DocumentUnitLinks: {
                    return "Collaga documenti";
                }
                case ActivityType.GenericActivity: {
                    return "Attività";
                }
                default: {
                    return "";
                }
            }
        };
        EnumHelper.prototype.getUDSTypologyStatusDescription = function (type) {
            switch (UDSTypologyStatus[type.toString()]) {
                case UDSTypologyStatus.Active: {
                    return "Attiva";
                }
                case UDSTypologyStatus.Inactive: {
                    return "Non attiva";
                }
                case UDSTypologyStatus.Invalid: {
                    return "Non valida";
                }
                default: {
                    return "";
                }
            }
        };
        EnumHelper.prototype.getUDSLogType = function (type) {
            switch (UDSLogType[type.toString()]) {
                case UDSLogType.Insert: {
                    return "Inserimento archivio";
                }
                case UDSLogType.Modify: {
                    return "Modifica archivio";
                }
                case UDSLogType.AuthorizationModify: {
                    return "Modifica autorizzazione archivio ";
                }
                case UDSLogType.DocumentModify: {
                    return "Modifica documento archivio";
                }
                case UDSLogType.ObjectModify: {
                    return "Modifica oggetto archivio";
                }
                case UDSLogType.Delete: {
                    return "Cancellazione archivio";
                }
                case UDSLogType.DocumentView: {
                    return "Visualizzazione documento archivio";
                }
                case UDSLogType.SummaryView: {
                    return "Visualizzazione Sommario archivio";
                }
                case UDSLogType.AuthorizationInsert: {
                    return "Inserimento autorizzazione archivio";
                }
                case UDSLogType.AuthorizationDelete: {
                    return "Rimozione autorizzazione archivio";
                }
                default: {
                    return "";
                }
            }
        };
        EnumHelper.prototype.getInvoiceTypeDescription = function (invoiceType, pecInvoiceDirection) {
            switch (InvoiceTypeEnum[invoiceType.toString()]) {
                case InvoiceTypeEnum.None: {
                    return "Nessuna";
                }
                case InvoiceTypeEnum.InvoicePA: {
                    return pecInvoiceDirection === PECMailDirection.Outgoing ? "Fattura PA attiva" : "Fattura PA passiva";
                }
                case InvoiceTypeEnum.InvoicePR: {
                    return pecInvoiceDirection === PECMailDirection.Outgoing ? "Fattura tra privati attiva" : "Fattura tra privati passiva";
                }
                default: {
                    return "";
                }
            }
        };
        EnumHelper.prototype.getInvoiceStatusDescription = function (invoiceStatus) {
            switch (InvoiceStatusEnum[invoiceStatus.toString()]) {
                case InvoiceStatusEnum.None: {
                    return "Flusso di lavoro non avviato";
                }
                case InvoiceStatusEnum.PAInvoiceSent: {
                    return "Inviato";
                }
                case InvoiceStatusEnum.PAInvoiceNotified: {
                    return "Fattura PA notificata";
                }
                case InvoiceStatusEnum.PAInvoiceAccepted: {
                    return "Consegnata";
                }
                case InvoiceStatusEnum.PAInvoiceSdiRefused: {
                    return "Scartata dallo SDI";
                }
                case InvoiceStatusEnum.PAInvoiceRefused: {
                    return "Rifiutata";
                }
                case InvoiceStatusEnum.InvoiceWorkflowStarted: {
                    return "Flusso di lavoro avviato";
                }
                case InvoiceStatusEnum.InvoiceWorkflowCompleted: {
                    return "Flusso di lavoro concluso";
                }
                case InvoiceStatusEnum.InvoiceWorkflowError: {
                    return "Flusso di lavoro in errore";
                }
                case InvoiceStatusEnum.InvoiceSignRequired: {
                    return "Fatture da firmare";
                }
                case InvoiceStatusEnum.InvoiceLookingMetadata: {
                    return "In attesa registrazione contabile";
                }
                case InvoiceStatusEnum.InvoiceAccounted: {
                    return "Contabilizzata";
                }
                default: {
                    return "";
                }
            }
        };
        EnumHelper.prototype.getChainType = function (val) {
            var valNum = parseInt(val.toString());
            if (isNaN(valNum)) {
                return ChainType.Miscellanea;
            }
            switch (valNum) {
                case 0:
                    return ChainType.UnitContainer;
                case 1:
                    return ChainType.MainChain;
                case 2:
                    return ChainType.AttachmentsChain;
                case 4:
                    return ChainType.AnnexedChain;
                case 8:
                    return ChainType.UnpublishedAnnexedChain;
                case 16:
                    return ChainType.ProposalChain;
                case 32:
                    return ChainType.ControllerChain;
                case 64:
                    return ChainType.AssumedProposalChain;
                case 128:
                    return ChainType.FrontespizioChain;
                case 256:
                    return ChainType.PrivacyAttachmentChain;
                case 512:
                    return ChainType.FrontalinoRitiroChain;
                case 1024:
                    return ChainType.PrivacyPublicationDocumentChain;
                case 2048:
                    return ChainType.MainOmissisChain;
                case 4096:
                    return ChainType.AttachmentOmissisChain;
                case 8192:
                    return ChainType.UltimaPaginaChain;
                case 16384:
                    return ChainType.SupervisoryBoardChain;
                case 32768:
                    return ChainType.DematerialisationChain;
                default:
            }
            return ChainType.Miscellanea;
        };
        EnumHelper.prototype.getLocationTypeDescription = function (loc) {
            var locNum = parseInt(loc.toString());
            if (!isNaN(locNum)) {
                locNum = LocationTypeEnum[loc.toString()];
            }
            switch (LocationTypeEnum[locNum.toString()]) {
                case LocationTypeEnum.DeskLocation: {
                    return "DeskLocation";
                }
                case LocationTypeEnum.DocmLocation: {
                    return "DocmLocation";
                }
                case LocationTypeEnum.DocumentSeriesLocation: {
                    return "DocumentSeriesLocation";
                }
                case LocationTypeEnum.ProtLocation: {
                    return "ProtLocation";
                }
                case LocationTypeEnum.ReslLocation: {
                    return "ReslLocation";
                }
                case LocationTypeEnum.UDSLocation: {
                    return "UDSLocation";
                }
                default: {
                    return "";
                }
            }
        };
        EnumHelper.prototype.getWorkflowInstanceLogDescription = function (instance) {
            switch (WorkflowInstanceLogType[instance.toString()]) {
                case WorkflowInstanceLogType.DocumentApproved: {
                    return "Documento approvato";
                }
                case WorkflowInstanceLogType.DocumentRefused: {
                    return "Documento rifiutato";
                }
                case WorkflowInstanceLogType.DocumentRegistered: {
                    return "Documento protocollato";
                }
                case WorkflowInstanceLogType.DocumentApproved: {
                    return "Documento in approvazione";
                }
                case WorkflowInstanceLogType.UDSCreated: {
                    return "Archivio creato";
                }
                case WorkflowInstanceLogType.UDSRegistered: {
                    return "Archivio protocollato";
                }
                case WorkflowInstanceLogType.PECSended: {
                    return "PEC inviata";
                }
                case WorkflowInstanceLogType.MailSended: {
                    return "Mail inviata";
                }
                case WorkflowInstanceLogType.Information: {
                    return "Informazione";
                }
                case WorkflowInstanceLogType.WFStarted: {
                    return "Workflow avviato";
                }
                case WorkflowInstanceLogType.WFRoleAssigned: {
                    return "Workflow avviato";
                }
                case WorkflowInstanceLogType.WFTakeCharge: {
                    return "Presa in carico workflow";
                }
                case WorkflowInstanceLogType.WFRelease: {
                    return "Workflow rilasciato";
                }
                case WorkflowInstanceLogType.WFRefused: {
                    return "Workflow rifiutato";
                }
                case WorkflowInstanceLogType.WFCompleted: {
                    return "Workflow completato";
                }
                default: {
                    return "";
                }
            }
        };
        EnumHelper.prototype.getTenantConfigurationTypeDescription = function (tenantConfigurationType) {
            switch (TenantConfigurationTypeEnum[tenantConfigurationType.toString()]) {
                case TenantConfigurationTypeEnum.Tenant: {
                    return "Azienda";
                }
                default: {
                    return "";
                }
            }
        };
        EnumHelper.prototype.getTenantWorkflowRepositoryTypeDescription = function (tenantWorkflowRepositoryType) {
            switch (TenantWorkflowRepositoryTypeEnum[tenantWorkflowRepositoryType.toString()]) {
                case TenantWorkflowRepositoryTypeEnum.WorkflowConfiguration: {
                    return "Configurazione flusso di lavoro";
                }
                default: {
                    return "";
                }
            }
        };
        EnumHelper.prototype.getWorkflowStartupDescription = function (workflowStartup) {
            switch (workflowStartup) {
                case ArgumentType.PropertyBoolean:
                    return WorkflowPropertyType.PropertyBoolean;
                case ArgumentType.PropertyInt:
                    return WorkflowPropertyType.PropertyInt;
                case ArgumentType.PropertyString:
                    return WorkflowPropertyType.PropertyString;
                case ArgumentType.Json:
                    return WorkflowPropertyType.Json;
                case ArgumentType.PropertyDate:
                    return WorkflowPropertyType.PropertyDate;
                case ArgumentType.PropertyDouble:
                    return WorkflowPropertyType.PropertyDouble;
                case ArgumentType.PropertyGuid:
                    return WorkflowPropertyType.PropertyGuid;
                case ArgumentType.RelationGuid:
                    return WorkflowPropertyType.RelationGuid;
                case ArgumentType.RelationInt:
                    return WorkflowPropertyType.RelationInt;
            }
        };
        EnumHelper.prototype.getFascicleTypeDescription = function (type) {
            switch (type) {
                case FascicleType.Activity:
                    return "Attività";
                case FascicleType.Period:
                    return "Periodico";
                case FascicleType.Procedure:
                    return "Procedimento";
                case FascicleType.SubFascicle:
                    return "Sottofascicolo";
                default:
                    return "";
            }
        };
        EnumHelper.prototype.getFascicleType = function (description) {
            switch (description) {
                case "Attività":
                    return FascicleType.Activity;
                case "Activity":
                    return FascicleType.Activity;
                case "Periodico":
                    return FascicleType.Period;
                case "Period":
                    return FascicleType.Period;
                case "Procedimento":
                    return FascicleType.Procedure;
                case "Procedure":
                    return FascicleType.Procedure;
                case "Sottofascicolo":
                    return FascicleType.SubFascicle;
                case "SubFascicle":
                    return FascicleType.SubFascicle;
            }
        };
        EnumHelper.prototype.getWorkflowActivityActionDescription = function (type) {
            switch (type) {
                case ActivityAction.Create:
                    return "Create";
                case ActivityAction.ToProtocol:
                    return "ToProtocol";
                case ActivityAction.ToPEC:
                    return "ToPEC";
                case ActivityAction.ToCollaboration:
                    return "ToCollaboration";
                case ActivityAction.ToDesk:
                    return "ToDesk";
                case ActivityAction.ToResolution:
                    return "ToResolution";
                case ActivityAction.ToSign:
                    return "ToSign";
                case ActivityAction.ToAssignment:
                    return "ToAssignment";
                case ActivityAction.ToSecure:
                    return "ToSecure";
                case ActivityAction.ToFascicle:
                    return "ToFascicle";
                case ActivityAction.ToDocumentUnit:
                    return "ToDocumentUnit";
                case ActivityAction.ToArchive:
                    return "ToArchive";
                case ActivityAction.ToMessage:
                    return "ToMessage";
                case ActivityAction.CancelProtocol:
                    return "CancelProtocol";
                case ActivityAction.CancelArchive:
                    return "CancelArchive";
                case ActivityAction.CancelDocumentUnit:
                    return "CancelDocumentUnit";
                case ActivityAction.ToApprove:
                    return "ToApprove";
                case ActivityAction.ToShare:
                    return "ToShare";
                case ActivityAction.UpdateArchive:
                    return "UpdateArchive";
                case ActivityAction.UpdateFascicle:
                    return "UpdateFascicle";
                case ActivityAction.ToIntegration:
                    return "ToIntegration";
                case ActivityAction.GenerateReport:
                    return "GenerateReport";
                case ActivityAction.CopyFascicleContents:
                    return "CopyFascicleContents";
                default: {
                    return "";
                }
            }
        };
        EnumHelper.prototype.getActivityAreaDescription = function (type) {
            switch (ActivityArea[type.toString()]) {
                case ActivityArea.Build: {
                    return "Build";
                }
                case ActivityArea.Collaboration: {
                    return "Collaboration";
                }
                case ActivityArea.Desk: {
                    return "Desk";
                }
                case ActivityArea.Dossier: {
                    return "Dossier";
                }
                case ActivityArea.Fascicle: {
                    return "Fascicle";
                }
                case ActivityArea.Link: {
                    return "Link";
                }
                case ActivityArea.Message: {
                    return "Message";
                }
                case ActivityArea.PEC: {
                    return "PEC";
                }
                case ActivityArea.Protocol: {
                    return "Protocol";
                }
                case ActivityArea.Resolution: {
                    return "Resolution";
                }
                case ActivityArea.UDS: {
                    return "UDS";
                }
                default: {
                    return "";
                }
            }
        };
        EnumHelper.prototype.getWorkflowAuthorizationType = function (type) {
            switch (type) {
                case WorkflowAuthorizationType.AllRoleUser:
                    return "AllRoleUser";
                case WorkflowAuthorizationType.AllSecretary:
                    return "AllSecretary";
                case WorkflowAuthorizationType.AllSigner:
                    return "AllSigner";
                case WorkflowAuthorizationType.AllManager:
                    return "AllManager";
                case WorkflowAuthorizationType.AllOChartRoleUser:
                    return "AllOChartRoleUser";
                case WorkflowAuthorizationType.AllOChartManager:
                    return "AllOChartManager";
                case WorkflowAuthorizationType.AllOChartHierarchyManager:
                    return "AllOChartHierarchyManager";
                case WorkflowAuthorizationType.UserName:
                    return "UserName";
                case WorkflowAuthorizationType.ADGroup:
                    return "ADGroup";
                case WorkflowAuthorizationType.MappingTags:
                    return "MappingTags";
                case WorkflowAuthorizationType.AllDematerialisationManager:
                    return "AllDematerialisationManager";
                default:
                    return "";
            }
        };
        EnumHelper.prototype.getRemoteSignDescription = function (type) {
            switch (type) {
                case ProviderSignType.ArubaAutomatic:
                    return "ArubaAutomatic";
                case ProviderSignType.ArubaRemote:
                    return "ArubaRemote";
                case ProviderSignType.InfocertAutomatic:
                    return "InfocertAutomatic";
                case ProviderSignType.InfocertRemote:
                    return "InfocertRemote";
                case ProviderSignType.Smartcard:
                    return "Smartcard";
                default: {
                    return "";
                }
            }
        };
        EnumHelper.prototype.getWorkflowActivityStatusDescription = function (type) {
            switch (type) {
                case WorkflowStatus.Done:
                    return "Done";
                case WorkflowStatus.Error:
                    return "Error";
                case WorkflowStatus.Progress:
                    return "Progress";
                case WorkflowStatus.Suspended:
                    return "Suspended";
                case WorkflowStatus.Todo:
                    return "Todo";
                default: {
                    return "";
                }
            }
        };
        EnumHelper.prototype.getAUSSubjectTypeDescription = function (type) {
            switch (type) {
                case AUSSubjectType.NaturalPersons: {
                    return "Persone fisiche";
                }
                case AUSSubjectType.EconomicOperators: {
                    return "Operatori economici";
                }
                default: {
                    return "";
                }
            }
        };
        EnumHelper.prototype.getCustomActionDescription = function (propertyName) {
            switch (propertyName) {
                case "AutoClose": {
                    return "Chiusura amministrativa a 60 giorni";
                }
                case "AutoCloseAndClone": {
                    return "Chiusura amministrativa a fine anno e riapertura automatica";
                }
                default: {
                    return "";
                }
            }
        };
        EnumHelper.prototype.getDossierTypeDescription = function (type) {
            switch (DossierType[type.toString()]) {
                case DossierType.Person: {
                    return "Persona fisica o giuridica";
                }
                case DossierType.PhysicalObject: {
                    return "Oggetto fisico";
                }
                case DossierType.Procedure: {
                    return "Procedimento";
                }
                case DossierType.Process: {
                    return "Serie archivistica";
                }
                default: {
                    return "";
                }
            }
        };
        EnumHelper.prototype.getDossierStatusDescription = function (type) {
            switch (DossierStatus[type.toString()]) {
                case DossierStatus.Open: {
                    return "Aperto";
                }
                case DossierStatus.Canceled: {
                    return "Annullato";
                }
                case DossierStatus.Closed: {
                    return "Chiuso";
                }
                default: {
                    return "";
                }
            }
        };
        EnumHelper.prototype.getValidationRuleType = function (description) {
            switch (description) {
                case "Esiste cartella di fascicolo":
                    return "IsExist";
                case "La cartella contiene almeno un file/inserto":
                    return "HasFile";
                case "La cartella contiene almeno una unità documentale":
                    return "HasDocumentUnit";
                case "La cartella contiene almeno un file/inserto firmato digitalmente":
                    return "HasSignedFile";
                default: {
                    return "";
                }
            }
        };
        EnumHelper.prototype.getValidationRuleDescription = function (type) {
            switch (WorkflowValidationRulesType[type.toString()]) {
                case WorkflowValidationRulesType.IsExist: {
                    return "Esiste cartella di fascicolo";
                }
                case WorkflowValidationRulesType.HasFile: {
                    return "La cartella contiene almeno un file/inserto";
                }
                case WorkflowValidationRulesType.HasDocumentUnit: {
                    return "La cartella contiene almeno una unità documentale";
                }
                case WorkflowValidationRulesType.HasSignedFile: {
                    return "La cartella contiene almeno un file/inserto firmato digitalmente";
                }
                default: {
                    return "";
                }
            }
        };
        /**
         * Returns the correct enum value as number from input of type number | string | misspelled-string | number-as-string
         */
        EnumHelper.prototype.fixEnumValue = function (model, _type) {
            if (typeof model === "string") {
                try {
                    // if value is a number passed as a string
                    var parsed = parseInt(model, 10);
                    if (parsed) {
                        return _type[_type[parsed]];
                    }
                }
                catch (_a) { }
                //if it's a string and is correctly spelled, we will imediately have a value
                var value = _type[model];
                if (value !== undefined && value !== null) {
                    return value;
                }
                else {
                    // string enum value but misspelled (lower/upper case)
                    var keys = Object
                        .keys(_type).map(function (key) { return _type[key]; })
                        .filter(function (value) { return typeof value === 'string'; })
                        .filter(function (value) { return value.toLowerCase() === model.toLowerCase(); });
                    if (keys.length > 0) {
                        value = _type[keys[0]];
                    }
                    else {
                        throw new Error("Enum value " + model + " is invalid");
                    }
                    return value;
                }
            }
            else {
                return model;
            }
        };
        return EnumHelper;
    }());
    return EnumHelper;
});
//# sourceMappingURL=EnumHelper.js.map