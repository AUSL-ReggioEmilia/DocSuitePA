define(["require", "exports", "UserControl/uscProcessDetails", "App/Models/Processes/ProcessNodeType"], function (require, exports, uscProcessDetails, ProcessNodeType) {
    var ProcessVisualizza = /** @class */ (function () {
        function ProcessVisualizza(serviceConfigurations) {
        }
        ProcessVisualizza.prototype.initialize = function () {
            this._initializeControls();
            this._showProcessDetails();
        };
        ProcessVisualizza.prototype._initializeControls = function () {
            this._uscProcessDetails = $("#" + this.uscProcessDetailsId).data();
        };
        ProcessVisualizza.prototype._showProcessDetails = function () {
            this._uscProcessDetails.clearProcessDetails();
            $("#" + this._uscProcessDetails.pnlDetailsId).show();
            this._uscProcessDetails.setPanelVisibility(uscProcessDetails.InformationDetails_PanelName, true);
            this._uscProcessDetails.setPanelVisibility(uscProcessDetails.CategoryInformationDetails_PanelName, false);
            this._uscProcessDetails.setPanelVisibility(uscProcessDetails.WorkflowDetails_PanelName, false);
            this._uscProcessDetails.setPanelVisibility(uscProcessDetails.FascicleDetails_PanelName, false);
            this._uscProcessDetails.setPanelVisibility(uscProcessDetails.RoleDetails_PanelName, true);
            uscProcessDetails.selectedProcessId = this.processId;
            uscProcessDetails.selectedEntityType = ProcessNodeType.Process;
            this._uscProcessDetails.setProcessDetails('', true);
        };
        return ProcessVisualizza;
    }());
    return ProcessVisualizza;
});
//# sourceMappingURL=ProcessVisualizza.js.map