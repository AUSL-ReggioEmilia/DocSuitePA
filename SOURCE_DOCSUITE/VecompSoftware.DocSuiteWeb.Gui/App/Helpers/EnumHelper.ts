﻿import DossierLogType = require("App/Models/Dossiers/DossierLogType");
import UDSLogType = require("App/Models/UDS/UDSLogType");
import WorkflowStatus = require('App/Models/Workflows/WorkflowStatus');
import UDSTypologyStatus = require('App/Models/UDS/UDSTypologyStatus');
import LocationTypeEnum = require('App/Models/Commons/LocationTypeEnum');
import WorkflowInstanceLogType = require('App/Models/Workflows/WorkflowInstanceLogType');
import InvoiceTypeEnum = require('App/Models/PECMails/InvoiceTypeEnum');
import InvoiceStatusEnum = require("App/Models/PECMails/InvoiceStatusEnum");
import PECMailDirection = require("App/Models/PECMails/PECMailDirection");
import ActivityType = require("App/Models/Workflows/ActivityType");
import TenantConfigurationTypeEnum = require('App/Models/Tenants/TenantConfigurationTypeEnum');
import TenantWorkflowRepositoryTypeEnum = require('App/Models/Tenants/TenantWorkflowRepositoryTypeEnum');
import WorkflowPropertyType = require('App/Models/Workflows/WorkflowPropertyType');
import ArgumentType = require('App/Models/Workflows/ArgumentType');
import FascicleType = require("App/Models/Fascicles/FascicleType");
import FascicleLogType = require("App/Models/Fascicles/FascicleLogType");
import ActivityAction = require("App/Models/Workflows/ActivityAction");
import ProviderSignType = require("App/Models/SignDocuments/ProviderSignType");
import ChainType = require("App/Models/DocumentUnits/ChainType");
import ActivityArea = require("App/Models/Workflows/ActivityArea");
import AUSSubjectType = require("App/Models/Commons/AUSSubjectType");
import WorkflowAuthorizationType = require("App/Models/Workflows/WorkflowAuthorizationType");
import DossierType = require("App/Models/Dossiers/DossierType");
import DossierStatus = require("App/Models/Dossiers/DossierStatus");
import WorkflowValidationRulesType = require("App/Models/Commons/WorkflowValidationRulesType");
import RoleUserType = require("App/Models/RoleUsers/RoleUserType");

class EnumHelper {

