import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import EnumHelper = require("App/Helpers/EnumHelper");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import ContactModel = require("APP/Models/Commons/ContactModel");

enum UscDomainUserSelRestEvent {
    ContactsAdded,
    ContactsRemoved
}

class uscDomainUserSelRest {
    radTreeContactId: string;
    btnSelContactDomainId: string;
    btnDelContactId: string;
    radWindowManagerId: string;
    pageContentId: string;
    contactList: ContactModel[] = [];

    private _radTreeContact: Telerik.Web.UI.RadTreeView;
    private _btnSelContactDomain: JQuery;
    private _btnDelContact: JQuery;
    private _radWindowManager: Telerik.Web.UI.RadWindowManager;

    private _serviceConfigurations: ServiceConfiguration[];
    private _enumHelper: EnumHelper;

    public uscSelRestEvents = UscDomainUserSelRestEvent;
    private _parentPageEventHandlersDictionary:
        {
            eventType: UscDomainUserSelRestEvent,
            promiseCallback: (data: any) => JQueryPromise<any>
        } = {} as {
            eventType: UscDomainUserSelRestEvent,
            promiseCallback: (data: any) => JQueryPromise<any>
        };

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._enumHelper = new EnumHelper();
    }

    initialize(): void {
        $(`#${this.pageContentId}`).data(this);
        this._radWindowManager = $find(this.radWindowManagerId) as Telerik.Web.UI.RadWindowManager;

        this._radTreeContact = <Telerik.Web.UI.RadTreeView>$find(this.radTreeContactId);
        this._radTreeContact.get_nodes().getNode(0).set_checkable(false);

        this._btnSelContactDomain = <JQuery>$(`#${this.btnSelContactDomainId}`);
        this._btnSelContactDomain.click(this.btnSelContactDomain_onClick);
        this._btnDelContact = <JQuery>$(`#${this.btnDelContactId}`);
        this._btnDelContact.click(this.btnDelContact_onClick);
    }

    btnSelContactDomain_onClick = (sender: any) => {
        var url = `../UserControl/CommonDomainUserSelRest.aspx?Type=Fasc&ManagerID=${this.radWindowManagerId}&Callback${window.location.href}&PageContentId=${this.pageContentId}`;
        return this.openWindow(url, "windowSelContact", 630, 500, this.closeWindowCallback);
    }

    btnDelContact_onClick = (sender: any) => {
        const removalNodes: any[] = [];
        const removalContacts: ContactModel[] = [];

        //collect contacts to remove
        for (let checkedNode of this._radTreeContact.get_checkedNodes()) {
            removalNodes.push(checkedNode);
            removalContacts.push(this.contactList.find(x => x.EmailAddress == checkedNode.get_value()));
        }

        if (removalNodes.length > 0) {

            //removing contacts in the event.
            this.chainParentEventToAction(() => {
                for (let i = 0; i < removalNodes.length; i++) {
                    this.contactList = this.contactList.filter(x => x.EmailAddress != removalContacts[i].EmailAddress);
                    this._radTreeContact.get_nodes().getNode(0).get_nodes().remove(removalNodes[i]);
                }
            }, UscDomainUserSelRestEvent.ContactsRemoved, removalContacts);

            return true;
        }

        return false;
    }

    closeWindowCallback = (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (!args.get_argument()) {
            return;
        }

        let contacts = args.get_argument();

        this.chainParentEventToAction(() => {
            this.createDomainUsersContactsTree(contacts);
        }, UscDomainUserSelRestEvent.ContactsAdded, contacts);

    }

    openWindow(url, name, width, height, onCloseCallback?): boolean {
        let manager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerId);
        let wnd: Telerik.Web.UI.RadWindow = manager.open(url, name, null);
        wnd.setSize(width, height);
        wnd.set_modal(true);
        wnd.center();
        if (onCloseCallback) {
            wnd.remove_close(onCloseCallback);
            wnd.add_close(onCloseCallback);
        }
        return false;
    }

    getContacts(): ContactModel[] {
        return this.contactList;
    }

    createDomainUsersContactsTree = (contacts: ContactModel[]) => {
        this._radTreeContact.get_nodes().getNode(0).get_nodes().clear();

        for (let contact of contacts) {
            this.contactList.push(contact);
            let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            node.set_text(`${contact.Description} (${contact.Code.split("\\")[0] ? contact.Code.split("\\")[0] : contact.Code})`);
            node.set_value(contact.EmailAddress);
            node.set_cssClass("font_node");
            node.set_imageUrl("../Comm/Images/Interop/Manuale.gif");
            this._radTreeContact.get_nodes().getNode(0).set_checkable(false);
            this._radTreeContact.get_nodes().getNode(0).set_expanded(true);
            this._radTreeContact.get_nodes().getNode(0).get_nodes().add(node);
        }
    }

    clearDomainUsersContactsTree = () => {
        this._radTreeContact.get_nodes().getNode(0).get_nodes().clear();
    }

    setImageButtonsVisibility(isVisible: boolean) {
        if (isVisible) {
            $(`#${this.btnSelContactDomainId}`).show();
            $(`#${this.btnDelContactId}`).show();
        } else {
            $(`#${this.btnSelContactDomainId}`).hide();
            $(`#${this.btnDelContactId}`).hide();
        }
    }

    public registerEventHandlerContactsAdded = (callback: (data: ContactModel[]) => JQueryPromise<any>): void => {
        this._parentPageEventHandlersDictionary[UscDomainUserSelRestEvent.ContactsAdded] = callback;
    }

    public registerEventHandlerContactsDeleted = (callback: (data: ContactModel[]) => JQueryPromise<any>): void => {
        this._parentPageEventHandlersDictionary[UscDomainUserSelRestEvent.ContactsRemoved] = callback;
    }

    private chainParentEventToAction<T>(action: () => void, eventType: UscDomainUserSelRestEvent, eventData?: T): void {
        //if there is an external event registered to the event we will call it first
        if (this._parentPageEventHandlersDictionary[eventType]) {

            let parentUpdateCallback: JQueryPromise<any>;
            if (eventData === null || eventData === undefined) {
                parentUpdateCallback = this._parentPageEventHandlersDictionary[eventType]();
            } else {
                //passing data to parent event
                parentUpdateCallback = this._parentPageEventHandlersDictionary[eventType](eventData);
            }

            parentUpdateCallback.then((data: any) => {
                //executing in the then branch because parent may chose to prevent default action to be executed
                action();
            });

        } else {
            action();
        }
    }
}
export = uscDomainUserSelRest;