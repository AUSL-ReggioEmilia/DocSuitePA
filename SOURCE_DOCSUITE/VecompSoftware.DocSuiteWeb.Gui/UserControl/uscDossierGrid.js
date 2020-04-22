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
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "Dossiers/DossierBase"], function (require, exports, ServiceConfigurationHelper, DossierBase) {
    var uscDossierGrid = /** @class */ (function (_super) {
        __extends(uscDossierGrid, _super);
        /**
      * Costruttore
      * @param webApiConfiguration
      */
        function uscDossierGrid(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIER_TYPE_NAME)) || this;
            $(document).ready(function () {
            });
            return _this;
        }
        /**
        * Inizializzazione
        */
        uscDossierGrid.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            this._dossierGrid = $find(this.dossierGridId);
            this._masterTableView = this._dossierGrid.get_masterTableView();
            this._masterTableView.set_currentPageIndex(0);
            this.bindLoaded();
        };
        /**
        *------------------------- Events -----------------------------
        */
        /**
         * Evento scatenato al click del pulsante di inserimento
         * @param sender
         * @param args
         */
        uscDossierGrid.prototype.onPageChanged = function () {
            var skip = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_pageSize();
            $("#".concat(this.pageId)).triggerHandler(uscDossierGrid.PAGE_CHANGED_EVENT);
        };
        uscDossierGrid.prototype.onGridDataBound = function () {
            var row = this._masterTableView.get_dataItems();
            for (var i = 0; i < row.length; i++) {
                if (i % 2) {
                    row[i].addCssClass("Chiaro");
                }
                else {
                    row[i].addCssClass("Scuro");
                }
            }
        };
        /**
        *------------------------- Methods -----------------------------
        */
        /**
         * Inizializza lo user control del sommario di fascicolo
         */
        uscDossierGrid.prototype.bindLoaded = function () {
            $("#".concat(this.pageId)).data(this);
            $("#".concat(this.pageId)).triggerHandler(uscDossierGrid.LOADED_EVENT);
        };
        uscDossierGrid.prototype.setDataSource = function (results) {
            this._masterTableView.set_dataSource(results);
            this._masterTableView.dataBind();
        };
        uscDossierGrid.prototype.setItemCount = function (count) {
            this._masterTableView.set_virtualItemCount(count);
            this._masterTableView.dataBind();
        };
        uscDossierGrid.prototype.getGridPageSize = function () {
            return this._masterTableView.get_pageSize();
        };
        uscDossierGrid.prototype.getGridCurrentPageIndex = function () {
            return this._masterTableView.get_currentPageIndex();
        };
        uscDossierGrid.LOADED_EVENT = "onLoaded";
        uscDossierGrid.PAGE_CHANGED_EVENT = "onPageChanged";
        return uscDossierGrid;
    }(DossierBase));
    return uscDossierGrid;
});
//# sourceMappingURL=uscDossierGrid.js.map