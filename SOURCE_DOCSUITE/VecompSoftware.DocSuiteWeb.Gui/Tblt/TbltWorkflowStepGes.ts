import WorkflowAuthorizationType = require("App/Models/Workflows/WorkflowAuthorizationType");
import ActivityType = require("App/Models/Workflows/ActivityType");
import ActivityAction = require("App/Models/Workflows/ActivityAction");
import ActivityArea = require("App/Models/Workflows/ActivityArea");
import WorkflowStep = require("App/Models/Workflows/WorkflowStep");
import WorkflowRepositoryModel = require("App/Models/Workflows/WorkflowRepositoryModel");
import WorkflowArgumentModel = require("App/Models/Workflows/WorkflowArgumentModel");
import WorkflowTreeNodeType = require("App/Models/Workflows/WorkflowTreeNodeType");
import EnumHelper = require('App/Helpers/EnumHelper');

class TbltWorkflowStepGes {
    txtWorkflowStepNameId: string;
    cmbWorkflowStepAuthorizationTypeId: string;
    cmbWorkflowStepActivityTypeId: string;
    cmbWorkflowStepActivityActionId: string;
    cmbWorkflowStepActivityAreaId: string;
    btnWorkflowStepSelectorCancelId: string;
    btnWorkflowStepSelectorOkId: string;
    ajaxManagerId: string;
    ajaxLoadingPanelId: string;
    pageContentId: string;
    workflowAuthorizationTypes: string[];
    workflowStepActivityTypes: string[];
    workflowStepActivityActions: string[];
    workflowStepActivityAreas: string[];
    actionPage: string;
    authorizationDataSourceId: string;
    activityDataSourceId: string;
    areaDataSourceId: string;
    actionDataSourceId: string;

    private static ACTION_PAGE_EDIT = "Edit";

    private _cmbWorkflowStepAuthorizationType: Telerik.Web.UI.RadComboBox;
    private _cmbWorkflowStepActivityType: Telerik.Web.UI.RadComboBox;
    private _cmbWorkflowStepActivityAction: Telerik.Web.UI.RadComboBox;
    private _cmbWorkflowStepActivityArea: Telerik.Web.UI.RadComboBox;
    private _btnWorkflowStepSelectorOk: Telerik.Web.UI.RadButton;
    private _btnWorkflowStepSelectorCancel: Telerik.Web.UI.RadButton;
    private _txtWorkflowStepName: Telerik.Web.UI.RadTextBox;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _authorizationDataSource: Telerik.Web.UI.RadClientDataSource;
    private _activityDataSource: Telerik.Web.UI.RadClientDataSource;
    private _areaDataSource: Telerik.Web.UI.RadClientDataSource;
    private _actionDataSource: Telerik.Web.UI.RadClientDataSource;
    private _enumHelper: EnumHelper;

    constructor() {
        this._enumHelper = new EnumHelper();
    }

    initialize(): void {
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._cmbWorkflowStepAuthorizationType = <Telerik.Web.UI.RadComboBox>$find(this.cmbWorkflowStepAuthorizationTypeId);
        this._cmbWorkflowStepActivityType = <Telerik.Web.UI.RadComboBox>$find(this.cmbWorkflowStepActivityTypeId);
        this._cmbWorkflowStepActivityAction = <Telerik.Web.UI.RadComboBox>$find(this.cmbWorkflowStepActivityActionId);
        this._cmbWorkflowStepActivityArea = <Telerik.Web.UI.RadComboBox>$find(this.cmbWorkflowStepActivityAreaId);
        this._btnWorkflowStepSelectorOk = <Telerik.Web.UI.RadButton>$find(this.btnWorkflowStepSelectorOkId);
        this._btnWorkflowStepSelectorOk.add_clicked(this._btnWorkflowStepSelectorOk_onClick);
        this._btnWorkflowStepSelectorCancel = <Telerik.Web.UI.RadButton>$find(this.btnWorkflowStepSelectorCancelId);
        this._btnWorkflowStepSelectorCancel.add_clicked(this._btnWorkflowStepSelectorCancel_onClick);
        this._txtWorkflowStepName = <Telerik.Web.UI.RadTextBox>$find(this.txtWorkflowStepNameId);
        this._authorizationDataSource = <Telerik.Web.UI.RadClientDataSource>$find(this.authorizationDataSourceId);
        this._activityDataSource = <Telerik.Web.UI.RadClientDataSource>$find(this.activityDataSourceId);
        this._areaDataSource = <Telerik.Web.UI.RadClientDataSource>$find(this.areaDataSourceId);
        this._actionDataSource = <Telerik.Web.UI.RadClientDataSource>$find(this.actionDataSourceId);
        this.populateWorkflowStepWindow();
    }

