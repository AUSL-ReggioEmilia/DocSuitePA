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
define(["require", "exports", "Dossiers/DossierBase", "App/Helpers/ServiceConfigurationHelper", "UserControl/uscDossierGrid"], function (require, exports, DossierBase, ServiceConfigurationHelper, UscDossierGrid) {
    var DossierRisultati = /** @class */ (function (_super) {
        __extends(DossierRisultati, _super);
        /**
        * Costruttore
        */
        function DossierRisultati(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIER_TYPE_NAME)) || this;
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        DossierRisultati.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._loadingPanel.show(this.uscDossierGridId);
            $("#".concat(this.uscDossierGridId)).bind(UscDossierGrid.LOADED_EVENT, function () {
                _this.loadDossierGrid();
            });
            this.loadDossierGrid();
            $("#".concat(this.uscDossierGridId)).bind(UscDossierGrid.PAGE_CHANGED_EVENT, function (args) {
                var uscDossierGrid = $("#".concat(_this.uscDossierGridId)).data();
                if (!jQuery.isEmptyObject(uscDossierGrid)) {
                    _this.pageChange(uscDossierGrid);
                }
            });
        };
        DossierRisultati.prototype.loadDossierGrid = function () {
            var uscDossierGrid = $("#".concat(this.uscDossierGridId)).data();
            if (!jQuery.isEmptyObject(uscDossierGrid)) {
                this.loadResults(uscDossierGrid, 0);
            }
        };
        DossierRisultati.prototype.pageChange = function (uscDossierGrid) {
            this._loadingPanel.show(this.uscDossierGridId);
            var skip = uscDossierGrid.getGridCurrentPageIndex() * uscDossierGrid.getGridPageSize();
            this.loadResults(uscDossierGrid, skip);
        };
        DossierRisultati.prototype.loadResults = function (uscDossierGrid, skip) {
            var _this = this;
            var top = skip + uscDossierGrid.getGridPageSize();
            var filter = sessionStorage.getItem("DossierSearch");
            var dossierSearchFilter;
            if (filter) {
                dossierSearchFilter = JSON.parse(filter);
            }
            this.service.getDossiers(skip, top, dossierSearchFilter, function (data) {
                if (!data)
                    return;
                uscDossierGrid.setDataSource(data);
                _this.service.countDossiers(dossierSearchFilter, function (data) {
                    if (data == undefined)
                        return;
                    uscDossierGrid.setItemCount(data);
                    _this._loadingPanel.hide(_this.uscDossierGridId);
                }, function (exception) {
                    _this._loadingPanel.hide(_this.uscDossierGridId);
                    $("#".concat(_this.uscDossierGridId)).hide();
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            }, function (exception) {
                _this._loadingPanel.hide(_this.uscDossierGridId);
                $("#".concat(_this.uscDossierGridId)).hide();
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        return DossierRisultati;
    }(DossierBase));
    return DossierRisultati;
});
//# sourceMappingURL=DossierRisultati.js.map