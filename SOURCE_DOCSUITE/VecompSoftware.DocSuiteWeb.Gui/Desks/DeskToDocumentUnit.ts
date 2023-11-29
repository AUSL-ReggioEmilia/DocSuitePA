/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import AjaxModel = require("App/Models/AjaxModel");
import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import CollaborationDocumentType = require("../App/Models/Collaborations/CollaborationDocumentType");
import Environment = require("../App/Models/Environment");

class DeskToDocumentUnit {

    /* Fields */
    private readonly _serviceConfigurations: ServiceConfiguration[];
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _ajaxLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _dgvDocuments: Telerik.Web.UI.RadGrid;
    private _btnCancel: Telerik.Web.UI.RadButton;
    private _btnConfirm: Telerik.Web.UI.RadButton;

    private static MAINDOCUMENT_CODE = "MAIN";
    private static MAINDOCUMENTOMISSIS_CODE = "MAINOMISSIS";
    private static ATTACHMENTOMISSIS_CODE = "ALLOMISSIS";

    /* Properties */
    ajaxManagerId: string;
    ajaxLoadingPanelId: string;
    pageContentId: string;
    rwmDocPreviewId: string;
    rblDocumentUnitId: string;
    dgvDocumentsId: string;
    resolutionMainDocumentOmissisEnabled: boolean;
    resolutionAttachmentsOmissisEnabled: boolean;
    btnCancelId: string;
    btnConfirmId: string;
    btnConfirmPostbackId: string;
    currentDeskId: string;
    rblDocumentUnitUniqueId: string;
    ddlCollaborationTypeId: string;
    pnlCollaborationTypeId: string;
    rblSendToId: string;

    /* Constructor */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    /* Events */
    ddlDocumentType_onClientItemSelected = (sender: Telerik.Web.UI.RadDropDownList, args: Telerik.Web.UI.DropDownListItemSelectedEventArgs): void => {
        const selectedValue: string = args.get_item().get_value();
        if (selectedValue == DeskToDocumentUnit.MAINDOCUMENT_CODE || selectedValue == DeskToDocumentUnit.MAINDOCUMENTOMISSIS_CODE) {
            this.selectSingleDocumentTypeOnly(sender, selectedValue);
        }
    }

    ddlCollaborationType_onClientItemSelected = (sender: Telerik.Web.UI.RadDropDownList, args: Telerik.Web.UI.DropDownListItemSelectedEventArgs): void => {
        const selectedValue: string = args.get_item().get_value();
        this.reloadDocumentTypes(selectedValue);
    }

    rblDocumentUnitId_onChangeItem = (eventObject: JQueryEventObject): void => {
        this._ajaxLoadingPanel.show(this.dgvDocumentsId);
        const ddlCollaborationType: Telerik.Web.UI.RadDropDownList = <Telerik.Web.UI.RadDropDownList>$find(this.ddlCollaborationTypeId);
        const selectedCollaborationType: string = ddlCollaborationType.get_selectedItem()?.get_value();
        this.reloadDocumentTypes(selectedCollaborationType);
        this._ajaxManager.ajaxRequestWithTarget(this.rblDocumentUnitUniqueId, '');
    }

