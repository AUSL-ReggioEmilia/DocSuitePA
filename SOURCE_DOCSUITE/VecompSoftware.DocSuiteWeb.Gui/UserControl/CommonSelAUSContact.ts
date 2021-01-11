import UscErrorNotification = require('UserControl/uscErrorNotification');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import EnumHelper = require("App/Helpers/EnumHelper");
import AUSSubjectType = require('App/Models/Commons/AUSSubjectType');
import AjaxModel = require('App/Models/AjaxModel');
import AUSContactModel = require('App/Models/Commons/AUSContactModel');
import ContactDSWModel = require('App/Models/Commons/ContactDSWModel');
import ContactTypeDSWModel = require('App/Models/Commons/ContactTypeDSWModel');

class CommonSelAUSContact {

    ajaxManagerId: string;
    ajaxLoadingPanelId: string;
    uscNotificationId: string;
    chkSubjectTypeId: string;
    txtSearchId: string;
    btnSearchId: string;
    txtCodeSearchId: string;
    btnCodeSearchId: string;
    rtvAUSContactsId: string;
    btnConfirmId: string;
    callerId: string;
    btnConfirmAndNewId: string;

    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _ajaxLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _uscNotification: UscErrorNotification;
    private _txtSearch: Telerik.Web.UI.RadTextBox;
    private _btnSearch: Telerik.Web.UI.RadButton;
    private _txtCodeSearch: Telerik.Web.UI.RadTextBox;
    private _btnCodeSearch: Telerik.Web.UI.RadButton;
    private _rtvAUSContacts: Telerik.Web.UI.RadTreeView;
    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _btnConfirmAndNew: Telerik.Web.UI.RadButton;

    private _serviceConfigurations: ServiceConfiguration[];
    private _enumHelper: EnumHelper;

