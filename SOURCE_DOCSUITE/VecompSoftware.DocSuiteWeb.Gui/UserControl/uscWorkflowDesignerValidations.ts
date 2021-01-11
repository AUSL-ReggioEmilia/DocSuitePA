import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import EnumHelper = require("App/Helpers/EnumHelper");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import WorkflowRule = require("App/Models/Workflows/WorkflowRule");
import WorkflowRuleDefinitionModel = require("App/Models/Workflows/WorkflowRuleDefinitionModel");
import DSWEnvironmentType = require("App/Models/Workflows/WorkflowDSWEnvironmentType");
import SessionStorageKeysHelper = require("App/Helpers/SessionStorageKeysHelper");

class uscWorkflowDesignerValidations {
    actionToolbarId: string;
    rtvValidationRulesId: string;
    radWindowManagerId: string;
    pageContentId: string;
    lblCaptionId: string;
    tblHeaderId: string;
    lblDSWMessageContainerId: string;
    workflowRules: WorkflowRule[] = [];

    private _serviceConfigurations: ServiceConfiguration[];
    private _enumHelper: EnumHelper;
    private _actionToolbar: Telerik.Web.UI.RadToolBar;
    private _rtvValidationRules: Telerik.Web.UI.RadTreeView;
    private _lblCaption: JQuery;
    private _lblDSWMessage: JQuery;
    private _tblHeader: JQuery;
    private _lblDSWMessageContainer: JQuery;

