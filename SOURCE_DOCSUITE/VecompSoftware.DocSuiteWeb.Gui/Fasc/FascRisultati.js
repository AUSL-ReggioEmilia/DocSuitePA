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
define(["require", "exports", "Fasc/FascBase", "App/Helpers/ServiceConfigurationHelper"], function (require, exports, FascicleBase, ServiceConfigurationHelper) {
    var FascRisultati = /** @class */ (function (_super) {
        __extends(FascRisultati, _super);
        /**
        * Costruttore
        */
        function FascRisultati(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME)) || this;
            /**
             * Evento scatenato al click del pulsante di Visualizza documenti
             * @param sender
             * @param args
             */
            _this.btnDocuments_OnClick = function (sender, args) {
                args.set_cancel(true);
                var selection = new Array();
                $.each(_this.getSelectedItemIDs(), function (index, id) {
                    selection.push(id);
                });
                if (selection.length > 0) {
                    if (selection.length > _this.selectableFasciclesThreshold) {
                        _this.showNotificationMessage(_this.uscNotificationId, "Non si possono selezionare più di ".concat(_this.selectableFasciclesThreshold.toString(), " fascicoli"));
                        return false;
                    }
                    _this._loadingPanel.show(_this.gridId);
                    var fascicleIds = encodeURIComponent(JSON.stringify(selection));
                    var editUrl = "../Viewers/FascicleViewer.aspx?Type=Fasc&FascicleIds=".concat(fascicleIds);
                    window.location.href = editUrl;
                }
                else {
                    _this.showWarningMessage(_this.uscNotificationId, "Nessun fascicolo selezionato");
                }
            };
            /**
            * Evento scatenato al click del pulsante di Seleziona tutti
            * @param sender
            * @param args
            */
            _this.btnSelectAll_OnClick = function (sender, args) {
                args.set_cancel(true);
                var count = 0;
                $.each(_this._grid.get_masterTableView().get_dataItems(), function (index, item) {
                    if (count >= _this.selectableFasciclesThreshold) {
                        _this.showWarningMessage(_this.uscNotificationId, "Non si possono selezionare più di ".concat(_this.selectableFasciclesThreshold.toString(), " fascicoli"));
                        return false;
                    }
                    var element = item.findElement("cbSelect");
                    if (!element.disabled) {
                        element.checked = true;
                        count++;
                    }
                });
            };
            /**
            * Evento scatenato al click del pulsante di Seleziona tutti
            * @param sender
            * @param args
            */
            _this.btnDeselectAll_OnClick = function (sender, args) {
                args.set_cancel(true);
                $.each(_this.getSelectedItems(), function (index, item) {
                    var element = item.findElement("cbSelect");
                    element.checked = false;
                });
            };
            return _this;
        }
        /**
         * Initialize
         */
        FascRisultati.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            this._backBtn = $find(this.backBtnId);
            if (this._backBtn) {
                this._backBtn.add_clicked(this.navigateBack);
            }
            this._btnDocuments = $find(this.btnDocumentsId);
            this._btnSelectAll = $find(this.btnSelectAllId);
            this._btnDeselectAll = $find(this.btnDeselectAllId);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._grid = $find(this.gridId);
            if (this._btnDocuments) {
                this._btnDocuments.add_clicking(this.btnDocuments_OnClick);
            }
            if (this._btnSelectAll) {
                this._btnSelectAll.add_clicking(this.btnSelectAll_OnClick);
            }
            if (this._btnDeselectAll) {
                this._btnDeselectAll.add_clicking(this.btnDeselectAll_OnClick);
            }
        };
        /**
        *------------------------- Events -----------------------------
        */
        FascRisultati.prototype.navigateBack = function () {
            window.history.back();
        };
        /**
         *------------------------- Methods -----------------------------
         */
        FascRisultati.prototype.getSelectedItems = function () {
            var selectedItems = new Array();
            var gridItems = this._grid.get_masterTableView().get_dataItems();
            $.each(gridItems, function (index, item) {
                var element = item.findElement("cbSelect");
                if (element.checked) {
                    selectedItems.push(item);
                }
            });
            return selectedItems;
        };
        FascRisultati.prototype.getSelectedItemIDs = function () {
            var ids = new Array();
            var selectedItems = this.getSelectedItems();
            $.each(selectedItems, function (index, item) {
                ids.push(item.getDataKeyValue("IdFascicle"));
            });
            return ids;
        };
        /**
     * Metodo per il recupero di una specifica radwindow
     */
        FascRisultati.prototype.getRadWindow = function () {
            var radWindow;
            if (window.radWindow)
                radWindow = window.radWindow;
            else if (window.frameElement.radWindow)
                radWindow = window.frameElement.radWindow;
            return radWindow;
        };
        /**
         * Metodo di chiusura di una radwindow
         * @param callback
         */
        FascRisultati.prototype.closeWindow = function (callback) {
            var radWindow = this.getRadWindow();
            if (radWindow != null)
                radWindow.close(callback);
        };
        FascRisultati.prototype.loadFolders = function (selectedFascicleFolderId, currentFascicleId, destinationFascicleId) {
            var url = "../Fasc/FascMoveItems.aspx?Type=Fasc&idFascicle=" + currentFascicleId + "&ItemsType=DocumentType&IdFascicleFolder=" + selectedFascicleFolderId + "&DestinationFascicleId=" + destinationFascicleId + "&MoveToFascicle=" + true;
            location.href = url;
        };
        return FascRisultati;
    }(FascicleBase));
    return FascRisultati;
});
//# sourceMappingURL=FascRisultati.js.map