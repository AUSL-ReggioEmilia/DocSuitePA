/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalR.d.ts" />

import WorkflowStorage = require("App/Core/WorkflowStorage/WorkflowStorage");
import MessageWorkflowResumeStatus = require("App/Core/WorkflowStorage/MessageWorkflowResumeStatus");
import UDSConstants = require("App/Core/UDS/UDSConstants");
import WorkflowStartModel = require("App/Models/Workflows/WorkflowStartModel");
import WorkflowPropertyHelper = require("App/Models/Workflows/WorkflowPropertyHelper");
import ArgumentType = require("App/Models/Workflows/ArgumentType");

class UDSInvoicesUpload {

    //internal fields
    private isPreview: boolean = false;
    private dswSignalR: DSWSignalR;
    private wstorage: WorkflowStorage;
    private wStorageEnabled: boolean = false;
    private correlationId: string = null;
    private correlatedChainId: string = null;
    private rdtpDataRicezioneSdiId: string;

    /**
     * If the resume status is returning with a "NotResumed" status, we will disconnect all created listeners
     * in this case the onErrorSignalRCallback message will be triggered signaling that the connection ended
     * with this flag we prevent the unwanted behaviour
     */
    private plannedConnectionStop: boolean = false;
    private _rdtpDataRicezioneSdi: Telerik.Web.UI.RadDateTimePicker;

    //- populated from UDSInvoicesUpload.aspx
    public currentUserTenantName: string;
    public currentUserTenantId: string;
    public currentUserTenantAOOId: string;
    public signalRServerAddress: string;

    /*
     * POPULATED BY UDSInvoicesUpload.aspx
     */
    public btnSaveId: string;
    public btnPreviewId: string;
    public radListMessagesId: string;
    public currentLoadingPanelId: string;
    public currentFlatLoadingPanelId: string;
    public ajaxManagerId: string;
    public rgvPreviewDocumentsId: string;
    public currentUpdatedControlId: string;
    public currentUpdatedToolbarId: string;
    public hFcorrelatedChainId: string;
    public hFcorrelatedIdDocument: string;
    //external functions
    private _pageClientValidate: () => boolean;

    constructor(pageClientValidator: () => boolean) {
        this._pageClientValidate = pageClientValidator;
    }

    public initialize = () => {
        try {

            //initializing a store to keep track of started activities
            this.wstorage = new WorkflowStorage();
            if (this.wstorage.IsValid) {
                this.wStorageEnabled = true;

                //now that we have the store, let's check if there is pending item in storage
            }
        }
        catch (err) {
            this.wStorageEnabled = false;
            //disabling the save button if the client does not support local or session storage
            this.getBtnSave().set_enabled(false);
            window.alert("Questa funzionalità non è supportata con l'attuale browser. E' necessario utilizzare un browser moderno come IE10+, Edge o Chrome");
        }

        if (this.wstorage.HasKey()) {
            //if we have a correlationId, there is a possibility that webapi has a running workflow and we can attach to it
            this.correlationId = this.wstorage.GetCorrelationId();
            this.correlatedChainId = this.wstorage.GetCorrelatedChainId();
            this.resumeWorkflowImport();
        }
        this.initializeDateRange();
    };

    private initializeDateRange = (): void => {
        this._rdtpDataRicezioneSdi = <Telerik.Web.UI.RadDateTimePicker>$find(this.rdtpDataRicezioneSdiId);

        var today = new Date();
        this._rdtpDataRicezioneSdi.set_selectedDate(today);
    }
    private validateDataRicezioneSdi = (): boolean => {
        this._rdtpDataRicezioneSdi = <Telerik.Web.UI.RadDateTimePicker>$find(this.rdtpDataRicezioneSdiId);

        var today = new Date();
        if (this._rdtpDataRicezioneSdi.get_selectedDate() > today) {
            alert("Non è possibile inserire una data dal futuro");
            return false;
        }

        return true;
    }

