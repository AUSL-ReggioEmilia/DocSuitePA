import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import EnumHelper = require("App/Helpers/EnumHelper");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import DomainUserService = require("App/Services/Securities/DomainUserService");
import DomainUserModel = require("App/Models/Securities/DomainUserModel");
import GenericHelper = require("App/Helpers/GenericHelper");
import ContactModel = require("APP/Models/Commons/ContactModel");
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');

class CommonDomainUserSelRest {
    btnSearchId: string;
    txtFilterId: string;
    tvwContactDomainId: string;
    btnConfirmId: string;
    contactList: ContactModel[] = [];
    pageContentId: string;
    uscNotificationId: string;

    private _tvwContactDomain: Telerik.Web.UI.RadTreeView;
    private _txtFilter: JQuery;
    private _btnSearch: Telerik.Web.UI.RadButton;
    private _btnConfirm: Telerik.Web.UI.RadButton;

    private _serviceConfigurations: ServiceConfiguration[];
    private _enumHelper: EnumHelper;
    private _domainUserService: DomainUserService;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._enumHelper = new EnumHelper();
    }

    initialize(): void {
        let serviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DomainUser");
        this._domainUserService = new DomainUserService(serviceConfiguration);

        this._tvwContactDomain = <Telerik.Web.UI.RadTreeView>$find(this.tvwContactDomainId);
        this._tvwContactDomain.get_nodes().clear();

        this._txtFilter = <JQuery>$(`#${this.txtFilterId}`);
        $(`#${this.txtFilterId}`).val("");

        this.contactList = [];
        this._btnSearch = <Telerik.Web.UI.RadButton>$find(this.btnSearchId);
        this._btnSearch.add_clicking(this.btnSearch_OnClick);
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
        this._btnConfirm.add_clicking(this.btnConfirm_OnClick);

        //TODO add functionality for conferma Nuovo as in uscContattiSel
    }

    btnSearch_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);

        let textFilterUserName: string = $(`#${this.txtFilterId}`).val();
        if (textFilterUserName.length < 2) {
            alert("Il filtro di ricerca deve essere di almeno 2 caratteri.");
            return;
        }

        this._domainUserService.userFinder(textFilterUserName, (data) => {
            if (!data) { return; }
            let domainUsersModel: DomainUserModel[] = data;
            this._tvwContactDomain.get_nodes().clear();

            let rootNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            rootNode.set_text("Contatti");
            rootNode.set_cssClass("font_node");
            rootNode.set_expanded(true);
            this._tvwContactDomain.get_nodes().add(rootNode);
            this._tvwContactDomain.get_nodes().getNode(0).get_nodes().clear();

            for (let domainUserModel of domainUsersModel) {
                let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
                node.set_text(`${domainUserModel.Account} - (${domainUserModel.DisplayName})`);
                node.set_value(domainUserModel.Account);
                this._tvwContactDomain.get_nodes().getNode(0).get_nodes().add(node);

                let newContact: ContactModel = <ContactModel>{
                    IdContactType: "A",
                    Code: domainUserModel.Account,
                    Description: domainUserModel.DisplayName,
                    SearchCode: domainUserModel.Account.split("\\")[1],
                    EmailAddress: domainUserModel.EmailAddress,
                    
                };
                this.contactList.push(newContact);

            }
            $(`#${this.tvwContactDomainId}`).show();
        },
            (exception: ExceptionDTO) => {
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    protected showNotificationException(uscNotificationId: string, exception: ExceptionDTO, customMessage?: string) {
        if (exception && exception instanceof ExceptionDTO) {
            let uscNotification: UscErrorNotification = <UscErrorNotification>$(`#${uscNotificationId}`).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotification(exception);
            }
        }
        else {
            this.showNotificationMessage(uscNotificationId, customMessage)
        }
    }

    protected showNotificationMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$(`#${uscNotificationId}`).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage);
        }
    }

    btnConfirm_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        let selNode = this._tvwContactDomain.get_selectedNode();
        let selectedContact: ContactModel[] = this.contactList.filter(x => x.Code == selNode.get_value());
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(selectedContact);
    }

    protected getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return wnd;
    }
}
export =CommonDomainUserSelRest;