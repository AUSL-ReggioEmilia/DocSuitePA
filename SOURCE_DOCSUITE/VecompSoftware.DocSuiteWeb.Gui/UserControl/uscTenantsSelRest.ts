import UscErrorNotification = require("UserControl/uscErrorNotification");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import EnumHelper = require("App/Helpers/EnumHelper");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import TenantAOOService = require('App/Services/Tenants/TenantAOOService');
import TenantAOOModel = require('App/Models/Tenants/TenantAOOModel');
import TenantModelSelection = require("App/Models/Tenants/TenantModelSelection");

class uscTenantsSelRest {
    rddtTenantTreeId: string;
    currentTenantId: string;
    uscNotificationId: string;
    tenants: TenantModelSelection[] = [];
    pageContentId: string;

    private _rddtTenantTree: Telerik.Web.UI.RadDropDownTree;
    private _rtvTenantTree: Telerik.Web.UI.RadTreeView;
    private _serviceConfigurations: ServiceConfiguration[];
    private _enumHelper: EnumHelper;
    protected _tenantAOOService: TenantAOOService;

    protected static TenantAOO_TYPE_NAME = "TenantAOO";
    public static SESSION_TENANT_SELECTION_MODEL;
    public static TenantAOO_Name = "TenantAOOName";
    public static TenantAOO_Id = "TenantAOOId";
    public static TENANT_CHANGE_EVENT: string = "OnTenantChange";

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._enumHelper = new EnumHelper();
    }

    initialize(): void {
        let tenantAOOConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, uscTenantsSelRest.TenantAOO_TYPE_NAME);
        this._tenantAOOService = new TenantAOOService(tenantAOOConfiguration);

        this._rddtTenantTree = <Telerik.Web.UI.RadDropDownTree>$find(this.rddtTenantTreeId);
        this._rtvTenantTree = this._rddtTenantTree.get_embeddedTree();
        this._rtvTenantTree.add_nodeClicked(this.rtvTenantTree_OnClick);

        this._rddtTenantTree.add_entryAdded(this.add_entryAdded);
        this._rddtTenantTree.add_entryRemoved(this.add_entryRemoved);

        uscTenantsSelRest.SESSION_TENANT_SELECTION_MODEL = `TenantModelSelection_${this.rddtTenantTreeId}`;
        this.populateDropdownTree(this.currentTenantId);

        $(`#${this.rddtTenantTreeId}`).data(this);
    }

    rtvTenantTree_OnClick = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs): void => {
        let tenantaooId: string = args.get_node().get_level() == 1 ? args.get_node().get_parent().get_value() : null;
        $(`#${this.pageContentId}`).triggerHandler(uscTenantsSelRest.TENANT_CHANGE_EVENT, tenantaooId);
    }

    public hasValue(): boolean {
        return this._rddtTenantTree.get_selectedValue() != "";
    }

    add_entryRemoved = (sender: Telerik.Web.UI.RadDropDownTree, args: Telerik.Web.UI.DropDownTreeEntryRemovedEventArgs) => {
        let selected = args.get_entry();

        if (sessionStorage.getItem(uscTenantsSelRest.SESSION_TENANT_SELECTION_MODEL) == null ||
            sessionStorage.getItem(uscTenantsSelRest.SESSION_TENANT_SELECTION_MODEL).length == 0) {
            return
        } else {
            let currentTenantsFromSession = JSON.parse(sessionStorage.getItem(uscTenantsSelRest.SESSION_TENANT_SELECTION_MODEL));
            if (currentTenantsFromSession.some(t => t.IdTenant === selected.get_value())) {
                this.tenants = currentTenantsFromSession.filter(t => t.IdTenant !== selected.get_value());
            } else {
                return;
            }
        }

        sessionStorage.setItem(uscTenantsSelRest.SESSION_TENANT_SELECTION_MODEL, JSON.stringify(this.tenants));
    }

    add_entryAdded = (sender: Telerik.Web.UI.RadDropDownTree, args: Telerik.Web.UI.DropDownTreeEntryAddedEventArgs) => {
        let selected = args.get_node();

        if (selected.get_level() == 0) {
            selected.unselect();
            this._rddtTenantTree.get_entries().clear();
            return;
        }

        let tenant: TenantModelSelection = {
            IdTenant: selected.get_value(),
            IdTenantAOO: selected.get_attributes().getAttribute(uscTenantsSelRest.TenantAOO_Id),
            TenantName: selected.get_text(),
            TenantAOOName: selected.get_attributes().getAttribute(uscTenantsSelRest.TenantAOO_Name)
        }

        if (sessionStorage.getItem(uscTenantsSelRest.SESSION_TENANT_SELECTION_MODEL) == null ||
            sessionStorage.getItem(uscTenantsSelRest.SESSION_TENANT_SELECTION_MODEL).length == 0) {
            this.tenants.push(tenant);
        } else {
            let currentTenantsFromSession = JSON.parse(sessionStorage.getItem(uscTenantsSelRest.SESSION_TENANT_SELECTION_MODEL));
            if (currentTenantsFromSession.some(t => t.IdTenant === tenant.IdTenant)) {
                return;
            } else {
                this.tenants.push(tenant);
            }
        }

        sessionStorage.setItem(uscTenantsSelRest.SESSION_TENANT_SELECTION_MODEL, JSON.stringify(this.tenants));
    }

    private populateDropdownTree(uniqueId: string) {
        this._tenantAOOService.getTenantsWithoutCurrentTenant(uniqueId, (data: TenantAOOModel[]) => {
            var elements: any = this._rddtTenantTree.get_embeddedTree();
            elements.trackChanges();
            let node: Telerik.Web.UI.RadTreeNode;
            let tenantNode: Telerik.Web.UI.RadTreeNode;
            for (let i = 0; i < data.length; i++) {
                let tenantAOO: TenantAOOModel = data[i];
                node = new Telerik.Web.UI.RadTreeNode();
                node.set_text(tenantAOO.Name);
                node.set_value(tenantAOO.UniqueId);
                node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/tenantAOO.png");
                node.set_checkable(false);
                elements.get_nodes().add(node);

                for (let j = 0; j < tenantAOO.Tenants.length; j++) {
                    tenantNode = new Telerik.Web.UI.RadTreeNode();
                    tenantNode.set_text(tenantAOO.Tenants[j].TenantName);
                    tenantNode.set_value(tenantAOO.Tenants[j].UniqueId);
                    tenantNode.get_attributes().setAttribute(uscTenantsSelRest.TenantAOO_Id, tenantAOO.UniqueId);
                    tenantNode.get_attributes().setAttribute(uscTenantsSelRest.TenantAOO_Name, tenantAOO.Name);
                    node.get_nodes().add(tenantNode);
                }
            }
            elements.commitChanges();
        }, (exception: ExceptionDTO) => {
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
}

export =uscTenantsSelRest;