/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports"], function (require, exports) {
    var TbltCategorySchema = /** @class */ (function () {
        function TbltCategorySchema() {
            var _this = this;
            /**
             *------------------------- Events -----------------------------
             */
            /**
             * Evento scatenato al click del pulsante di modifica
             */
            this.btnEdit_OnClicking = function (sender, eventArgs) {
                eventArgs.set_cancel(true);
                _this.openWindow('Action=Edit');
            };
            /**
             * Evento scatenato al click del pulsante di elimina
             */
            this.btnDelete_OnClicking = function (sender, eventArgs) {
                eventArgs.set_cancel(true);
                _this.openWindow('Action=Delete');
            };
            /**
             * Chiude la window corrente
             */
            this.closeWindow = function (sender, args) {
                if (args.get_argument() != undefined) {
                    $find(_this.ajaxManagerId).ajaxRequest('ReloadSchemas');
                }
            };
        }
        /**
         *------------------------- Methods -----------------------------
         */
        /**
         * Inizializzazione
         */
        TbltCategorySchema.prototype.initialize = function () {
            var wndManager = $find(this.radWindowManagerId);
            wndManager.getWindowByName(TbltCategorySchema.WINDOW_NAME).add_close(this.closeWindow);
            this._btnEdit = $find(this.btnEditId);
            this._btnEdit.add_clicking(this.btnEdit_OnClicking);
            this._btnDelete = $find(this.btnDeleteId);
            this._btnDelete.add_clicking(this.btnDelete_OnClicking);
        };
        /**
         * Apre una nuova window
         * @param parameters
         */
        TbltCategorySchema.prototype.openWindow = function (parameters) {
            this._grdCategorySchema = $find(this.grdCategorySchemaId);
            var selectedRows = this._grdCategorySchema.get_selectedItems();
            if (selectedRows.length == 0) {
                alert("Nessuna Versione selezionata");
                return;
            }
            var selectedIdCategorySchema = selectedRows[0].getDataKeyValue("Id");
            var url = 'TbltCategorySchemaGes.aspx?'.concat(parameters, "&IdCategorySchema=", selectedIdCategorySchema);
            var manager = $find(this.radWindowManagerId);
            var wnd = manager.open(url, TbltCategorySchema.WINDOW_NAME, undefined);
            wnd.setSize(600, 500);
            wnd.add_close(this.closeWindow);
            wnd.center();
        };
        TbltCategorySchema.WINDOW_NAME = 'rwEdit';
        return TbltCategorySchema;
    }());
    return TbltCategorySchema;
});
//# sourceMappingURL=TbltCategorySchema.js.map