    btnCancel_onClicked = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs): void => {
        window.location.href = sender.get_navigateUrl();
    }

    btnConfirm_onClicked = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs): void => {
        const selectedDocumentUnitType: string = $('input[type=radio]:checked', `#${this.rblDocumentUnitId}`).val();
        const selectedCollaborationSendTo: string = $('input[type=radio]:checked', `#${this.rblSendToId}`).val();
        if (selectedDocumentUnitType == "0"
            && selectedCollaborationSendTo == "1"
            && !this.checkAllSelectedDocumentsIsSigned()) {
                alert("Non è possibile procedere con l'inserimento, tutti i documenti selezionati devono essere firmati");
                return;
        }

        if (!this.checkAllSelectedDocumentsHasDocumentType()) {
            alert("Non è possibile procedere con l'inserimento, tutti i documenti selezionati devono avere una tipo documento selezionato");
            return;
        }
        $(`#${this.btnConfirmPostbackId}`).click();
    }

    dgvDocuments_onRowDeselect = (sender: Telerik.Web.UI.RadGrid, args: Telerik.Web.UI.GridDataItemEventArgs): void => {
        const rowItem: Telerik.Web.UI.GridDataItem = args.get_gridDataItem();
        if (!rowItem.get_selected()) {
            const ddlDocumentType: Telerik.Web.UI.RadDropDownList = rowItem.findElement("ddlDocumentType").control;
            ddlDocumentType.getItem(0).select();
        }        
    }

    /* Methods */
    initialize(): void {
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._ajaxLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._dgvDocuments = <Telerik.Web.UI.RadGrid>$find(this.dgvDocumentsId);
        this._btnCancel = <Telerik.Web.UI.RadButton>$find(this.btnCancelId);
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);

        this._btnCancel.add_clicked(this.btnCancel_onClicked);
        this._btnConfirm.add_clicked(this.btnConfirm_onClicked);
        $(`#${this.rblDocumentUnitId}`).change(this.rblDocumentUnitId_onChangeItem);

        this.initializeData();
    }

    private initializeData(): void {
        let ajaxModel: AjaxModel = <AjaxModel>{
            ActionName: "initialize"
        };
        this._ajaxLoadingPanel.show(this.pageContentId);
        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
    }

    initializeDataCallback(): void {
        this._ajaxLoadingPanel.hide(this.pageContentId);
    }

    setPostbackUrlCallback(): void {
        this._ajaxLoadingPanel.hide(this.dgvDocumentsId);
    }

    openPreviewDocumentWindow(url: string): void {
        const winManager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.rwmDocPreviewId);
        const window: Telerik.Web.UI.RadWindow = winManager.open(url, 'windowPreviewDocument', null);
        window.setSize(750, 450);
        window.center();
    }

    private selectSingleDocumentTypeOnly(selectedItem: Telerik.Web.UI.RadDropDownList, documentType: string): void {
        const masterTable: Telerik.Web.UI.GridTableView = this._dgvDocuments.get_masterTableView();
        const rows = masterTable.get_dataItems();

        for (let rowItem of rows) {
            const ddlDocumentType: Telerik.Web.UI.RadDropDownList = rowItem.findElement("ddlDocumentType").control;

            if (ddlDocumentType.get_id() == selectedItem.get_id()) {
                continue;
            }

            if (ddlDocumentType.get_selectedItem().get_value() == documentType) {
                ddlDocumentType.getItem(0).select();
            }
        }
    }

    private checkAllSelectedDocumentsIsSigned(): boolean {
        const masterTable: Telerik.Web.UI.GridTableView = this._dgvDocuments.get_masterTableView();
        const rows = masterTable.get_selectedItems();

        let documentSigned;
        for (let rowItem of rows) {
            documentSigned = masterTable.getCellByColumnUniqueName(rowItem, "IsSigned");
            if (documentSigned.outerText == "False") {
                return false;
            }
        }
        return true;
    }

    private checkAllSelectedDocumentsHasDocumentType(): boolean {
        const masterTable: Telerik.Web.UI.GridTableView = this._dgvDocuments.get_masterTableView();
        const rows = masterTable.get_selectedItems();

        let documentSigned;
        for (let rowItem of rows) {
            const ddlDocumentType: Telerik.Web.UI.RadDropDownList = rowItem.findElement("ddlDocumentType").control;
            if (!ddlDocumentType.get_selectedItem().get_value()) {
                return false;
            }
        }
        return true;
    }

    private reloadDocumentTypes(selectedCollaborationType: string): void {
        const selectedDocumentUnitType: string = $('input[type=radio]:checked', `#${this.rblDocumentUnitId}`).val();
        const masterTable: Telerik.Web.UI.GridTableView = this._dgvDocuments.get_masterTableView();
        const rows = masterTable.get_dataItems();        

        let func;
        let ddlDocumentType: Telerik.Web.UI.RadDropDownList;
        for (let rowItem of rows) {
            ddlDocumentType = rowItem.findElement("ddlDocumentType").control;

            func = () => {
                if (ddlDocumentType.findItemByValue(DeskToDocumentUnit.MAINDOCUMENTOMISSIS_CODE)) {
                    if (ddlDocumentType.get_selectedItem().get_value() == DeskToDocumentUnit.MAINDOCUMENTOMISSIS_CODE) {
                        ddlDocumentType.getItem(0).select();
                    }
                    ddlDocumentType.get_items().remove(ddlDocumentType.findItemByValue(DeskToDocumentUnit.MAINDOCUMENTOMISSIS_CODE));
                }

                if (ddlDocumentType.findItemByValue(DeskToDocumentUnit.ATTACHMENTOMISSIS_CODE)) {
                    if (ddlDocumentType.get_selectedItem().get_value() == DeskToDocumentUnit.ATTACHMENTOMISSIS_CODE) {
                        ddlDocumentType.getItem(0).select();
                    }
                    ddlDocumentType.get_items().remove(ddlDocumentType.findItemByValue(DeskToDocumentUnit.ATTACHMENTOMISSIS_CODE));
                }
            };

            if (selectedDocumentUnitType == (<number>Environment.Resolution).toString() ||
                selectedDocumentUnitType == "0" && (selectedCollaborationType == CollaborationDocumentType[CollaborationDocumentType.A] || selectedCollaborationType == CollaborationDocumentType[CollaborationDocumentType.D])) {
                func = () => {
                    const mainOmissis: Telerik.Web.UI.DropDownListItem = new Telerik.Web.UI.DropDownListItem();
                    mainOmissis.set_text("Documento principale omissis");
                    mainOmissis.set_value(DeskToDocumentUnit.MAINDOCUMENTOMISSIS_CODE);

                    const attachmentsOmissis: Telerik.Web.UI.DropDownListItem = new Telerik.Web.UI.DropDownListItem();
                    attachmentsOmissis.set_text("Allegato omissis");
                    attachmentsOmissis.set_value(DeskToDocumentUnit.ATTACHMENTOMISSIS_CODE);

                    ddlDocumentType.trackChanges();
                    if (!ddlDocumentType.findItemByValue(DeskToDocumentUnit.MAINDOCUMENTOMISSIS_CODE) && this.resolutionMainDocumentOmissisEnabled) {
                        ddlDocumentType.get_items().add(mainOmissis);
                    }

                    if (!ddlDocumentType.findItemByValue(DeskToDocumentUnit.ATTACHMENTOMISSIS_CODE) && this.resolutionAttachmentsOmissisEnabled) {
                        ddlDocumentType.get_items().add(attachmentsOmissis);
                    }
                    ddlDocumentType.commitChanges();
                };
            }

            func();
        }
    }
}

export = DeskToDocumentUnit;