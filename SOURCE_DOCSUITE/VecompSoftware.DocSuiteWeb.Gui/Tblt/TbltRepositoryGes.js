define(["require", "exports", "App/Models/Workflows/WorkflowDSWEnvironmentType", "App/Models/Workflows/WorkflowTreeNodeType", "App/Helpers/GuidHelper", "App/Models/Workflows/WorkflowRepositoryStatus"], function (require, exports, DSWEnvironmentType, WorkflowTreeNodeType, Guid, WorkflowRepositoryStatus) {
    var TbltRepositoryGes = /** @class */ (function () {
        function TbltRepositoryGes() {
            var _this = this;
            this.workflowStatusType = [TbltRepositoryGes.STATUS_DRAFT, TbltRepositoryGes.STATUS_PUBLISHED];
            this._btnWorkflowSelectorOk_onClick = function (sender, args) {
                var workflowName = _this._txtWorkflowName.get_textBoxValue();
                var workflowVersion = _this._rntbVersionValue.get_value();
                var workflowActiveFromDate = _this._rdpValueDate.get_selectedDate();
                var workflowEnvironment = parseInt(_this._cmbEnvironment.get_value());
                var workflowStatus = _this._cmbStatus.get_value();
                var workflowRepository = {
                    UniqueId: Guid.newGuid(),
                    Name: workflowName,
                    Version: workflowVersion,
                    ActiveFrom: workflowActiveFromDate,
                    ActiveTo: null,
                    DSWEnvironment: workflowEnvironment,
                    Status: WorkflowRepositoryStatus[workflowStatus],
                    Json: '{}',
                    CustomActivities: null,
                    Xaml: null,
                    WorkflowEvaluationProperties: new Array(),
                    WorkflowRoleMappings: new Array(),
                    Roles: new Array()
                };
                if (_this.actionPage === TbltRepositoryGes.ACTION_PAGE_EDIT) {
                    var workflowRepositoryForEdit = JSON.parse(sessionStorage[WorkflowTreeNodeType.Workflow]);
                    workflowRepository.ActiveFrom = workflowRepositoryForEdit.ActiveFrom;
                    workflowRepository.ActiveTo = workflowRepositoryForEdit.ActiveTo;
                    workflowRepository.UniqueId = workflowRepositoryForEdit.UniqueId;
                    workflowRepository.CustomActivities = workflowRepositoryForEdit.CustomActivities;
                    workflowRepository.Json = workflowRepositoryForEdit.Json;
                    workflowRepository.Xaml = workflowRepositoryForEdit.Xaml;
                    workflowRepository.WorkflowEvaluationProperties = workflowRepositoryForEdit.WorkflowEvaluationProperties;
                    workflowRepository.WorkflowRoleMappings = workflowRepositoryForEdit.WorkflowRoleMappings;
                    workflowRepository.Roles = workflowRepositoryForEdit.Roles;
                }
                if (!_this.validateFields(workflowRepository)) {
                    alert("Tutti i campi sono obbligatori");
                    return;
                }
                _this.closeWindow(workflowRepository);
            };
            this._btnWorkflowSelectorCancel_onClick = function (sender, args) {
                var wnd = _this.getRadWindow();
                wnd.close();
            };
        }
        TbltRepositoryGes.prototype.initialize = function () {
            this._txtWorkflowName = $find(this.txtWorkflowNameId);
            this._rntbVersionValue = $find(this.rntbVersionValueId);
            this._rdpValueDate = $find(this.rdpValueDateId);
            this._environmentDataSource = $find(this.environmentDataSourceId);
            this._cmbEnvironment = $find(this.cmbEnvironmentId);
            this._cmbStatus = $find(this.cmbStatusId);
            this._statusDataSource = $find(this.statusDataSourceId);
            this._btnWorkflowSelectorOk = $find(this.btnWorkflowSelectorOkId);
            this._btnWorkflowSelectorOk.add_clicked(this._btnWorkflowSelectorOk_onClick);
            this._btnWorkflowSelectorCancel = $find(this.btnWorkflowSelectorCancelId);
            this._btnWorkflowSelectorCancel.add_clicked(this._btnWorkflowSelectorCancel_onClick);
            this.populateWorkflowWindow();
        };
        TbltRepositoryGes.prototype.populateWorkflowWindow = function () {
            this._rntbVersionValue.set_value('1');
            this._rdpValueDate.set_selectedDate(new Date());
            this.workflowEnvironmentType = Object.keys(DSWEnvironmentType).filter(function (x) { return !(parseInt(x) >= -1); });
            this._cmbEnvironment.get_items().clear();
            for (var _i = 0, _a = this.workflowEnvironmentType; _i < _a.length; _i++) {
                var data = _a[_i];
                this._environmentDataSource.add({ Name: data, Value: DSWEnvironmentType[data] });
            }
            this._cmbStatus.get_items().clear();
            for (var _b = 0, _c = this.workflowStatusType; _b < _c.length; _b++) {
                var data = _c[_b];
                switch (data) {
                    case TbltRepositoryGes.STATUS_DRAFT:
                        this._statusDataSource.add({ Name: data, Value: WorkflowRepositoryStatus.Draft });
                        break;
                    case TbltRepositoryGes.STATUS_PUBLISHED:
                        this._statusDataSource.add({ Name: data, Value: WorkflowRepositoryStatus.Published });
                        break;
                    default:
                        this._statusDataSource.add({ Name: data, Value: '' });
                        break;
                }
            }
            if (this.actionPage == TbltRepositoryGes.ACTION_PAGE_EDIT) {
                var workflowRepositoryForEdit = JSON.parse(sessionStorage[WorkflowTreeNodeType.Workflow]);
                this._txtWorkflowName.set_value(workflowRepositoryForEdit.Name);
                this._rntbVersionValue.set_value(workflowRepositoryForEdit.Version);
                this._cmbEnvironment.set_text(workflowRepositoryForEdit.DSWEnvironment.toString());
                this._cmbEnvironment.set_value(DSWEnvironmentType[workflowRepositoryForEdit.DSWEnvironment]);
                switch (workflowRepositoryForEdit.Status.toString()) {
                    case WorkflowRepositoryStatus[WorkflowRepositoryStatus.Draft]:
                        this._cmbStatus.set_text(TbltRepositoryGes.STATUS_DRAFT);
                        break;
                    case WorkflowRepositoryStatus[WorkflowRepositoryStatus.Published]:
                        this._cmbStatus.set_text(TbltRepositoryGes.STATUS_PUBLISHED);
                        break;
                }
                this._cmbStatus.set_value(WorkflowRepositoryStatus[workflowRepositoryForEdit.Status]);
                this._rdpValueDate.set_selectedDate(new Date(workflowRepositoryForEdit.ActiveFrom));
                this._rdpValueDate.set_enabled(false);
            }
        };
        TbltRepositoryGes.prototype.closeWindow = function (workflowRepository) {
            var wnd = this.getRadWindow();
            var obj = {
                WorkflowRepository: workflowRepository,
                Action: this.actionPage
            };
            wnd.close(obj);
            this.clearFields();
        };
        TbltRepositoryGes.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        TbltRepositoryGes.prototype.clearFields = function () {
            this._txtWorkflowName.clear();
            this._rntbVersionValue.set_value('1');
            this._rdpValueDate.set_selectedDate(new Date());
            this._cmbEnvironment.clearSelection();
            this._cmbStatus.clearSelection();
        };
        TbltRepositoryGes.prototype.validateFields = function (workflowRepository) {
            if (workflowRepository.Name === "" || workflowRepository.DSWEnvironment < 0 || workflowRepository.Status < 0) {
                return false;
            }
            return true;
        };
        TbltRepositoryGes.ACTION_PAGE_EDIT = "Edit";
        TbltRepositoryGes.STATUS_DRAFT = "Bozza";
        TbltRepositoryGes.STATUS_PUBLISHED = "Pubblicato";
        return TbltRepositoryGes;
    }());
    return TbltRepositoryGes;
});
//# sourceMappingURL=TbltRepositoryGes.js.map