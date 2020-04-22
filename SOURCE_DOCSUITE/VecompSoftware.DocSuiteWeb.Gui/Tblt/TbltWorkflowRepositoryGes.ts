/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

class TbltWorkflowRepositoryGes {
    rowOldMappingTagId: string;
    rowOldAuthorizationTypeId: string;
    rowOldroleId: string;
    rowOldContactId: string;
    titleId: string;
    action: string;
    btnConfermaId: string;
    ajaxManagerId: string;
    ajaxLoadingPanelId: string;
    pageContentId: string;

    private _btnConferma: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;

    constructor() {
    }

    initialize() {
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._btnConferma = <Telerik.Web.UI.RadButton>$find(this.btnConfermaId);
        this._btnConferma.add_clicked(this.btnConferma_onClick);

        switch (this.action) {
            case 'Add':
                $('#'.concat(this.titleId)).html('Gestione attività - Aggiungi Tag');
                $('#'.concat(this.rowOldAuthorizationTypeId)).hide();
                $('#'.concat(this.rowOldMappingTagId)).hide();
                $('#'.concat(this.rowOldroleId)).hide();
                $('#'.concat(this.rowOldContactId)).hide();
                break;
            case 'Edit':
                $('#'.concat(this.titleId)).html('Gestione attività - Modifica');
                break;
        }
    }

    private btnConferma_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {
        this._loadingPanel.show(this.pageContentId);
        (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest('SaveMapping');
        return false;
    }

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

export = TbltWorkflowRepositoryGes;