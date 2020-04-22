/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports"], function (require, exports) {
    var TbltCategorySchemaGes = /** @class */ (function () {
        function TbltCategorySchemaGes() {
        }
        TbltCategorySchemaGes.prototype.initialize = function () {
        };
        /**
         *------------------------- Methods -----------------------------
         */
        /**
         * restituisce un riferimento alla radwindow
         */
        TbltCategorySchemaGes.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        /**
         * Chiude la radwindow di riferimento
         * @param operator
         */
        TbltCategorySchemaGes.prototype.closeWindow = function (operator) {
            var wnd = this.getRadWindow();
            wnd.close(operator);
        };
        return TbltCategorySchemaGes;
    }());
    return TbltCategorySchemaGes;
});
//# sourceMappingURL=TbltCategorySchemaGes.js.map