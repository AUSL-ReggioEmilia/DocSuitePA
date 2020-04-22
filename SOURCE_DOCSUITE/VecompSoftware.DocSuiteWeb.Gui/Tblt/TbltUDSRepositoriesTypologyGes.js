/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Services/UDS/UDSTypologyService", "App/Services/UDS/UDSRepositoryService", "App/Services/Commons/ContainerService", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO", "App/Models/Commons/LocationTypeEnum"], function (require, exports, UDSTypologyService, UDSRepositoryService, ContainerService, ServiceConfigurationHelper, ExceptionDTO, LocationTypeEnum) {
    var TbltUDSRepositoriesTypologyGes = /** @class */ (function () {
        /**
       * Costruttore
       * @param serviceConfigurations
       */
        function TbltUDSRepositoriesTypologyGes(serviceConfigurations) {
            var _this = this;
            /**
            *------------------------- Events -----------------------------
            */
            this.btnConfirm_OnClick = function (sender, eventArgs) {
                if (!_this._grdUDSRepositories.get_selectedItems() || _this._grdUDSRepositories.get_selectedItems().length < 1) {
                    _this.showNotificationException(_this.uscNotificationId, null, "Selezionare almeno un archivio");
                    return;
                }
                _this._btnConfirm.set_enabled(false);
                _this._loadingPanel.show(_this.pageContentId);
                var udsRepositories = _this._grdUDSRepositories.get_selectedItems();
                var typology = JSON.parse(sessionStorage[_this.currentUDSTypologyId]);
                udsRepositories.forEach(function (item) { if (item)
                    typology.UDSRepositories.push(item._dataItem); });
                _this._udsTypologyService.updateUDSTypology(typology, function (data) {
                    if (data) {
                        var ajaxModel = {};
                        ajaxModel.ActionName = 'AddUDSRepositories';
                        ajaxModel.Value = [JSON.stringify(data)];
                        _this.closeWindow(ajaxModel);
                    }
                }, function (exception) {
                    _this._loadingPanel.hide(_this.pageContentId);
                    _this._btnConfirm.set_enabled(true);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            };
            this.btnSearch_OnClick = function (sender, eventArgs) {
                _this.loadAvailableUDSRepositories();
            };
            this.loadAvailableUDSRepositories = function () {
                var name = _this._txtName.get_value();
                var alias = _this._txtAlias.get_value();
                var idContainer = "";
                if (_this._rdlContainer.get_selectedItem()) {
                    idContainer = _this._rdlContainer.get_selectedItem().get_value();
                }
                _this._loadingPanel.show(_this.pageContentId);
                _this._udsRepositoryService.getAvailableCQRSPublishedUDSRepositories(_this.currentUDSTypologyId, name, alias, idContainer, function (data) {
                    if (data) {
                        _this.fillTable(data);
                    }
                }, function (exception) {
                    _this._loadingPanel.hide(_this.pageContentId);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            };
            this.loadContainers = function () {
                _this._containerService.getContainers(LocationTypeEnum.UDSLocation, function (data) {
                    if (!data)
                        return;
                    var containers = data;
                    _this.addContainers(containers, _this._rdlContainer);
                    _this._loadingPanel.hide(_this.pageContentId);
                    _this._btnSearch.set_enabled(true);
                }, function (exception) {
                    _this._loadingPanel.hide(_this.pageContentId);
                    _this._btnSearch.set_enabled(false);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            };
            this.fillTable = function (udsRepositories) {
                _this._grdUDSRepositories = $find(_this.grdUDSRepositoriesId);
                var grdUDSRepositoriesMasterTableView = _this._grdUDSRepositories.get_masterTableView();
                grdUDSRepositoriesMasterTableView.set_dataSource(udsRepositories);
                grdUDSRepositoriesMasterTableView.clearSelectedItems();
                grdUDSRepositoriesMasterTableView.dataBind();
                _this._loadingPanel.hide(_this.pageContentId);
            };
            var serviceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "UDSTypology");
            this._udsTypologyService = new UDSTypologyService(serviceConfiguration);
            var udsRepositoryConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, TbltUDSRepositoriesTypologyGes.UDSREPOSITORY_TYPE_NAME);
            this._udsRepositoryService = new UDSRepositoryService(udsRepositoryConfiguration);
            var containerConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Container");
            this._containerService = new ContainerService(containerConfiguration);
        }
        /**
       * Inizializzazione classe
       */
        TbltUDSRepositoriesTypologyGes.prototype.initialize = function () {
            this._btnConfirm = $find(this.btnConfirmId);
            this._btnConfirm.add_clicked(this.btnConfirm_OnClick);
            this._rdlContainer = $find(this.rdlContainerId);
            this._btnSearch = $find(this.btnSearchId);
            this._btnSearch.add_clicked(this.btnSearch_OnClick);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._loadingPanel.show(this.pageContentId);
            this._grdUDSRepositories = $find(this.grdUDSRepositoriesId);
            this._txtName = $find(this.txtNameId);
            this._txtAlias = $find(this.txtAliasId);
            this.loadContainers();
            this.loadAvailableUDSRepositories();
        };
        /**
         *------------------------- Methods -----------------------------
         */
        TbltUDSRepositoriesTypologyGes.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                if (exception && exception instanceof ExceptionDTO) {
                    uscNotification.showNotification(exception);
                }
                else {
                    uscNotification.showNotificationMessage(customMessage);
                }
            }
        };
        TbltUDSRepositoriesTypologyGes.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        TbltUDSRepositoriesTypologyGes.prototype.closeWindow = function (message) {
            var wnd = this.getRadWindow();
            wnd.close(message);
        };
        TbltUDSRepositoriesTypologyGes.prototype.addContainers = function (containers, rdlContainer) {
            var item;
            for (var _i = 0, containers_1 = containers; _i < containers_1.length; _i++) {
                var container = containers_1[_i];
                item = new Telerik.Web.UI.DropDownListItem();
                item.set_text(container.Name);
                item.set_value(container.EntityShortId.toString());
                rdlContainer.get_items().add(item);
                if (!container.isActive) {
                    $(item.get_element()).attr('style', 'color: grey');
                }
            }
            rdlContainer.trackChanges();
        };
        TbltUDSRepositoriesTypologyGes.UDSREPOSITORY_TYPE_NAME = "UDSRepository";
        return TbltUDSRepositoriesTypologyGes;
    }());
    return TbltUDSRepositoriesTypologyGes;
});
//# sourceMappingURL=TbltUDSRepositoriesTypologyGes.js.map