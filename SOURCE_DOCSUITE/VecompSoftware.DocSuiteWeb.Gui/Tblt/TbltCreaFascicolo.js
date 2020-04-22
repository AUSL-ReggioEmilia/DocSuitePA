/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Services/Commons/CategoryFascicleService", "App/Models/Fascicles/FascicleType", "App/Services/Commons/CategoryFascicleRightsService"], function (require, exports, ServiceConfigurationHelper, CategoryFascicleService, FascicleType, CategoryFascicleRightsService) {
    var TbltCreaFascicolo = /** @class */ (function () {
        /**
        * Costruttore
        * @param webApiConfiguration
        */
        function TbltCreaFascicolo(serviceConfigurations) {
            var _this = this;
            this.btnSave_OnClick = function (sender, args) {
                _this._loadingPanel.show(_this.pageLayoutId);
                var item = {};
                item.FascicleType = FascicleType['Period'];
                item.DSWEnvironment = parseInt(_this._ddlUDSs.get_selectedItem().get_value());
                var fasciclePeriod = {};
                fasciclePeriod.UniqueId = _this._ddlPeriods.get_selectedItem().get_value();
                item.FasciclePeriod = fasciclePeriod;
                var category = {};
                category.EntityShortId = Number(_this.idCategory);
                item.Category = category;
                _this._categoryFascicleService.insertCategoryFascicle(item, function (data) {
                    var insertedCategoryFascicle = data;
                    _this._loadingPanel.hide(_this.pageLayoutId);
                    _this.closeWindow(JSON.stringify(insertedCategoryFascicle));
                }, function (exception) {
                    var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                    if (!jQuery.isEmptyObject(uscNotification)) {
                        uscNotification.showNotification(exception);
                    }
                });
            };
            this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
        }
        /**
         *------------------------- Events -----------------------------
         */
        /**
         * Inizializza la classe
         */
        TbltCreaFascicolo.prototype.initialize = function () {
            this._ajaxManager = $find(this.ajaxManagerId);
            this._btnSave = $find(this.btnSaveId);
            this._btnSave.add_clicking(this.btnSave_OnClick);
            this._ddlPeriods = $find(this.ddlPeriodsId);
            this._ddlUDSs = $find(this.ddlUDSId);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._rowPeriod = $("#".concat(this.rowPeriodId));
            this._rowPeriod.show();
            var categoryFascicleService = ServiceConfigurationHelper.getService(this._serviceConfigurations, "CategoryFascicle");
            this._categoryFascicleService = new CategoryFascicleService(categoryFascicleService);
            var categoryFascicleRightService = ServiceConfigurationHelper.getService(this._serviceConfigurations, "CategoryFascicleRight");
            this._categoryFascicleRightService = new CategoryFascicleRightsService(categoryFascicleRightService);
        };
        /**
         *------------------------- Methods -----------------------------
         */
        /**
         * Recupera una RadWindow dalla pagina
         */
        TbltCreaFascicolo.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        /**
         * Chiude la RadWindow
         */
        TbltCreaFascicolo.prototype.closeWindow = function (idCategory) {
            var wnd = this.getRadWindow();
            wnd.close(idCategory);
        };
        return TbltCreaFascicolo;
    }());
    return TbltCreaFascicolo;
});
//# sourceMappingURL=TbltCreaFascicolo.js.map