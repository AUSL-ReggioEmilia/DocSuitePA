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
import DSWEnvironment = require('App/Models/Environment');
import WorkflowReferenceType = require('App/Models/Workflows/WorkflowReferenceType');
import DocumentModel = require('App/Models/Commons/DocumentModel');
import UscUploadDocumentRest = require('UserControl/uscUploadDocumentRest');
import ChainType = require('App/Models/DocumentUnits/ChainType');
import WorkflowDocumentModel = require('App/Models/Commons/WorkflowDocumentModel');
import WorkflowAccountModel = require('App/Models/Workflows/WorkflowAccountModel');
import UscContattiSel = require('UserControl/uscContattiSel');
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

    docSuiteVersion: string;

    private _ddlWorkflowActivity: Telerik.Web.UI.RadDropDownList;
    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _workflowStartModel: WorkflowStartModel;
    private _uscUploadDocumentRest: UscUploadDocumentRest;
    private _txtNote: Telerik.Web.UI.RadTextBox;
    private _uscStartWorkflow: UscStartWorkflow;
    private _manager: Telerik.Web.UI.RadWindowManager;


    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(serviceConfigurations);
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {

        });
    }

    initialize() {
        sessionStorage.clear(); //clear session of contacts
        super.initialize();
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

        let selectedWorkflowRepository: Telerik.Web.UI.DropDownListItem = this._ddlWorkflowActivity.get_selectedItem();
        this._workflowStartModel.WorkflowName = selectedWorkflowRepository.get_text();
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME] = this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, ArgumentType.PropertyString, this.tenantName);
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID] = this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, ArgumentType.PropertyGuid, this.tenantId);

        //Documents
        this.addDocuments();

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

                    this._ddlWorkflowActivity.get_items().add(comboItem);
                }

                this._loadingPanel.hide(this.ddlWorkflowActivityId);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.ddlWorkflowActivityId);
            });
    }

    addDocuments(): void {
        let referenceModel: WorkflowReferenceModel = <WorkflowReferenceModel>{};
        referenceModel.ReferenceType = DSWEnvironment.Desk;
        referenceModel.WorkflowReferenceType = WorkflowReferenceType.Json;

        let sessionDocuments: Array<DocumentModel> = this.getUscDocument();
        let workflowDocumentModel: WorkflowDocumentModel = <WorkflowDocumentModel>{};
        workflowDocumentModel.Documents = [];

        //iterate through session documents to assign the key value of type Miscellanea
        for (let doc of sessionDocuments) {
            let obj: object = {
                Key: ChainType[ChainType.Miscellanea],
                Value: doc
            };
            workflowDocumentModel.Documents.push(obj);
        }

        referenceModel.ReferenceModel = JSON.stringify(workflowDocumentModel);
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_REFERENCE_MODEL] = this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_REFERENCE_MODEL, ArgumentType.Json, JSON.stringify(referenceModel));
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