    private static AUS_CONTACT_SUBJECT_NAME_TYPE_NAME = "SubjectName";
    private static AUS_CONTACT_EMAIL_TYPE_NAME = "Email";
    private static AUS_CONTACT_ERROR_NODE_CSS_CLASS_NAME = "error-tree-node";

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._enumHelper = new EnumHelper();
        $(document).ready(() => {

        });
    }


    initialize(): void {
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._ajaxLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._uscNotification = <UscErrorNotification>$(`#${this.uscNotificationId}`).data();
        this._txtSearch = <Telerik.Web.UI.RadTextBox>$find(this.txtSearchId);
        this._btnSearch = <Telerik.Web.UI.RadButton>$find(this.btnSearchId);
        this._btnSearch.add_clicked(this.btnSearch_onClick);
        this._txtCodeSearch = <Telerik.Web.UI.RadTextBox>$find(this.txtCodeSearchId);
        this._btnCodeSearch = <Telerik.Web.UI.RadButton>$find(this.btnCodeSearchId);
        this._btnCodeSearch.add_clicked(this.btnCodeSearch_onClick);
        this._rtvAUSContacts = <Telerik.Web.UI.RadTreeView>$find(this.rtvAUSContactsId);
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
        this._btnConfirm.add_clicked(this.btnConfirm_onClick);
        this._btnConfirmAndNew = <Telerik.Web.UI.RadButton>$find(this.btnConfirmAndNewId);
        this._btnConfirmAndNew.add_clicked(this.btnConfirmAndNew_onClick);
    }

    btnSearch_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        let searchText: string = this._txtSearch.get_textBoxValue();
        let selectedSubjectType: string = $("#".concat(this.chkSubjectTypeId).concat(" input:checked")).val();
        let subjectType: AUSSubjectType = AUSSubjectType[selectedSubjectType];
        let ajaxModel: AjaxModel = <AjaxModel>{};
        ajaxModel.ActionName = "SearchByText";
        ajaxModel.Value = [searchText, subjectType.toString()];
        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
    }

    btnCodeSearch_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        let searchCode: string = this._txtCodeSearch.get_textBoxValue();
        let selectedSubjectType: string = $("#".concat(this.chkSubjectTypeId).concat(" input:checked")).val();
        let subjectType: AUSSubjectType = AUSSubjectType[selectedSubjectType];
        let ajaxModel: AjaxModel = <AjaxModel>{};
        ajaxModel.ActionName = "SearchByCode";
        ajaxModel.Value = [searchCode, subjectType.toString()];
        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
    }

    contactsCallback(jsonResult: string): void {
        let contacts: AUSContactModel[] = JSON.parse(jsonResult);
        this._rtvAUSContacts.get_nodes().getNode(0).get_nodes().clear();
        for (let contact of contacts) {
            let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            if (contact.Email) {
                node.set_text(`${contact.Name} (${contact.Code} - ${contact.Email})`);
            }
            else {
                node.set_text(`${contact.Name} (${contact.Code})`);
            }
            node.set_value(contact.Code);
            node.get_attributes().setAttribute(CommonSelAUSContact.AUS_CONTACT_SUBJECT_NAME_TYPE_NAME, contact.Name);
            node.get_attributes().setAttribute(CommonSelAUSContact.AUS_CONTACT_EMAIL_TYPE_NAME, contact.Email);
            this._rtvAUSContacts.get_nodes().getNode(0).get_nodes().add(node);
        }
        this._rtvAUSContacts.get_nodes().getNode(0).expand();
        $(`#${this.rtvAUSContactsId}`).show();
        this._btnConfirm.set_enabled(true);
    }

    contactsErrorCallback(errorMessage: string): void {
        if (!errorMessage) {
            return;
        }
        this._rtvAUSContacts.get_nodes().getNode(0).get_nodes().clear();
        let errorNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        errorNode.set_text(errorMessage);
        errorNode.set_imageUrl("../App_Themes/DocSuite2008/imgset16/error.png");
        errorNode.set_cssClass(CommonSelAUSContact.AUS_CONTACT_ERROR_NODE_CSS_CLASS_NAME);
        errorNode.get_attributes().setAttribute("IsErrorNode", true);
        this._rtvAUSContacts.get_nodes().getNode(0).get_nodes().add(errorNode);
    }

    btnConfirm_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        this.addAUSContact(true);
    }

    btnConfirmAndNew_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        this.addAUSContact(false);
    }

    private addAUSContact(closeWindow: boolean) {
        if (!this._rtvAUSContacts.get_selectedNode() || this._rtvAUSContacts.get_selectedNode().get_level() === 0) {
            this._uscNotification.showWarningMessage("Seleziona almeno un contatto");
            return;
        }
        if (this._rtvAUSContacts.get_selectedNode().get_attributes().getAttribute("IsErrorNode")) {
            this._uscNotification.showWarningMessage("Non è possibile selezionare un nodo di errore");
            return;
        }
        let code: number = this._rtvAUSContacts.get_selectedNode().get_value();
        let name: string = this._rtvAUSContacts.get_selectedNode().get_attributes().getAttribute(CommonSelAUSContact.AUS_CONTACT_SUBJECT_NAME_TYPE_NAME);
        let email: string = this._rtvAUSContacts.get_selectedNode().get_attributes().getAttribute(CommonSelAUSContact.AUS_CONTACT_EMAIL_TYPE_NAME);
        let selectedContact: AUSContactModel = {
            Code: code,
            Name: name,
            Email: email
        };
        let contact: ContactDSWModel = <ContactDSWModel>{};
        contact.FiscalCode = selectedContact.Code.toString();
        contact.Code = selectedContact.Code.toString();
        contact.SearchCode = selectedContact.Code.toString();
        contact.Description = selectedContact.Name;
        contact.EmailAddress = selectedContact.Email;
        contact.ContactType = <ContactTypeDSWModel>{};
        contact.ContactType.ContactTypeId = "I";
        let dataToReturn: any = {
            Action: "Ins",
            Contact: JSON.stringify(contact)
        };


        if (closeWindow) {
            this.closeWindow(dataToReturn);
        } else {
            let returnModel: any = { contactType: contact.ContactType.ContactTypeId, contact: JSON.stringify(contact) };
            this.getRadWindow().BrowserWindow[`${this.callerId}_UpdateSmart`](returnModel.contactType, returnModel.contact);
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

export = CommonSelAUSContact;