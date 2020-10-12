define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Services/DocumentUnits/DocumentUnitService", "App/Helpers/SessionStorageKeysHelper"], function (require, exports, ServiceConfigurationHelper, DocumentUnitService, SessionStorageKeysHelper) {
    var ProtVisualizza = /** @class */ (function () {
        function ProtVisualizza(serviceConfigurations) {
            var _this = this;
            this.btnWorkflow_onClick = function (sender) {
                _this.setSessionVariables();
                var url = "../Workflows/StartWorkflow.aspx?Type=Prot&ManagerID=" + _this.radWindowManagerId + "&DSWEnvironment=Protocol&Callback" + window.location.href;
                return _this.openWindow(url, "windowStartWorkflow", 730, 550);
            };
            this.onWorkflowCloseWindow = function (sender, args) {
                if (args.get_argument()) {
                    var result = {};
                    result = args.get_argument();
                    _this._notificationInfo.show();
                    _this._notificationInfo.set_text(result.ActionName);
                    _this._loadingPanel.show(_this.pnlMainContentId);
                    location.reload();
                }
            };
            this._serviceConfigurations = serviceConfigurations;
        }
        ProtVisualizza.prototype.initialize = function () {
            var _this = this;
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._notificationInfo = $find(this.radNotificationInfoId);
            this._radWindowManager = $find(this.radWindowManagerId);
            var documentUnitConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DocumentUnit");
            this._documentUnitService = new DocumentUnitService(documentUnitConfiguration);
            this._documentUnitService.getDocumentUnitById(this.currentDocumentUnitId, function (data) {
                _this._documentUnitModel = data;
            });
            if (this.btnWorkflowId && this.btnWorkflowId !== "") {
                this._btnWorkflow = $("#" + this.btnWorkflowId);
                this._btnWorkflow.click(this.btnWorkflow_onClick);
                this._radWindowManager.add_close((this.onWorkflowCloseWindow));
            }
        };
        ProtVisualizza.prototype.setSessionVariables = function () {
            sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL, JSON.stringify(this._documentUnitModel));
            sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_ID, this._documentUnitModel.UniqueId);
            sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_TITLE, this._documentUnitModel.Title + " - " + this._documentUnitModel.Subject);
        };
        ProtVisualizza.prototype.openWindow = function (url, name, width, height) {
            var manager = $find(this.radWindowManagerId);
            var wnd = manager.open(url, name, null);
            wnd.setSize(width, height);
            wnd.set_modal(true);
            wnd.center();
            return false;
        };
        return ProtVisualizza;
    }());
    return ProtVisualizza;
});
//# sourceMappingURL=ProtVisualizza.js.map