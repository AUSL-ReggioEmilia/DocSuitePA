/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import WorkflowActivityInsertBase = require("Workflows/WorkflowActivityInsertBase");
import WorkflowDSWEnvironmentType = require('App/Models/Workflows/WorkflowDSWEnvironmentType');
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import WorkflowRepositoryModel = require('App/Models/Workflows/WorkflowRepositoryModel');
import WorkflowStartModel = require('App/Models/Workflows/WorkflowStartModel');
import WorkflowStartService = require('App/Services/Workflows/WorkflowStartService');
import WorkflowPropertyHelper = require('App/Models/Workflows/WorkflowPropertyHelper');
import UscStartWorkflow = require("UserControl/uscStartWorkflow");
import ArgumentType = require('App/Models/Workflows/ArgumentType');
import ContactModel = require('App/Models/Commons/ContactModel');
import WorkflowReferenceModel = require('App/Models/Workflows/WorkflowReferenceModel');
import WorkflowReferenceType = require('App/Models/Workflows/WorkflowReferenceType');
import DocumentModel = require('App/Models/Commons/DocumentModel');
import UscUploadDocumentRest = require('UserControl/uscUploadDocumentRest');
import ChainType = require('App/Models/DocumentUnits/ChainType');
import WorkflowAccountModel = require('App/Models/Workflows/WorkflowAccountModel');
import UscContattiSel = require('UserControl/uscContattiSel');
import WorkflowReferenceBiblosModel = require('App/Models/Workflows/WorkflowReferenceBiblosModel');
import BuildActionModel = require('App/Models/Commons/BuildActionModel');
import BuildActionType = require('App/Models/Commons/BuildActionType');
import ReferenceBuildModelType = require('App/Models/Commons/ReferenceBuildModelType');
import BuilderService = require("App/Services/Builders/BuilderService");
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import BuildArchiveDocumentModel = require('App/Models/Commons/BuildArchiveDocumentModel');
declare var Page_IsValid: any;

class WorkflowActivityInsert extends WorkflowActivityInsertBase {

    ddlWorkflowActivityId: string;
    rblPriorityId: string;
    btnConfirmId: string;
    ajaxLoadingPanelId: string;
    radWindowManagerId: string;
    pnlWorkflowActivityInsertId: string;
    uscDestinatariId: string;
    uscSettoriId: string;
    uscDocumentId: string;
    tenantName: string;
    tenantId: string;
    txtNoteId: string;
    fullUserName: string;
    fullName: string;
    email: string;
    rblPriorityVal: string;
    destinatariName: string;
    dataScadentaId: string
    workflowRepositoriesResult: WorkflowRepositoryModel[];
    workflowArchiveName: string;

    docSuiteVersion: string;
    idTenantAOO: string;

