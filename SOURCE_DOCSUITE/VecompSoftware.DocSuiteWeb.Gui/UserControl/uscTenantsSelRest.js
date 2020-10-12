define(["require", "exports", "App/DTOs/ExceptionDTO", "App/Helpers/EnumHelper", "App/Helpers/ServiceConfigurationHelper", "App/Services/Tenants/TenantAOOService"], function (require, exports, ExceptionDTO, EnumHelper, ServiceConfigurationHelper, TenantAOOService) {
    var uscTenantsSelRest = /** @class */ (function () {
        function uscTenantsSelRest(serviceConfigurations) {
            var _this = this;
            this.tenants = [];
            this.add_entryRemoved = function (sender, args) {
                var selected = args.get_entry();
                if (sessionStorage.getItem(uscTenantsSelRest.SESSION_TENANT_SELECTION_MODEL) == null ||
                    sessionStorage.getItem(uscTenantsSelRest.SESSION_TENANT_SELECTION_MODEL).length == 0) {
                    return;
                }
                else {
                    var currentTenantsFromSession = JSON.parse(sessionStorage.getItem(uscTenantsSelRest.SESSION_TENANT_SELECTION_MODEL));
                    if (currentTenantsFromSession.some(function (t) { return t.IdTenant === selected.get_value(); })) {
                        _this.tenants = currentTenantsFromSession.filter(function (t) { return t.IdTenant !== selected.get_value(); });
                    }
                    else {
                        return;
                    }
                }
                sessionStorage.setItem(uscTenantsSelRest.SESSION_TENANT_SELECTION_MODEL, JSON.stringify(_this.tenants));
            };
            this.add_entryAdded = function (sender, args) {
                var selected = args.get_node();
                if (selected.get_level() == 0) {
                    selected.unselect();
                    _this._rddtTenantTree.get_entries().clear();
                    return;
                }
                var tenant = {
                    IdTenant: selected.get_value(),
                    IdTenantAOO: selected.get_attributes().getAttribute(uscTenantsSelRest.TenantAOO_Id),
                    TenantName: selected.get_text(),
                    TenantAOOName: selected.get_attributes().getAttribute(uscTenantsSelRest.TenantAOO_Name)
                };
                if (sessionStorage.getItem(uscTenantsSelRest.SESSION_TENANT_SELECTION_MODEL) == null ||
                    sessionStorage.getItem(uscTenantsSelRest.SESSION_TENANT_SELECTION_MODEL).length == 0) {
                    _this.tenants.push(tenant);
                }
                else {
                    var currentTenantsFromSession = JSON.parse(sessionStorage.getItem(uscTenantsSelRest.SESSION_TENANT_SELECTION_MODEL));
                    if (currentTenantsFromSession.some(function (t) { return t.IdTenant === tenant.IdTenant; })) {
                        return;
                    }
                    else {
                        _this.tenants.push(tenant);
                    }
                }
                sessionStorage.setItem(uscTenantsSelRest.SESSION_TENANT_SELECTION_MODEL, JSON.stringify(_this.tenants));
            };
            this._serviceConfigurations = serviceConfigurations;
            this._enumHelper = new EnumHelper();
        }
        uscTenantsSelRest.prototype.initialize = function () {
            var tenantAOOConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, uscTenantsSelRest.TenantAOO_TYPE_NAME);
            this._tenantAOOService = new TenantAOOService(tenantAOOConfiguration);
            this._rddtTenantTree = $find(this.rddtTenantTreeId);
            this._rddtTenantTree.add_entryAdded(this.add_entryAdded);
            this._rddtTenantTree.add_entryRemoved(this.add_entryRemoved);
            uscTenantsSelRest.SESSION_TENANT_SELECTION_MODEL = "TenantModelSelection_" + this.rddtTenantTreeId;
            this.populateDropdownTree(this.currentTenantId);
            $("#" + this.rddtTenantTreeId).data(this);
        };
        uscTenantsSelRest.prototype.hasValue = function () {
            return this._rddtTenantTree.get_selectedValue() != "";
        };
        uscTenantsSelRest.prototype.populateDropdownTree = function (uniqueId) {
            var _this = this;
            this._tenantAOOService.getTenantsWithoutCurrentTenant(uniqueId, function (data) {
                var elements = _this._rddtTenantTree.get_embeddedTree();
                elements.trackChanges();
                var node;
                var tenantNode;
                for (var i = 0; i < data.length; i++) {
                    var tenantAOO = data[i];
                    node = new Telerik.Web.UI.RadTreeNode();
                    node.set_text(tenantAOO.Name);
                    node.set_value(tenantAOO.UniqueId);
                    node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/tenantAOO.png");
                    node.set_checkable(false);
                    elements.get_nodes().add(node);
                    for (var j = 0; j < tenantAOO.Tenants.length; j++) {
                        tenantNode = new Telerik.Web.UI.RadTreeNode();
                        tenantNode.set_text(tenantAOO.Tenants[j].TenantName);
                        tenantNode.set_value(tenantAOO.Tenants[j].UniqueId);
                        tenantNode.get_attributes().setAttribute(uscTenantsSelRest.TenantAOO_Id, tenantAOO.UniqueId);
                        tenantNode.get_attributes().setAttribute(uscTenantsSelRest.TenantAOO_Name, tenantAOO.Name);
                        node.get_nodes().add(tenantNode);
                    }
                }
                elements.commitChanges();
            }, function (exception) {
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        uscTenantsSelRest.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
            if (exception && exception instanceof ExceptionDTO) {
                var uscNotification = $("#" + uscNotificationId).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
            }
            else {
                this.showNotificationMessage(uscNotificationId, customMessage);
            }
        };
        uscTenantsSelRest.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#" + uscNotificationId).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        uscTenantsSelRest.TenantAOO_TYPE_NAME = "TenantAOO";
        uscTenantsSelRest.TenantAOO_Name = "TenantAOOName";
        uscTenantsSelRest.TenantAOO_Id = "TenantAOOId";
        return uscTenantsSelRest;
    }());
    return uscTenantsSelRest;
});
//# sourceMappingURL=uscTenantsSelRest.js.map