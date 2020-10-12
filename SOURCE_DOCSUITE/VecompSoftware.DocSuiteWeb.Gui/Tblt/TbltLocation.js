define(["require", "exports", "App/DTOs/ExceptionDTO", "App/Helpers/EnumHelper", "App/Helpers/ServiceConfigurationHelper", "App/Services/Commons/LocationService", "UserControl/uscLocationDetails"], function (require, exports, ExceptionDTO, EnumHelper, ServiceConfigurationHelper, LocationService, uscLocationDetails) {
    var TbltLocation = /** @class */ (function () {
        function TbltLocation(serviceConfigurations) {
            var _this = this;
            this.rbConfirmInsert_onCLick = function (sender, args) {
                if ($("#insertLocation").is(":visible")) {
                    return _this.insertOrUpdateLocation();
                }
            };
            this.rtvLocations_nodeClicked = function (sender, args) {
                var selectedNode = args.get_node();
                if (selectedNode.get_level() === 0) {
                    $("#" + _this._uscLocationDetails.pnlDetailsId).hide();
                    _this._actionToolBar.findItemByValue(TbltLocation.TOOLBAR_CREATE).enable();
                    _this._actionToolBar.findItemByValue(TbltLocation.TOOLBAR_MODIFY).disable();
                }
                else {
                    $("#" + _this._uscLocationDetails.pnlDetailsId).show();
                    _this._actionToolBar.findItemByValue(TbltLocation.TOOLBAR_CREATE).disable();
                    _this._actionToolBar.findItemByValue(TbltLocation.TOOLBAR_MODIFY).enable();
                    uscLocationDetails.selectedLocationUniqueId = selectedNode.get_value();
                    _this._uscLocationDetails.setLocationDetails();
                }
            };
            this.filterToolbar_onClick = function (sender, args) {
                var name = _this._filterToolbar.findItemByValue(TbltLocation.TOOLBAR_SEARCH_INPUTS).findControl("txtSearch");
                var archive = _this._filterToolbar.findItemByValue(TbltLocation.TOOLBAR_SEARCH_INPUTS).findControl("txtSearchArchive");
                switch (args.get_item().get_value()) {
                    case TbltLocation.TOOLBAR_SEARCH: {
                        _this._rtvLocations.get_nodes().getNode(0).get_nodes().clear();
                        _this.loadTreeViewFiltered(name.get_textBoxValue(), archive.get_textBoxValue());
                        break;
                    }
                }
            };
            this.actionToolBar_onClick = function (sender, args) {
                var selectedNode = _this._rtvLocations.get_selectedNode();
                switch (args.get_item().get_value()) {
                    case TbltLocation.TOOLBAR_CREATE: {
                        _this.clearInputs();
                        _this.selectedLocationId = "";
                        _this._rwInsert.show();
                        _this._rwInsert.set_title("Aggiungi deposito documentale");
                        break;
                    }
                    case TbltLocation.TOOLBAR_MODIFY: {
                        _this.selectedLocationId = selectedNode.get_value();
                        _this.populateLocationInputs(_this.selectedLocationId);
                        _this._rwInsert.show();
                        _this._rwInsert.set_title("Modifica deposito documentale");
                        break;
                    }
                }
            };
            this._serviceConfigurations = serviceConfigurations;
            this._enumHelper = new EnumHelper();
        }
        TbltLocation.prototype.initialize = function () {
            var locationConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltLocation.LOCATION_SERVICE);
            this._locationService = new LocationService(locationConfiguration);
            this._ajaxLoadingPanel = $find(this.ajaxLoadingPanelId);
            this._ajaxLoadingPanel.show(this.locationViewPaneId);
            this._rtvLocations = $find(this.rtvLocationId);
            this._rtvLocations.add_nodeClicked(this.rtvLocations_nodeClicked);
            this._uscLocationDetails = $("#" + this.uscLocationDetailsId).data();
            this._filterToolbar = $find(this.filterToolbarId);
            this._filterToolbar.add_buttonClicked(this.filterToolbar_onClick);
            this._actionToolBar = $find(this.actionToolBarId);
            this._actionToolBar.add_buttonClicked(this.actionToolBar_onClick);
            this._actionToolBar.findItemByValue(TbltLocation.TOOLBAR_CREATE).enable();
            this._actionToolBar.findItemByValue(TbltLocation.TOOLBAR_MODIFY).disable();
            this._rwInsert = $find(this.rwInsertId);
            this._rtbLocationName = $find(this.rtbLocationNameId);
            this._rtbArchivioProtocollo = $find(this.rtbArchivioProtocolloId);
            this._rtbArchivioDossier = $find(this.rtbArchivioDossierId);
            this._rtbArchivioAtti = $find(this.rtbArchivioAttiId);
            this._rbConfirm = $find(this.rbConfirmId);
            this._rbConfirm.add_clicked(this.rbConfirmInsert_onCLick);
            this.loadTreeView();
        };
        ;
        TbltLocation.prototype.loadTreeViewFiltered = function (name, archive) {
            var _this = this;
            this._ajaxLoadingPanel.show(this.locationViewPaneId);
            this._locationService.filterLocationByNameAndArchive(name, archive, function (data) {
                _this.locationModel = data;
                for (var _i = 0, _a = _this.locationModel; _i < _a.length; _i++) {
                    var location_1 = _a[_i];
                    var node = new Telerik.Web.UI.RadTreeNode();
                    node.set_value(location_1.UniqueId);
                    node.set_text(location_1.Name);
                    node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/location.png");
                    _this._rtvLocations.get_nodes().getNode(0).get_nodes().add(node);
                    _this._rtvLocations.commitChanges();
                }
                _this._rtvLocations.get_nodes().getNode(0).expand();
                _this._ajaxLoadingPanel.hide(_this.locationViewPaneId);
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this.locationViewPaneId);
                _this.showNotificationException(error);
            });
        };
        TbltLocation.prototype.loadTreeView = function () {
            var _this = this;
            this._locationService.getLocations(function (data) {
                _this.locationModel = data;
                for (var _i = 0, _a = _this.locationModel; _i < _a.length; _i++) {
                    var location_2 = _a[_i];
                    var node = new Telerik.Web.UI.RadTreeNode();
                    node.set_value(location_2.UniqueId);
                    node.set_text(location_2.Name);
                    node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/location.png");
                    _this._rtvLocations.get_nodes().getNode(0).get_nodes().add(node);
                    _this._rtvLocations.commitChanges();
                }
                _this._rtvLocations.get_nodes().getNode(0).expand();
                _this._ajaxLoadingPanel.hide(_this.locationViewPaneId);
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this.locationViewPaneId);
                _this.showNotificationException(error);
            });
        };
        TbltLocation.prototype.clearInputs = function () {
            this._rtbLocationName.clear();
            this._rtbArchivioProtocollo.clear();
            this._rtbArchivioDossier.clear();
            this._rtbArchivioAtti.clear();
        };
        TbltLocation.prototype.populateLocationInputs = function (locationId) {
            var location = {};
            for (var _i = 0, _a = this.locationModel; _i < _a.length; _i++) {
                var locationToFind = _a[_i];
                if (locationToFind.UniqueId === locationId) {
                    location = locationToFind;
                    break;
                }
            }
            this._rtbLocationName.set_value(location.Name);
            this._rtbArchivioProtocollo.set_value(location.ProtocolArchive);
            this._rtbArchivioDossier.set_value(location.DossierArchive);
            this._rtbArchivioAtti.set_value(location.ResolutionArchive);
        };
        TbltLocation.prototype.insertOrUpdateLocation = function () {
            var exists = this.selectedLocationId !== "";
            if (this._rtbLocationName.get_value() == "") {
                alert("Il campo nome e obbligatorio");
                return;
            }
            var location = {};
            location.Name = this._rtbLocationName.get_value();
            location.ProtocolArchive = this._rtbArchivioProtocollo.get_value();
            location.DossierArchive = this._rtbArchivioDossier.get_value();
            location.ResolutionArchive = this._rtbArchivioAtti.get_value();
            if (exists) {
                this.updateLocation(location);
            }
            else {
                this.insertLocation(location);
            }
        };
        TbltLocation.prototype.updateLocation = function (location) {
            var _this = this;
            this._ajaxLoadingPanel.show(this.rtvLocationId);
            location.UniqueId = this.selectedLocationId;
            this._locationService.update(location, function (data) {
                var node = new Telerik.Web.UI.RadTreeNode();
                node.set_value(data.UniqueId);
                node.set_text(data.Name);
                node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/location.png");
                _this._rtvLocations.get_nodes().getNode(0).get_nodes().add(node);
                _this._rtvLocations.commitChanges();
                var locationModelToUpdate = _this.locationModel.filter(function (x) { return x.UniqueId === data.UniqueId; })[0];
                var locationModelToUpdateIndex = _this.locationModel.indexOf(locationModelToUpdate);
                _this.locationModel[locationModelToUpdateIndex] = data;
                _this._rwInsert.close();
                _this._rtvLocations.get_selectedNode().set_text(data.Name);
                _this._uscLocationDetails.clearLocationDetails();
                _this._uscLocationDetails.setLocationDetails();
                _this._ajaxLoadingPanel.hide(_this.rtvLocationId);
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this.rtvLocationId);
                _this.showNotificationException(error);
            });
        };
        TbltLocation.prototype.insertLocation = function (location) {
            var _this = this;
            this._ajaxLoadingPanel.show(this.rtvLocationId);
            this._locationService.insert(location, function (data) {
                var node = new Telerik.Web.UI.RadTreeNode();
                node.set_value(data.UniqueId);
                node.set_text(data.Name);
                node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/location.png");
                _this._rtvLocations.get_nodes().getNode(0).get_nodes().add(node);
                _this._rtvLocations.commitChanges();
                if (!_this.locationModel) {
                    _this.locationModel = [];
                }
                _this.locationModel.push(data);
                _this._rwInsert.close();
                _this._ajaxLoadingPanel.hide(_this.rtvLocationId);
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this.rtvLocationId);
                _this.showNotificationException(error);
            });
        };
        TbltLocation.prototype.showNotificationException = function (exception, customMessage) {
            if (exception && exception instanceof ExceptionDTO) {
                var uscNotification = $("#" + this.uscNotificationId).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
            }
            else {
                this.showNotificationMessage(customMessage);
            }
        };
        TbltLocation.prototype.showNotificationMessage = function (customMessage) {
            var uscNotification = $("#" + this.uscNotificationId).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        TbltLocation.TOOLBAR_CREATE = "create";
        TbltLocation.TOOLBAR_MODIFY = "modify";
        TbltLocation.TOOLBAR_SEARCH = "search";
        TbltLocation.TOOLBAR_SEARCH_INPUTS = "searchInput";
        TbltLocation.LOCATION_SERVICE = "Location";
        return TbltLocation;
    }());
    return TbltLocation;
});
//# sourceMappingURL=TbltLocation.js.map