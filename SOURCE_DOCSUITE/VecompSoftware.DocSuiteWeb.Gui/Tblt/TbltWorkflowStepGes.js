define(["require", "exports", "App/Models/Workflows/WorkflowAuthorizationType", "App/Models/Workflows/ActivityType", "App/Models/Workflows/ActivityAction", "App/Models/Workflows/ActivityArea", "App/Models/Workflows/WorkflowTreeNodeType", "App/Helpers/EnumHelper"], function (require, exports, WorkflowAuthorizationType, ActivityType, ActivityAction, ActivityArea, WorkflowTreeNodeType, EnumHelper) {
    var TbltWorkflowStepGes = /** @class */ (function () {
        function TbltWorkflowStepGes() {
            var _this = this;
            this._btnWorkflowStepSelectorOk_onClick = function (sender, args) {
                var repository = JSON.parse(sessionStorage.getItem(WorkflowTreeNodeType.Workflow.toString()));
                var steps = JSON.parse(repository.Json);
                var workflowSteps = Object.keys(steps).map(function (i) { return steps[i]; });
                var workflowStepPositions = workflowSteps.map(function (step) { return step.Position; });
                var maxStepPosition = Math.max.apply(Math, workflowStepPositions);
                var workflowStepAuthType = _this._cmbWorkflowStepAuthorizationType.get_text() != "" ? _this._cmbWorkflowStepAuthorizationType.get_value() : undefined;
                var workflowStepActivityType = _this._cmbWorkflowStepActivityType.get_text() != "" ? _this._cmbWorkflowStepActivityType.get_value() : undefined;
                var workflowStepActivityAction = _this._cmbWorkflowStepActivityAction.get_text() != "" ? _this._cmbWorkflowStepActivityAction.get_value() : undefined;
                var workflowStepActivityArea = _this._cmbWorkflowStepActivityArea.get_text() != "" ? _this._cmbWorkflowStepActivityArea.get_value() : undefined;
                var workflowStepName = _this._txtWorkflowStepName.get_textBoxValue();
                var workflowStep = {
                    Position: maxStepPosition + 1,
                    Name: workflowStepName,
                    AuthorizationType: WorkflowAuthorizationType[workflowStepAuthType],
                    ActivityType: ActivityType[workflowStepActivityType],
                    ActivityOperation: {
                        Action: ActivityAction[workflowStepActivityAction],
                        Area: ActivityArea[workflowStepActivityArea]
                    },
                    EvaluationArguments: new Array(),
                    InputArguments: new Array(),
                    OutputArguments: new Array()
                };
                if (!_this.validateFields(workflowStep)) {
                    alert("Tutti i campi sono obbligatori");
                    return;
                }
                if (_this.actionPage == TbltWorkflowStepGes.ACTION_PAGE_EDIT) {
                    var workflowStepForEdit = JSON.parse(sessionStorage.getItem(WorkflowTreeNodeType.Step.toString()));
                    workflowStep.Position = workflowStepForEdit.Position;
                    workflowStep.EvaluationArguments = workflowStepForEdit.EvaluationArguments;
                    workflowStep.InputArguments = workflowStepForEdit.InputArguments;
                    workflowStep.OutputArguments = workflowStepForEdit.OutputArguments;
                }
                _this.closeWindow(workflowStep, repository.UniqueId);
            };
            this._btnWorkflowStepSelectorCancel_onClick = function (sender, args) {
                var wnd = _this.getRadWindow();
                wnd.close();
            };
            this._enumHelper = new EnumHelper();
        }
        TbltWorkflowStepGes.prototype.initialize = function () {
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._cmbWorkflowStepAuthorizationType = $find(this.cmbWorkflowStepAuthorizationTypeId);
            this._cmbWorkflowStepActivityType = $find(this.cmbWorkflowStepActivityTypeId);
            this._cmbWorkflowStepActivityAction = $find(this.cmbWorkflowStepActivityActionId);
            this._cmbWorkflowStepActivityArea = $find(this.cmbWorkflowStepActivityAreaId);
            this._btnWorkflowStepSelectorOk = $find(this.btnWorkflowStepSelectorOkId);
            this._btnWorkflowStepSelectorOk.add_clicked(this._btnWorkflowStepSelectorOk_onClick);
            this._btnWorkflowStepSelectorCancel = $find(this.btnWorkflowStepSelectorCancelId);
            this._btnWorkflowStepSelectorCancel.add_clicked(this._btnWorkflowStepSelectorCancel_onClick);
            this._txtWorkflowStepName = $find(this.txtWorkflowStepNameId);
            this._authorizationDataSource = $find(this.authorizationDataSourceId);
            this._activityDataSource = $find(this.activityDataSourceId);
            this._areaDataSource = $find(this.areaDataSourceId);
            this._actionDataSource = $find(this.actionDataSourceId);
            this.populateWorkflowStepWindow();
        };
        TbltWorkflowStepGes.prototype.validateFields = function (workflowStep) {
            for (var ws in workflowStep) {
                if (workflowStep[ws] === undefined || workflowStep[ws] === null || workflowStep[ws] === "") {
                    return false;
                }
                if (workflowStep[ws] === workflowStep.ActivityOperation) {
                    for (var ao in workflowStep.ActivityOperation) {
                        if (workflowStep.ActivityOperation[ao] === undefined || workflowStep.ActivityOperation[ao] === null || workflowStep.ActivityOperation[ao] === "") {
                            return false;
                        }
                    }
                }
            }
            return true;
        };
        TbltWorkflowStepGes.prototype.closeWindow = function (workflowStep, uniqueId) {
            var wnd = this.getRadWindow();
            wnd.close([workflowStep, uniqueId]);
            this.clearFields();
        };
        TbltWorkflowStepGes.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        TbltWorkflowStepGes.prototype.clearFields = function () {
            this._txtWorkflowStepName.clear();
            this._cmbWorkflowStepActivityAction.clearSelection();
            this._cmbWorkflowStepActivityArea.clearSelection();
            this._cmbWorkflowStepActivityType.clearSelection();
            this._cmbWorkflowStepAuthorizationType.clearSelection();
        };
        TbltWorkflowStepGes.prototype.populateWorkflowStepWindow = function () {
            this.populateWorkflowAuthorizationTypeCombo();
            this.populateActivityTypeCombo();
            this.populateActivityActionCombo();
            this.populateActivityAreaCombo();
            if (this.actionPage == TbltWorkflowStepGes.ACTION_PAGE_EDIT) {
                var workflowStepForEdit = JSON.parse(sessionStorage.getItem(WorkflowTreeNodeType.Step.toString()));
                this._txtWorkflowStepName.set_value(workflowStepForEdit.Name);
                this.setWorkflowAuthorizationTypeSelection(workflowStepForEdit.AuthorizationType);
                this.setActivityTypeCurrentSelection(workflowStepForEdit.ActivityType);
                this.setActivityActionSelection(workflowStepForEdit.ActivityOperation.Action);
                this.setActivityAreaSelection(workflowStepForEdit.ActivityOperation.Area);
            }
        };
        TbltWorkflowStepGes.prototype.setActivityAreaSelection = function (value) {
            if (value === undefined) {
                //select nothing and return
                return;
            }
            var key = (typeof (value) === "number" || Number(value))
                ? (Number(value))
                : ActivityArea[value];
            //using this.comboBox.set_selectedIndex(index) will not work.
            //you have to find the combo box item and select itself like below
            //reference: https://community-archive.progress.com/forums/00295/41954.html
            var item = this._cmbWorkflowStepActivityArea.findItemByValue(key.toString());
            item.select();
        };
        TbltWorkflowStepGes.prototype.setActivityActionSelection = function (value) {
            if (value === undefined) {
                //select nothing and return
                return;
            }
            var key = (typeof (value) === "number" || Number(value))
                ? (Number(value))
                : ActivityAction[value];
            //using this.comboBox.set_selectedIndex(index) will not work.
            //you have to find the combo box item and select itself like below
            //reference: https://community-archive.progress.com/forums/00295/41954.html
            var item = this._cmbWorkflowStepActivityAction.findItemByValue(key.toString());
            item.select();
        };
        TbltWorkflowStepGes.prototype.setWorkflowAuthorizationTypeSelection = function (value) {
            if (value === undefined) {
                //select nothing and return
                return;
            }
            var key = (typeof (value) === "number" || Number(value))
                ? (Number(value))
                : WorkflowAuthorizationType[value];
            //using this.comboBox.set_selectedIndex(index) will not work.
            //you have to find the combo box item and select itself like below
            //reference: https://community-archive.progress.com/forums/00295/41954.html
            var item = this._cmbWorkflowStepAuthorizationType.findItemByValue(key.toString());
            item.select();
        };
        TbltWorkflowStepGes.prototype.setActivityTypeCurrentSelection = function (value) {
            if (value === undefined) {
                //select nothing and return
                return;
            }
            var key = (typeof (value) === "number" || Number(value))
                ? (Number(value))
                : ActivityType[value];
            //using this.comboBox.set_selectedIndex(index) will not work.
            //you have to find the combo box item and select itself like below
            //reference: https://community-archive.progress.com/forums/00295/41954.html
            var item = this._cmbWorkflowStepActivityType.findItemByValue(key.toString());
            item.select();
        };
        /**
         * Clears the _cmbWorkflowStepAuthorizationType combobox.
         * Populates it's datasource with WorkflowAuthorizationType keys and descriptions
         **/
        TbltWorkflowStepGes.prototype.populateWorkflowAuthorizationTypeCombo = function () {
            this._cmbWorkflowStepAuthorizationType.get_items().clear();
            //do not use a datasource. It enables lazy binding and even if you set the flag EnableOnLoad=false on the combo
            //items will be moved from datasource to combobox at first drop down select. This makes it impossible to set the 
            //current selection.
            var keys = Object.keys(WorkflowAuthorizationType).filter(function (x) { return parseInt(x) >= 1; }).map(function (x) { return parseInt(x); });
            for (var _i = 0, keys_1 = keys; _i < keys_1.length; _i++) {
                var key = keys_1[_i];
                var item = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(this.getWorkflowAuthorizationType(key));
                item.set_value(key.toString());
                this._cmbWorkflowStepAuthorizationType.get_items().add(item);
            }
        };
        /**
         * Clears the _cmbWorkflowStepActivityType combobox.
         * Populates it's datasource with ActivityType keys and descriptions
         **/
        TbltWorkflowStepGes.prototype.populateActivityTypeCombo = function () {
            this._cmbWorkflowStepActivityType.get_items().clear();
            //do not use a datasource. It enables lazy binding and even if you set the flag EnableOnLoad=false on the combo
            //items will be moved from datasource to combobox at first drop down select. This makes it impossible to set the 
            //current selection.
            var keys = Object.keys(ActivityType).filter(function (x) { return parseInt(x) >= 0; }).map(function (x) { return parseInt(x); });
            for (var _i = 0, keys_2 = keys; _i < keys_2.length; _i++) {
                var key = keys_2[_i];
                var item = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(this.getActivityTypeDescription(key));
                item.set_value(key.toString());
                this._cmbWorkflowStepActivityType.get_items().add(item);
            }
        };
        /**
         * Clears the _cmbWorkflowStepActivityAction combobox.
         * Populates it's datasource with ActivityAction keys and descriptions
         **/
        TbltWorkflowStepGes.prototype.populateActivityActionCombo = function () {
            this._cmbWorkflowStepActivityAction.get_items().clear();
            //do not use a datasource. It enables lazy binding and even if you set the flag EnableOnLoad=false on the combo
            //items will be moved from datasource to combobox at first drop down select. This makes it impossible to set the 
            //current selection.
            var keys = Object.keys(ActivityAction).filter(function (x) { return parseInt(x) >= 0; }).map(function (x) { return parseInt(x); });
            for (var _i = 0, keys_3 = keys; _i < keys_3.length; _i++) {
                var key = keys_3[_i];
                var item = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(this.getActivityActionDescription(key));
                item.set_value(key.toString());
                this._cmbWorkflowStepActivityAction.get_items().add(item);
            }
        };
        /**
         * Clears the _cmbWorkflowStepActivityArea combobox.
         * Populates it's datasource with ActivityArea keys and descriptions
         **/
        TbltWorkflowStepGes.prototype.populateActivityAreaCombo = function () {
            this._cmbWorkflowStepActivityArea.get_items().clear();
            //do not use a datasource. It enables lazy binding and even if you set the flag EnableOnLoad=false on the combo
            //items will be moved from datasource to combobox at first drop down select. This makes it impossible to set the 
            //current selection.
            var keys = Object.keys(ActivityArea).filter(function (x) { return parseInt(x) >= 0; }).map(function (x) { return parseInt(x); });
            for (var _i = 0, keys_4 = keys; _i < keys_4.length; _i++) {
                var key = keys_4[_i];
                var item = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(this.getActivityAreaDescription(key));
                item.set_value(key.toString());
                this._cmbWorkflowStepActivityArea.get_items().add(item);
            }
        };
        /**
         * Returns the description of an WorkflowAuthorizationType value.
         * @param data The enum value as number of string.
         */
        TbltWorkflowStepGes.prototype.getWorkflowAuthorizationType = function (data) {
            if (data === undefined) {
                return "";
            }
            if (typeof (data) === "number" || Number(data)) {
                return this._enumHelper.getWorkflowAuthorizationType(Number(data));
            }
            return this._enumHelper.getWorkflowAuthorizationType(WorkflowAuthorizationType[data]);
        };
        /**
         * Returns the description of an ActivityType value.
         * @param data The enum value as number of string.
         */
        TbltWorkflowStepGes.prototype.getActivityTypeDescription = function (data) {
            if (data === undefined) {
                return "";
            }
            if (typeof (data) === "number" || Number(data)) {
                return this._enumHelper.getActivityTypeDescription(ActivityType[Number(data)]);
            }
            return this._enumHelper.getActivityTypeDescription(data);
        };
        /**
         * Returns the description of an ActivityAction value.
         * @param data The enum value as number of string.
         */
        TbltWorkflowStepGes.prototype.getActivityActionDescription = function (data) {
            if (data === undefined) {
                return "";
            }
            if (typeof (data) === "number" || Number(data)) {
                return this._enumHelper.getWorkflowActivityActionDescription(Number(data));
            }
            return this._enumHelper.getWorkflowActivityActionDescription(ActivityAction[data]);
        };
        /**
         * Returns the description of an ActivityArea value.
         * @param data The enum value as number of string.
         */
        TbltWorkflowStepGes.prototype.getActivityAreaDescription = function (data) {
            if (data === undefined) {
                return "";
            }
            if (typeof (data) === "number" || Number(data)) {
                return this._enumHelper.getActivityAreaDescription(ActivityArea[Number(data)]);
            }
            return this._enumHelper.getActivityAreaDescription(data);
        };
        TbltWorkflowStepGes.ACTION_PAGE_EDIT = "Edit";
        return TbltWorkflowStepGes;
    }());
    return TbltWorkflowStepGes;
});
//# sourceMappingURL=TbltWorkflowStepGes.js.map