    private static ADD_ACTION: string = "create";
    private static MODIFY_ACTION: string = "modify";
    private static DELETE_ACTION: string = "delete";

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._enumHelper = new EnumHelper();
    }

    initialize(): void {
        $(`#${this.pageContentId}`).data(this);
        this._lblCaption = $("#".concat(this.lblCaptionId));
        this._actionToolbar = $find(this.actionToolbarId) as Telerik.Web.UI.RadToolBar;
        this._actionToolbar.add_buttonClicked(this.actionToolbar_ButtonClicked);
        this._rtvValidationRules = <Telerik.Web.UI.RadTreeView>$find(this.rtvValidationRulesId);
        this._rtvValidationRules.add_nodeClicking(this.rtvValidationRule_NodeClicking);
        this._rtvValidationRules.get_nodes().getNode(0).get_nodes().clear();
        this._tblHeader = $(`#${this.tblHeaderId}`);
        this._lblDSWMessageContainer = $(`#${this.lblDSWMessageContainerId}`);
    }

    rtvValidationRule_NodeClicking = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs) => {
        this.setButtonEnable(false);

        if (args.get_node().get_level() == 0) {
            this._actionToolbar.findItemByValue(uscWorkflowDesignerValidations.ADD_ACTION).set_enabled(true);
        }

        if (args.get_node().get_level() == 1) {
            sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_WORKFLOW_RULE_MODEL, JSON.stringify(this.workflowRules.filter(x => x.Name == args.get_node().get_text())[0]));
            this._actionToolbar.findItemByValue(uscWorkflowDesignerValidations.MODIFY_ACTION).set_enabled(true);
            this._actionToolbar.findItemByValue(uscWorkflowDesignerValidations.DELETE_ACTION).set_enabled(true);
        }
    }

    private setButtonEnable(isEnable: boolean) {
        this._actionToolbar.findItemByValue(uscWorkflowDesignerValidations.ADD_ACTION).set_enabled(isEnable);
        this._actionToolbar.findItemByValue(uscWorkflowDesignerValidations.MODIFY_ACTION).set_enabled(isEnable);
        this._actionToolbar.findItemByValue(uscWorkflowDesignerValidations.DELETE_ACTION).set_enabled(isEnable);
    }

    protected actionToolbar_ButtonClicked = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        switch (args.get_item().get_value()) {
            case uscWorkflowDesignerValidations.ADD_ACTION: {
                sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_WORKFLOW_RULE_MODEL);
                var url = `../UserControl/CommonWorkflowDesignerValidations.aspx?Type=Fasc&ManagerID=${this.radWindowManagerId}&Callback${window.location.href}&PageContentId=${this.pageContentId}`;
                this.openWindow(url, "windowSelValidation", 450, 300, this.closeWindowCallback);
                break;
            }
            case uscWorkflowDesignerValidations.MODIFY_ACTION: {
                let wfRule: Telerik.Web.UI.RadTreeNode = this._rtvValidationRules.get_selectedNode();
                let selectedValidationNames: string[] = [];
                for (let i = 0; i < wfRule.get_nodes().get_count(); i++) {
                    selectedValidationNames.push(wfRule.get_nodes().getNode(i).get_text());
                }
                var url = `../UserControl/CommonWorkflowDesignerValidations.aspx?Type=Fasc&ManagerID=${this.radWindowManagerId}&Callback${window.location.href}&PageContentId=${this.pageContentId}&SelectedKeys=${JSON.stringify(selectedValidationNames)}`;
                this.openWindow(url, "windowSelModifyValidation", 450, 300, this.closeModifyWindowCallback);

                break;
            }
            case uscWorkflowDesignerValidations.DELETE_ACTION: {
                sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_WORKFLOW_RULE_MODEL);
                this.workflowRules = this.workflowRules.filter(x => x.Name != this._rtvValidationRules.get_selectedNode().get_text());
                this._rtvValidationRules.get_selectedNode().get_parent().get_nodes().remove(this._rtvValidationRules.get_selectedNode());
                break;
            }
        }
    }

    closeModifyWindowCallback = (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (!args.get_argument()) {
            return;
        }

        let validationRule: WorkflowRule = args.get_argument();
        this.workflowRules = this.workflowRules.filter(x => x.Name != this._rtvValidationRules.get_selectedNode().get_text());
        this.workflowRules.push(validationRule);

        this._rtvValidationRules.get_selectedNode().set_text(validationRule.Name);
        this._rtvValidationRules.get_selectedNode().get_nodes().clear();
        this.createFilterNode(this._rtvValidationRules.get_selectedNode(), validationRule);        
        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_WORKFLOW_RULE_MODEL, JSON.stringify(validationRule));
    }

    closeWindowCallback = (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (!args.get_argument()) {
            return;
        }

        let wfRuleList: WorkflowRule[] = [];
        let validationRule: WorkflowRule = args.get_argument();
        wfRuleList.push(validationRule);

        this.createValidationTree(wfRuleList);
    }

    private openWindow(url, name, width, height, onCloseCallback?): boolean {
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

    createValidationTree = (workflowRule: WorkflowRule[]) => {
        for (let item of workflowRule) {
            let node: Telerik.Web.UI.RadTreeNode = this.createNode(this._rtvValidationRules.get_nodes().getNode(0), item.Name, "");
            this.createFilterNode(node, item);
            this.workflowRules.push(item);
        }
    }

    private createFilterNode(node: Telerik.Web.UI.RadTreeNode, item: WorkflowRule) {
        if (item.HasFile) {
            this.createNode(node, this._enumHelper.getValidationRuleDescription("HasFile"), "../App_Themes/DocSuite2008/imgset16/validation_checkIcon.png")
        }

        if (item.HasSignedFile) {
            this.createNode(node, this._enumHelper.getValidationRuleDescription("HasSignedFile"), "../App_Themes/DocSuite2008/imgset16/validation_checkIcon.png")
        }

        if (item.HasDocumentUnit) {
            this.createNode(node, this._enumHelper.getValidationRuleDescription("HasDocumentUnit"), "../App_Themes/DocSuite2008/imgset16/validation_checkIcon.png")
        }

        if (item.IsExist) {
            this.createNode(node, this._enumHelper.getValidationRuleDescription("IsExist"), "../App_Themes/DocSuite2008/imgset16/validation_checkIcon.png")
        }
    }

    private createNode(parentNode: Telerik.Web.UI.RadTreeNode, text: string, imageUrl: string) {
        let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        node.set_text(text);
        node.set_cssClass("font_node");
        node.set_imageUrl(imageUrl);
        parentNode.set_expanded(true);
        parentNode.get_nodes().add(node);

        return node;
    }

    displayDisableEnvironmentMessage(): void {
        $(`#${this.rtvValidationRulesId}`).hide();
        $(`#${this.actionToolbarId}`).hide();
        $(`#${this.lblCaptionId}`).hide();
        $(`#${this.tblHeaderId}`).hide();
        $(`#${this.lblDSWMessageContainerId}`).show();
    }

    getWorkflowRulesModel = (workflowEnv: string) => {
        let workflowRuleDefinition: WorkflowRuleDefinitionModel = <WorkflowRuleDefinitionModel>{ Environment: DSWEnvironmentType[workflowEnv], Rules: this.workflowRules }
        let jsonDefinition = { [workflowEnv]: workflowRuleDefinition }
        return JSON.stringify(jsonDefinition);
    }
}
export = uscWorkflowDesignerValidations;