    private _ddlWorkflowActivity: Telerik.Web.UI.RadDropDownList;
    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _workflowStartModel: WorkflowStartModel;
    private _uscUploadDocumentRest: UscUploadDocumentRest;
    private _txtNote: Telerik.Web.UI.RadTextBox;
    private _uscStartWorkflow: UscStartWorkflow;
    private _manager: Telerik.Web.UI.RadWindowManager;
    private _builderService: BuilderService;


    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(serviceConfigurations);
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {

        });
    }

    initialize() {
        sessionStorage.clear(); //clear session of contacts
        super.initialize();

        let builderServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "BuildActionModel");
        this._builderService = new BuilderService(builderServiceConfiguration);

        this._manager = $find(this.radWindowManagerId) as Telerik.Web.UI.RadWindowManager;
        this._ddlWorkflowActivity = <Telerik.Web.UI.RadDropDownList>$find(this.ddlWorkflowActivityId);
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
        this._btnConfirm.add_clicking(this._btnConfirm_onClick);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._uscUploadDocumentRest = <UscUploadDocumentRest>$("#".concat(this.uscDocumentId)).data();
        this._txtNote = <Telerik.Web.UI.RadTextBox>$find(this.txtNoteId);
        this._uscStartWorkflow = new UscStartWorkflow(this._serviceConfigurations);
        this._workflowStartModel = <WorkflowStartModel>{};
        this._workflowStartModel.Arguments = {};
        this._workflowStartModel.StartParameters = {};
        this.addComboboxValues();
    }

    _btnConfirm_onClick = () => {

        if (!Page_IsValid) {
            return;
        }
        this._btnConfirm.set_enabled(false);
        this._loadingPanel.show(this.pnlWorkflowActivityInsertId);

        this.addDocuments()
            .done((documents: WorkflowReferenceBiblosModel[]) => {
                let selectedWorkflowRepository: Telerik.Web.UI.DropDownListItem = this._ddlWorkflowActivity.get_selectedItem();
                this._workflowStartModel.WorkflowName = selectedWorkflowRepository.get_text();
                this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME] = this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, ArgumentType.PropertyString, this.tenantName);
                this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID] = this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, ArgumentType.PropertyGuid, this.tenantId);
                this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID] = this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, ArgumentType.PropertyGuid, this.idTenantAOO);

                const workflowReferenceModel: WorkflowReferenceModel = {} as WorkflowReferenceModel;
                workflowReferenceModel.WorkflowReferenceType = WorkflowReferenceType.Json;
                workflowReferenceModel.ReferenceModel = JSON.stringify({});
                workflowReferenceModel.Documents = documents;
                workflowReferenceModel.DocumentUnits = []
                this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL] = this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, ArgumentType.Json, JSON.stringify(workflowReferenceModel));

                //Contacts
                this.addContact();

                //Priority
                this.rblPriorityVal = $("#".concat(this.rblPriorityId).concat(" input:checked")).val();
                this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_PRIORITY] = this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_PRIORITY, ArgumentType.PropertyInt, this.rblPriorityVal);

                //Date
                let dataScadentaVal: string = $("#".concat(this.dataScadentaId)).val();
                this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_DUEDATE] = this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_DUEDATE, ArgumentType.PropertyDate, dataScadentaVal);

                //Motivation
                this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_MOTIVATION] = this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_MOTIVATION, ArgumentType.PropertyString, this._txtNote.get_textBoxValue());

                this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT] = this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT, ArgumentType.PropertyString, this._txtNote.get_textBoxValue());
                //Proposer User
                let workflowAccountModel: WorkflowAccountModel = {
                    AccountName: this.fullUserName,
                    DisplayName: this.fullName,
                    EmailAddress: this.email,
                    Required: false
                };

                this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_USER] = this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_USER, ArgumentType.Json, JSON.stringify(workflowAccountModel));

                this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_FIELD_PRODUCT_NAME] = this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_FIELD_PRODUCT_NAME, ArgumentType.PropertyString, "DocSuite");
                this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_FIELD_PRODUCT_VERSION] = this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_FIELD_PRODUCT_VERSION, ArgumentType.PropertyString, this.docSuiteVersion);

                (<WorkflowStartService>this.workflowStartService).startWorkflow(this._workflowStartModel,
                    (data: any) => {
                        this._btnConfirm.set_enabled(true);
                        window.location.href = "../User/UserWorkflow.aspx?Type=Comm";
                    },
                    (exception: ExceptionDTO) => {
                        this._btnConfirm.set_enabled(true);
                        alert("Anomalia nel avvio dell'attività. Contattare l'assistenza");
                        location.reload();
                    }
                );
            })
            .fail((exception: ExceptionDTO) => {
                console.error(exception);
                alert("Anomalia nel avvio dell'attività. Contattare l'assistenza");
                location.reload();
            });
    }

    addComboboxValues(): void {
        this._loadingPanel.show(this.ddlWorkflowActivityId);
        this.workflowRepositoryService.getRepositoryByEnvironment(WorkflowDSWEnvironmentType.Desk,
            (data: any) => {
                if (!data) return;
                this.workflowRepositoriesResult = data;

                for (let workflowRepository of this.workflowRepositoriesResult) {
                    let comboItem: Telerik.Web.UI.DropDownListItem = new Telerik.Web.UI.DropDownListItem();
                    comboItem.set_text(workflowRepository.Name);
                    comboItem.set_value(workflowRepository.UniqueId);

                    if (this.workflowRepositoriesResult.length == 1) {
                        this._ddlWorkflowActivity.get_items().insert(0, comboItem);
                        this._ddlWorkflowActivity.findItemByValue(comboItem.get_value()).select();
                        this._ddlWorkflowActivity.set_enabled(false);
                    } else {
                        this._ddlWorkflowActivity.get_items().add(comboItem);
                    }
                }

                this._loadingPanel.hide(this.ddlWorkflowActivityId);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.ddlWorkflowActivityId);
            });
    }

    private addDocuments(): JQueryPromise<WorkflowReferenceBiblosModel[]> {
        let promise: JQueryDeferred<WorkflowReferenceBiblosModel[]> = $.Deferred<WorkflowReferenceBiblosModel[]>();
        let results: WorkflowReferenceBiblosModel[] = [];
        let sessionDocuments: Array<DocumentModel> = this.getUscDocument();
        if (sessionDocuments && sessionDocuments.length == 0) {
            promise.resolve(results);
            return promise.promise();
        }

        let buildModel: BuildActionModel;
        let documentModel: BuildArchiveDocumentModel;
        let buildModels: Array<BuildActionModel> = [];
        for (let document of sessionDocuments) {
            buildModel = {} as BuildActionModel;
            buildModel.BuildType = BuildActionType.Build;
            buildModel.ReferenceType = ReferenceBuildModelType.Document;

            documentModel = {
                Name: document.FileName,
                Archive: this.workflowArchiveName,
                ContentStream: document.ContentStream
            } as BuildArchiveDocumentModel;
            buildModel.Model = JSON.stringify(documentModel);
            buildModels.push(buildModel);
        }

        const iterator = () => {
            this.buildDocument(buildModels.shift())
                .done((model: WorkflowReferenceBiblosModel) => {
                    results.push(model);
                    if (buildModels.length > 0) {
                        return iterator();
                    }
                    promise.resolve(results);
                })
                .fail((exception: ExceptionDTO) => promise.reject(exception));
        }
        iterator();

        return promise.promise();
    }

    private buildDocument(model: BuildActionModel): JQueryPromise<WorkflowReferenceBiblosModel> {
        let promise: JQueryDeferred<WorkflowReferenceBiblosModel> = $.Deferred<WorkflowReferenceBiblosModel>();
        this._builderService.sendBuild(model,
            (data: any) => {
                let buildedItem: BuildActionModel = data as BuildActionModel;
                let buildedDocument: any = JSON.parse(buildedItem.Model);
                promise.resolve({
                    ArchiveChainId: buildedDocument.IdChain,
                    ArchiveDocumentId: buildedDocument.IdDocument,
                    ArchiveName: buildedDocument.Archive,
                    ChainType: ChainType.Miscellanea,
                    DocumentName: buildedDocument.Name
                } as WorkflowReferenceBiblosModel)
            },
            (exception: ExceptionDTO) => promise.reject(exception));

        return promise.promise();
    }

    addContact(): void {
        let uscContacts: UscContattiSel = <UscContattiSel>$("#".concat(this.uscSettoriId)).data();
        let workflowAccounts: WorkflowAccountModel[] = [];
        let contactsModel: ContactModel[] = JSON.parse(uscContacts.getContacts());

        for (let contactModel of contactsModel) {
            let workflowAccount: WorkflowAccountModel = {
                AccountName: contactModel.Code,
                DisplayName: contactModel.Description,
                EmailAddress: contactModel.EmailAddress,
                Required: false
            };

            workflowAccounts.push(workflowAccount);
        }

        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS] = this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS, ArgumentType.Json, JSON.stringify(workflowAccounts));
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_ACTION_PARALLEL_ACTIVITY] = this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_ACTION_PARALLEL_ACTIVITY, ArgumentType.PropertyBoolean, true);

    }

    getUscDocument = () => {
        let workflowDocuments: Array<DocumentModel> = new Array<DocumentModel>();
        if (!jQuery.isEmptyObject(this._uscUploadDocumentRest)) {
            let source: any = JSON.parse(this._uscUploadDocumentRest.getDocument());
            if (source != null) {
                for (let s of source) {
                    let document: DocumentModel = <DocumentModel>{};
                    document.FileName = s.FileName;
                    document.ContentStream = s.ContentStream;
                    workflowDocuments.push(document);
                }
            }
        }
        return workflowDocuments;
    }


}

export = WorkflowActivityInsert;