    _btnWorkflowStepSelectorOk_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        let repository: WorkflowRepositoryModel = JSON.parse(sessionStorage.getItem(WorkflowTreeNodeType.Workflow.toString()));
        let steps = JSON.parse(repository.Json);
        let workflowSteps: WorkflowStep[] = Object.keys(steps).map(function (i) { return steps[i]; });
        let workflowStepPositions: number[] = workflowSteps.map(step => step.Position);
        let maxStepPosition: number = Math.max(...workflowStepPositions);
        let workflowStepAuthType: string = this._cmbWorkflowStepAuthorizationType.get_text() != "" ? this._cmbWorkflowStepAuthorizationType.get_value() : undefined;
        let workflowStepActivityType: string = this._cmbWorkflowStepActivityType.get_text() != "" ? this._cmbWorkflowStepActivityType.get_value() : undefined;
        let workflowStepActivityAction: string = this._cmbWorkflowStepActivityAction.get_text() != "" ? this._cmbWorkflowStepActivityAction.get_value() : undefined;
        let workflowStepActivityArea: string = this._cmbWorkflowStepActivityArea.get_text() != "" ? this._cmbWorkflowStepActivityArea.get_value() : undefined;
        let workflowStepName: string = this._txtWorkflowStepName.get_textBoxValue();

        let workflowStep: WorkflowStep = {
            Position: maxStepPosition + 1,
            Name: workflowStepName,
            AuthorizationType: WorkflowAuthorizationType[workflowStepAuthType],
            ActivityType: ActivityType[workflowStepActivityType],
            ActivityOperation: {
                Action: ActivityAction[workflowStepActivityAction],
                Area: ActivityArea[workflowStepActivityArea]
            },
            EvaluationArguments: new Array<WorkflowArgumentModel>(),
            InputArguments: new Array<WorkflowArgumentModel>(),
            OutputArguments: new Array<WorkflowArgumentModel>()
        };

        if (!this.validateFields(workflowStep)) {
            alert("Tutti i campi sono obbligatori");
            return;
        }

        if (this.actionPage == TbltWorkflowStepGes.ACTION_PAGE_EDIT) {
            let workflowStepForEdit: WorkflowStep = JSON.parse(sessionStorage.getItem(WorkflowTreeNodeType.Step.toString()));
            workflowStep.Position = workflowStepForEdit.Position;
            workflowStep.EvaluationArguments = workflowStepForEdit.EvaluationArguments;
            workflowStep.InputArguments = workflowStepForEdit.InputArguments;
            workflowStep.OutputArguments = workflowStepForEdit.OutputArguments;
        }

