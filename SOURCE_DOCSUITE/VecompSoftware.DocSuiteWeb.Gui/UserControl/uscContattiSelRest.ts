import ContactModel = require("../App/Models/Commons/ContactModel");
import ContactService = require("../App/Services/Commons/ContactService");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import uscContactRest = require("UserControl/uscContactREST");
import ExceptionDTO = require("../App/DTOs/ExceptionDTO");

enum UscContattiSelRestEvent {
    NewContactsAdded,
    ContactDeleted
}

enum contactTypeIcons {
    Administration = "../comm/images/interop/Amministrazione.gif",
    AOO = "../comm/images/interop/Aoo.gif",
    AO = "../comm/images/interop/Uo.gif",
    Role = "../comm/images/interop/Ruolo.gif",
    Citizen = "../App_Themes/DocSuite2008/imgset16/user.png",
    Group = "../comm/images/interop/Gruppo.gif",
    Sector = "../App_Themes/DocSuite2008/imgset16/GroupMembers.png",
    M = Administration,
    A = AOO,
    U = AO,
    R = Role,
    P = Citizen,
    G = Group,
    S = Sector
}

declare var ValidatorEnable: any;
class uscContactSelRest {
    public treeContactId: string;
    public pnlContentId: string;
    public uscContactRestId: string;
    public tbContactsControlId: string;
    public rwContactSelectorId: string;
    public btnContactConfirmId: string;
    public btnContactConfirmAndNewId: string;
    public validatorAnyNodeId: string;
    public requiredValidationEnabled: string;
    public filterByParentId?: number;

    public uscContattiSelRestEvents = UscContattiSelRestEvent;

    private _treeContact: Telerik.Web.UI.RadTreeView;
    private _rwContactSelector: Telerik.Web.UI.RadWindow;
    private _btnContactConfirm: Telerik.Web.UI.RadButton;
    private _btnContactConfirmAndNew: Telerik.Web.UI.RadButton;
    private _tbContactsControl: Telerik.Web.UI.RadToolBar;

    private get _requiredValidationEnabled(): boolean {
        return JSON.parse(this.requiredValidationEnabled.toLowerCase());
    }


    private readonly _contactService: ContactService;
    private _serviceConfiguration: ServiceConfiguration[];
    private _uscContactRest: uscContactRest;

