/// <reference path="../../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../../scripts/typings/telerik/microsoft.ajax.d.ts" />

class RadGridHelper {

    static findControlFromClassName(tableView, uniqueName, className: string) {
        return $find($telerik.getElementByClassName(tableView._getFilterCellByColumnUniqueName(uniqueName), className, null).id); 
    }
}

export = RadGridHelper;