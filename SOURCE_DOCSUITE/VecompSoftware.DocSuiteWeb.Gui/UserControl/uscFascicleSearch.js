/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
define(["require", "exports", "App/Services/Fascicles/FascicleService", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO"], function (require, exports, FascicleService, ServiceConfigurationHelper, ExceptionDTO) {
    var uscFascicleSearch = /** @class */ (function () {
        function uscFascicleSearch(serviceConfigurations) {
            var _this = this;
            /**
             *------------------------- Events -----------------------------
             */
            this.btnSearch_OnClick = function (sender, args) {
                var url = '../Fasc/FascRicerca.aspx?Type=Fasc&Action=SearchFascicles';
                _this.openWindow(url, "searchFascicles", 750, 600, _this.closeSearchFasciclesWindow);
            };
            this.closeSearchFasciclesWindow = function (sender, args) {
                if (args.get_argument()) {
                    try {
                        var fascicleId = args.get_argument();
                        sessionStorage.removeItem(_this._sessionStorageFascicleKey);
                        _this._flatLoadingPanel.show(_this.btnSearchId);
                        _this.loadFascicle(fascicleId)
                            .fail(function (exception) {
                            _this.showNotificationError(exception);
                        })
                            .always(function () { return _this._flatLoadingPanel.hide(_this.btnSearchId); });
                    }
                    catch (error) {
                        console.error(JSON.stringify(error));
                        _this.showNotificationError("Errore nella richiesta. Nessun fascicolo selezionato.");
                    }
                }
            };
            this._serviceConfigurations = serviceConfigurations;
        }
        Object.defineProperty(uscFascicleSearch.prototype, "summaryContentPanel", {
            get: function () {
                return $("#" + this.summaryContentId);
            },
            enumerable: true,
            configurable: true
        });
        /**
         *------------------------- Methods -----------------------------
         */
        uscFascicleSearch.prototype.initialize = function () {
            this._flatLoadingPanel = $find(this.ajaxFlatLoadingPanelId);
            this._btnSearch = $find(this.btnSearchId);
            this._btnSearch.add_clicked(this.btnSearch_OnClick);
            this._sessionStorageFascicleKey = this.pageId + "_selectedFascicle";
            sessionStorage.removeItem(this._sessionStorageFascicleKey);
            var fascicleServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Fascicle");
            this._fascicleService = new FascicleService(fascicleServiceConfiguration);
            this.bindLoaded();
        };
        uscFascicleSearch.prototype.bindLoaded = function () {
            $("#" + this.pageId).data(this);
            $("#" + this.pageId).triggerHandler(uscFascicleSearch.LOADED_EVENT);
        };
        uscFascicleSearch.prototype.clearSelections = function () {
            sessionStorage.removeItem(this._sessionStorageFascicleKey);
            this.summaryContentPanel.hide();
        };
        uscFascicleSearch.prototype.loadFascicle = function (fascicleId) {
            var _this = this;
            var promise = $.Deferred();
            if (!fascicleId) {
                return promise.reject("Nessun id fascicolo definito per la ricerca");
            }
            this._fascicleService.getFascicle(fascicleId, function (data) {
                if (!data) {
                    return promise.reject("Nessun fascicolo trovato con id " + fascicleId);
                }
                _this._uscFascicleSummary = $("#" + _this.uscFascicleSummaryId).data();
                if (jQuery.isEmptyObject(_this._uscFascicleSummary)) {
                    return promise.reject("E' avvenuto un errore durante il carimento delle informazioni del fascicolo selezionato. Si prega di riprovare.");
                }
                sessionStorage.setItem(_this._sessionStorageFascicleKey, JSON.stringify(data));
                $("#" + _this.pageId).triggerHandler(uscFascicleSearch.FASCICLE_SELECTED_EVENT, data);
                _this.summaryContentPanel.show();
                _this._uscFascicleSummary.loadData(data)
                    .done(function () { return promise.resolve(); })
                    .fail(function (exception) { return promise.reject(exception); });
            }, function (exception) {
                promise.reject(exception);
            });
            return promise.promise();
        };
        uscFascicleSearch.prototype.getSelectedFascicle = function () {
            if (sessionStorage[this._sessionStorageFascicleKey]) {
                return JSON.parse(sessionStorage[this._sessionStorageFascicleKey]);
            }
            return undefined;
        };
        uscFascicleSearch.prototype.setButtonSearchEnabled = function (value) {
            this._btnSearch.set_enabled(value);
        };
        uscFascicleSearch.prototype.openWindow = function (url, name, width, height, closeHandler) {
            var manager = $find(this.managerWindowsId);
            var wnd = manager.open(url, name, null);
            wnd.setSize(width, height);
            wnd.set_modal(true);
            wnd.add_close(closeHandler);
            wnd.center();
            return false;
        };
        uscFascicleSearch.prototype.showNotificationError = function (exception) {
            var uscNotification = $("#" + this.uscNotificationId).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                if (exception instanceof ExceptionDTO) {
                    uscNotification.showNotification(exception);
                }
                else {
                    uscNotification.showNotificationMessage(exception);
                }
            }
        };
        uscFascicleSearch.LOADED_EVENT = "onLoaded";
        uscFascicleSearch.FASCICLE_SELECTED_EVENT = "onFascicleSelected";
        return uscFascicleSearch;
    }());
    return uscFascicleSearch;
});
//# sourceMappingURL=uscFascicleSearch.js.map