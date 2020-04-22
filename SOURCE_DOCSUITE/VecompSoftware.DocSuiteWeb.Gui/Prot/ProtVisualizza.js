define(["require", "exports", "../App/Helpers/ServiceConfigurationHelper", "../App/Services/DocumentUnits/DocumentUnitService"], function (require, exports, ServiceConfigurationHelper, DocumentUnitService) {
    var ProtVisualizza = /** @class */ (function () {
        function ProtVisualizza(serviceConfigurations) {
            var _this = this;
            this.btnWorkflow_OnClick = function (sender) {
                _this.setSessionVariables();
                var url = "../Workflows/StartWorkflow.aspx?Type=Prot&ManagerID=" + _this.radWindowManagerId + "&DSWEnvironment=Protocol&Callback" + window.location.href;
                return _this.openWindow(url, "windowStartWorkflow", 730, 550);
            };
            this._serviceConfigurations = serviceConfigurations;
        }
        ProtVisualizza.prototype.initialize = function () {
            var _this = this;
            this._btnWorkflow = $("#" + this.btnWorkflowId);
            this._btnWorkflow.click(this.btnWorkflow_OnClick);
            var documentUnitConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DocumentUnit");
            this._documentUnitService = new DocumentUnitService(documentUnitConfiguration);
            this._documentUnitService.getDocumentUnitById(this.currentId, function (data) {
                _this._documentUnitModel = data;
            });
        };
        ProtVisualizza.prototype.setSessionVariables = function () {
            sessionStorage.setItem("ReferenceModel", JSON.stringify(this._documentUnitModel));
            sessionStorage.setItem("ReferenceId", this._documentUnitModel.UniqueId);
            sessionStorage.setItem("ReferenceTitle", this._documentUnitModel.Title + "- " + this._documentUnitModel.Subject);
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