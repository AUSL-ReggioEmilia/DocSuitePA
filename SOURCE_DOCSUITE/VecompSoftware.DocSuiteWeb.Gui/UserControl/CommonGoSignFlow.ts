/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />

class CommonGoSignFlow {

// #region [fields]
    private readonly _sessionId: string;
    private readonly _proxySignUrl: string;
    private readonly _frameId: string = "goSignFrame";

    btnSignCompletedId: string;
    btnCancelId: string;
    windowManagerId: string;
// #endregion

// #region [properties]
    private get btnSignCompleted(): Telerik.Web.UI.RadButton {
        return $find(this.btnSignCompletedId) as Telerik.Web.UI.RadButton;
    }

    private get btnCancel(): Telerik.Web.UI.RadButton {
        return $find(this.btnCancelId) as Telerik.Web.UI.RadButton;
    }

    private get windowManager(): Telerik.Web.UI.RadWindowManager {
        return $find(this.windowManagerId) as Telerik.Web.UI.RadWindowManager;
    }
// #endregion

// #region [constructor]
    constructor(sessionId: string, proxySignUrl: string) {
        this._sessionId = sessionId;
        this._proxySignUrl = proxySignUrl;
    }
// #endregion

// #region [events]
    btnCancel_onClicking = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs): void => {
        args.set_cancel(true);
        this.closeWindow(false);
    }

    btnSignCompleted_onClicking = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs): void => {
        args.set_cancel(true);
        this.closeWindow(true);
    }
// #endregion

// #region [methods]
    initialize(): void {
        if (!this._sessionId) {
            this.windowManager.radalert(`L'identificativo della sessione di firma non è stato definito.`);
            this.btnSignCompleted.set_enabled(false);
            this.btnCancel.set_enabled(false);
            return;
        }

        this.initializeButtonsHandler();
        this.createProxySignFrame();
    }

    private initializeButtonsHandler(): void {
        this.btnCancel.add_clicking(this.btnCancel_onClicking);
        this.btnSignCompleted.add_clicking(this.btnSignCompleted_onClicking);
    }

    private createProxySignFrame(): void {
        const path = `${this._proxySignUrl}/local/get-html/${this._sessionId}/IT`;
        const iframe = document.getElementById(this._frameId) as HTMLFrameElement;
        iframe.src = path;
    }

    private closeWindow(isCompleted: boolean): void {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(isCompleted);
    }

    private getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return wnd;
    }
// #endregion
}

export = CommonGoSignFlow;