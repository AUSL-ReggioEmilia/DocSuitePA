/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ReportBuilderConditionModel = require('App/Models/Reports/ReportBuilderConditionModel');
import ReportBuilderPropertyModel = require('App/Models/Reports/ReportBuilderPropertyModel');
import ReportBuilderItem = require('App/Models/Reports/ReportBuilderItem');
import ReportToolboxItemViewModel = require('App/ViewModels/Reports/ReportToolboxItemViewModel');
import UscErrorNotification = require('UserControl/uscErrorNotification');

class uscReportDesignerToolbox {
    rtvToolBoxId: string;
    pnlContentId: string;
    uscNotificationId: string;

    static ON_END_LOAD_EVENT = "onEndLoad";
    static ON_NODE_DROPPING = "onNodeDropping";

    private _rtvToolBox: Telerik.Web.UI.RadTreeView;
    static ROOT_TOOLBOX_ITEM_VALUE = "roottoolbox";
    private static CONDITION_ICON_URL = "../App_Themes/DocSuite2008/imgset16/conditions_editor.png";
    private static PROJECTION_ICON_URL = "../App_Themes/DocSuite2008/imgset16/extended_property.png";

    constructor() {

    }

    /**
     *------------------------- Events -----------------------------
     */
    private rtvToolBox_NodeDropping = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeDroppingEventArgs) => {
        let sourceNode: Telerik.Web.UI.RadTreeNode = (<any>args).get_sourceNode();
        if (!sourceNode) {
            return;
        }

        if (sourceNode.get_value() == uscReportDesignerToolbox.ROOT_TOOLBOX_ITEM_VALUE) {
            return;
        }

        let itemData: any | ReportBuilderItem = $(sourceNode.get_element()).data();
        if (itemData.Children && itemData.Children.length > 0) {
            return;
        }
        $("#".concat(this.pnlContentId)).trigger(uscReportDesignerToolbox.ON_NODE_DROPPING, [sender, args, itemData]);
    };

    /**
     *------------------------- Methods -----------------------------
     */
    initialize() {
        this._rtvToolBox = $find(this.rtvToolBoxId) as Telerik.Web.UI.RadTreeView;
        this._rtvToolBox.add_nodeDropping(this.rtvToolBox_NodeDropping);

        this.completeLoad();
    }

    private completeLoad() {
        $("#".concat(this.pnlContentId)).data(this);
        $("#".concat(this.pnlContentId)).trigger(uscReportDesignerToolbox.ON_END_LOAD_EVENT);
    }

    loadToolbox(toolboxItems: ReportToolboxItemViewModel[]) {
        try {
            this._rtvToolBox.get_nodes().clear();
            this.loadToolboxInternal(toolboxItems);
        } catch (e) {
            console.error(e);
            this.showNotification("Errore in fase di caricamento elementi della toolbox.");
        }
    }

    private loadToolboxInternal(toolboxItems: ReportToolboxItemViewModel[]) {
        let node: Telerik.Web.UI.RadTreeNode;
        for (let toolboxItem of toolboxItems) {
            node = new Telerik.Web.UI.RadTreeNode();
            node.set_text(toolboxItem.Description);
            node.set_value(uscReportDesignerToolbox.ROOT_TOOLBOX_ITEM_VALUE);
            node.set_imageUrl(toolboxItem.IconUrl);
            node.expand();
            this._rtvToolBox.get_nodes().add(node);
            this.loadReportItemRecursive(toolboxItem.ReportItems, node);
        }
    }

    private loadReportItemRecursive(reportItems: ReportBuilderItem[], parent: Telerik.Web.UI.RadTreeNode) {
        let internalNode: Telerik.Web.UI.RadTreeNode;
        for (let item of reportItems) {
            internalNode = new Telerik.Web.UI.RadTreeNode();
            parent.get_nodes().add(internalNode);
            if (item instanceof ReportBuilderPropertyModel) {
                let description: string = item.DisplayName;
                if (!description) {
                    description = item.Name;
                }
                internalNode.set_text(description);
                internalNode.set_imageUrl(uscReportDesignerToolbox.PROJECTION_ICON_URL);
                if (item.HasChildren) {
                    this.loadReportItemRecursive(item.Children, internalNode);
                }
            } else if (item instanceof ReportBuilderConditionModel) {
                internalNode.set_text(item.ConditionName);
                internalNode.set_imageUrl(uscReportDesignerToolbox.CONDITION_ICON_URL);
            }
            $(internalNode.get_element()).data(item);
        }
    }

    private showNotification(message: string): void {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(message);
        }
    }
}

export = uscReportDesignerToolbox;