/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

class TbltCategorySchema {
    ajaxManagerId: string;
    radWindowManagerId: string;
    grdCategorySchemaId: string;
    btnEditId: string;
    btnDeleteId: string;

    private _grdCategorySchema: Telerik.Web.UI.RadGrid;
    private static WINDOW_NAME: string = 'rwEdit';
    private _btnEdit: Telerik.Web.UI.RadButton;
    private _btnDelete: Telerik.Web.UI.RadButton;

    constructor() {
    }

    /**
     *------------------------- Events -----------------------------
     */

    /**
     * Evento scatenato al click del pulsante di modifica
     */
    private btnEdit_OnClicking = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonCancelEventArgs) => {
        eventArgs.set_cancel(true);
        this.openWindow('Action=Edit');
    }

    /**
     * Evento scatenato al click del pulsante di elimina
     */
    private btnDelete_OnClicking = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonCancelEventArgs) => {
        eventArgs.set_cancel(true);
        this.openWindow('Action=Delete');
    }

    /**
     *------------------------- Methods -----------------------------
     */

    /**
     * Inizializzazione
     */
    initialize(): void {
        let wndManager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerId);
        wndManager.getWindowByName(TbltCategorySchema.WINDOW_NAME).add_close(this.closeWindow);        
        this._btnEdit = <Telerik.Web.UI.RadButton>$find(this.btnEditId);
        this._btnEdit.add_clicking(this.btnEdit_OnClicking);
        this._btnDelete = <Telerik.Web.UI.RadButton>$find(this.btnDeleteId);
        this._btnDelete.add_clicking(this.btnDelete_OnClicking);
    }

    /**
     * Chiude la window corrente
     */
    closeWindow = (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs): void => {
        if (args.get_argument() != undefined) {
            (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest('ReloadSchemas');
        }
    }    

    /**
     * Apre una nuova window
     * @param parameters
     */
    openWindow(parameters: string): void {
        this._grdCategorySchema = <Telerik.Web.UI.RadGrid>$find(this.grdCategorySchemaId);
        let selectedRows: Array<Telerik.Web.UI.GridDataItem> = this._grdCategorySchema.get_selectedItems();
        if (selectedRows.length == 0) {
            alert("Nessuna Versione selezionata");
            return;
        }

        let selectedIdCategorySchema: string = selectedRows[0].getDataKeyValue("Id");
        let url: string = 'TbltCategorySchemaGes.aspx?'.concat(parameters, "&IdCategorySchema=", selectedIdCategorySchema);
        let manager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerId);        
        let wnd: Telerik.Web.UI.RadWindow = manager.open(url, TbltCategorySchema.WINDOW_NAME, undefined);
        wnd.setSize(600, 500);
        wnd.add_close(this.closeWindow);
        wnd.center();
    }
}

export = TbltCategorySchema;