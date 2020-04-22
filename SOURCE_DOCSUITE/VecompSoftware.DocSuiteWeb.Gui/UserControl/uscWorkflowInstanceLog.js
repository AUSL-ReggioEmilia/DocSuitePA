define(["require", "exports"], function (require, exports) {
    var uscWorkflowInstanceLog = /** @class */ (function () {
        function uscWorkflowInstanceLog(serviceConfigurations) {
            this._serviceConfigurations = serviceConfigurations;
        }
        /**
         * evento scatenato al cambio di pagina
         */
        uscWorkflowInstanceLog.prototype.onPageChanged = function () {
            var skip = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_pageSize();
            $("#".concat(this.pageContentId)).triggerHandler(uscWorkflowInstanceLog.ON_PAGE_CHANGE, skip);
        };
        /**
         * evento scatenato al caricamento di dati nella griglia
         */
        uscWorkflowInstanceLog.prototype.onGridDataBound = function () {
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
         * Inizializzazione
         */
        uscWorkflowInstanceLog.prototype.initialize = function () {
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._workflowInstanceLogGrid = $find(this.workflowInstanceLogGridId);
            this._masterTableView = this._workflowInstanceLogGrid.get_masterTableView();
            this._masterTableView.set_currentPageIndex(0);
            this._masterTableView.set_virtualItemCount(0);
            this.bindLoaded();
        };
        /**
         * Carico i dati nella griglia
         * @param responseModel
         */
        uscWorkflowInstanceLog.prototype.setGrid = function (responseModel) {
            this._masterTableView.set_dataSource(responseModel.value);
            this._masterTableView.set_virtualItemCount(responseModel.count);
            this._masterTableView.dataBind();
            this.bindLoaded();
        };
        /**
        * Scateno l'evento di "Load Completed" del controllo
        */
        uscWorkflowInstanceLog.prototype.bindLoaded = function () {
            $("#".concat(this.pageContentId)).data(this);
        };
        uscWorkflowInstanceLog.ON_PAGE_CHANGE = "onPageChange";
        return uscWorkflowInstanceLog;
    }());
    return uscWorkflowInstanceLog;
});
//# sourceMappingURL=uscWorkflowInstanceLog.js.map