    private _parentPageEventHandlersDictionary:
        {
            eventType: UscContattiSelRestEvent,
            promiseCallback: (data: any) => JQueryPromise<any>
        } = {} as {
            eventType: UscContattiSelRestEvent,
            promiseCallback: (data: any) => JQueryPromise<any>
        };

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        let contactServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Contact");
        this._contactService = new ContactService(contactServiceConfiguration);
    }

    initialize(): void {
        this._treeContact = $find(this.treeContactId) as Telerik.Web.UI.RadTreeView;
        this._tbContactsControl = $find(this.tbContactsControlId) as Telerik.Web.UI.RadToolBar;
        if (this._tbContactsControl) {
            this._tbContactsControl.add_buttonClicking(this.toolbarContacts_onClick);
        }
        this._rwContactSelector = $find(this.rwContactSelectorId) as Telerik.Web.UI.RadWindow;
        this._rwContactSelector.add_show(this._rwContactSelector_show);
        this._btnContactConfirm = $find(this.btnContactConfirmId) as Telerik.Web.UI.RadButton;
        this._btnContactConfirm.add_clicking(this.btnContactConfirm_onClick);
        this._btnContactConfirmAndNew = $find(this.btnContactConfirmAndNewId) as Telerik.Web.UI.RadButton;
        this._btnContactConfirmAndNew.add_clicking(this.btnContactConfirmAndNew_onClick);

        this._uscContactRest = <uscContactRest>$(`#${this.uscContactRestId}`).data();
        $(`#${this.pnlContentId}`).data(this);
    }

    public registerEventHandler = (eventType: UscContattiSelRestEvent, callback: (data: any) => JQueryPromise<any>): void => {
        this._parentPageEventHandlersDictionary[eventType] = callback;
    }

    public renderContactsTree(contactCollection: ContactModel[]): void {
        this.enableValidators(contactCollection.length === 0 && this._requiredValidationEnabled ? true : false);
        this.populateContactsTreeView(contactCollection);
    }

    toolbarContacts_onClick = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        let btn = args.get_item();
        switch (btn.get_index()) {
            case 0:
                this._rwContactSelector.show();
                break;
            case 1:
                this.deleteContacts();
                break;
        }
    }

    btnContactConfirm_onClick = (sender: any, args: any) => {
        let contact: ContactModel = this._uscContactRest.getLastSearchedContactModel();
        if (contact) {
            this._rwContactSelector.close();
            let parentUpdateCallback: JQueryPromise<any> = this._parentPageEventHandlersDictionary[this.uscContattiSelRestEvents.NewContactsAdded](contact);
            parentUpdateCallback.then((data: any) => {
                this.createNode(contact);
            });
        }
    }

    btnContactConfirmAndNew_onClick = (sender: any, args: any) => {
        this.createContact().then(([rubrica, newlyAddedContact]) => {
            if (newlyAddedContact) {
                let parentUpdateCallback: JQueryPromise<any> = this._parentPageEventHandlersDictionary[this.uscContattiSelRestEvents.NewContactsAdded](newlyAddedContact);
                parentUpdateCallback.then((data: any) => {
                    this.createNode(newlyAddedContact);
                });
            }
            this._uscContactRest.clear();
        });
    }

    _rwContactSelector_show = (sender: any, args: any) => {
        this._uscContactRest.clear();
    }

    private createContact(): JQueryPromise<[string, ContactModel]> {
        let promise: JQueryDeferred<[string, ContactModel]> = $.Deferred<[string, ContactModel]>();
        if (!jQuery.isEmptyObject(this._uscContactRest)) {
            this._uscContactRest.createContact()
                .done((data: [string, ContactModel]) => {
                    promise.resolve(data);
                })
                .fail((exception: (string | ExceptionDTO)) => promise.reject(exception));
        }
        return promise.promise();
    }

    private deleteContacts = (): void => {
        let selectedNodeToDelete: Telerik.Web.UI.RadTreeNode = this._treeContact.get_selectedNode();
        if (selectedNodeToDelete) {
            let parentDeleteCallback: JQueryPromise<any> = this._parentPageEventHandlersDictionary[this.uscContattiSelRestEvents.ContactDeleted](+selectedNodeToDelete.get_value());
            parentDeleteCallback.then(
                (contactParentId: any) => {
                    this._treeContact.trackChanges();
                    selectedNodeToDelete.get_parent().get_nodes().removeAt(selectedNodeToDelete.get_index());
                    let selectedParentToDelete: Telerik.Web.UI.RadTreeNode = this._treeContact.get_allNodes().filter(node => +node.get_value() === contactParentId)[0];
                    if (!selectedParentToDelete.get_allNodes().length && selectedParentToDelete.get_contentCssClass() == 'initial') {
                        this._treeContact.get_nodes().remove(selectedParentToDelete);
                    }

                    let treeHasChildrenLeft: boolean = this._treeContact.get_allNodes().length > 0;

                    if (!treeHasChildrenLeft && this._requiredValidationEnabled) {
                        this.enableValidators(true);
                    }

                    this._treeContact.commitChanges();
                }
            );
        } else {
            alert("Selezionare un contatto");
        }
    }

    private createNode(contactModel: any): JQueryPromise<Telerik.Web.UI.RadTreeNode> {
        if (this._requiredValidationEnabled)
            this.enableValidators(false);

        let promise: JQueryDeferred<Telerik.Web.UI.RadTreeNode> = $.Deferred<Telerik.Web.UI.RadTreeNode>();
        let currentNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        let currentNodeDescription: string = contactModel.Description;
        let currentNodeImageUrl: string = `${contactModel.IdContactType || contactModel.ContactType}`;

        currentNode.set_text(currentNodeDescription);
        currentNode.set_value(`${contactModel.EntityId || contactModel.Id}`);
        currentNode.set_imageUrl(contactTypeIcons[currentNodeImageUrl]);
        currentNode.set_expanded(true);
        currentNode.set_contentCssClass('dsw-text-bold');

        let currentNodeFromTree: Telerik.Web.UI.RadTreeNode = this._treeContact.get_allNodes().filter(node => +node.get_value() === +currentNode.get_value())[0];

        if (currentNodeFromTree) {
            return promise.resolve(currentNodeFromTree);
        }

        if (contactModel.IncrementalFather) {
            let parentNode: Telerik.Web.UI.RadTreeNode = this._treeContact.get_allNodes().filter(node => +node.get_value() === contactModel.IncrementalFather)[0];
            if (!parentNode) {
                this._contactService.getContactParents(contactModel.IncrementalFather, (data: ContactModel[]) => {
                    this.createNode(data[0])
                        .done((node) => {
                            node.set_contentCssClass('initial');
                            node.get_nodes().add(currentNode);
                            node.set_expanded(true);
                            promise.resolve(currentNode);
                        });
                });
            } else {
                parentNode.get_nodes().add(currentNode);
            }
        } else {
            this._treeContact.get_nodes().add(currentNode);
            return promise.resolve(currentNode);
        }
        return promise.promise();
    }

    private populateContactsTreeView(contactTreeModels: ContactModel[]): void {
        if (this._treeContact.get_allNodes().length > 0)
            this._treeContact.get_nodes().clear();

        //build and add other tree nodes
        contactTreeModels.forEach((contactModel: ContactModel) => {
            this.createNode(contactModel);
        });
    }

    private enableValidators = (state: boolean) => {
        ValidatorEnable($get(this.validatorAnyNodeId), state);
    }

    public setToolbarVisibility(isVisible: boolean): void {
        this._tbContactsControl.get_items().forEach(function (item: Telerik.Web.UI.RadToolBarItem) {
            item.set_enabled(isVisible);
        });
    }
}

export = uscContactSelRest;