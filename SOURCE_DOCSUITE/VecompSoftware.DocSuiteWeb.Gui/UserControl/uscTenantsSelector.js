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
define(["require", "exports", "PEC/PECInvoiceBase"], function (require, exports, PECInvoiceBase) {
    var uscTenantsSelector = /** @class */ (function (_super) {
        __extends(uscTenantsSelector, _super);
        function uscTenantsSelector(serviceConfigurations) {
            var _this = _super.call(this, serviceConfigurations) || this;
            _this.cmbSelectAzienda_onClick = function (sender, args) {
                _this._cmbSelectPecMailBox.clearSelection();
                _this._cmbWorkflowRepositories.clearSelection();
                _this.pupulatePECMailBoxes(sender._value);
                _this.populateWorkflowRepositories(sender._value);
            };
            _this.cmbSelectPecMailBox_onClick = function (sender, args) {
                if (_this._cmbWorkflowRepositories.get_selectedItem())
                    _this._btnContainerSelectorOk.set_enabled(true);
            };
            _this._cmbWorkflowRepositories_onClick = function (sender, args) {
                if (_this._cmbSelectPecMailBox.get_selectedItem())
                    _this._btnContainerSelectorOk.set_enabled(true);
            };
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        uscTenantsSelector.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            this._btnContainerSelectorOk = $find(this.btnContainerSelectorOkId);
            this._cmbSelectAzienda = $find(this.cmbSelectAziendaId);
            this._cmbSelectAzienda.add_selectedIndexChanged(this.cmbSelectAzienda_onClick);
            this._cmbSelectPecMailBox = $find(this.cmbSelectPecMailBoxId);
            this._cmbSelectPecMailBox.add_selectedIndexChanged(this.cmbSelectPecMailBox_onClick);
            this._cmbWorkflowRepositories = $find(this.cmbWorkflowRepositoriesId);
            this._cmbWorkflowRepositories.add_selectedIndexChanged(this._cmbWorkflowRepositories_onClick);
            this.loadResults();
            this._cmbSelectPecMailBox.disable();
            this._cmbWorkflowRepositories.disable();
        };
        uscTenantsSelector.prototype.loadResults = function () {
            var _this = this;
            var comboItem = null;
            this._tenantService.getTenants(function (data) {
                if (!data)
                    return;
                $.each(data, function (idx, con) {
                    comboItem = new Telerik.Web.UI.RadComboBoxItem();
                    comboItem.set_text(con.CompanyName);
                    comboItem.set_value(con.UniqueId);
                    _this._cmbSelectAzienda.get_items().add(comboItem);
                });
            });
        };
        uscTenantsSelector.prototype.pupulatePECMailBoxes = function (tenantId) {
            var _this = this;
            this._cmbSelectPecMailBox.get_items().clear();
            var comboItem = null;
            this._tenantService.getTenantPECMailBoxes(tenantId, function (data) {
                if (data === undefined || data.length < 1) {
                    _this._cmbSelectPecMailBox.disable();
                    _this._btnContainerSelectorOk.set_enabled(false);
                    return;
                }
                else {
                    $.each(data, function (idx, con) {
                        comboItem = new Telerik.Web.UI.RadComboBoxItem();
                        comboItem.set_text(con.MailBoxRecipient);
                        comboItem.set_value(con.EntityShortId);
                        _this._cmbSelectPecMailBox.get_items().add(comboItem);
                        _this._cmbSelectPecMailBox.enable();
                    });
                }
            });
        };
        uscTenantsSelector.prototype.populateWorkflowRepositories = function (tenantId) {
            var _this = this;
            this._cmbWorkflowRepositories.get_items().clear();
            var comboItem = null;
            this._tenantService.getTenantWorkflowRepositories(tenantId, function (data) {
                if (data === undefined || data.length < 1) {
                    _this._cmbWorkflowRepositories.disable();
                    _this._btnContainerSelectorOk.set_enabled(false);
                    return;
                }
                else {
                    $.each(data, function (idx, con) {
                        comboItem = new Telerik.Web.UI.RadComboBoxItem();
                        comboItem.set_text(con.WorkflowRepository.Name);
                        comboItem.set_value(con.WorkflowRepository.UniqueId);
                        _this._cmbWorkflowRepositories.get_items().add(comboItem);
                        _this._cmbWorkflowRepositories.enable();
                    });
                }
            });
        };
        uscTenantsSelector.prototype.onClientShow = function () {
            this._cmbSelectAzienda.clearSelection();
            this._cmbSelectPecMailBox.clearSelection();
            this._cmbWorkflowRepositories.clearSelection();
            this._cmbSelectPecMailBox.disable();
            this._cmbWorkflowRepositories.disable();
            this._btnContainerSelectorOk.set_enabled(false);
        };
        return uscTenantsSelector;
    }(PECInvoiceBase));
    return uscTenantsSelector;
});
//# sourceMappingURL=uscTenantsSelector.js.map