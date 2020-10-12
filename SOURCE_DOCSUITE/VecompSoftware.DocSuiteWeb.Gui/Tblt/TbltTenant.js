/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />
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
var __spreadArrays = (this && this.__spreadArrays) || function () {
    for (var s = 0, i = 0, il = arguments.length; i < il; i++) s += arguments[i].length;
    for (var r = Array(s), k = 0, i = 0; i < il; i++)
        for (var a = arguments[i], j = 0, jl = a.length; j < jl; j++, k++)
            r[k] = a[j];
    return r;
};
define(["require", "exports", "./TbltTenantBase", "App/ViewModels/Tenants/TenantViewModel", "App/DTOs/TenantSearchFilterDTO", "App/Models/Tenants/TenantConfigurationModel", "App/Mappers/Tenants/TenantViewModelMapper", "App/Models/Tenants/TenantConfigurationTypeEnum", "App/Models/Tenants/TenantWorkflowRepositoryTypeEnum", "App/Helpers/EnumHelper", "App/Models/UpdateActionType", "App/Models/Tenants/TenantAOOAttributeEnum", "App/Models/Tenants/TenantTypologyTypeEnum", "../app/core/extensions/string"], function (require, exports, TbltTenantBase, TenantViewModel, TenantSearchFilterDTO, TenantConfigurationModel, TenantViewModelMapper, TenantConfigurationTypeEnum, TenantWorkflowRepositoryTypeEnum, EnumHelper, UpdateActionType, TenantAOOAttribute, TenantTypologyTypeEnum) {
    var TbltTenant = /** @class */ (function (_super) {
        __extends(TbltTenant, _super);
        function TbltTenant(serviceConfigurations) {
            var _this = _super.call(this, serviceConfigurations) || this;
            //region [ Tenants ]
            _this.rtvTenants_onClick = function (sender, args) {
                _this._rtbCompanyOptions.findItemByValue("create").set_enabled(true);
                _this._rtbCompanyOptions.findItemByValue("modify").set_enabled(true);
                if (args.get_node().get_level() === 0) {
                    $("#tenantLinkOptions").hide();
                    $("#tenantAOOInfo").hide();
                    _this._rtbCompanyOptions.findItemByValue("modify").set_enabled(false);
                }
                if (args.get_node().get_attributes().getAttribute("nodeType") === TenantAOOAttribute.Tenant) {
                    $("#tenantLinkOptions").show();
                    $("#tenantAOOInfo").hide();
                    _this.populateTenantConfigurations();
                    _this._rtbCompanyOptions.findItemByValue("create").set_enabled(false);
                    _this.loadTenantDetails(args.get_node());
                    _this._currentSelectedTenant.UniqueId = args.get_node().get_value();
                }
                if (args.get_node().get_attributes().getAttribute("nodeType") === TenantAOOAttribute.AOO) {
                    $("#tenantLinkOptions").hide();
                    $("#tenantAOOInfo").show();
                    _this.loadTenantAOODetails(args.get_node());
                }
            };
            _this.tenantAOO_onExpanded = function (sender, args) {
                if (args.get_node().get_attributes().getAttribute("nodeType") === TenantAOOAttribute.AOO) {
                    _this._loadingPanel.show(_this.splitterMainId);
                    _this.getTenantsForTenantAOO(args);
                }
            };
            _this.rtbCompanyOptions_onClick = function (sender, args) {
                if (_this._rtvTenants.get_selectedNode() === null) {
                    return;
                }
                switch (args.get_item().get_value()) {
                    case "create": {
                        if (_this._rtvTenants.get_selectedNode().get_attributes().getAttribute("nodeType") === TenantAOOAttribute.AOO) {
                            _this.openInsertTenantWindow();
                        }
                        else {
                            _this.openInsertTenantAOOWindow();
                        }
                        break;
                    }
                    case "modify": {
                        if (_this._rtvTenants.get_selectedNode().get_attributes().getAttribute("nodeType") === TenantAOOAttribute.AOO) {
                            _this.openEditTenantAOOWindow();
                        }
                        if (_this._rtvTenants.get_selectedNode().get_attributes().getAttribute("nodeType") === TenantAOOAttribute.Tenant) {
                            _this.openEditTenantWindow();
                        }
                        break;
                    }
                }
            };
            _this.updateAllTenantRolesPromise = function () {
                return _this.addAllContactsOrRolesToTenant(UpdateActionType.TenantRoleAddAll);
            };
            _this.deleteAllTenantRolesPromise = function () {
                return _this.deleteAllContactsOrRolesFromTenant(UpdateActionType.TenantRoleRemoveAll);
            };
            _this.deleteTenantRolePromise = function (roleIdToDelete, instanceId) {
                var promise = $.Deferred();
                _this._manager.radconfirm("Sei sicuro di voler eliminare il settore selezionato?", function (arg) {
                    if (arg) {
                        if (!roleIdToDelete)
                            return promise.promise();
                        var roleToDelete = _this._currentSelectedTenant.Roles
                            .filter(function (role) { return role.IdRole === roleIdToDelete && role.FullIncrementalPath.indexOf(roleIdToDelete.toString()) !== -1; })[0];
                        _this._currentSelectedTenant.Roles = _this._currentSelectedTenant.Roles
                            .filter(function (role) { return role.IdRole !== roleIdToDelete && role.FullIncrementalPath.indexOf(roleIdToDelete.toString()) === -1; });
                        var tenantToUpdate = _this.constructTenant();
                        tenantToUpdate.Roles = [roleToDelete];
                        _this._loadingPanel.show(_this.splitterMainId);
                        _this._tenantService.updateTenant(tenantToUpdate, UpdateActionType.TenantRoleRemove, function (data) {
                            promise.resolve(data);
                            _this._loadingPanel.hide(_this.splitterMainId);
                        }, function (exception) {
                            _this._loadingPanel.hide(_this.splitterMainId);
                            $("#".concat(_this.rtvTenantsId)).hide();
                            _this.showNotificationException(_this.uscNotificationId, exception);
                        });
                    }
                    document.getElementsByTagName("body")[0].setAttribute("class", "comm chrome");
                }, 400, 300);
                return promise.promise();
            };
            _this.updateTenantRolesPromise = function (newAddedRoles, instanceId) {
                var promise = $.Deferred();
                if (!newAddedRoles.length)
                    return promise.promise();
                _this._currentSelectedTenant.Roles = __spreadArrays(_this._currentSelectedTenant.Roles, newAddedRoles);
                _this._loadingPanel.show(_this.splitterMainId);
                var tenantToUpdate = _this.constructTenant();
                tenantToUpdate.Roles = newAddedRoles;
                //multi selection on roles??
                _this._tenantService.updateTenant(tenantToUpdate, UpdateActionType.TenantRoleAdd, function (data) {
                    promise.resolve(data);
                    _this._loadingPanel.hide(_this.splitterMainId);
                }, function (exception) {
                    _this._loadingPanel.hide(_this.splitterMainId);
                    $("#".concat(_this.rtvTenantsId)).hide();
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
                return promise.promise();
            };
            // endregion
            _this.toolbarSearch_onClick = function (sender, args) {
                switch (args.get_item().get_value()) {
                    case "search": {
                        var tenantName = _this._txtSearchTenantName.get_textBoxValue();
                        var companyName = _this._txtSearchCompanyName.get_textBoxValue();
                        var searchDTO = new TenantSearchFilterDTO();
                        if (tenantName) {
                            searchDTO.tenantName = tenantName;
                        }
                        if (companyName) {
                            searchDTO.companyName = companyName;
                        }
                        var activeItem = _this._toolbarSearch.findItemByValue("active");
                        var disabledItem = _this._toolbarSearch.findItemByValue("disabled");
                        searchDTO.isActive = activeItem.get_checked()
                            ? disabledItem.get_checked() ? null : true
                            : disabledItem.get_checked() ? false : null;
                        if (tenantName === "" && companyName === "" && searchDTO.isActive === null) {
                            searchDTO = null;
                        }
                        $("#tenantLinkOptions").hide();
                        $("#tenantAOOInfo").hide();
                        _this._rtbCompanyOptions.findItemByValue("modify").set_enabled(false);
                        _this.loadTenantsAOO(searchDTO);
                        break;
                    }
                }
            };
            _this.btnTenantSelectorOk_onClick = function (sender, args) {
                if (_this._txtTenantName.get_value() == "") {
                    alert("Il sigla UO e obbligatorio");
                    return;
                }
                if (_this._txtTenantCompany.get_value() == "") {
                    alert("Il nome UO e obbligatorio");
                    return;
                }
                if (_this._dpTenantDateFrom && _this._dpTenantDateFrom.get_selectedDate() &&
                    _this._txtTenantName && _this._txtTenantName.get_textBoxValue() !== "" &&
                    _this._txtTenantCompany && _this._txtTenantCompany.get_textBoxValue() !== "") {
                    var tenant_1 = {
                        Configurations: [],
                        Containers: [],
                        PECMailBoxes: [],
                        Roles: [],
                        Contacts: [],
                        TenantWorkflowRepositories: [],
                        UniqueId: "",
                        StartDate: _this._dpTenantDateFrom.get_selectedDate(),
                        EndDate: (_this._dpTenantDateTo && _this._dpTenantDateTo.get_selectedDate())
                            ? _this._dpTenantDateTo.get_selectedDate()
                            : null,
                        TenantName: _this._txtTenantName.get_textBoxValue(),
                        Note: _this._txtTenantNote ? _this._txtTenantNote.get_textBoxValue() : "",
                        CompanyName: _this._txtTenantCompany.get_textBoxValue(),
                        Location: null,
                        LastChangedDate: null,
                        LastChangedUser: null,
                        RegistrationDate: null,
                        RegistrationUser: null,
                        TenantAOO: { UniqueId: _this._rtvTenants.get_selectedNode().get_value() },
                        TenantTypology: TenantTypologyTypeEnum.InternalTenant
                    };
                    _this._rwTenantSelector.close();
                    _this._loadingPanel.show(_this.splitterMainId);
                    var nodeValue_1 = tenant_1.UniqueId;
                    var nodeText_1 = tenant_1.CompanyName + " (" + tenant_1.TenantName + ")";
                    var alreadySavedInTree = _this.isTenantUpdate;
                    if (!alreadySavedInTree) {
                        tenant_1.UniqueId = "";
                        _this._tenantService.insertTenant(tenant_1, function (data) {
                            var rtvNode;
                            rtvNode = new Telerik.Web.UI.RadTreeNode();
                            rtvNode.set_text(nodeText_1);
                            rtvNode.set_value(data.UniqueId);
                            rtvNode.get_attributes().setAttribute("nodeType", TenantAOOAttribute.Tenant);
                            tenant_1.UniqueId = data.UniqueId;
                            var tenantAOONode = _this._rtvTenants.get_selectedNode();
                            if (data.TenantTypology == TenantTypologyTypeEnum.InternalTenant) {
                                _this._rtvTenants.get_nodes().getNode(0).get_nodes().getNode(tenantAOONode.get_index()).get_nodes().add(rtvNode);
                            }
                        }, function (exception) {
                            _this._loadingPanel.hide(_this.splitterMainId);
                            $("#".concat(_this.rtvTenantsId)).hide();
                            _this.showNotificationException(_this.uscNotificationId, exception);
                        });
                    }
                    else {
                        if (_this._rtvTenants.get_selectedNode() !== null) {
                            tenant_1.UniqueId = _this._rtvTenants.get_selectedNode().get_value();
                            tenant_1.Configurations = _this._currentSelectedTenant.Configurations;
                            tenant_1.Containers = _this._currentSelectedTenant.Containers;
                            tenant_1.PECMailBoxes = _this._currentSelectedTenant.PECMailBoxes;
                            tenant_1.Roles = _this._currentSelectedTenant.Roles;
                            tenant_1.TenantWorkflowRepositories = _this._currentSelectedTenant.TenantWorkflowRepositories;
                            tenant_1.TenantAOO = { UniqueId: _this._rtvTenants.get_selectedNode().get_parent().get_value() };
                            nodeValue_1 = tenant_1.UniqueId;
                        }
                        _this._tenantService.updateTenant(tenant_1, null, function (data) {
                            _this._rtvTenants.get_selectedNode().set_text(nodeText_1);
                            _this._rtvTenants.get_selectedNode().set_value(nodeValue_1);
                            _this._lblCompanyNameId.innerText = tenant_1.CompanyName;
                            _this._lblTenantNameId.innerText = tenant_1.TenantName;
                            _this._lblTenantNoteId.innerText = tenant_1.Note;
                            _this._lblTenantDataDiAttivazioneId.innerText = moment(tenant_1.StartDate).isValid() ? moment(tenant_1.StartDate).format("DD-MM-YYYY") : "";
                            _this._lblTenantDataDiDisattivazioneId.innerText = moment(tenant_1.EndDate).isValid() ? moment(tenant_1.EndDate).format("DD-MM-YYYY") : "";
                        }, function (exception) {
                            _this._loadingPanel.hide(_this.splitterMainId);
                            $("#".concat(_this.rtvTenantsId)).hide();
                            _this.showNotificationException(_this.uscNotificationId, exception);
                        });
                    }
                    _this._loadingPanel.hide(_this.splitterMainId);
                }
            };
            _this.btnTenantSelectorCancel_onClick = function (sender, args) {
                _this._rwTenantSelector.close();
            };
            _this.btnTenantAOOSelectorOk_onClick = function (sender, args) {
                if (_this._txtTenantAOOName.get_value() == "") {
                    alert("Il campo nome e obbligatorio");
                    return;
                }
                var tenantAOOModel = {
                    Name: _this._txtTenantAOOName.get_textBoxValue(),
                    Note: _this._txtTenantAOONote.get_textBoxValue(),
                    CategorySuffix: _this._txtCategorySuffix.get_textBoxValue(),
                    Tenants: [],
                    TenantTypology: TenantTypologyTypeEnum.InternalTenant
                };
                if (_this.isTenantAOOUpdate) {
                    _this.updateTenantAOO(tenantAOOModel);
                }
                else {
                    _this.insertTenantAOO(tenantAOOModel);
                }
            };
            _this.btnTenantAOOSelectorCancel_onClick = function (sender, args) {
                _this._rwTenantSelector.close();
            };
            //endregion
            //region [ Add/Delete Containers from RadTreeView ]
            _this.toolbarContainer_onClick = function (sender, args) {
                var btn = args.get_item();
                switch (btn.get_index()) {
                    case 0: {
                        _this._rwContainer.show();
                        _this._containerService.getContainers(null, function (data) {
                            _this.containers = data;
                            _this.addContainers(_this.containers, _this._cmbContainer);
                        }, function (exception) {
                            _this._loadingPanel.hide(_this.splitterMainId);
                            _this.showNotificationException(_this.uscNotificationId, exception);
                        });
                        args.set_cancel(true);
                        break;
                    }
                    case 1: {
                        if (_this._rtvContainers.get_selectedNode() !== null) {
                            _this._manager.radconfirm("Sei sicuro di voler eliminare il contenitore selezionato?", function (arg) {
                                if (arg) {
                                    var tenantToUpdate = _this.constructTenant();
                                    tenantToUpdate.Containers = _this._currentSelectedTenant.Containers.filter(function (x) { return x.EntityShortId === Number(_this._rtvContainers.get_selectedNode().get_value()); });
                                    var removeIndex = _this._currentSelectedTenant.Containers.map(function (item) { return item.EntityShortId; }).indexOf(Number(_this._rtvContainers.get_selectedNode().get_value()));
                                    _this._currentSelectedTenant.Containers.splice(removeIndex, 1);
                                    _this._tenantService.updateTenant(tenantToUpdate, UpdateActionType.TenantContainerRemove, function (data) {
                                        _this._rtvContainers.get_nodes().getNode(0).get_nodes().removeAt(_this._rtvContainers.get_selectedNode().get_index());
                                        if (_this._rtvContainers.get_nodes().getNode(0).get_nodes().getItem(0) === undefined)
                                            _this._rtvContainers.get_nodes().clear();
                                    }, function (exception) {
                                        _this._loadingPanel.hide(_this.splitterMainId);
                                        $("#".concat(_this.rtvTenantsId)).hide();
                                        _this.showNotificationException(_this.uscNotificationId, exception);
                                    });
                                }
                            }, 400, 300);
                        }
                        else {
                            alert("Selezionare un Contentitore");
                        }
                        args.set_cancel(true);
                        break;
                    }
                    case 2: {
                        _this.addOrRemoveAllTenantContainers("Sei sicuro di voler aggiungere tutti i contenitori?", UpdateActionType.TenantContainerAddAll);
                        break;
                    }
                    case 3: {
                        _this.addOrRemoveAllTenantContainers("Sei sicuro di voler eliminare tutti i contenitori?", UpdateActionType.TenantContainerRemoveAll);
                        break;
                    }
                }
            };
            _this.btnContainerOk_onClick = function (sender, args) {
                if (_this._cmbContainer && _this.selectedContainer) {
                    _this._rwContainer.close();
                    _this._loadingPanel.show(_this.tbContainersControlId);
                    var nodeImageUrl_1 = "../App_Themes/DocSuite2008/imgset16/box_open.png";
                    var nodeValue_2 = _this.selectedContainer.EntityShortId.toString();
                    var nodeText_2 = _this.selectedContainer.Name;
                    var alreadySavedInTree = _this.alreadySavedInTree(nodeValue_2, _this._rtvContainers);
                    if (!alreadySavedInTree) {
                        _this._currentSelectedTenant.Containers.push(_this.selectedContainer);
                        var tenantToUpdate = _this.constructTenant();
                        tenantToUpdate.Containers = [_this.selectedContainer];
                        _this._tenantService.updateTenant(tenantToUpdate, UpdateActionType.TenantContainerAdd, function (data) {
                            _this.addNodesToRadTreeView(nodeValue_2, nodeText_2, "Contenitori", nodeImageUrl_1, _this._rtvContainers);
                        }, function (exception) {
                            _this._loadingPanel.hide(_this.splitterMainId);
                            $("#".concat(_this.rtvTenantsId)).hide();
                            _this.showNotificationException(_this.uscNotificationId, exception);
                        });
                    }
                    _this._loadingPanel.hide(_this.tbContainersControlId);
                }
            };
            _this.btnContainerCancel_onClick = function (sender, args) {
                _this._rwContainer.close();
            };
            _this.cmbContainers_onClick = function (sender, args) {
                _this.selectedContainer = _this.containers.filter(function (x) {
                    return x.EntityShortId.toString() === args.get_item().get_value();
                })[0];
            };
            _this._cmbContainer_OnClientItemsRequested = function (sender, args) {
                var containerNumberOfItems = sender.get_items().get_count();
                _this._containerService.getAllContainers(args.get_text(), _this.maxNumberElements, containerNumberOfItems, function (data) {
                    try {
                        _this.refreshContainers(data.value);
                        var scrollToPosition = args.get_domEvent() == undefined;
                        if (scrollToPosition) {
                            if (sender.get_items().get_count() > 0) {
                                var scrollContainer = $(sender.get_dropDownElement()).find('div.rcbScroll');
                                scrollContainer.scrollTop($(sender.get_items().getItem(containerNumberOfItems + 1).get_element()).position().top);
                            }
                        }
                        sender.get_attributes().setAttribute('otherContainerCount', data.count.toString());
                        sender.get_attributes().setAttribute('updating', 'false');
                        if (sender.get_items().get_count() > 0) {
                            containerNumberOfItems = sender.get_items().get_count() - 1;
                        }
                        _this._cmbContainer.get_moreResultsBoxMessageElement().innerText = "Visualizzati " + containerNumberOfItems.toString() + " di " + data.count.toString();
                    }
                    catch (error) {
                    }
                }, function (exception) {
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            };
            _this.refreshContainers = function (data) {
                if (data.length > 0) {
                    _this._cmbContainer.beginUpdate();
                    if (_this._cmbContainer.get_items().get_count() === 0) {
                        var emptyItem = new Telerik.Web.UI.RadComboBoxItem();
                        emptyItem.set_text("");
                        emptyItem.set_value("");
                        _this._cmbContainer.get_items().insert(0, emptyItem);
                    }
                    $.each(data, function (index, container) {
                        var item = new Telerik.Web.UI.RadComboBoxItem();
                        item.set_text(container.Name);
                        item.set_value(container.EntityShortId.toString());
                        _this._cmbContainer.get_items().add(item);
                        _this.containers.push(container);
                    });
                    _this._cmbContainer.showDropDown();
                    _this._cmbContainer.endUpdate();
                }
                else {
                    if (_this._cmbContainer.get_items().get_count() === 0) {
                    }
                }
            };
            _this._rwContainer_OnShow = function (sender, args) {
                _this._cmbContainer.clearSelection();
                _this.selectedContainer = null;
            };
            //endregion
            //region [ Add/Delete PECMailBoxes from RadTreeView ]
            _this.toolbarPECMailBox_onClick = function (sender, args) {
                var btn = args.get_item();
                switch (btn.get_index()) {
                    case 0:
                        _this._rwPECMailBox.show();
                        _this._pecMailBoxService.getPECMailBoxes("", function (data) {
                            _this.pecMailBoxes = data;
                            _this.addPECMailBoxes(_this.pecMailBoxes, _this._cmbPECMailBox);
                        }, function (exception) {
                            _this._loadingPanel.hide(_this.splitterMainId);
                            _this.showNotificationException(_this.uscNotificationId, exception);
                        });
                        args.set_cancel(true);
                        break;
                    case 1:
                        if (_this._rtvPECMailBoxes.get_selectedNode() !== null) {
                            _this._manager.radconfirm("Sei sicuro di voler eliminare il casella PEC selezionato?", function (arg) {
                                if (arg) {
                                    var tenantToUpdate = _this.constructTenant();
                                    tenantToUpdate.PECMailBoxes = _this._currentSelectedTenant.PECMailBoxes.filter(function (x) { return x.EntityShortId === Number(_this._rtvPECMailBoxes.get_selectedNode().get_value()); });
                                    var removeIndex = _this._currentSelectedTenant.PECMailBoxes.map(function (item) { return item.EntityShortId; }).indexOf(Number(_this._rtvPECMailBoxes.get_selectedNode().get_value()));
                                    _this._currentSelectedTenant.PECMailBoxes.splice(removeIndex, 1);
                                    _this._tenantService.updateTenant(tenantToUpdate, UpdateActionType.TenantPECMailBoxRemove, function (data) {
                                        _this._rtvPECMailBoxes.get_nodes().getNode(0).get_nodes().removeAt(_this._rtvPECMailBoxes.get_selectedNode().get_index());
                                        if (_this._rtvPECMailBoxes.get_nodes().getNode(0).get_nodes().getItem(0) === undefined)
                                            _this._rtvPECMailBoxes.get_nodes().clear();
                                    }, function (exception) {
                                        _this._loadingPanel.hide(_this.splitterMainId);
                                        $("#".concat(_this.rtvTenantsId)).hide();
                                        _this.showNotificationException(_this.uscNotificationId, exception);
                                    });
                                }
                            }, 400, 300);
                        }
                        else {
                            alert("Selezionare una caselle PEC");
                        }
                        args.set_cancel(true);
                        break;
                }
            };
            _this.btnPECMailBoxOk_onClick = function (sender, args) {
                if (_this._cmbPECMailBox && _this.selectedPECMailBox) {
                    _this._rwPECMailBox.close();
                    _this._loadingPanel.show(_this.tbPECMailBoxesControlId);
                    var nodeImageUrl_2 = "../App_Themes/DocSuite2008/imgset16/box_open.png";
                    var nodeValue_3 = _this.selectedPECMailBox.EntityShortId.toString();
                    var nodeText_3 = _this.selectedPECMailBox.MailBoxRecipient;
                    var alreadySavedInTree = _this.alreadySavedInTree(nodeValue_3, _this._rtvPECMailBoxes);
                    if (!alreadySavedInTree) {
                        _this._currentSelectedTenant.PECMailBoxes.push(_this.selectedPECMailBox);
                        var tenantToUpdate = _this.constructTenant();
                        tenantToUpdate.PECMailBoxes = [_this.selectedPECMailBox];
                        _this._tenantService.updateTenant(tenantToUpdate, UpdateActionType.TenantPECMailBoxAdd, function (data) {
                            _this.addNodesToRadTreeView(nodeValue_3, nodeText_3, "Caselle PEC", nodeImageUrl_2, _this._rtvPECMailBoxes);
                        }, function (exception) {
                            _this._loadingPanel.hide(_this.splitterMainId);
                            $("#".concat(_this.rtvTenantsId)).hide();
                            _this.showNotificationException(_this.uscNotificationId, exception);
                        });
                    }
                    _this._loadingPanel.hide(_this.tbPECMailBoxesControlId);
                }
            };
            _this.btnPECMailBoxCancel_onClick = function (sender, args) {
                _this._rwPECMailBox.close();
            };
            _this.cmbPECMailBoxes_onClick = function (sender, args) {
                _this.selectedPECMailBox = _this.pecMailBoxes.filter(function (x) {
                    return x.EntityShortId.toString() === args.get_item().get_value();
                })[0];
            };
            _this._cmbPECMailBox_OnClientItemsRequested = function (sender, args) {
                var pecMailBoxNumberOfItems = sender.get_items().get_count();
                _this._pecMailBoxService.getAllPECMailBoxes(args.get_text(), _this.maxNumberElements, pecMailBoxNumberOfItems, function (data) {
                    try {
                        _this.refreshPECMailBoxes(data.value);
                        var scrollToPosition = args.get_domEvent() == undefined;
                        if (scrollToPosition) {
                            if (sender.get_items().get_count() > 0) {
                                var scrollContainer = $(sender.get_dropDownElement()).find('div.rcbScroll');
                                scrollContainer.scrollTop($(sender.get_items().getItem(pecMailBoxNumberOfItems + 1).get_element()).position().top);
                            }
                        }
                        sender.get_attributes().setAttribute('otherContainerCount', data.count.toString());
                        sender.get_attributes().setAttribute('updating', 'false');
                        if (sender.get_items().get_count() > 0) {
                            pecMailBoxNumberOfItems = sender.get_items().get_count() - 1;
                        }
                        _this._cmbPECMailBox.get_moreResultsBoxMessageElement().innerText = "Visualizzati " + pecMailBoxNumberOfItems.toString() + " di " + data.count.toString();
                    }
                    catch (error) {
                    }
                }, function (exception) {
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            };
            _this.refreshPECMailBoxes = function (data) {
                if (data.length > 0) {
                    _this._cmbPECMailBox.beginUpdate();
                    if (_this._cmbPECMailBox.get_items().get_count() === 0) {
                        var emptyItem = new Telerik.Web.UI.RadComboBoxItem();
                        emptyItem.set_text("");
                        emptyItem.set_value("");
                        _this._cmbPECMailBox.get_items().insert(0, emptyItem);
                    }
                    $.each(data, function (index, pecMailBox) {
                        var item = new Telerik.Web.UI.RadComboBoxItem();
                        item.set_text(pecMailBox.MailBoxRecipient);
                        item.set_value(pecMailBox.EntityShortId.toString());
                        _this._cmbPECMailBox.get_items().add(item);
                        _this.pecMailBoxes.push(pecMailBox);
                    });
                    _this._cmbPECMailBox.showDropDown();
                    _this._cmbPECMailBox.endUpdate();
                }
                else {
                    if (_this._cmbPECMailBox.get_items().get_count() === 0) {
                    }
                }
            };
            _this._rwPECMailBox_OnShow = function (sender, args) {
                _this._cmbPECMailBox.clearSelection();
                _this.selectedPECMailBox = null;
            };
            //endregion
            //region [ Add/Delete WorkflowRepositories from RadTreeView ]
            _this.toolbarWorkflowRepository_onClick = function (sender, args) {
                var btn = args.get_item();
                switch (btn.get_index()) {
                    case 0:
                        _this._dpTenantWorkflowRepositoryDateFrom.clear();
                        _this._dpTenantWorkflowRepositoryDateTo.clear();
                        _this.currentTenantWorkflowRepositoryUniqueId = "";
                        _this._txtTenantWorkflowRepositoryJsonValue.clear();
                        _this._txtTenantWorkflowRepositoryIntegrationModuleName.clear();
                        _this._txtTenantWorkflowRepositoryConditions.clear();
                        _this._cmbTenantWorkflowRepositoryType.set_selectedIndex(0);
                        if (_this._rtvTenantWorkflowRepositories.get_selectedNode()) {
                            _this._rtvTenantWorkflowRepositories.get_selectedNode().set_selected(false);
                            _this.selectedTenantWorkflowRepository = undefined;
                        }
                        _this._rwWorkflowRepository.show();
                        args.set_cancel(true);
                        break;
                    case 1:
                        if (_this._rtvTenantWorkflowRepositories.get_selectedNode() &&
                            _this._rtvTenantWorkflowRepositories.get_selectedNode() !== _this._rtvTenantWorkflowRepositories.get_nodes().getNode(0)) {
                            var uniqueId = _this._rtvTenantWorkflowRepositories.get_selectedNode().get_value();
                            _this._tenantWorkflowRepositoryService.getTenantWorkflowRepositoryById(uniqueId, function (data) {
                                var editIndex = data.map(function (item) { return item.UniqueId; })
                                    .indexOf(_this._rtvTenantWorkflowRepositories.get_selectedNode().get_value());
                                _this._rwWorkflowRepository.show();
                                var cmbWorkflowRepositoryItem = _this._cmbWorkflowRepository.findItemByValue(data[editIndex].WorkflowRepository.UniqueId);
                                cmbWorkflowRepositoryItem.select();
                                _this._dpTenantWorkflowRepositoryDateFrom.set_selectedDate(new Date(data[editIndex].StartDate));
                                if (data[editIndex].EndDate && data[editIndex].EndDate !== "")
                                    _this._dpTenantWorkflowRepositoryDateTo.set_selectedDate(new Date(data[editIndex].EndDate));
                                else
                                    _this._dpTenantWorkflowRepositoryDateTo.clear();
                                _this._txtTenantWorkflowRepositoryJsonValue.set_value(data[editIndex].JsonValue);
                                _this._txtTenantWorkflowRepositoryIntegrationModuleName.set_value(data[editIndex].IntegrationModuleName);
                                _this._txtTenantWorkflowRepositoryConditions.set_value(data[editIndex].Conditions);
                                var cmbTenantWorkflowRepositoryTypeItem = _this._cmbTenantWorkflowRepositoryType.findItemByValue(data[editIndex].ConfigurationType);
                                cmbTenantWorkflowRepositoryTypeItem.select();
                                _this.currentTenantWorkflowRepositoryUniqueId = data[editIndex].UniqueId;
                            }, function (exception) {
                                _this._loadingPanel.hide(_this.splitterMainId);
                                $("#".concat(_this.rtvTenantsId)).hide();
                                _this.showNotificationException(_this.uscNotificationId, exception);
                            });
                        }
                        else {
                            alert("Selezionare una attività ");
                        }
                        args.set_cancel(true);
                        break;
                    case 2:
                        if (_this._rtvTenantWorkflowRepositories.get_selectedNode() !== null) {
                            var removeIndex_1 = _this._currentSelectedTenant.TenantWorkflowRepositories.map(function (item) { return item.UniqueId; }).indexOf(_this._rtvTenantWorkflowRepositories.get_selectedNode().get_value());
                            _this._tenantWorkflowRepositoryService.deleteTenantWorkflowRepository(_this._currentSelectedTenant.TenantWorkflowRepositories[removeIndex_1], function (data) {
                                _this._currentSelectedTenant.TenantWorkflowRepositories.splice(removeIndex_1, 1);
                                _this._rtvTenantWorkflowRepositories.get_nodes().getNode(0).get_nodes().removeAt(_this._rtvTenantWorkflowRepositories.get_selectedNode().get_index());
                                if (_this._rtvTenantWorkflowRepositories.get_nodes().getNode(0).get_nodes().getItem(0) === undefined)
                                    _this._rtvTenantWorkflowRepositories.get_nodes().clear();
                            }, function (exception) {
                                _this._loadingPanel.hide(_this.splitterMainId);
                                $("#".concat(_this.rtvTenantsId)).hide();
                                _this.showNotificationException(_this.uscNotificationId, exception);
                            });
                        }
                        else {
                            alert("Selezionare una attività");
                        }
                        args.set_cancel(true);
                        break;
                }
            };
            _this.btnWorkflowRepositoryOk_onClick = function (sender, args) {
                if (_this._cmbWorkflowRepository && _this.selectedWorkflowRepository &&
                    _this._txtTenantWorkflowRepositoryJsonValue && _this._txtTenantWorkflowRepositoryJsonValue.get_textBoxValue() !== "" &&
                    _this._cmbTenantWorkflowRepositoryType && _this._cmbTenantWorkflowRepositoryType.get_selectedItem().get_text() !== "" &&
                    _this._dpTenantWorkflowRepositoryDateFrom && _this._dpTenantWorkflowRepositoryDateFrom.get_selectedDate()) {
                    _this._loadingPanel.show(_this.tbWorkflowRepositoryControlId);
                    var nodeImageUrl_3 = "../App_Themes/DocSuite2008/imgset16/box_open.png";
                    var nodeValue_4 = _this.selectedTenantWorkflowRepository !== undefined
                        ? _this.selectedTenantWorkflowRepository.UniqueId
                        : "";
                    var nodeText_4 = _this._cmbWorkflowRepository.get_selectedItem().get_text();
                    var viewModelMapper = new TenantViewModelMapper();
                    var tenantWorkflowRepository = {
                        Tenant: viewModelMapper.Map(_this._currentSelectedTenant),
                        WorkflowRepository: _this.selectedWorkflowRepository,
                        JsonValue: _this._txtTenantWorkflowRepositoryJsonValue ? _this._txtTenantWorkflowRepositoryJsonValue.get_textBoxValue() : "",
                        IntegrationModuleName: _this._txtTenantWorkflowRepositoryIntegrationModuleName ? _this._txtTenantWorkflowRepositoryIntegrationModuleName.get_textBoxValue() : "",
                        Conditions: _this._txtTenantWorkflowRepositoryConditions ? _this._txtTenantWorkflowRepositoryConditions.get_textBoxValue() : "",
                        ConfigurationType: _this._cmbTenantWorkflowRepositoryType.get_selectedItem().get_value(),
                        EndDate: (_this._dpTenantWorkflowRepositoryDateTo && _this._dpTenantWorkflowRepositoryDateTo.get_selectedDate())
                            ? moment(_this._dpTenantWorkflowRepositoryDateTo.get_selectedDate()).format("MM-DD-YYYY")
                            : "",
                        StartDate: (_this._dpTenantWorkflowRepositoryDateFrom && _this._dpTenantWorkflowRepositoryDateFrom.get_selectedDate())
                            ? moment(_this._dpTenantWorkflowRepositoryDateFrom.get_selectedDate()).format("MM-DD-YYYY")
                            : "",
                        LastChangedDate: null,
                        LastChangedUser: null,
                        RegistrationDate: null,
                        RegistrationUser: null,
                        UniqueId: _this.currentTenantWorkflowRepositoryUniqueId
                    };
                    var alreadySavedInTree = _this.alreadySavedInTree(nodeValue_4, _this._rtvTenantWorkflowRepositories);
                    if (!alreadySavedInTree) {
                        var asd = _this._cmbWorkflowRepository.get_selectedItem().get_text();
                        console.log(_this._cmbWorkflowRepository.get_selectedItem().get_text());
                        var thisObj_1 = _this;
                        if (_this._currentSelectedTenant.TenantWorkflowRepositories.length === 0 ||
                            _this._currentSelectedTenant.TenantWorkflowRepositories.filter(function (x) {
                                return x.WorkflowRepository.Name !== thisObj_1._cmbWorkflowRepository.get_selectedItem().get_text();
                            })[0])
                            _this._tenantWorkflowRepositoryService.insertTenantWorkflowRepository(tenantWorkflowRepository, function (data) {
                                nodeValue_4 = data.UniqueId;
                                _this.addNodesToRadTreeView(nodeValue_4, nodeText_4, "Attività", nodeImageUrl_3, _this._rtvTenantWorkflowRepositories);
                                _this._currentSelectedTenant.TenantWorkflowRepositories.push(data);
                                _this._currentSelectedTenant.TenantWorkflowRepositories[_this._currentSelectedTenant.TenantWorkflowRepositories.length - 1].ConfigurationType =
                                    TenantWorkflowRepositoryTypeEnum[data.ConfigurationType];
                                _this._rwWorkflowRepository.close();
                            }, function (exception) {
                                _this._loadingPanel.hide(_this.splitterMainId);
                                $("#".concat(_this.rtvTenantsId)).hide();
                                _this.showNotificationException(_this.uscNotificationId, exception);
                            });
                    }
                    else {
                        var existsInTree_cmbSelectedExcluded = false;
                        for (var i = 0; i < _this._rtvTenantWorkflowRepositories.get_nodes().getNode(0).get_nodes().get_count(); i++) {
                            if (_this._cmbWorkflowRepository.get_selectedItem().get_text() === _this._rtvTenantWorkflowRepositories.get_nodes().getNode(0).get_nodes().getItem(i).get_text() &&
                                _this._cmbWorkflowRepository.get_selectedItem().get_text() !== _this._rtvTenantWorkflowRepositories.get_selectedNode().get_text())
                                existsInTree_cmbSelectedExcluded = true;
                        }
                        if (!existsInTree_cmbSelectedExcluded)
                            _this._tenantWorkflowRepositoryService.updateTenantWorkflowRepository(tenantWorkflowRepository, function (data) {
                                var thisObj = _this;
                                _this._currentSelectedTenant.TenantWorkflowRepositories.filter(function (x) {
                                    return x.UniqueId === thisObj._rtvTenantWorkflowRepositories.get_selectedNode().get_value();
                                })[0].WorkflowRepository = data.WorkflowRepository;
                                _this._rtvTenantWorkflowRepositories.get_selectedNode().set_text(nodeText_4);
                                _this._rtvTenantWorkflowRepositories.get_selectedNode().set_value(nodeValue_4);
                                _this._rwWorkflowRepository.close();
                            }, function (exception) {
                                _this._loadingPanel.hide(_this.splitterMainId);
                                $("#".concat(_this.rtvTenantsId)).hide();
                                _this.showNotificationException(_this.uscNotificationId, exception);
                            });
                    }
                    _this._loadingPanel.hide(_this.tbWorkflowRepositoryControlId);
                }
            };
            _this.rtvTenantWorkflowrepositories_onNodeClick = function (sender, args) {
                _this.selectedTenantWorkflowRepository = _this.tenantWorkflowRepositories.filter(function (x) {
                    return x.UniqueId === args.get_node().get_value();
                })[0];
            };
            _this.btnWorkflowRepositoryCancel_onClick = function (sender, args) {
                _this._rwWorkflowRepository.close();
            };
            _this.cmbWorkflowRepositories_onClick = function (sender, args) {
                _this.selectedWorkflowRepository = _this.workflowRepositories.filter(function (x) {
                    return x.UniqueId === args.get_item().get_value();
                })[0];
            };
            _this._cmbWorkflowRepository_OnClientItemsRequested = function (sender, args) {
                var workflowRepositoryNumberOfItems = sender.get_items().get_count();
                _this._workflowRepositoryService.getAllWorkflowRepositories(args.get_text(), _this.maxNumberElements, workflowRepositoryNumberOfItems, function (data) {
                    try {
                        _this.refreshWorkflowRepositories(data.value);
                        var scrollToPosition = args.get_domEvent() == undefined;
                        if (scrollToPosition) {
                            if (sender.get_items().get_count() > 0) {
                                var scrollContainer = $(sender.get_dropDownElement()).find('div.rcbScroll');
                                scrollContainer.scrollTop($(sender.get_items().getItem(workflowRepositoryNumberOfItems + 1).get_element()).position().top);
                            }
                        }
                        sender.get_attributes().setAttribute('otherContainerCount', data.count.toString());
                        sender.get_attributes().setAttribute('updating', 'false');
                        if (sender.get_items().get_count() > 0) {
                            workflowRepositoryNumberOfItems = sender.get_items().get_count() - 1;
                        }
                        _this._cmbWorkflowRepository.get_moreResultsBoxMessageElement().innerText = "Visualizzati " + workflowRepositoryNumberOfItems.toString() + " di " + data.count.toString();
                    }
                    catch (error) {
                    }
                }, function (exception) {
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            };
            _this.refreshWorkflowRepositories = function (data) {
                if (data.length > 0) {
                    _this._cmbWorkflowRepository.beginUpdate();
                    if (_this._cmbWorkflowRepository.get_items().get_count() === 0) {
                        var emptyItem = new Telerik.Web.UI.RadComboBoxItem();
                        emptyItem.set_text("");
                        emptyItem.set_value("");
                        _this._cmbWorkflowRepository.get_items().insert(0, emptyItem);
                    }
                    $.each(data, function (index, workflowRepository) {
                        var item = new Telerik.Web.UI.RadComboBoxItem();
                        item.set_text(workflowRepository.Name);
                        item.set_value(workflowRepository.UniqueId);
                        _this._cmbWorkflowRepository.get_items().add(item);
                        _this.workflowRepositories.push(workflowRepository);
                    });
                    _this._cmbWorkflowRepository.showDropDown();
                    _this._cmbWorkflowRepository.endUpdate();
                }
                else {
                    if (_this._cmbWorkflowRepository.get_items().get_count() === 0) {
                    }
                }
            };
            _this._rwWorkflowRepository_OnShow = function (sender, args) {
                _this._cmbWorkflowRepository.clearSelection();
                _this.selectedWorkflowRepository = null;
            };
            //endregion
            //region [ Add/Update/Delete TenantConfigurations from RadTreeView ]
            _this.toolbarConfiguration_onClick = function (sender, args) {
                var btn = args.get_item();
                switch (btn.get_index()) {
                    case 0:
                        _this._dpStartDateFrom.clear();
                        _this._dpEndDateFrom.clear();
                        _this._txtTenantConfigurationNote.clear();
                        _this._txtTenantConfigurationJsonValue.clear();
                        _this._cmbConfigurationType.set_selectedIndex(0);
                        _this.currentTenantConfigurationUniqueId = "";
                        _this._rwTenantConfiguration.show();
                        args.set_cancel(true);
                        break;
                    case 1:
                        if (_this._rtvTenantConfigurations.get_selectedNode() !== null) {
                            var editIndex = _this._currentSelectedTenant.Configurations.map(function (item) { return item.UniqueId; })
                                .indexOf(_this._rtvTenantConfigurations.get_selectedNode().get_value());
                            _this._dpStartDateFrom.set_selectedDate(new Date(_this._currentSelectedTenant.Configurations[editIndex].StartDate));
                            if (_this._currentSelectedTenant.Configurations[editIndex].EndDate && _this._currentSelectedTenant.Configurations[editIndex].EndDate !== "")
                                _this._dpEndDateFrom.set_selectedDate(new Date(_this._currentSelectedTenant.Configurations[editIndex].EndDate));
                            else
                                _this._dpEndDateFrom.clear();
                            _this._txtTenantConfigurationNote.set_value(_this._currentSelectedTenant.Configurations[editIndex].Note);
                            _this._txtTenantConfigurationJsonValue.set_value(_this._currentSelectedTenant.Configurations[editIndex].JsonValue);
                            var item = _this._cmbConfigurationType.findItemByValue(_this._currentSelectedTenant.Configurations[editIndex].ConfigurationType);
                            item.select();
                            _this.currentTenantConfigurationUniqueId = _this._currentSelectedTenant.Configurations[editIndex].UniqueId;
                            _this._rwTenantConfiguration.show();
                        }
                        else {
                            alert("Selezionare un configurazione");
                        }
                        args.set_cancel(true);
                        break;
                    case 2:
                        if (_this._rtvTenantConfigurations.get_selectedNode() !== null) {
                            _this._manager.radconfirm("Sei sicuro di voler eliminare il configurazione selezionato?", function (arg) {
                                if (arg) {
                                    var tenantToUpdate = _this.constructTenant();
                                    tenantToUpdate.Configurations = _this._currentSelectedTenant.Configurations.filter(function (x) { return x.UniqueId == _this._rtvTenantConfigurations.get_selectedNode().get_value(); });
                                    var removeIndex = _this._currentSelectedTenant.Configurations.map(function (item) { return item.UniqueId; }).indexOf(_this._rtvTenantConfigurations.get_selectedNode().get_value());
                                    _this._currentSelectedTenant.Configurations.splice(removeIndex, 1);
                                    _this._tenantService.updateTenant(tenantToUpdate, UpdateActionType.TenantConfigurationRemove, function (data) {
                                        _this._rtvTenantConfigurations.get_nodes().getNode(0).get_nodes().removeAt(_this._rtvTenantConfigurations.get_selectedNode().get_index());
                                        if (_this._rtvTenantConfigurations.get_nodes().getNode(0).get_nodes().getItem(0) === undefined)
                                            _this._rtvTenantConfigurations.get_nodes().clear();
                                    }, function (exception) {
                                        _this._loadingPanel.hide(_this.splitterMainId);
                                        $("#".concat(_this.rtvTenantsId)).hide();
                                        _this.showNotificationException(_this.uscNotificationId, exception);
                                    });
                                }
                            }, 400, 300);
                        }
                        else {
                            alert("Selezionare un configurazione");
                        }
                        args.set_cancel(true);
                        break;
                }
            };
            _this.btnTenantConfigurationOk_onClick = function (sender, args) {
                if (_this._dpStartDateFrom && _this._dpStartDateFrom.get_selectedDate() &&
                    _this._txtTenantConfigurationJsonValue && _this._txtTenantConfigurationJsonValue.get_textBoxValue() !== "" &&
                    _this._cmbConfigurationType && _this._cmbConfigurationType.get_selectedItem().get_text() !== "") {
                    var viewModelMapper = new TenantViewModelMapper();
                    var tenantConfiguration_1 = {
                        Tenant: viewModelMapper.Map(_this._currentSelectedTenant),
                        ConfigurationType: _this._cmbConfigurationType.get_selectedItem().get_value(),
                        EndDate: (_this._dpEndDateFrom && _this._dpEndDateFrom.get_selectedDate())
                            ? moment(_this._dpEndDateFrom.get_selectedDate()).format("MM-DD-YYYY")
                            : "",
                        StartDate: (_this._dpStartDateFrom && _this._dpStartDateFrom.get_selectedDate())
                            ? moment(_this._dpStartDateFrom.get_selectedDate()).format("MM-DD-YYYY")
                            : "",
                        JsonValue: _this._txtTenantConfigurationJsonValue ? _this._txtTenantConfigurationJsonValue.get_textBoxValue() : "",
                        Note: _this._txtTenantConfigurationNote ? _this._txtTenantConfigurationNote.get_textBoxValue() : "",
                        UniqueId: _this.currentTenantConfigurationUniqueId
                    };
                    _this._rwTenantConfiguration.close();
                    _this._loadingPanel.show(_this.tbConfigurationControlId);
                    var nodeImageUrl_4 = "../App_Themes/DocSuite2008/imgset16/box_open.png";
                    var nodeValue_5 = tenantConfiguration_1.UniqueId;
                    var nodeText_5 = _this._cmbConfigurationType.get_selectedItem().get_text();
                    var alreadySavedInTree = _this.alreadySavedInTree(nodeValue_5, _this._rtvTenantConfigurations);
                    if (!alreadySavedInTree) {
                        _this._currentSelectedTenant.Configurations.push(tenantConfiguration_1);
                        var tenantToUpdate = _this.constructTenant();
                        tenantToUpdate.Configurations = [tenantConfiguration_1];
                        _this._tenantService.updateTenant(tenantToUpdate, UpdateActionType.TenantConfigurationAdd, function (data) {
                            var selectedIndex = _this._currentSelectedTenant.Configurations.map(function (item) { return item.UniqueId; })
                                .indexOf(tenantConfiguration_1.UniqueId);
                            _this._currentSelectedTenant.Configurations[selectedIndex].UniqueId = data.Configurations.$values[selectedIndex].UniqueId;
                            nodeValue_5 = _this._currentSelectedTenant.Configurations[selectedIndex].UniqueId;
                            _this.addNodesToRadTreeView(nodeValue_5, nodeText_5, "Configurazioni", nodeImageUrl_4, _this._rtvTenantConfigurations);
                        }, function (exception) {
                            _this._loadingPanel.hide(_this.splitterMainId);
                            $("#".concat(_this.rtvTenantsId)).hide();
                            _this.showNotificationException(_this.uscNotificationId, exception);
                        });
                    }
                    else {
                        var editIndex = _this._currentSelectedTenant.Configurations.map(function (item) { return item.UniqueId; })
                            .indexOf(_this._rtvTenantConfigurations.get_selectedNode().get_value());
                        _this._currentSelectedTenant.Configurations[editIndex] = tenantConfiguration_1;
                        _this._tenantConfigurationService.updateTenantConfiguration(tenantConfiguration_1, function (data) {
                            _this._rtvTenantConfigurations.get_selectedNode().set_text(nodeText_5);
                            _this._rtvTenantConfigurations.get_selectedNode().set_value(nodeValue_5);
                        }, function (exception) {
                            _this._loadingPanel.hide(_this.splitterMainId);
                            $("#".concat(_this.rtvTenantsId)).hide();
                            _this.showNotificationException(_this.uscNotificationId, exception);
                        });
                    }
                    _this._loadingPanel.hide(_this.tbConfigurationControlId);
                }
            };
            _this.btnTenantConfigurationCancel_onClick = function (sender, args) {
                _this._rwTenantConfiguration.close();
            };
            _this.deleteTenantContactPromise = function (contactId) {
                var promise = $.Deferred();
                if (contactId) {
                    _this._manager.radconfirm("Sei sicuro di voler eliminare il contatto selezionato?", function (arg) {
                        if (arg) {
                            var contactParent = _this._currentSelectedTenant.Contacts.filter(function (contact) { return contact.EntityId === contactId; })[0];
                            var contactParentId_1 = null;
                            if (contactParent) {
                                contactParentId_1 = contactParent.IncrementalFather;
                            }
                            var tenantToUpdate = _this.constructTenant();
                            tenantToUpdate.Contacts = _this._currentSelectedTenant.Contacts.filter(function (contact) { return contact.EntityId === contactId && contact.IncrementalFather !== contactId; });
                            _this._currentSelectedTenant.Contacts = _this._currentSelectedTenant.Contacts.filter(function (contact) { return contact.EntityId !== contactId && contact.IncrementalFather !== contactId; });
                            _this._tenantService.updateTenant(tenantToUpdate, UpdateActionType.TenantContactRemove, function (data) {
                                promise.resolve(contactParentId_1);
                                _this._loadingPanel.hide(_this.splitterMainId);
                            }, function (exception) {
                                _this._loadingPanel.hide(_this.splitterMainId);
                                $("#".concat(_this.rtvTenantsId)).hide();
                                _this.showNotificationException(_this.uscNotificationId, exception);
                            });
                        }
                    }, 400, 300);
                }
                return promise.promise();
            };
            _this.updateTenantContactPromise = function (newContactAdded) {
                var promise = $.Deferred();
                if (newContactAdded) {
                    _this._currentSelectedTenant.Contacts.push(newContactAdded);
                    _this._loadingPanel.show(_this.splitterMainId);
                    var tenantToUpdate = _this.constructTenant();
                    tenantToUpdate.Contacts = [newContactAdded];
                    _this._tenantService.updateTenant(tenantToUpdate, UpdateActionType.TenantContactAdd, function (data) {
                        promise.resolve(data);
                        _this._loadingPanel.hide(_this.splitterMainId);
                    }, function (exception) {
                        _this._loadingPanel.hide(_this.splitterMainId);
                        $("#".concat(_this.rtvTenantsId)).hide();
                        _this.showNotificationException(_this.uscNotificationId, exception);
                    });
                }
                return promise.promise();
            };
            _this.updateAllTenantContactPromise = function () {
                return _this.addAllContactsOrRolesToTenant(UpdateActionType.TenantContactAddAll);
            };
            _this.deleteAllTenantContactPromise = function () {
                return _this.deleteAllContactsOrRolesFromTenant(UpdateActionType.TenantContactRemoveAll);
            };
            _this._serviceConfiguration = serviceConfigurations;
            _this._currentSelectedTenant = new TenantViewModel();
            _this.selectedTenantConfiguration = new TenantConfigurationModel();
            _this._enumHelper = new EnumHelper();
            return _this;
        }
        TbltTenant.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._manager = $find(this.managerId);
            this._toolbarSearch = $find(this.toolBarSearchId);
            this._toolbarSearch.add_buttonClicking(this.toolbarSearch_onClick);
            //rad tree views
            this._rtvTenants = $find(this.rtvTenantsId);
            this._rtvTenants.add_nodeExpanded(this.tenantAOO_onExpanded);
            this._rtvTenants.add_nodeClicked(this.rtvTenants_onClick);
            this._rtvContainers = $find(this.rtvContainersId);
            this._rtvPECMailBoxes = $find(this.rtvPECMailBoxesId);
            this._rtvTenantWorkflowRepositories = $find(this.rtvWorkflowRepositoriesId);
            this._rtvTenantWorkflowRepositories.add_nodeClicked(this.rtvTenantWorkflowrepositories_onNodeClick);
            this._rtvTenantConfigurations = $find(this.rtvTenantConfigurationsId);
            // details right zone
            this._toolbarItemSearchTenantName = this._toolbarSearch.findItemByValue("searchTenantName");
            this._toolbarItemSearchCompanyName = this._toolbarSearch.findItemByValue("searchCompanyName");
            this._txtSearchTenantName = this._toolbarItemSearchTenantName.findControl("txtSearchTenantName");
            this._txtSearchCompanyName = this._toolbarItemSearchCompanyName.findControl("txtSearchCompanyName");
            this._lblCompanyNameId = document.getElementById(this.lblCompanyNameId);
            this._lblTenantNameId = document.getElementById(this.lblTenantNameId);
            this._lblTenantNoteId = document.getElementById(this.lblTenantNoteId);
            this._lblTenantDataDiAttivazioneId = document.getElementById(this.lblTenantDataDiAttivazioneId);
            this._lblTenantDataDiDisattivazioneId = document.getElementById(this.lblTenantDataDiDisattivazioneId);
            this._txtTenantAOONameInfo = document.getElementById(this.txtTenantAOONameInfoId);
            this._txtTenantAOONoteInfo = document.getElementById(this.txtTenantAOONoteInfoId);
            this._txtTenantAOOSuffixInfo = document.getElementById(this.txtTenantAOOSuffixInfoId);
            //Containers, PECMailBoxes, Rules, WorkflowReposiory, TenantConfiguration
            this._toolbarContainer = $find(this.tbContainersControlId);
            this._toolbarContainer.add_buttonClicking(this.toolbarContainer_onClick);
            this._toolbarPECMailBox = $find(this.tbPECMailBoxesControlId);
            this._toolbarPECMailBox.add_buttonClicking(this.toolbarPECMailBox_onClick);
            this._toolbarWorkflowRepository = $find(this.tbWorkflowRepositoryControlId);
            this._toolbarWorkflowRepository.add_buttonClicking(this.toolbarWorkflowRepository_onClick);
            this._tbConfigurationControl = $find(this.tbConfigurationControlId);
            this._tbConfigurationControl.add_buttonClicking(this.toolbarConfiguration_onClick);
            this._rtbCompanyOptions = $find(this.rtbCompanyOptionsId);
            this._rtbCompanyOptions.add_buttonClicked(this.rtbCompanyOptions_onClick);
            // windows
            this._rwContainer = $find(this.rwContainerId);
            this._rwContainer.add_show(this._rwContainer_OnShow);
            this._rwPECMailBox = $find(this.rwPECMailBoxId);
            this._rwPECMailBox.add_show(this._rwPECMailBox_OnShow);
            this._rwTenantConfiguration = $find(this.rwTenantConfigurationId);
            this._rwWorkflowRepository = $find(this.rwWorkflowRepositoryId);
            this._rwWorkflowRepository.add_show(this._rwWorkflowRepository_OnShow);
            this._rwTenantSelector = $find(this.rwTenantSelectorId);
            //combos from windows
            this._cmbContainer = $find(this.cmbContainerId);
            this._cmbContainer.add_selectedIndexChanged(this.cmbContainers_onClick);
            this._cmbContainer.add_itemsRequested(this._cmbContainer_OnClientItemsRequested);
            this._cmbPECMailBox = $find(this.cmbPECMailBoxId);
            this._cmbPECMailBox.add_selectedIndexChanged(this.cmbPECMailBoxes_onClick);
            this._cmbPECMailBox.add_itemsRequested(this._cmbPECMailBox_OnClientItemsRequested);
            this._cmbRole = $find(this.cmbRoleId);
            this._cmbWorkflowRepository = $find(this.cmbWorkflowRepositoryId);
            this._cmbWorkflowRepository.add_selectedIndexChanged(this.cmbWorkflowRepositories_onClick);
            this._cmbWorkflowRepository.add_itemsRequested(this._cmbWorkflowRepository_OnClientItemsRequested);
            this._cmbConfigurationType = $find(this.cmbConfigurationTypeId);
            this._cmbTenantWorkflowRepositoryType = $find(this.cmbTenantWorkflowRepositoryTypeId);
            // Window buttons Confirm, Cancel
            this._btnContainerOk = $find(this.btnContainerSelectorOkId);
            this._btnContainerOk.add_clicking(this.btnContainerOk_onClick);
            this._btnContainerCancel = $find(this.btnContainerSelectorCancelId);
            this._btnContainerCancel.add_clicking(this.btnContainerCancel_onClick);
            this._btnPECMailBoxSelectorOk = $find(this.btnPECMailBoxSelectorOkId);
            this._btnPECMailBoxSelectorOk.add_clicking(this.btnPECMailBoxOk_onClick);
            this._btnPECMailBoxSelectorCancel = $find(this.btnPECMailBoxSelectorCancelId);
            this._btnPECMailBoxSelectorCancel.add_clicking(this.btnPECMailBoxCancel_onClick);
            this._btnWorkflowRepositorySelectorOk = $find(this.btnWorkflowRepositorySelectorOkId);
            this._btnWorkflowRepositorySelectorOk.add_clicking(this.btnWorkflowRepositoryOk_onClick);
            this._btnWorkflowRepositorySelectorCancel = $find(this.btnWorkflowRepositorySelectorCancelId);
            this._btnWorkflowRepositorySelectorCancel.add_clicking(this.btnWorkflowRepositoryCancel_onClick);
            this._btnTenantConfigurationSelectorOk = $find(this.btnTenantConfigurationSelectorOkId);
            this._btnTenantConfigurationSelectorOk.add_clicking(this.btnTenantConfigurationOk_onClick);
            this._btnTenantConfigurationSelectorCancel = $find(this.btnTenantConfigurationSelectorCancelId);
            this._btnTenantConfigurationSelectorCancel.add_clicking(this.btnTenantConfigurationCancel_onClick);
            this._btnTenantSelectorOk = $find(this.btnTenantSelectorOkId);
            this._btnTenantSelectorOk.add_clicking(this.btnTenantSelectorOk_onClick);
            this._btnTenantSelectorCancel = $find(this.btnTenantSelectorCancelId);
            this._btnTenantSelectorCancel.add_clicking(this.btnTenantSelectorCancel_onClick);
            this._btnTenantAOOSelectorOk = $find(this.btnTenantAOOSelectorOkId);
            this._btnTenantAOOSelectorOk.add_clicked(this.btnTenantAOOSelectorOk_onClick);
            this._btnTenantAOOSelectorCancel = $find(this.btnTenantAOOSelectorCancelId);
            this._btnTenantAOOSelectorCancel.add_clicked(this.btnTenantAOOSelectorCancel_onClick);
            // window configuration fields
            this._dpStartDateFrom = $find(this.dpStartDateFromId);
            this._dpEndDateFrom = $find(this.dpEndDateFromId);
            this._txtTenantConfigurationNote = $find(this.tenantConfigurationNoteId);
            this._dpTenantDateFrom = $find(this.dpTenantDateFromId);
            this._dpTenantDateTo = $find(this.dpTenantDateToId);
            this._txtTenantName = $find(this.txtTenantNameId);
            this._txtTenantCompany = $find(this.txtTenantCompanyId);
            this._txtTenantNote = $find(this.txtTenantNoteId);
            this._txtTenantConfigurationJsonValue = $find(this.txtTenantConfigurationJsonValueId);
            this._dpTenantWorkflowRepositoryDateFrom = $find(this.dpTenantWorkflowRepositoryDateFromId);
            this._dpTenantWorkflowRepositoryDateTo = $find(this.dpTenantWorkflowRepositoryDateToId);
            this._txtTenantWorkflowRepositoryJsonValue = $find(this.txtTenantWorkflowRepositoryJsonValueId);
            this._txtTenantWorkflowRepositoryIntegrationModuleName = $find(this.txtTenantWorkflowRepositoryIntegrationModuleNameId);
            this._txtTenantWorkflowRepositoryConditions = $find(this.txtTenantWorkflowRepositoryConditionsId);
            this._txtTenantAOOName = $find(this.txtTenantAOONameId);
            this._txtTenantAOONote = $find(this.txtTenantAOONoteId);
            this._txtCategorySuffix = $find(this.txtCategorySuffixId);
            this._uscRoleRest = $("#" + this.uscRoleRestId).data();
            this._uscContattiSelRest = $("#" + this.uscContattiSelRestId).data();
            var searchDTO = null;
            $("#tenantLinkOptions").hide();
            $("#tenantAOOInfo").hide();
            this.loadTenantsAOO(searchDTO);
        };
        TbltTenant.prototype.populateTenantConfigurations = function () {
            this._cmbConfigurationType.clearItems();
            this._cmbTenantWorkflowRepositoryType.clearItems();
            var cmbItem = null;
            for (var n in TenantConfigurationTypeEnum) {
                if (typeof TenantConfigurationTypeEnum[n] === 'string') {
                    cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                    cmbItem.set_text(this._enumHelper.getTenantConfigurationTypeDescription(TenantConfigurationTypeEnum[n]));
                    cmbItem.set_value(TenantConfigurationTypeEnum[n]);
                    this._cmbConfigurationType.get_items().add(cmbItem);
                }
            }
            for (var n in TenantWorkflowRepositoryTypeEnum) {
                if (typeof TenantWorkflowRepositoryTypeEnum[n] === 'string') {
                    cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                    cmbItem.set_text(this._enumHelper.getTenantWorkflowRepositoryTypeDescription(TenantWorkflowRepositoryTypeEnum[n]));
                    cmbItem.set_value(TenantWorkflowRepositoryTypeEnum[n]);
                    this._cmbTenantWorkflowRepositoryType.get_items().add(cmbItem);
                }
            }
        };
        TbltTenant.prototype.getTenantsForTenantAOO = function (args) {
            var _this = this;
            this._tenantAOOService.getTenantsByTenantAOOId(args.get_node().get_value(), function (data) {
                var node;
                args.get_node().get_nodes().clear();
                for (var i = 0; i < data[0].Tenants.length; i++) {
                    var tenant = data[0].Tenants[i];
                    node = new Telerik.Web.UI.RadTreeNode();
                    var rtvNodeText = tenant.CompanyName + " (" + tenant.TenantName + ")";
                    node.set_text(rtvNodeText);
                    node.set_value(tenant.UniqueId);
                    node.get_attributes().setAttribute("nodeType", TenantAOOAttribute.Tenant);
                    node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/tenant.png");
                    var nodeIndex = args.get_node().get_index();
                    _this._rtvTenants.get_nodes().getNode(0).get_nodes().getNode(nodeIndex).get_nodes().add(node);
                }
                _this._loadingPanel.hide(_this.splitterMainId);
            }, function (exception) {
                _this._loadingPanel.hide(_this.splitterMainId);
                $("#".concat(_this.rtvTenantsId)).hide();
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        TbltTenant.prototype.loadTenantAOODetails = function (node) {
            var _this = this;
            if (node.get_attributes().getAttribute("nodeType") !== TenantAOOAttribute.AOO) {
                return;
            }
            this._tenantAOOService.getTenantAOOById(node.get_value(), function (data) {
                _this._txtTenantAOONameInfo.innerText = data.Name;
                _this._txtTenantAOONoteInfo.innerText = data.Note;
                _this._txtTenantAOOSuffixInfo.innerText = data.CategorySuffix;
            }, function (exception) {
                _this._loadingPanel.hide(_this.splitterMainId);
                $("#".concat(_this.rtvTenantsId)).hide();
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        TbltTenant.prototype.loadTenantDetails = function (node) {
            var _this = this;
            if (node.get_attributes().getAttribute("nodeType") !== TenantAOOAttribute.Tenant) {
                return;
            }
            var tenantId = node.get_value();
            this._tenantService.getTenantById(tenantId, function (data) {
                _this._currentSelectedTenant = $.extend({}, data);
                _this._lblCompanyNameId.innerText = data !== undefined ? data.CompanyName : "";
                _this._lblTenantNameId.innerText = data !== undefined ? data.TenantName : "";
                _this._lblTenantNoteId.innerText = data.Note !== null ? data.Note : "";
                _this._lblTenantDataDiAttivazioneId.innerText = data !== undefined && moment(data.StartDate).isValid() ? moment(data.StartDate).format("DD-MM-YYYY") : "";
                _this._lblTenantDataDiDisattivazioneId.innerText = data !== undefined && moment(data.EndDate).isValid() ? moment(data.EndDate).format("DD-MM-YYYY") : "";
                _this.populateContainersTreeView();
                _this.populatePECMailBoxesTreeView();
                _this.populateTenantWorkflowRepositoriesTreeView();
                _this.populateTenantConfigurationsTreeView();
                _this.populateWorkflowRepositoryComboBox();
                _this.populateContactTreeView();
                _this.registerUscContattiRestEventHandlers();
                _this.populateRolesTree();
                _this.registerUscRoleRestEventHandlers();
            }, function (exception) {
                _this._loadingPanel.hide(_this.splitterMainId);
                $("#".concat(_this.rtvTenantsId)).hide();
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        //region [ Roles tree view ]
        TbltTenant.prototype.populateRolesTree = function () {
            var _this = this;
            this._loadingPanel.show(this.splitterMainId);
            this._roleService.getTenantRoles(this._currentSelectedTenant.UniqueId, function (tenantRoles) {
                _this._uscRoleRest.renderRolesTree(tenantRoles);
                _this._currentSelectedTenant.Roles = tenantRoles;
                _this._loadingPanel.hide(_this.splitterMainId);
            }, function (exception) {
                _this._loadingPanel.hide(_this.splitterMainId);
                $("#".concat(_this.rtvTenantsId)).hide();
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        TbltTenant.prototype.openEditTenantWindow = function () {
            $("#TenantSelectorSelectorWindowTable").show();
            $("#TenantAOOSelectorWindowTable").hide();
            if (this._rtvTenants.get_selectedNode() !== null) {
                this._dpTenantDateFrom.set_selectedDate(moment(this._currentSelectedTenant.StartDate).isValid() ? new Date(this._currentSelectedTenant.StartDate) : null);
                this._dpTenantDateTo.set_selectedDate(moment(this._currentSelectedTenant.EndDate).isValid() ? new Date(this._currentSelectedTenant.EndDate) : null);
                this._txtTenantName.set_value(this._currentSelectedTenant.TenantName);
                this._txtTenantCompany.set_value(this._currentSelectedTenant.CompanyName);
                this._txtTenantNote.set_value(this._currentSelectedTenant.Note);
                this.isTenantUpdate = true;
                this._dpTenantDateTo.get_element().parentElement.style.visibility = "visible";
                this._rwTenantSelector.set_title("Modifica UO");
                this._rwTenantSelector.set_height(220);
                this._rwTenantSelector.show();
            }
            else {
                alert("Selezionare un UP");
            }
        };
        TbltTenant.prototype.openEditTenantAOOWindow = function () {
            var _this = this;
            $("#TenantSelectorSelectorWindowTable").hide();
            $("#TenantAOOSelectorWindowTable").show();
            this.isTenantAOOUpdate = true;
            var tenantAOOId = this._rtvTenants.get_selectedNode().get_value();
            this._tenantAOOService.getTenantAOOById(tenantAOOId, function (data) {
                _this._txtTenantAOOName.set_value(data.Name);
                _this._txtTenantAOONote.set_value(data.Note);
                _this._txtCategorySuffix.set_value(data.CategorySuffix);
                _this._rwTenantSelector.set_title("Modifica AOO");
                _this._rwTenantSelector.set_height(180);
                _this._rwTenantSelector.show();
            });
        };
        TbltTenant.prototype.openInsertTenantAOOWindow = function () {
            $("#TenantSelectorSelectorWindowTable").hide();
            $("#TenantAOOSelectorWindowTable").show();
            this._txtTenantAOOName.clear();
            this._txtTenantAOONote.clear();
            this._txtCategorySuffix.clear();
            this.isTenantAOOUpdate = false;
            this._rwTenantSelector.set_title("Aggiungi AOO");
            this._rwTenantSelector.set_height(180);
            this._rwTenantSelector.show();
        };
        TbltTenant.prototype.openInsertTenantWindow = function () {
            $("#TenantSelectorSelectorWindowTable").show();
            $("#TenantAOOSelectorWindowTable").hide();
            this._dpTenantDateFrom.clear();
            this._txtTenantName.clear();
            this._txtTenantCompany.clear();
            this._txtTenantNote.clear();
            this.isTenantUpdate = false;
            this._dpTenantDateTo.get_element().parentElement.style.visibility = "hidden";
            this._dpTenantDateFrom.set_selectedDate(new Date());
            this._rwTenantSelector.set_title("Aggiungi UO");
            this._rwTenantSelector.set_height(220);
            this._rwTenantSelector.show();
        };
        TbltTenant.prototype.registerUscRoleRestEventHandlers = function () {
            var uscRoleRestEvents = this._uscRoleRest.uscRoleRestEvents;
            this._uscRoleRest.registerEventHandler(uscRoleRestEvents.RoleDeleted, this.deleteTenantRolePromise);
            this._uscRoleRest.registerEventHandler(uscRoleRestEvents.NewRolesAdded, this.updateTenantRolesPromise);
            this._uscRoleRest.registerEventHandler(uscRoleRestEvents.AllRolesAdded, this.updateAllTenantRolesPromise);
            this._uscRoleRest.registerEventHandler(uscRoleRestEvents.AllRolesDeleted, this.deleteAllTenantRolesPromise);
        };
        TbltTenant.prototype.loadTenantsAOO = function (searchDTO) {
            var _this = this;
            this._loadingPanel.show(this.splitterMainId);
            if (searchDTO) {
                this._tenantAOOService.getFilteredTenants(searchDTO, function (data) {
                    _this.createTenantAOORootNode();
                    var node;
                    for (var i = 0; i < data.length; i++) {
                        var tenantAOO = data[i];
                        if (tenantAOO.Tenants.length == 0) {
                            continue;
                        }
                        node = new Telerik.Web.UI.RadTreeNode();
                        node.set_text(tenantAOO.Name);
                        node.set_value(tenantAOO.UniqueId);
                        node.get_attributes().setAttribute("nodeType", TenantAOOAttribute.AOO);
                        node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/tenantAOO.png");
                        node.expand();
                        _this._rtvTenants.get_nodes().getNode(0).get_nodes().add(node);
                        var nodeIndex = node.get_index();
                        for (var j = 0; j < tenantAOO.Tenants.length; j++) {
                            var tenant = tenantAOO.Tenants[j];
                            node = new Telerik.Web.UI.RadTreeNode();
                            var rtvNodeText = tenant.CompanyName + " (" + tenant.TenantName + ")";
                            node.set_text(rtvNodeText);
                            node.set_value(tenant.UniqueId);
                            node.get_attributes().setAttribute("nodeType", TenantAOOAttribute.Tenant);
                            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/tenant.png");
                            _this._rtvTenants.get_nodes().getNode(0).get_nodes().getNode(nodeIndex).get_nodes().add(node);
                            var rtMinusElement = node.get_element().parentElement.parentElement.getElementsByClassName("rtMinus").item(0);
                            rtMinusElement.style.display = "none";
                        }
                    }
                    _this._loadingPanel.hide(_this.splitterMainId);
                }, function (exception) {
                    _this._loadingPanel.hide(_this.splitterMainId);
                    $("#".concat(_this.rtvTenantsId)).hide();
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            }
            else {
                this._tenantAOOService.getTenantsAOO(function (data) {
                    _this.createTenantAOORootNode();
                    var node;
                    for (var i = 0; i < data.length; i++) {
                        var tenantAOO = data[i];
                        node = new Telerik.Web.UI.RadTreeNode();
                        node.set_text(tenantAOO.Name);
                        node.set_value(tenantAOO.UniqueId);
                        node.get_attributes().setAttribute("nodeType", TenantAOOAttribute.AOO);
                        node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/tenantAOO.png");
                        _this._rtvTenants.get_nodes().getNode(0).get_nodes().add(node);
                        node = new Telerik.Web.UI.RadTreeNode();
                        node.set_text("");
                        _this._rtvTenants.get_nodes().getNode(0).get_nodes().getNode(i).get_nodes().add(node);
                    }
                    _this._loadingPanel.hide(_this.splitterMainId);
                }, function (exception) {
                    _this._loadingPanel.hide(_this.splitterMainId);
                    $("#".concat(_this.rtvTenantsId)).hide();
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            }
        };
        TbltTenant.prototype.createTenantAOORootNode = function () {
            this._rtvTenants.get_nodes().clear();
            var rtvNode;
            rtvNode = new Telerik.Web.UI.RadTreeNode();
            rtvNode.set_text("AOO");
            rtvNode.expand();
            this._rtvTenants.get_nodes().add(rtvNode);
            rtvNode.select();
            this._rtbCompanyOptions.findItemByValue("modify").set_enabled(false);
        };
        TbltTenant.prototype.insertTenantAOO = function (tenantAOOModel) {
            var _this = this;
            this._tenantAOOService.insertTenantAOO(tenantAOOModel, function (data) {
                var node = new Telerik.Web.UI.RadTreeNode();
                node.set_text(data.Name);
                node.set_value(data.UniqueId);
                node.get_attributes().setAttribute("nodeType", TenantAOOAttribute.AOO);
                if (tenantAOOModel.TenantTypology == TenantTypologyTypeEnum.InternalTenant) {
                    _this._rtvTenants.get_nodes().getNode(0).get_nodes().add(node);
                }
                _this._rwTenantSelector.close();
            }, function (exception) {
                _this._loadingPanel.hide(_this.splitterMainId);
                $("#".concat(_this.rtvTenantsId)).hide();
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        TbltTenant.prototype.updateTenantAOO = function (tenantAOOModel) {
            var _this = this;
            tenantAOOModel.UniqueId = this._rtvTenants.get_selectedNode().get_value();
            this._tenantAOOService.updateTenantAOO(tenantAOOModel, function (data) {
                _this._rtvTenants.get_selectedNode().set_text(data.Name);
                _this._rwTenantSelector.close();
                _this._txtTenantAOONameInfo.innerHTML = tenantAOOModel.Name;
                _this._txtTenantAOONoteInfo.innerHTML = tenantAOOModel.Note;
                _this._txtTenantAOOSuffixInfo.innerHTML = tenantAOOModel.CategorySuffix;
            }, function (exception) {
                _this._loadingPanel.hide(_this.splitterMainId);
                $("#".concat(_this.rtvTenantsId)).hide();
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        TbltTenant.prototype.addContainers = function (containers, cmbContainer) {
            this.containers = containers;
            cmbContainer.get_items().clear();
            var item;
            item = new Telerik.Web.UI.RadComboBoxItem();
            item.set_text("");
            item.set_value("");
            cmbContainer.get_items().add(item);
            for (var _i = 0, containers_1 = containers; _i < containers_1.length; _i++) {
                var container = containers_1[_i];
                item = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(container.Name);
                item.set_value(container.EntityShortId.toString());
                cmbContainer.get_items().add(item);
            }
        };
        TbltTenant.prototype.populateContainersTreeView = function () {
            var _this = this;
            this._tenantService.getTenantContainers(this._currentSelectedTenant.UniqueId, function (data) {
                if (data === undefined) {
                    return;
                }
                else {
                    _this._rtvContainers.get_nodes().clear();
                    var thisObj_2 = _this;
                    _this._currentSelectedTenant.Containers = data;
                    $.each(data, function (i, value) {
                        var nodeImageUrl = "../App_Themes/DocSuite2008/imgset16/box_open.png";
                        var nodeValue = value.EntityShortId.toString();
                        var nodeText = value.Name;
                        var alreadySavedInTree = thisObj_2.alreadySavedInTree(nodeValue, thisObj_2._rtvContainers);
                        if (!alreadySavedInTree) {
                            thisObj_2.addNodesToRadTreeView(nodeValue, nodeText, "Contenitori", nodeImageUrl, thisObj_2._rtvContainers);
                        }
                    });
                }
            }, function (exception) {
                _this._loadingPanel.hide(_this.splitterMainId);
                $("#".concat(_this.rtvTenantsId)).hide();
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        TbltTenant.prototype.addOrRemoveAllTenantContainers = function (message, actionType) {
            var _this = this;
            this._manager.radconfirm(message, function (arg) {
                if (arg) {
                    _this._loadingPanel.show(_this.tbContainersControlId);
                    _this._tenantService.updateTenant(_this._currentSelectedTenant, actionType, function (data) {
                        _this.populateContainersTreeView();
                        _this._loadingPanel.hide(_this.tbContainersControlId);
                    }, function (exception) {
                        _this._loadingPanel.hide(_this.tbContainersControlId);
                        _this.showNotificationException(_this.uscNotificationId, exception);
                    });
                }
                document.getElementsByTagName("body")[0].setAttribute("class", "comm chrome");
            }, 400, 300);
        };
        TbltTenant.prototype.addPECMailBoxes = function (pecMailBoxes, cmbPECMailBox) {
            this.pecMailBoxes = pecMailBoxes;
            cmbPECMailBox.get_items().clear();
            var item;
            item = new Telerik.Web.UI.RadComboBoxItem();
            item.set_text("");
            item.set_value("");
            cmbPECMailBox.get_items().add(item);
            for (var _i = 0, pecMailBoxes_1 = pecMailBoxes; _i < pecMailBoxes_1.length; _i++) {
                var pecMailBox = pecMailBoxes_1[_i];
                item = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(pecMailBox.MailBoxRecipient);
                item.set_value(pecMailBox.EntityShortId.toString());
                cmbPECMailBox.get_items().add(item);
            }
        };
        TbltTenant.prototype.populatePECMailBoxesTreeView = function () {
            var _this = this;
            this._tenantService.getTenantPECMailBoxes(this._currentSelectedTenant.UniqueId, function (data) {
                if (data === undefined) {
                    return;
                }
                else {
                    _this._rtvPECMailBoxes.get_nodes().clear();
                    var thisObj_3 = _this;
                    _this._currentSelectedTenant.PECMailBoxes = data;
                    $.each(data, function (i, value) {
                        var nodeImageUrl = "../App_Themes/DocSuite2008/imgset16/box_open.png";
                        var nodeValue = value.EntityShortId.toString();
                        var nodeText = value.MailBoxRecipient;
                        var alreadySavedInTree = thisObj_3.alreadySavedInTree(nodeValue, thisObj_3._rtvPECMailBoxes);
                        if (!alreadySavedInTree) {
                            thisObj_3.addNodesToRadTreeView(nodeValue, nodeText, "Caselle PEC", nodeImageUrl, thisObj_3._rtvPECMailBoxes);
                        }
                    });
                }
            }, function (exception) {
                _this._loadingPanel.hide(_this.splitterMainId);
                $("#".concat(_this.rtvTenantsId)).hide();
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        TbltTenant.prototype.addWorkflowRepositories = function (workflowRepositories, cmbWorkflowRepository) {
            this.workflowRepositories = workflowRepositories;
            cmbWorkflowRepository.get_items().clear();
            var item;
            item = new Telerik.Web.UI.RadComboBoxItem();
            item.set_text("");
            item.set_value("");
            cmbWorkflowRepository.get_items().add(item);
            for (var _i = 0, workflowRepositories_1 = workflowRepositories; _i < workflowRepositories_1.length; _i++) {
                var workflowRepository = workflowRepositories_1[_i];
                item = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(workflowRepository.Name);
                item.set_value(workflowRepository.UniqueId);
                cmbWorkflowRepository.get_items().add(item);
            }
        };
        TbltTenant.prototype.populateTenantWorkflowRepositoriesTreeView = function () {
            var _this = this;
            this._tenantService.getTenantWorkflowRepositories(this._currentSelectedTenant.UniqueId, function (data) {
                if (data === undefined) {
                    return;
                }
                else {
                    _this._rtvTenantWorkflowRepositories.get_nodes().clear();
                    var thisObj_4 = _this;
                    _this._currentSelectedTenant.TenantWorkflowRepositories = data;
                    _this.tenantWorkflowRepositories = _this._currentSelectedTenant.TenantWorkflowRepositories;
                    $.each(data, function (i, value) {
                        var nodeImageUrl = "../App_Themes/DocSuite2008/imgset16/box_open.png";
                        var nodeValue = value.UniqueId;
                        var nodeText = value.WorkflowRepository.Name;
                        var alreadySavedInTree = thisObj_4.alreadySavedInTree(nodeValue, thisObj_4._rtvTenantWorkflowRepositories);
                        if (!alreadySavedInTree) {
                            thisObj_4.addNodesToRadTreeView(nodeValue, nodeText, "Attività", nodeImageUrl, thisObj_4._rtvTenantWorkflowRepositories);
                        }
                    });
                }
            }, function (exception) {
                _this._loadingPanel.hide(_this.splitterMainId);
                $("#".concat(_this.rtvTenantsId)).hide();
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        TbltTenant.prototype.populateWorkflowRepositoryComboBox = function () {
            var _this = this;
            this._workflowRepositoryService.getWorkflowRepositories(function (data) {
                _this.addWorkflowRepositories(data, _this._cmbWorkflowRepository);
            }, function (exception) {
                _this._loadingPanel.hide(_this.splitterMainId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        TbltTenant.prototype.populateTenantConfigurationsTreeView = function () {
            var _this = this;
            this._tenantService.getTenantConfigurations(this._currentSelectedTenant.UniqueId, function (data) {
                if (data === undefined) {
                    return;
                }
                else {
                    _this._rtvTenantConfigurations.get_nodes().clear();
                    var thisObj_5 = _this;
                    _this._currentSelectedTenant.Configurations = data;
                    $.each(data, function (i, value) {
                        var nodeImageUrl = "../App_Themes/DocSuite2008/imgset16/box_open.png";
                        var nodeValue = value.UniqueId;
                        var nodeText = thisObj_5._enumHelper.getTenantConfigurationTypeDescription(value.ConfigurationType);
                        var alreadySavedInTree = thisObj_5.alreadySavedInTree(nodeValue, thisObj_5._rtvTenantConfigurations);
                        if (!alreadySavedInTree) {
                            thisObj_5.addNodesToRadTreeView(nodeValue, nodeText, "Configurazioni", nodeImageUrl, thisObj_5._rtvTenantConfigurations);
                        }
                    });
                }
            }, function (exception) {
                _this._loadingPanel.hide(_this.splitterMainId);
                $("#".concat(_this.rtvTenantsId)).hide();
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        //endregion
        //region [Add/Delete TenantContact]
        TbltTenant.prototype.registerUscContattiRestEventHandlers = function () {
            var uscContattiSelRestEvents = this._uscContattiSelRest.uscContattiSelRestEvents;
            this._uscContattiSelRest.registerEventHandler(uscContattiSelRestEvents.ContactDeleted, this.deleteTenantContactPromise);
            this._uscContattiSelRest.registerEventHandler(uscContattiSelRestEvents.NewContactsAdded, this.updateTenantContactPromise);
            this._uscContattiSelRest.registerEventHandler(uscContattiSelRestEvents.AllContactsDeleted, this.deleteAllTenantContactPromise);
            this._uscContattiSelRest.registerEventHandler(uscContattiSelRestEvents.AllContactsAdded, this.updateAllTenantContactPromise);
        };
        TbltTenant.prototype.populateContactTreeView = function () {
            var _this = this;
            this._loadingPanel.show(this.splitterMainId);
            this._tenantService.getTenantContacts(this._currentSelectedTenant.UniqueId, function (data) {
                if (data === undefined) {
                    return;
                }
                else {
                    _this._currentSelectedTenant.Contacts = data;
                    _this._uscContattiSelRest.renderContactsTree(data);
                    _this._loadingPanel.hide(_this.splitterMainId);
                }
            }, function (exception) {
                _this._loadingPanel.hide(_this.splitterMainId);
                $("#".concat(_this.rtvTenantsId)).hide();
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        //endregion
        TbltTenant.prototype.addNodesToRadTreeView = function (nodeValue, nodeText, text, nodeImageUrl, radTreeView) {
            var rtvNode;
            if (radTreeView.get_nodes().get_count() === 0) {
                rtvNode = new Telerik.Web.UI.RadTreeNode();
                rtvNode.set_text(text);
                radTreeView.get_nodes().add(rtvNode);
            }
            rtvNode = new Telerik.Web.UI.RadTreeNode();
            rtvNode.set_text(nodeText);
            rtvNode.set_value(nodeValue);
            rtvNode.set_imageUrl(nodeImageUrl);
            radTreeView.get_nodes().getNode(0).get_nodes().add(rtvNode);
            radTreeView.get_nodes().getNode(0).expand();
        };
        TbltTenant.prototype.alreadySavedInTree = function (nodeValue, radTreeView) {
            var alreadySavedInTree = false;
            if (radTreeView.get_nodes().get_count() !== 0) {
                var allNodes = radTreeView.get_nodes().getNode(0).get_allNodes();
                for (var i = 0; i < allNodes.length; i++) {
                    var node = allNodes[i];
                    if (node.get_value() === nodeValue) {
                        alreadySavedInTree = true;
                        break;
                    }
                }
            }
            return alreadySavedInTree;
        };
        TbltTenant.prototype.addAllContactsOrRolesToTenant = function (actionType) {
            var _this = this;
            var promise = $.Deferred();
            this._tenantService.updateTenant(this._currentSelectedTenant, actionType, function (data) {
                if (actionType === UpdateActionType.TenantContactAddAll) {
                    _this.populateContactTreeView();
                }
                else {
                    _this.populateRolesTree();
                }
            }, function (exception) {
                _this.showNotificationException(_this.uscNotificationId, exception);
                promise.reject(exception);
            });
            return promise.promise();
        };
        TbltTenant.prototype.deleteAllContactsOrRolesFromTenant = function (actionType) {
            var _this = this;
            var promise = $.Deferred();
            this._tenantService.updateTenant(this._currentSelectedTenant, actionType, function (data) {
                promise.resolve();
            }, function (exception) {
                _this.showNotificationException(_this.uscNotificationId, exception);
                promise.reject(exception);
            });
            return promise.promise();
        };
        TbltTenant.prototype.constructTenant = function () {
            var newTenant = new TenantViewModel();
            newTenant.UniqueId = this._currentSelectedTenant.UniqueId;
            newTenant.CompanyName = this._currentSelectedTenant.CompanyName;
            newTenant.TenantName = this._currentSelectedTenant.TenantName;
            newTenant.Note = this._currentSelectedTenant.Note;
            newTenant.StartDate = this._currentSelectedTenant.StartDate;
            newTenant.EndDate = this._currentSelectedTenant.EndDate;
            newTenant.TenantAOO = this._currentSelectedTenant.TenantAOO;
            return newTenant;
        };
        return TbltTenant;
    }(TbltTenantBase));
    return TbltTenant;
});
//# sourceMappingURL=TbltTenant.js.map