    getLogTypeDescription(type: DossierLogType): string {
        switch (<DossierLogType>DossierLogType[type.toString()]) {
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
    }

    getFascicleLogTypeDescription(type: FascicleLogType): string {
        switch (<FascicleLogType>FascicleLogType[type.toString()]) {
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
    }

    getWorkflowStatusDescription(type: string): string {
        switch (<WorkflowStatus>WorkflowStatus[type.toString()]) {
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
    }

    getActivityTypeDescription(type: string): string {
        switch (<ActivityType>ActivityType[type.toString()]) {
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
    }

    getUDSTypologyStatusDescription(type: UDSTypologyStatus): string {
        switch (<UDSTypologyStatus>UDSTypologyStatus[type.toString()]) {
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
    }

    getUDSLogType(type: UDSLogType): string {
        switch (<UDSLogType>UDSLogType[type.toString()]) {
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
    }

    getPECMailDirection(direction: string): PECMailDirection {
        if (direction) {
            if (direction == "Incoming") {
                return PECMailDirection.Incoming;
            }
            if (direction == "Outgoing") {
                return PECMailDirection.Outgoing;
            }
        }
        return null;
    }

    getInvoiceTypeDescription(invoiceType: string, pecInvoiceDirection: PECMailDirection): string {
        switch (<InvoiceTypeEnum>InvoiceTypeEnum[invoiceType.toString()]) {
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
    }

    getInvoiceStatusDescription(invoiceStatus: string): string {
        switch (<InvoiceStatusEnum>InvoiceStatusEnum[invoiceStatus.toString()]) {
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
    }

    getChainType(val: string): ChainType {
        let valNum: number = parseInt(val.toString());
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
    }

    getLocationTypeDescription(loc: LocationTypeEnum): string {
        let locNum: number = parseInt(loc.toString());
        if (!isNaN(locNum)) {
            locNum = LocationTypeEnum[loc.toString()]
        }

        switch (<LocationTypeEnum>LocationTypeEnum[locNum.toString()]) {
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
    }

    getWorkflowInstanceLogDescription(instance: WorkflowInstanceLogType): string {

        switch (<WorkflowInstanceLogType>WorkflowInstanceLogType[instance.toString()]) {
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
    }

    getTenantConfigurationTypeDescription(tenantConfigurationType: string): string {
        switch (<TenantConfigurationTypeEnum>TenantConfigurationTypeEnum[tenantConfigurationType.toString()]) {
            case TenantConfigurationTypeEnum.Tenant: {
                return "Azienda";
            }
            default: {
                return "";
            }
        }
    }

    getTenantWorkflowRepositoryTypeDescription(tenantWorkflowRepositoryType: string): string {
        switch (<TenantWorkflowRepositoryTypeEnum>TenantWorkflowRepositoryTypeEnum[tenantWorkflowRepositoryType.toString()]) {
            case TenantWorkflowRepositoryTypeEnum.WorkflowConfiguration: {
                return "Configurazione flusso di lavoro";
            }
            default: {
                return "";
            }
        }
    }

    getWorkflowStartupDescription(workflowStartup: ArgumentType): WorkflowPropertyType {
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
    }

    getFascicleTypeDescription(type: FascicleType): string {
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
    }

    getFascicleType(description: string): FascicleType {
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
    }

    getWorkflowActivityActionDescription(type: number): string {
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
            case ActivityAction.UpdateProtocol:
                return "UpdateProtocol";
            case ActivityAction.Authorize:
                return "Authorize";
            default: {
                return "";
            }
        }
    }

    getActivityAreaDescription(type: string): string {
        switch (<ActivityArea>ActivityArea[type.toString()]) {
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
    }

    getWorkflowAuthorizationType(type: WorkflowAuthorizationType): string {
        switch (type) {
            case WorkflowAuthorizationType.None:
                return "Nessuno";
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
            case WorkflowAuthorizationType.AllProtocolSecurityUsers:
                return "AllProtocolSecurityUsers";
            case WorkflowAuthorizationType.AllUDSSecurityUsers:
                return "AllUDSSecurityUsers";
            case WorkflowAuthorizationType.AllPECMailBoxRoleUser:
                return "AllPECMailBoxRoleUser";
            default:
                return "";
        }
    }

    getRemoteSignDescription(type: number): string {
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
    }

    getWorkflowActivityStatusDescription(type: WorkflowStatus): string {
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
    }

    getAUSSubjectTypeDescription(type: AUSSubjectType): string {
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
    }

    getCustomActionDescription(propertyName: string): string {
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
    }

    getDossierTypeDescription(type: string): string {
        switch (<DossierType>DossierType[type.toString()]) {
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
    }

    getDossierStatusDescription(type: string): string {
        switch (<DossierStatus>DossierStatus[type.toString()]) {
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
    }

    getValidationRuleType(description: string): string {
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
    }

    getValidationRuleDescription(type: string): string {
        switch (<WorkflowValidationRulesType>WorkflowValidationRulesType[type.toString()]) {
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
    }

    getRoleUserTypeDescription(type: string): string {
        switch (<RoleUserType>RoleUserType[type.toString()]) {
            case RoleUserType.RP: {
                return "Responsabile di procedimento";
            }
            case RoleUserType.SP: {
                return "Segreteria di procedimento";
            }
            case RoleUserType.M: {
                return "Manager di protocollo";
            }
            case RoleUserType.U: {
                return "Utente di protocollo";
            }
            case RoleUserType.D: {
                return "Direttori";
            }
            case RoleUserType.V: {
                return "Vice-Direttori";
            }
            case RoleUserType.S: {
                return "Segreteria";
            }
            case RoleUserType.X: {
                return "Nessun ruolo in collaborazione";
            }
            case RoleUserType.MP: {
                return "Responsabili privacy";
            }
            default: {
                return type;
            }
        }
    }


    /**
     * Returns the correct enum value as number from input of type number | string | misspelled-string | number-as-string
     */
    fixEnumValue<T>(model: T | string | number, _type: any): T {
        if (typeof model === "string") {
            try {
                // if value is a number passed as a string
                let parsed = parseInt(model, 10);
                if (parsed) {
                    return _type[_type[parsed]];
                }
            } catch{ }
            //if it's a string and is correctly spelled, we will imediately have a value
            let value = (<any>_type)[model];
            if (value !== undefined && value !== null) {
                return value;
            } else {
                // string enum value but misspelled (lower/upper case)
                let keys = Object
                    .keys(_type).map(key => _type[key])
                    .filter(value => typeof value === 'string')
                    .filter(value => value.toLowerCase() === model.toLowerCase())

                if (keys.length > 0) {
                    value = _type[keys[0]]
                } else {
                    throw new Error(`Enum value ${model} is invalid`)
                }
                return value;
            }
        } else {
            return <T>model;
        }
    }
}
export = EnumHelper;