        this.closeWindow(workflowStep, repository.UniqueId);
    }

    private validateFields(workflowStep: WorkflowStep): boolean {
        for (let ws in workflowStep) {
            if (workflowStep[ws] === undefined || workflowStep[ws] === null || workflowStep[ws] === "") {
                return false;
            }
            if (workflowStep[ws] === workflowStep.ActivityOperation) {
                for (let ao in workflowStep.ActivityOperation) {
                    if (workflowStep.ActivityOperation[ao] === undefined || workflowStep.ActivityOperation[ao] === null || workflowStep.ActivityOperation[ao] === "") {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    private closeWindow(workflowStep: WorkflowStep, uniqueId: string): void {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close([workflowStep, uniqueId]);
        this.clearFields();
    }

    private getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return wnd;
    }

    _btnWorkflowStepSelectorCancel_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close();
    }

    private clearFields(): void {
        this._txtWorkflowStepName.clear();
        this._cmbWorkflowStepActivityAction.clearSelection();
        this._cmbWorkflowStepActivityArea.clearSelection();
        this._cmbWorkflowStepActivityType.clearSelection();
        this._cmbWorkflowStepAuthorizationType.clearSelection();
    }

    private populateWorkflowStepWindow() {

        this.populateWorkflowAuthorizationTypeCombo();
        this.populateActivityTypeCombo();
        this.populateActivityActionCombo();
        this.populateActivityAreaCombo();


        if (this.actionPage == TbltWorkflowStepGes.ACTION_PAGE_EDIT) {
            let workflowStepForEdit: WorkflowStep = JSON.parse(sessionStorage.getItem(WorkflowTreeNodeType.Step.toString()));
            this._txtWorkflowStepName.set_value(workflowStepForEdit.Name);

            this.setWorkflowAuthorizationTypeSelection(workflowStepForEdit.AuthorizationType);
            this.setActivityTypeCurrentSelection(workflowStepForEdit.ActivityType);
            this.setActivityActionSelection(workflowStepForEdit.ActivityOperation.Action);
            this.setActivityAreaSelection(workflowStepForEdit.ActivityOperation.Area);

        }
    }

    private setActivityAreaSelection(value: ActivityArea | string | undefined): void {
        if (value === undefined) {
            //select nothing and return
            return;
        }

        let key: ActivityArea =
            (typeof (value) === "number" || Number(value))
                ? <ActivityArea>(Number(value))
                : ActivityArea[value];

        //using this.comboBox.set_selectedIndex(index) will not work.
        //you have to find the combo box item and select itself like below
        //reference: https://community-archive.progress.com/forums/00295/41954.html
        var item = this._cmbWorkflowStepActivityArea.findItemByValue(key.toString());
        item.select();
    }

    private setActivityActionSelection(value: ActivityAction | string | undefined): void {
        if (value === undefined) {
            //select nothing and return
            return;
        }

        let key: ActivityAction =
            (typeof (value) === "number" || Number(value))
                ? <ActivityAction>(Number(value))
                : ActivityAction[value];

        //using this.comboBox.set_selectedIndex(index) will not work.
        //you have to find the combo box item and select itself like below
        //reference: https://community-archive.progress.com/forums/00295/41954.html
        var item = this._cmbWorkflowStepActivityAction.findItemByValue(key.toString());
        item.select();
    }

    private setWorkflowAuthorizationTypeSelection(value: WorkflowAuthorizationType | string | undefined): void {
        if (value === undefined) {
            //select nothing and return
            return;
        }

        let key: WorkflowAuthorizationType =
            (typeof (value) === "number" || Number(value))
                ? <WorkflowAuthorizationType>(Number(value))
                : WorkflowAuthorizationType[value];

        //using this.comboBox.set_selectedIndex(index) will not work.
        //you have to find the combo box item and select itself like below
        //reference: https://community-archive.progress.com/forums/00295/41954.html
        var item = this._cmbWorkflowStepAuthorizationType.findItemByValue(key.toString());
        item.select();
    }

    private setActivityTypeCurrentSelection(value: ActivityType | string | undefined): void {
        if (value === undefined) {
            //select nothing and return
            return;
        }

        let key: ActivityType =
            (typeof (value) === "number" || Number(value))
                ? <ActivityType>(Number(value))
                : ActivityType[value];

        //using this.comboBox.set_selectedIndex(index) will not work.
        //you have to find the combo box item and select itself like below
        //reference: https://community-archive.progress.com/forums/00295/41954.html
        var item = this._cmbWorkflowStepActivityType.findItemByValue(key.toString());
        item.select();
    }

    /**
     * Clears the _cmbWorkflowStepAuthorizationType combobox.
     * Populates it's datasource with WorkflowAuthorizationType keys and descriptions
     **/
    private populateWorkflowAuthorizationTypeCombo() {
        this._cmbWorkflowStepAuthorizationType.get_items().clear();
        //do not use a datasource. It enables lazy binding and even if you set the flag EnableOnLoad=false on the combo
        //items will be moved from datasource to combobox at first drop down select. This makes it impossible to set the 
        //current selection.

        let keys: number[] = Object.keys(WorkflowAuthorizationType).filter(x => parseInt(x) >= 1).map(x => parseInt(x));

        for (let key of keys) {
            var item = new Telerik.Web.UI.RadComboBoxItem();
            item.set_text(this.getWorkflowAuthorizationType(key));
            item.set_value(key.toString());
            this._cmbWorkflowStepAuthorizationType.get_items().add(item);
        }
    }

    /**
     * Clears the _cmbWorkflowStepActivityType combobox.
     * Populates it's datasource with ActivityType keys and descriptions
     **/
    private populateActivityTypeCombo(): void {
        this._cmbWorkflowStepActivityType.get_items().clear();
        //do not use a datasource. It enables lazy binding and even if you set the flag EnableOnLoad=false on the combo
        //items will be moved from datasource to combobox at first drop down select. This makes it impossible to set the 
        //current selection.

        let keys: number[] = Object.keys(ActivityType).filter(x => parseInt(x) >= 0).map(x => parseInt(x));

        for (let key of keys) {
            var item = new Telerik.Web.UI.RadComboBoxItem();
            item.set_text(this.getActivityTypeDescription(key));
            item.set_value(key.toString());
            this._cmbWorkflowStepActivityType.get_items().add(item);
        }
    }

    /**
     * Clears the _cmbWorkflowStepActivityAction combobox.
     * Populates it's datasource with ActivityAction keys and descriptions
     **/
    private populateActivityActionCombo(): void {
        this._cmbWorkflowStepActivityAction.get_items().clear();
        //do not use a datasource. It enables lazy binding and even if you set the flag EnableOnLoad=false on the combo
        //items will be moved from datasource to combobox at first drop down select. This makes it impossible to set the 
        //current selection.

        let keys: number[] = Object.keys(ActivityAction).filter(x => parseInt(x) >= 0).map(x => parseInt(x));

        for (let key of keys) {
            var item = new Telerik.Web.UI.RadComboBoxItem();
            item.set_text(this.getActivityActionDescription(key));
            item.set_value(key.toString());
            this._cmbWorkflowStepActivityAction.get_items().add(item);
        }
    }

    /**
     * Clears the _cmbWorkflowStepActivityArea combobox.
     * Populates it's datasource with ActivityArea keys and descriptions
     **/
    private populateActivityAreaCombo(): void {
        this._cmbWorkflowStepActivityArea.get_items().clear();
        //do not use a datasource. It enables lazy binding and even if you set the flag EnableOnLoad=false on the combo
        //items will be moved from datasource to combobox at first drop down select. This makes it impossible to set the 
        //current selection.

        let keys: number[] = Object.keys(ActivityArea).filter(x => parseInt(x) >= 0).map(x => parseInt(x));

        for (let key of keys) {
            var item = new Telerik.Web.UI.RadComboBoxItem();
            item.set_text(this.getActivityAreaDescription(key));
            item.set_value(key.toString());
            this._cmbWorkflowStepActivityArea.get_items().add(item);
        }
    }

    /**
     * Returns the description of an WorkflowAuthorizationType value.
     * @param data The enum value as number of string.
     */
    private getWorkflowAuthorizationType(data: number | string | undefined): string {
        if (data === undefined) {
            return "";
        }

        if (typeof (data) === "number" || Number(data)) {
            return this._enumHelper.getWorkflowAuthorizationType(Number(data));
        }

        return this._enumHelper.getWorkflowAuthorizationType(WorkflowAuthorizationType[data]);
    }

    /**
     * Returns the description of an ActivityType value.
     * @param data The enum value as number of string.
     */
    private getActivityTypeDescription(data: number | string | undefined): string {
        if (data === undefined) {
            return "";
        }

        if (typeof (data) === "number" || Number(data)) {
            return this._enumHelper.getActivityTypeDescription(ActivityType[Number(data)]);
        }

        return this._enumHelper.getActivityTypeDescription(data);
    }

    /**
     * Returns the description of an ActivityAction value.
     * @param data The enum value as number of string.
     */
    private getActivityActionDescription(data: number | string | undefined): string {
        if (data === undefined) {
            return "";
        }

        if (typeof (data) === "number" || Number(data)) {
            return this._enumHelper.getWorkflowActivityActionDescription(Number(data));
        }

        return this._enumHelper.getWorkflowActivityActionDescription(ActivityAction[data]);
    }

    /**
     * Returns the description of an ActivityArea value.
     * @param data The enum value as number of string.
     */
    private getActivityAreaDescription(data: number | string | undefined): string {
        if (data === undefined) {
            return "";
        }

        if (typeof (data) === "number" || Number(data)) {
            return this._enumHelper.getActivityAreaDescription(ActivityArea[Number(data)])
        }

        return this._enumHelper.getActivityAreaDescription(data);
    }
}

export = TbltWorkflowStepGes;