/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

class TbltCategorySchemaGes {

    constructor() {
    }

    initialize() {
    }

    /**
     *------------------------- Methods -----------------------------
     */

    /**
     * restituisce un riferimento alla radwindow
     */
    getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd = null;
        if ((<any>window).radWindow) wnd = (<any>window).radWindow;
        else if ((<any>window).frameElement.radWindow) wnd = (<any>window).frameElement.radWindow;
        return wnd;
    }

    /**
     * Chiude la radwindow di riferimento
     * @param operator
     */
    closeWindow(operator): void {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(operator);
    }
}

export = TbltCategorySchemaGes;