/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Services/Commons/ContainerService", "App/Models/Commons/CategoryFascicleRightModel", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO", "App/Services/Commons/CategoryFascicleService", "App/Services/Commons/CategoryFascicleRightsService"], function (require, exports, ContainerService, CategoryFascicleRightModel, ServiceConfigurationHelper, ExceptionDTO, CategoryFascicleService, CategoryFascicleRightsService) {
    var TbltContainerGes = /** @class */ (function () {
        /**
       * Costruttore
       * @param serviceConfigurations
       */
        function TbltContainerGes(serviceConfigurations) {
            var _this = this;
            /**
            *------------------------- Events -----------------------------
            */
            this.btnConfirm_OnClick = function (sender, eventArgs) {
                if (!_this._grdUDSRepositories.get_selectedItems() || _this._grdUDSRepositories.get_selectedItems().length < 1) {
                    _this.showNotificationException(_this.uscNotificationId, null, "Selezionare almeno un contenitore");
                    return;
                }
                _this._btnConfirm.set_enabled(false);
                _this._loadingPanel.show(_this.pageContentId);
                _this.addContainersToCategory()
                    .done(function () {
                    var ajaxModel = {};
                    ajaxModel.ActionName = 'AddContainers';
                    _this.closeWindow(ajaxModel);
                }).fail(function (exception) {
                    var ajaxModel = {};
                    ajaxModel.ActionName = 'AddContainers';
                    _this.closeWindow(ajaxModel);
                });
            };
            this.btnSearch_OnClick = function (sender, eventArgs) {
                _this.loadContainers();
            };
            this.loadContainers = function () {
                var name = _this._txtName.get_value();
                _this._containerService.getContainersByNameFascicleRights(name, null, function (data) {
                    if (!data)
                        return;
                    if (data) {
                        _this.fillTable(data);
                    }
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
            var containerConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Container");
            this._containerService = new ContainerService(containerConfiguration);
            var categoryFascicleService = ServiceConfigurationHelper.getService(serviceConfigurations, "CategoryFascicle");
            this._categoryFascicleService = new CategoryFascicleService(categoryFascicleService);
            var categoryFascicleRightsService = ServiceConfigurationHelper.getService(serviceConfigurations, "CategoryFascicleRight");
            this._categoryFascicleRightsService = new CategoryFascicleRightsService(categoryFascicleRightsService);
        }
        /**
       * Inizializzazione classe
       */
        TbltContainerGes.prototype.initialize = function () {
            this._btnConfirm = $find(this.btnConfirmId);
            this._btnConfirm.add_clicked(this.btnConfirm_OnClick);
            this._btnSearch = $find(this.btnSearchId);
            this._btnSearch.add_clicked(this.btnSearch_OnClick);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._loadingPanel.show(this.pageContentId);
            this._grdUDSRepositories = $find(this.grdUDSRepositoriesId);
            this._txtName = $find(this.txtNameId);
            this.loadContainers();
        };
        TbltContainerGes.prototype.addContainersToCategory = function () {
            var _this = this;
            var promise = $.Deferred();
            var udsRepositories = this._grdUDSRepositories.get_selectedItems();
            var categoryFascicleRightModel;
            this._categoryFascicleRightsService.deleteCategoryFascicleRight;
            this._categoryFascicleService.getAvailableProcedureCategoryFascicleByCategory(+this.idCategory, function (data) {
                if (data) {
                    var categoryFascicle_1 = data[0];
                    $.when(udsRepositories.forEach(function (item) {
                        if (item) {
                            categoryFascicleRightModel = new CategoryFascicleRightModel();
                            categoryFascicleRightModel.Container = item._dataItem;
                            categoryFascicleRightModel.CategoryFascicle = categoryFascicle_1;
                            var testo = void 0;
                            _this._categoryFascicleRightsService.insertCategoryFascicleRight(categoryFascicleRightModel, function (data) {
                                promise.resolve();
                            });
                        }
                    }));
                }
            }, function (exception) {
                var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
                promise.reject(exception);
            });
            return promise.promise();
        };
        /**
         *------------------------- Methods -----------------------------
         */
        TbltContainerGes.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
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
        TbltContainerGes.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        TbltContainerGes.prototype.closeWindow = function (message) {
            var wnd = this.getRadWindow();
            wnd.close(message);
        };
        TbltContainerGes.prototype.addContainers = function (containers, rdlContainer) {
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
        return TbltContainerGes;
    }());
    return TbltContainerGes;
});
//# sourceMappingURL=TbltContainerGes.js.map