    private getBtnSave: () => Telerik.Web.UI.RadButton = () => <Telerik.Web.UI.RadButton>$find(this.btnSaveId);
    private getBtnPreview: () => Telerik.Web.UI.RadButton = () => <Telerik.Web.UI.RadButton>$find(this.btnPreviewId);
    private getRadListMessages: () => Telerik.Web.UI.RadListBox = () => <Telerik.Web.UI.RadListBox>$find(this.radListMessagesId);
    private getCurrentLoadingPanel: () => Telerik.Web.UI.RadAjaxLoadingPanel = () => <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.currentLoadingPanelId);
    private getCurrentFlatLoadingPanel: () => Telerik.Web.UI.RadAjaxLoadingPanel = () => <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.currentFlatLoadingPanelId);
    private getAjaxManager: () => Telerik.Web.UI.RadAjaxManager = () => <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
    private getRgvPreviewDocuments: () => Telerik.Web.UI.RadGrid = () => <Telerik.Web.UI.RadGrid>$find(this.rgvPreviewDocumentsId);

    private enableBtnSave = () => this.getBtnSave().set_enabled(true);
    private disableBtnSave = () => this.getBtnSave().set_enabled(false);
    private enableBtnPreview = () => this.getBtnPreview().set_enabled(true);
    private disableBtnPreview = () => this.getBtnPreview().set_enabled(false);

    public showLoadingPanel = () => {
        this.getCurrentLoadingPanel().show(this.currentUpdatedControlId);
        this.getCurrentFlatLoadingPanel().show(this.currentUpdatedToolbarId);
    }

    public hideLoadingPanel = () => {
        this.getCurrentLoadingPanel().show(this.currentUpdatedControlId);
        this.getCurrentFlatLoadingPanel().hide(this.currentUpdatedToolbarId);
        this.getCurrentLoadingPanel().hide(this.currentUpdatedControlId);
    }

    public confirmImport = (sender: any, args: any) => {
        if (this.isPreview) {
            this.generateInvoiceZip();
            return;
        }

        if (this.validateDataRicezioneSdi()) {
            this.startWorkflowImport(false);
        }
    }

    public previewImport = (sender: any, args: any) => {
        this.isPreview = true;
        if (this.validateDataRicezioneSdi()) {
            this.startWorkflowImport(true);
        }
    }

    public resumeWorkflowImport = () => {

        this.getBtnSave().set_enabled(false);
        this.getBtnPreview().set_enabled(false);

        this.dswSignalR = new DSWSignalR(this.signalRServerAddress);
        this.dswSignalR.setup("WorkflowHub", {
            'correlationId': this.correlationId
        });

        this.dswSignalR.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowStatusDone, this.actionHubWorkflowStatusDone);
        this.dswSignalR.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowStatusError, this.actionHubWorkflowStatusError);
        this.dswSignalR.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowNotificationInfo, this.actionHubWorkflowNotificationInfo);
        this.dswSignalR.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowNotificationWarning, this.actionHubWorkflowNotificationWarning);
        this.dswSignalR.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowNotificationError, this.actionHubWorkflowNotificationError);
        //connect to resume channel and wait for response
        this.dswSignalR.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowResumeStatus, this.actionHubWorkflowResumeStatus);

        this.dswSignalR.startConnection(this.onDoneResumeSignalRConnectionCallback, this.onErrorSignalRCallback);
    }

    public startWorkflowImport = (_isPreview: boolean) => {
        var validated: boolean = this._pageClientValidate();

        if (validated) {

            this.disableBtnSave();
            this.disableBtnPreview();

            this.dswSignalR = new DSWSignalR(this.signalRServerAddress);
            this.correlatedChainId = $get(this.hFcorrelatedChainId).value;
            this.correlationId = this.dswSignalR.newGuid();
            this.dswSignalR.setup("WorkflowHub", {
                'correlationId': this.correlationId
            });

            if (this.isPreview && this.isPreview === true) {
                this.dswSignalR.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowNotificationInfoAsModel, this.actionHubWorkflowNotificationInfoAsModel);
            } else {

                this.dswSignalR.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowStatusDone, this.actionHubWorkflowStatusDone);
                this.dswSignalR.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowStatusError, this.actionHubWorkflowStatusError);
                this.dswSignalR.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowNotificationInfo, this.actionHubWorkflowNotificationInfo);
                this.dswSignalR.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowNotificationWarning, this.actionHubWorkflowNotificationWarning);
                this.dswSignalR.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowNotificationError, this.actionHubWorkflowNotificationError);
            }
            this.dswSignalR.startConnection(this.onDoneSignalRConnectionCallback, this.onErrorSignalRCallback);
        }
    }

    public generateInvoiceZip = () => {
        var list = [];
        var masterTable: Telerik.Web.UI.GridTableView = this.getRgvPreviewDocuments().get_masterTableView();
        var inputList = masterTable.get_dataItems();

        for (var i = 0; i < inputList.length; i++) {
            if (inputList[i]._selected) {
                list.push({
                    "InvoiceFilename": inputList[i]._dataItem.InvoiceFilename,
                    "InvoiceMetadataFilename": inputList[i]._dataItem.InvoiceMetadataFilename,
                    "Description": "", //passing empty values so object can be deserialized
                    "Result": "",
                    "Selectable": false
                })
            }
        }

        if (list.length === 0) {
            alert('si prega di selezionare almeno una fattura');
            return;
        }

        var ajaxModel: any = {};
        ajaxModel.Value = [];
        ajaxModel.ActionName = "GenerateInvoiceZIP";
        ajaxModel.Value.push(JSON.stringify(list));
        this.getAjaxManager().ajaxRequest(JSON.stringify(ajaxModel));
    }

    //lamba function to solve scoping of a callback 
    public onDoneResumeSignalRConnectionCallback = () => {

        if (!this.isPreview && this.wStorageEnabled) {
            //we want to store the correlationId for the started worflow if we are not in preview mode
            this.wstorage.Set(this.correlationId, this.correlatedChainId);
        }

        var serverFunction = UDSConstants.HubMethods.SubscribeResumeWorkflow;

        this.addItemInfo("Una attività di importazione è già stata avviata ed è in corso. Attendere prego...");
        this.dswSignalR.sendServerMessage(serverFunction, this.correlationId, this.onDoneSignalRSubscriptionCallback, this.onErrorSignalRCallback);
    }

    public onDoneSignalRConnectionCallback = () => {
        if (!this.isPreview && this.wStorageEnabled) {
            //we wnat to store the correlationId for the started worflow if we are not in preview mode
            this.wstorage.Set(this.correlationId, this.correlatedChainId);
        }

        var serverFunction = UDSConstants.HubMethods.SubscribeStartWorkflow;

        this.addItemInfo("Preparazione importazione in corso. Attendere prego...");

        //this object needs to adapt -  SubscribeStartWorkflow now requires a list of objects to deserialize
        var workflowReferenceBiblos: any = [];
        var obj = { "ArchiveName": this.currentUserTenantName, "ArchiveChainId": this.correlatedChainId, "Simulation": this.isPreview };
        workflowReferenceBiblos.push(obj);

        let startImportModel: any = { "Documents": workflowReferenceBiblos };
        const serializedModel = JSON.stringify(startImportModel);

        const workflowName: string = "Fatturazione elettronica - Importa da cassetto fiscale"
        let evt: any = {
            "WorkflowName": workflowName, "WorkflowAutoComplete": true, "EventModel": { "CustomProperties": {} }
        };
        evt.EventModel.CustomProperties["DocumentManagementRequestModel"] = serializedModel;
        evt.EventModel.CustomProperties["DataRicezioneSdi"] = moment(this._rdtpDataRicezioneSdi.get_selectedDate()).format("YYYY-MM-DD HH:mm:ss");

        let referenceModel: any = { "ReferenceId": this.correlationId, "ReferenceModel": JSON.stringify(evt) };

        let workflowStartModel: WorkflowStartModel = <WorkflowStartModel>{};
        workflowStartModel.WorkflowName = workflowName;
        workflowStartModel.Arguments = {};
        workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME] = { "PropertyType": ArgumentType.PropertyString, "Name": WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME, "ValueString": "Importa da cassetto fiscale" };
        workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID] = { "PropertyType": ArgumentType.PropertyGuid, "Name": WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, "ValueGuid": this.currentUserTenantId };
        workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID] = { "PropertyType": ArgumentType.PropertyGuid, "Name": WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, "ValueGuid": this.currentUserTenantAOOId };
        workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME] = { "PropertyType": ArgumentType.PropertyString, "Name": WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, "ValueString": this.currentUserTenantName };
        workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL] = { "PropertyType": ArgumentType.Json, "Name": WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, "ValueString": JSON.stringify(referenceModel) };

        this.dswSignalR.sendServerMessages(serverFunction, this.correlationId, JSON.stringify(workflowStartModel),
            this.onDoneSignalRSubscriptionCallback, this.onErrorSignalRCallback);
    }

    private onErrorSignalRCallback = (error) => {
        if (this.plannedConnectionStop) {
            this.plannedConnectionStop = false;
            return;
        }

        if (this.wStorageEnabled) {
            //if we are trying to resume and there is an error, we should remove
            this.wstorage.Unset();
        }

        //console.log(error);
        this.addItemError("Impossibile procedere con la gestione dell'importazione delle fatture. Contattare l'assistenza : Errore di comunicazione con le WebAPI.");
        this.enableBtnSave();
    }

    private onDoneSignalRSubscriptionCallback = (error) => {
        this.addItemInfo("Importazione avviata, a breve verranno visualizzate le attività realtive allo stato di importazione fatture.");
        this.addItemInfo("Attendere prego...");
    }

    public actionHubWorkflowStatusDone = (model) => {
        if (!this.isPreview && this.wStorageEnabled) {
            //if the workflow status error is received, it means that workflow has completed and we can remove
            //correlation id from store
            this.wstorage.Unset();
        }

        this.addItemDone(model);
    }

    public actionHubWorkflowStatusError = (model) => {
        if (!this.isPreview && this.wStorageEnabled) {
            //if the workflow status error is received, it means that workflow has completed and we can remove
            //correlation id from store
            this.wstorage.Unset();
        }

        this.addItemError(model);
    }

    public actionHubWorkflowNotificationInfo = (model) => {
        this.addItemInfo(model);
    }

    public actionHubWorkflowNotificationInfoAsModel = (model) => {
        this.getBtnSave().set_enabled(true);
        this.addDocumentsToGrid(model);
        this.ClearMessages();
    }

    public actionHubWorkflowNotificationWarning = (model) => {
        this.addItemWarning(model);
    }

    public actionHubWorkflowNotificationError = (model) => {
        this.addItemError(model);
    }

    /**
     * When the dialog attempts to attach to an alleged running worflow, the webapi returns a message if resume fails.
     * Failure occurs if the process is already finished.
     * @param model
     */
    public actionHubWorkflowResumeStatus = (status) => {
        if (status == MessageWorkflowResumeStatus.DidNotResume) {
            //if the status is 1, resume has failed and we can show the original dialog
            this.wstorage.Unset();
            //set flag to true, because when closing the connection signalR the function onErrorSignalRCallback will be triggered
            //saying that the connection was closed before receiving the message
            this.plannedConnectionStop = true;
            this.dswSignalR.stopClient();

            this.getBtnSave().set_enabled(true);
            this.getBtnPreview().set_enabled(true);

            //remove any pending messages in the listbox because we are starting from scratch
            this.ClearMessages();
        }
    }

    public addItemDone = (text: string) => {
        this.addItem(text, "../App_Themes/DocSuite2008/imgset16/star.png");
    }

    public addItemInfo = (text: string) => {
        this.addItem(text, "../App_Themes/DocSuite2008/imgset16/information.png");
    }

    public addItemWarning = (text: string) => {
        this.addItem(text, "../App_Themes/DocSuite2008/imgset16/StatusSecurityWarning_16x.png");
    }

    public addItemError = (text: string) => {
        this.addItem(text, "../App_Themes/DocSuite2008/imgset16/StatusSecurityCritical_16x.png");
    }

    public addDocumentsToGrid = (model) => {
        this.ShowGrid();
        var masterTable = this.getRgvPreviewDocuments().get_masterTableView();
        masterTable.set_dataSource(JSON.parse(model));
        masterTable.dataBind();
    }

    public ShowGrid = () => {
        this.getRgvPreviewDocuments().get_element().style.display = "";
    }

    public ClearMessages = () => {
        this.getRadListMessages().get_items().clear();
    }

    public addItem = (text: string, imageUrl: string) => {
        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_text(text);
        item.set_imageUrl(imageUrl)
        this.getRadListMessages().get_items().add(item);
        this.getRadListMessages().commitChanges();
    }

    public SetChainId = (id, startWorkflow) => {
        this.isPreview = false;
        $get(this.hFcorrelatedChainId).value = id;
        if (startWorkflow) {
            this.startWorkflowImport(false);
        }
    }

    public SetIdDocument = (id) => {
        $get(this.hFcorrelatedIdDocument).value = id;
    }

    /*
     * Specifies if checkboxes are selectable by binding the Selectable attribute from model to the table.
     */
    public RowBinding = (sender, args) => {
        var masterTable = sender.get_masterTableView();
        var matchedModel = this.arrayObjectIndexOf(masterTable._dataSource,
            masterTable._dataItems[masterTable._dataItems.length - 1]._dataItem.InvoiceFilename,
            "InvoiceFilename"
        );
        masterTable._dataItems[masterTable._dataItems.length - 1]._selectable = matchedModel.Selectable;
        masterTable.get_dataItems()[masterTable._dataItems.length - 1].get_element().getElementsByTagName("INPUT")[0].disabled = !matchedModel.Selectable;
    }

    /*
     * Returns array matched by a searchTerm.
     * Used for IE compatibility.
     */
    public arrayObjectIndexOf(myArray, searchTerm, property): any {
        for (var i = 0, len = myArray.length; i < len; i++) {
            if (myArray[i][property] === searchTerm) return myArray[i];
        }
        return -1;
    }
}
export = UDSInvoicesUpload
