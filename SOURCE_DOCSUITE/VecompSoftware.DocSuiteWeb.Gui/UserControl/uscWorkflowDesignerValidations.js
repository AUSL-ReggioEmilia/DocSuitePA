define(["require", "exports", "App/Helpers/EnumHelper", "App/Models/Workflows/WorkflowDSWEnvironmentType", "App/Helpers/SessionStorageKeysHelper"], function (require, exports, EnumHelper, DSWEnvironmentType, SessionStorageKeysHelper) {
    var uscWorkflowDesignerValidations = /** @class */ (function () {
        function uscWorkflowDesignerValidations(serviceConfigurations) {
            var _this = this;
            this.workflowRules = [];
            this.rtvValidationRule_NodeClicking = function (sender, args) {
                _this.setButtonEnable(false);
                if (args.get_node().get_level() == 0) {
                    _this._actionToolbar.findItemByValue(uscWorkflowDesignerValidations.ADD_ACTION).set_enabled(true);
                }
                if (args.get_node().get_level() == 1) {
                    sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_WORKFLOW_RULE_MODEL, JSON.stringify(_this.workflowRules.filter(function (x) { return x.Name == args.get_node().get_text(); })[0]));
                    _this._actionToolbar.findItemByValue(uscWorkflowDesignerValidations.MODIFY_ACTION).set_enabled(true);
                    _this._actionToolbar.findItemByValue(uscWorkflowDesignerValidations.DELETE_ACTION).set_enabled(true);
                }
            };
            this.actionToolbar_ButtonClicked = function (sender, args) {
                switch (args.get_item().get_value()) {
                    case uscWorkflowDesignerValidations.ADD_ACTION: {
                        sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_WORKFLOW_RULE_MODEL);
                        var url = "../UserControl/CommonWorkflowDesignerValidations.aspx?Type=Fasc&ManagerID=" + _this.radWindowManagerId + "&Callback" + window.location.href + "&PageContentId=" + _this.pageContentId;
                        _this.openWindow(url, "windowSelValidation", 450, 300, _this.closeWindowCallback);
                        break;
                    }
                    case uscWorkflowDesignerValidations.MODIFY_ACTION: {
                        var wfRule = _this._rtvValidationRules.get_selectedNode();
                        var selectedValidationNames = [];
                        for (var i = 0; i < wfRule.get_nodes().get_count(); i++) {
                            selectedValidationNames.push(wfRule.get_nodes().getNode(i).get_text());
                        }
                        var url = "../UserControl/CommonWorkflowDesignerValidations.aspx?Type=Fasc&ManagerID=" + _this.radWindowManagerId + "&Callback" + window.location.href + "&PageContentId=" + _this.pageContentId + "&SelectedKeys=" + JSON.stringify(selectedValidationNames);
                        _this.openWindow(url, "windowSelModifyValidation", 450, 300, _this.closeModifyWindowCallback);
                        break;
                    }
                    case uscWorkflowDesignerValidations.DELETE_ACTION: {
                        sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_WORKFLOW_RULE_MODEL);
                        _this.workflowRules = _this.workflowRules.filter(function (x) { return x.Name != _this._rtvValidationRules.get_selectedNode().get_text(); });
                        _this._rtvValidationRules.get_selectedNode().get_parent().get_nodes().remove(_this._rtvValidationRules.get_selectedNode());
                        break;
                    }
                }
            };
            this.closeModifyWindowCallback = function (sender, args) {
                if (!args.get_argument()) {
                    return;
                }
                var validationRule = args.get_argument();
                _this.workflowRules = _this.workflowRules.filter(function (x) { return x.Name != _this._rtvValidationRules.get_selectedNode().get_text(); });
                _this.workflowRules.push(validationRule);
                _this._rtvValidationRules.get_selectedNode().set_text(validationRule.Name);
                _this._rtvValidationRules.get_selectedNode().get_nodes().clear();
                _this.createFilterNode(_this._rtvValidationRules.get_selectedNode(), validationRule);
                sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_WORKFLOW_RULE_MODEL, JSON.stringify(validationRule));
            };
            this.closeWindowCallback = function (sender, args) {
                if (!args.get_argument()) {
                    return;
                }
                var wfRuleList = [];
                var validationRule = args.get_argument();
                wfRuleList.push(validationRule);
                _this.createValidationTree(wfRuleList);
            };
            this.createValidationTree = function (workflowRule) {
                for (var _i = 0, workflowRule_1 = workflowRule; _i < workflowRule_1.length; _i++) {
                    var item = workflowRule_1[_i];
                    var node = _this.createNode(_this._rtvValidationRules.get_nodes().getNode(0), item.Name, "");
                    _this.createFilterNode(node, item);
                    _this.workflowRules.push(item);
                }
            };
            this.getWorkflowRulesModel = function (workflowEnv) {
                var _a;
                var workflowRuleDefinition = { Environment: DSWEnvironmentType[workflowEnv], Rules: _this.workflowRules };
                var jsonDefinition = (_a = {}, _a[workflowEnv] = workflowRuleDefinition, _a);
                return JSON.stringify(jsonDefinition);
            };
            this._serviceConfigurations = serviceConfigurations;
            this._enumHelper = new EnumHelper();
        }
        uscWorkflowDesignerValidations.prototype.initialize = function () {
            $("#" + this.pageContentId).data(this);
            this._lblCaption = $("#".concat(this.lblCaptionId));
            this._actionToolbar = $find(this.actionToolbarId);
            this._actionToolbar.add_buttonClicked(this.actionToolbar_ButtonClicked);
            this._rtvValidationRules = $find(this.rtvValidationRulesId);
            this._rtvValidationRules.add_nodeClicking(this.rtvValidationRule_NodeClicking);
            this._rtvValidationRules.get_nodes().getNode(0).get_nodes().clear();
            this._tblHeader = $("#" + this.tblHeaderId);
            this._lblDSWMessageContainer = $("#" + this.lblDSWMessageContainerId);
        };
        uscWorkflowDesignerValidations.prototype.setButtonEnable = function (isEnable) {
            this._actionToolbar.findItemByValue(uscWorkflowDesignerValidations.ADD_ACTION).set_enabled(isEnable);
            this._actionToolbar.findItemByValue(uscWorkflowDesignerValidations.MODIFY_ACTION).set_enabled(isEnable);
            this._actionToolbar.findItemByValue(uscWorkflowDesignerValidations.DELETE_ACTION).set_enabled(isEnable);
        };
        uscWorkflowDesignerValidations.prototype.openWindow = function (url, name, width, height, onCloseCallback) {
            var manager = $find(this.radWindowManagerId);
            var wnd = manager.open(url, name, null);
            wnd.setSize(width, height);
            wnd.set_modal(true);
            wnd.center();
            if (onCloseCallback) {
                wnd.remove_close(onCloseCallback);
                wnd.add_close(onCloseCallback);
            }
            return false;
        };
        uscWorkflowDesignerValidations.prototype.createFilterNode = function (node, item) {
            if (item.HasFile) {
                this.createNode(node, this._enumHelper.getValidationRuleDescription("HasFile"), "../App_Themes/DocSuite2008/imgset16/validation_checkIcon.png");
            }
            if (item.HasSignedFile) {
                this.createNode(node, this._enumHelper.getValidationRuleDescription("HasSignedFile"), "../App_Themes/DocSuite2008/imgset16/validation_checkIcon.png");
            }
            if (item.HasDocumentUnit) {
                this.createNode(node, this._enumHelper.getValidationRuleDescription("HasDocumentUnit"), "../App_Themes/DocSuite2008/imgset16/validation_checkIcon.png");
            }
            if (item.IsExist) {
                this.createNode(node, this._enumHelper.getValidationRuleDescription("IsExist"), "../App_Themes/DocSuite2008/imgset16/validation_checkIcon.png");
            }
        };
        uscWorkflowDesignerValidations.prototype.createNode = function (parentNode, text, imageUrl) {
            var node = new Telerik.Web.UI.RadTreeNode();
            node.set_text(text);
            node.set_cssClass("font_node");
            node.set_imageUrl(imageUrl);
            parentNode.set_expanded(true);
            parentNode.get_nodes().add(node);
            return node;
        };
        uscWorkflowDesignerValidations.prototype.displayDisableEnvironmentMessage = function () {
            $("#" + this.rtvValidationRulesId).hide();
            $("#" + this.actionToolbarId).hide();
            $("#" + this.lblCaptionId).hide();
            $("#" + this.tblHeaderId).hide();
            $("#" + this.lblDSWMessageContainerId).show();
        };
        uscWorkflowDesignerValidations.ADD_ACTION = "create";
        uscWorkflowDesignerValidations.MODIFY_ACTION = "modify";
        uscWorkflowDesignerValidations.DELETE_ACTION = "delete";
        return uscWorkflowDesignerValidations;
    }());
    return uscWorkflowDesignerValidations;
});
//# sourceMappingURL=uscWorkflowDesignerValidations.js.map