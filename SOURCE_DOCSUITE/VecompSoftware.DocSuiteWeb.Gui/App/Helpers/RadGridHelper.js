/// <reference path="../../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports"], function (require, exports) {
    var RadGridHelper = /** @class */ (function () {
        function RadGridHelper() {
        }
        RadGridHelper.findControlFromClassName = function (tableView, uniqueName, className) {
            return $find($telerik.getElementByClassName(tableView._getFilterCellByColumnUniqueName(uniqueName), className, null).id);
        };
        return RadGridHelper;
    }());
    return RadGridHelper;
});
//# sourceMappingURL=RadGridHelper.js.map