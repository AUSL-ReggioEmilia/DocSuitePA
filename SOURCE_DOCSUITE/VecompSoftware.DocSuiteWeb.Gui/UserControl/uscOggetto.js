/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../app/helpers/stringhelper.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />
define(["require", "exports", "../app/core/extensions/string"], function (require, exports) {
    var uscOggetto = /** @class */ (function () {
        function uscOggetto() {
            var _this = this;
            this.enableVaidators = function (state) {
                ValidatorEnable(document.getElementById(_this.validationId), state);
            };
            this.getText = function () {
                var txtOggetto = $find(_this.txtObjectId);
                return txtOggetto.get_textBoxValue();
            };
            this.isValid = function () {
                var txtOggetto = $find(_this.txtObjectId);
                if (txtOggetto.get_maxLength() != 0 && txtOggetto.get_textBoxValue().length > txtOggetto.get_maxLength()) {
                    return false;
                }
                return true;
            };
            this.getMaxLength = function () {
                var txtOggetto = $find(_this.txtObjectId);
                return txtOggetto.get_maxLength();
            };
        }
        /**
         *------------------------- Events -----------------------------
         */
        /**
         *------------------------- Methods -----------------------------
         */
        /**
        * Metodo di inizializzazione
        */
        uscOggetto.prototype.initialize = function () {
            this.bindLoaded();
        };
        /**
        * Scateno l'evento di "Load Completed" del controllo
        */
        uscOggetto.prototype.bindLoaded = function () {
            $("#".concat(this.contentId)).data(this);
            $("#".concat(this.contentId)).triggerHandler(uscOggetto.LOADED_EVENT);
        };
        uscOggetto.LOADED_EVENT = "onLoaded";
        return uscOggetto;
    }());
    return uscOggetto;
});
//# sourceMappingURL=uscOggetto.js.map