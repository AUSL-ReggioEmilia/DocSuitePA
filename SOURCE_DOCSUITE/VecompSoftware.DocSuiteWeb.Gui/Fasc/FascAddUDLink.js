/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
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
define(["require", "exports", "Fasc/FascBase", "App/Helpers/ServiceConfigurationHelper", "UserControl/uscFascicleLink"], function (require, exports, FascicleBase, ServiceConfigurationHelper, UscFascicleLink) {
    var FascAddUDLink = /** @class */ (function (_super) {
        __extends(FascAddUDLink, _super);
        /**
    * Costruttore
    * @param serviceConfiguration
    */
        function FascAddUDLink(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME)) || this;
            /**
            * Evento scatenato al click del pulsante ConfermaInserimento
            * @method
            * @param sender
            * @param eventArgs
            * @returns
            */
            _this.btnConferma_ButtonClicked = function (sender, eventArgs) {
                _this._loadingPanel.show(_this.currentPageId);
                _this._btnConferma.set_enabled(false);
                var uscFascLink = $("#".concat(_this.uscFascLinkId)).data();
                if (!jQuery.isEmptyObject(uscFascLink)) {
                    if (!uscFascLink.currentFascicleId) {
                        _this.showNotificationMessage(_this.uscNotificationId, "Nessun Fascicolo selezionato");
                        _this._loadingPanel.hide(_this.currentPageId);
                        _this._btnConferma.set_enabled(true);
                        return;
                    }
                    _this.closeWindow(uscFascLink.currentFascicleId);
                }
            };
            _this._serviceConfigurations = serviceConfigurations;
            return _this;
        }
        /**
        *------------------------- Events -----------------------------
        */
        /**
        * Inizializzazione
        */
        FascAddUDLink.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._btnConferma = $find(this.btnConfermaId);
            this._btnConferma.add_clicked(this.btnConferma_ButtonClicked);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._manager = $find(this.managerId);
            $("#".concat(this.uscFascLinkId)).bind(UscFascicleLink.LOADED_EVENT, function (args) {
            });
        };
        /**
        *------------------------- Methods -----------------------------
        */
        /**
    * Chiude la RadWindow
    */
        /**
    * Chiude la RadWindow
    */
        FascAddUDLink.prototype.closeWindow = function (message) {
            var wnd = this.getRadWindow();
            wnd.close(message);
        };
        /**
        * Recupera una RadWindow dalla pagina
        */
        FascAddUDLink.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        return FascAddUDLink;
    }(FascicleBase));
    return FascAddUDLink;
});
//# sourceMappingURL=FascAddUDLink.js.map