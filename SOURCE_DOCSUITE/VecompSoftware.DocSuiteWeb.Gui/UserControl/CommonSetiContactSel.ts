import UscErrorNotification = require('UserControl/uscErrorNotification');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import AjaxModel = require('App/Models/AjaxModel');
import SetiContactModel = require('App/Models/Commons/SetiContactModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import ExceptionStatusCode = require('App/DTOs/ExceptionStatusCode');


class CommonSetiContactSel {

    ajaxManagerId: string;
    ajaxLoadingPanelId: string;
    uscNotificationId: string;
    txtSearchId: string;
    btnSearchId: string;
    rtvSetiContactId: string;
    btnConfirmId: string;

    private _serviceConfigurations: ServiceConfiguration[];
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _ajaxLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _uscNotification: UscErrorNotification;
    private _txtSearch: Telerik.Web.UI.RadTextBox;
    private _btnSearch: Telerik.Web.UI.RadButton;
    private _rtvSetiContact: Telerik.Web.UI.RadTreeView;;
    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _wndSetiContacts: Telerik.Web.UI.RadWindow;
    private static SETI_CONTACT_MODEL: string = "contactModel";
    private static SEARCH_SETI_CONTACT: string = "SearchByText";


    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {

        });
    }

    initialize() {
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._ajaxLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._uscNotification = <UscErrorNotification>$(`#${this.uscNotificationId}`).data();
        this._txtSearch = <Telerik.Web.UI.RadTextBox>$find(this.txtSearchId);
        this._btnSearch = <Telerik.Web.UI.RadButton>$find(this.btnSearchId);
        this._btnSearch.add_clicked(this.btnSearch_onClick);
        this._rtvSetiContact = <Telerik.Web.UI.RadTreeView>$find(this.rtvSetiContactId);
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
        this._btnConfirm.add_clicked(this.btnConfirm_onClick);
        this._wndSetiContacts = this.getRadWindow();
        this._wndSetiContacts.add_show(this.onShowing);

        this._btnConfirm.set_enabled(false);
    }


    onShowing = (sender: any, args: any) => {
        this._txtSearch.set_textBoxValue("");
        this._rtvSetiContact.get_nodes().getNode(0).get_nodes().clear();
    }

    btnSearch_onClick = (sender: any, args: any) => {
        if (!this._txtSearch.get_textBoxValue() || this._txtSearch.get_textBoxValue().length < 2) {
            alert("Inserisci almeno due caratteri");
            return;
        }

        this._ajaxLoadingPanel.show(this.rtvSetiContactId);
        let ajaxModel: AjaxModel = <AjaxModel>{};
        ajaxModel.ActionName = CommonSetiContactSel.SEARCH_SETI_CONTACT;
        ajaxModel.Value = [this._txtSearch.get_textBoxValue()];
        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
    }

    contactsCallback(jsonResonse: string) {
        let contacts: SetiContactModel[] = JSON.parse(jsonResonse);
        this._rtvSetiContact.get_nodes().getNode(0).get_nodes().clear();
        for (let contact of contacts) {
            let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            node.set_text(`${contact.Nome} ${contact.Cognome} - (${contact.TesseraSanitaria} - ${contact.CodiceFiscale})`);
            node.set_value(contact.AnasId);
            node.get_attributes().setAttribute(CommonSetiContactSel.SETI_CONTACT_MODEL, contact);
            this._rtvSetiContact.get_nodes().getNode(0).get_nodes().add(node);
        }

        this._rtvSetiContact.get_nodes().getNode(0).expand();
        this._ajaxLoadingPanel.hide(this.rtvSetiContactId);
        this._btnConfirm.set_enabled(true);
    }
    contactsErrorCallback = (errorMessage: string) => {
        let exception: ExceptionDTO = new ExceptionDTO();
        exception.statusCode = ExceptionStatusCode.InternalServerError;
        exception.statusText = "Anomalia critica nell'esecuzione della richiesta. Contattare l'assistenza.";
        this._uscNotification.showNotification(exception);
        console.error(errorMessage);
        this._ajaxLoadingPanel.hide(this.rtvSetiContactId);
        this._btnConfirm.set_enabled(false);
    }
    btnConfirm_onClick = (sender: any, args: any) => {
        if (this._rtvSetiContact.get_selectedNode().get_level() > 0) {
            this.closeWindow(this._rtvSetiContact.get_selectedNode().get_attributes().getAttribute(CommonSetiContactSel.SETI_CONTACT_MODEL));
        }
    }

    getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return wnd;
    }

    closeWindow(dataToReturn: any): void {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(dataToReturn);
    }
}

export = CommonSetiContactSel;