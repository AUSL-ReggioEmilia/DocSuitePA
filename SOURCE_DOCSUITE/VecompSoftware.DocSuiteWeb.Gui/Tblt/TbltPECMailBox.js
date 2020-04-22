var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "Tblt/TbltPECMailBoxBase", "App/Helpers/ServiceConfigurationHelper", "App/Models/PECMails/PECMailBoxRulesetModel"], function (require, exports, TbltPECMailBoxBase, ServiceConfigurationHelper, PECMailBoxRulesetModel) {
    var TbltPECMailBox = /** @class */ (function (_super) {
        __extends(TbltPECMailBox, _super);
        function TbltPECMailBox(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, TbltPECMailBoxBase.PECMailBox_TYPE_NAME)) || this;
            _this.PECMailBox_TYPE_NAME = "PECMailBox";
            _this.toolbarSearch_onClick = function (sender, args) {
                _this._loadingPanel.show(_this.splitterMainId);
                _this.loadResults(_this._txtSearchName.get_textBoxValue());
                _this._rpbDetails.findItemByValue("rpiDetails").set_visible(false);
            };
            _this.rtvPECMailBoxes_onClick = function (sender, args) {
                _this._rpbDetails.findItemByValue("rpiDetails").set_visible(true);
                _this.loadPECMailBoxDetails(args.get_node().get_value());
            };
            _this.btnPECMailBoxSetRule_onClick = function (sender, args) {
                _this._windowSetRule.show();
                _this._loadingPanel.show(_this.windowSetRuleId);
                _this.pecMailBoxService.getPECMailBoxes("", function (data) {
                    if (!data)
                        return;
                    var pecMailBoxesExceptSelected = data.filter(function (x) { return x.EntityShortId !== +_this._lblPECMailBoxId.innerText; });
                    _this._rlbSpecifyPECMailBox.get_items().clear();
                    var rlbItem;
                    var thisObj = _this;
                    $.each(pecMailBoxesExceptSelected, function (i, value) {
                        rlbItem = new Telerik.Web.UI.DropDownListItem();
                        rlbItem.set_text(value.MailBoxRecipient);
                        rlbItem.set_value(value.EntityShortId.toString());
                        thisObj._rlbSpecifyPECMailBox.get_items().add(rlbItem);
                    });
                    _this._loadingPanel.hide(_this.windowSetRuleId);
                }, function (exception) {
                    _this._loadingPanel.hide(_this.windowSetRuleId);
                    $("#".concat(_this.rtvPECMailBoxesId)).hide();
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            };
            _this.btnPECMailBoxSave_onClick = function (sender, args) {
                var thisObj = _this;
                var pecMailBox = _this.rtvResult.filter(function (x) {
                    return x.EntityShortId === +thisObj._lblPECMailBoxId.innerText;
                })[0];
                var ruleset = pecMailBox.RulesetDefinition === null ? new PECMailBoxRulesetModel() : JSON.parse(pecMailBox.RulesetDefinition);
                ruleset.Name = _this._txtRulesetName.get_value();
                pecMailBox.MailBoxRecipient = _this._txtSpecifySender.get_value();
                var selectedPECMailBoxId = +_this._rlbSpecifyPECMailBox.get_selectedItem().get_value();
                var selectedPECMailBox = _this.rtvResult.filter(function (x) {
                    return x.EntityShortId === selectedPECMailBoxId;
                })[0];
                ruleset.Reference = selectedPECMailBox;
                pecMailBox.RulesetDefinition = JSON.stringify(ruleset);
                _this.updateCallback(pecMailBox);
            };
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        TbltPECMailBox.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._toolbarSearch = $find(this.ToolBarSearchId);
            this._toolbarSearch.add_buttonClicking(this.toolbarSearch_onClick);
            this._rtvPECMailBoxes = $find(this.rtvPECMailBoxesId);
            this._rtvPECMailBoxes.add_nodeClicking(this.rtvPECMailBoxes_onClick);
            this._toolbarItemSearchName = this._toolbarSearch.findItemByValue("searchName");
            this._txtSearchName = this._toolbarItemSearchName.findControl("txtName");
            this._toolbarItemButtonSearch = this._toolbarSearch.findItemByValue("searchCommand");
            this._rpbDetails = $find(this.rpbDetailsId);
            this._rpbDetails.findItemByValue("rpiDetails").set_visible(false);
            this._lblPECMailBoxId = document.getElementById(this.lblPECMailBoxIdId);
            this._lblMailBoxRecipient = document.getElementById(this.lblMailBoxRecipientId);
            this._lblIncomingServer = document.getElementById(this.lblIncomingServerId);
            this._lblOutgoingServer = document.getElementById(this.lblOutgoingServerId);
            this._lblRulesetName = document.getElementById(this.lblRulesetNameId);
            this._lblRulesetCondition = document.getElementById(this.lblRulesetConditionId);
            this._lblRulesetType = document.getElementById(this.lblRulesetTypeId);
            this._btnPECMailBoxSetRule = $find(this.btnPECMailBoxSetRuleId);
            this._btnPECMailBoxSetRule.add_clicking(this.btnPECMailBoxSetRule_onClick);
            this._windowSetRule = $find(this.windowSetRuleId);
            this._txtRulesetName = $find(this.txtRulesetNameId);
            this._txtSpecifySender = $find(this.txtSpecifySenderId);
            this._rlbSpecifyPECMailBox = $find(this.rlbSpecifyPECMailBoxId);
            this._btnPECMailBoxSave = $find(this.btnPECMAilBoxSaveId);
            this._btnPECMailBoxSave.add_clicking(this.btnPECMailBoxSave_onClick);
            this._cmdUpdatePEC = document.getElementById(this.cmdUpdatePECId);
            this._cmdAddPECMailBox = document.getElementById(this.cmdAddPECMailBoxId);
            this._windowPECMailBoxSettings = $find(this.uscPECMailBoxSettingsId);
            this._cmdUpdatePEC.onclick = function (event) {
                if (_this._rtvPECMailBoxes.get_selectedNode() !== null) {
                    document.getElementById(_this.hfPECMailBoxIdId).innerText = _this._rtvPECMailBoxes.get_selectedNode().get_value();
                    _this._windowPECMailBoxSettings.show();
                }
                else
                    alert("Nessun elemento selezionato per la modifica.");
                return false;
            };
            this._cmdAddPECMailBox.onclick = function (event) {
                document.getElementById(_this.hfPECMailBoxIdId).innerText = "";
                _this._windowPECMailBoxSettings.show();
                return false;
            };
        };
        TbltPECMailBox.prototype.loadResults = function (searchFilter) {
            var _this = this;
            this.pecMailBoxService.getPECMailBoxes(searchFilter, function (data) {
                if (!data)
                    return;
                _this.rtvResult = data;
                _this._rtvPECMailBoxes.get_nodes().clear();
                var rtvNode;
                rtvNode = new Telerik.Web.UI.RadTreeNode();
                rtvNode.set_text("Caselle PEC");
                _this._rtvPECMailBoxes.get_nodes().add(rtvNode);
                var thisObj = _this;
                $.each(_this.rtvResult, function (i, value) {
                    rtvNode = new Telerik.Web.UI.RadTreeNode();
                    rtvNode.set_text(value.MailBoxRecipient);
                    rtvNode.set_value(value.EntityShortId);
                    thisObj._rtvPECMailBoxes.get_nodes().getNode(0).get_nodes().add(rtvNode);
                });
                _this._rtvPECMailBoxes.get_nodes().getNode(0).expand();
                _this._loadingPanel.hide(_this.splitterMainId);
            }, function (exception) {
                _this._loadingPanel.hide(_this.splitterMainId);
                $("#".concat(_this.rtvPECMailBoxesId)).hide();
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        TbltPECMailBox.prototype.loadPECMailBoxDetails = function (pecMailBoxId) {
            this._btnPECMailBoxSetRule.set_enabled(true);
            var pecMailBox = this.rtvResult.filter(function (x) {
                return x.EntityShortId === pecMailBoxId;
            })[0];
            this._lblPECMailBoxId.innerText = pecMailBox.EntityShortId.toString();
            this._lblMailBoxRecipient.innerText = pecMailBox.MailBoxRecipient;
            this._lblIncomingServer.innerText = pecMailBox.IncomingServer;
            this._lblOutgoingServer.innerText = pecMailBox.OutgoingServer;
            if (pecMailBox.RulesetDefinition !== null) {
                var ruleset = JSON.parse(pecMailBox.RulesetDefinition);
                this._lblRulesetName.innerText = ruleset.Name === undefined ? "" : ruleset.Name;
                this._lblRulesetCondition.innerText = ruleset.Condition === undefined ? "" : ruleset.Condition;
                this._lblRulesetType.innerText = ruleset.Rule === undefined ? "" : ruleset.Rule;
            }
            else {
                this._lblRulesetName.innerText = "";
                this._lblRulesetCondition.innerText = "";
                this._lblRulesetType.innerText = "";
            }
        };
        TbltPECMailBox.prototype.updateCallback = function (pecMailBoxModel) {
            var _this = this;
            this.pecMailBoxService.updatePECMailBox(pecMailBoxModel, function (data) {
                if (!data)
                    return;
                _this._loadingPanel.show(_this.splitterMainId);
                _this.loadResults(_this._txtSearchName.get_textBoxValue());
                _this._windowSetRule.close();
                _this.loadPECMailBoxDetails(pecMailBoxModel.EntityShortId);
            }, function (exception) {
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        return TbltPECMailBox;
    }(TbltPECMailBoxBase));
    return TbltPECMailBox;
});
//# sourceMappingURL=TbltPECMailBox.js.map