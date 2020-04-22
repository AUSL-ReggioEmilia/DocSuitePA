/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports"], function (require, exports) {
    var TbltWorkflowRepositoryGes = /** @class */ (function () {
        function TbltWorkflowRepositoryGes() {
            var _this = this;
            this.btnConferma_onClick = function (sender, args) {
                _this._loadingPanel.show(_this.pageContentId);
                $find(_this.ajaxManagerId).ajaxRequest('SaveMapping');
                return false;
            };
        }
        TbltWorkflowRepositoryGes.prototype.initialize = function () {
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._btnConferma = $find(this.btnConfermaId);
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
        };
        /**
         * restituisce un riferimento alla radwindow
         */
        TbltWorkflowRepositoryGes.prototype.getRadWindow = function () {
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
        TbltWorkflowRepositoryGes.prototype.closeWindow = function (operator) {
            var wnd = this.getRadWindow();
            wnd.close(operator);
        };
        return TbltWorkflowRepositoryGes;
    }());
    return TbltWorkflowRepositoryGes;
});
//# sourceMappingURL=TbltWorkflowRepositoryGes.js.map