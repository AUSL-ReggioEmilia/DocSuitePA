/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Models/Reports/ReportBuilderConditionModel", "App/Models/Reports/ReportBuilderPropertyModel"], function (require, exports, ReportBuilderConditionModel, ReportBuilderPropertyModel) {
    var uscReportDesignerToolbox = /** @class */ (function () {
        function uscReportDesignerToolbox() {
            var _this = this;
            /**
             *------------------------- Events -----------------------------
             */
            this.rtvToolBox_NodeDropping = function (sender, args) {
                var sourceNode = args.get_sourceNode();
                if (!sourceNode) {
                    return;
                }
                if (sourceNode.get_value() == uscReportDesignerToolbox.ROOT_TOOLBOX_ITEM_VALUE) {
                    return;
                }
                var itemData = $(sourceNode.get_element()).data();
                if (itemData.Children && itemData.Children.length > 0) {
                    return;
                }
                $("#".concat(_this.pnlContentId)).trigger(uscReportDesignerToolbox.ON_NODE_DROPPING, [sender, args, itemData]);
            };
        }
        /**
         *------------------------- Methods -----------------------------
         */
        uscReportDesignerToolbox.prototype.initialize = function () {
            this._rtvToolBox = $find(this.rtvToolBoxId);
            this._rtvToolBox.add_nodeDropping(this.rtvToolBox_NodeDropping);
            this.completeLoad();
        };
        uscReportDesignerToolbox.prototype.completeLoad = function () {
            $("#".concat(this.pnlContentId)).data(this);
            $("#".concat(this.pnlContentId)).trigger(uscReportDesignerToolbox.ON_END_LOAD_EVENT);
        };
        uscReportDesignerToolbox.prototype.loadToolbox = function (toolboxItems) {
            try {
                this._rtvToolBox.get_nodes().clear();
                this.loadToolboxInternal(toolboxItems);
            }
            catch (e) {
                console.error(e);
                this.showNotification("Errore in fase di caricamento elementi della toolbox.");
            }
        };
        uscReportDesignerToolbox.prototype.loadToolboxInternal = function (toolboxItems) {
            var node;
            for (var _i = 0, toolboxItems_1 = toolboxItems; _i < toolboxItems_1.length; _i++) {
                var toolboxItem = toolboxItems_1[_i];
                node = new Telerik.Web.UI.RadTreeNode();
                node.set_text(toolboxItem.Description);
                node.set_value(uscReportDesignerToolbox.ROOT_TOOLBOX_ITEM_VALUE);
                node.set_imageUrl(toolboxItem.IconUrl);
                node.expand();
                this._rtvToolBox.get_nodes().add(node);
                this.loadReportItemRecursive(toolboxItem.ReportItems, node);
            }
        };
        uscReportDesignerToolbox.prototype.loadReportItemRecursive = function (reportItems, parent) {
            var internalNode;
            for (var _i = 0, reportItems_1 = reportItems; _i < reportItems_1.length; _i++) {
                var item = reportItems_1[_i];
                internalNode = new Telerik.Web.UI.RadTreeNode();
                parent.get_nodes().add(internalNode);
                if (item instanceof ReportBuilderPropertyModel) {
                    var description = item.DisplayName;
                    if (!description) {
                        description = item.Name;
                    }
                    internalNode.set_text(description);
                    internalNode.set_imageUrl(uscReportDesignerToolbox.PROJECTION_ICON_URL);
                    if (item.HasChildren) {
                        this.loadReportItemRecursive(item.Children, internalNode);
                    }
                }
                else if (item instanceof ReportBuilderConditionModel) {
                    internalNode.set_text(item.ConditionName);
                    internalNode.set_imageUrl(uscReportDesignerToolbox.CONDITION_ICON_URL);
                }
                $(internalNode.get_element()).data(item);
            }
        };
        uscReportDesignerToolbox.prototype.showNotification = function (message) {
            var uscNotification = $("#".concat(this.uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(message);
            }
        };
        uscReportDesignerToolbox.ON_END_LOAD_EVENT = "onEndLoad";
        uscReportDesignerToolbox.ON_NODE_DROPPING = "onNodeDropping";
        uscReportDesignerToolbox.ROOT_TOOLBOX_ITEM_VALUE = "roottoolbox";
        uscReportDesignerToolbox.CONDITION_ICON_URL = "../App_Themes/DocSuite2008/imgset16/conditions_editor.png";
        uscReportDesignerToolbox.PROJECTION_ICON_URL = "../App_Themes/DocSuite2008/imgset16/extended_property.png";
        return uscReportDesignerToolbox;
    }());
    return uscReportDesignerToolbox;
});
//# sourceMappingURL=uscReportDesignerToolbox.js.map