import WorkflowAuthorizationType = require("../App/Models/Workflows/WorkflowAuthorizationType");
import ActivityType = require("../App/Models/Workflows/ActivityType");
import ActivityAction = require("../App/Models/Workflows/ActivityAction");
import ActivityArea = require("../App/Models/Workflows/ActivityArea");
import WorkflowStep = require("../App/Models/Workflows/WorkflowStep");
import ActivityOperation = require("../App/Models/Workflows/ActivityOperationModel");
import WorkflowRepositoryModel = require("../App/Models/Workflows/WorkflowRepositoryModel");
import WorkflowArgumentModel = require("../App/Models/Workflows/WorkflowArgumentModel");
import WorkflowTreeNodeType = require("../App/Models/Workflows/WorkflowTreeNodeType");

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

    constructor() {
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

    _btnWorkflowStepSelectorOk_onClick = (sender: any, args: Telerik.Web.UI.RadButtonEventArgs) => {
        let repository: WorkflowRepositoryModel = JSON.parse(sessionStorage[WorkflowTreeNodeType.Workflow]);
        let steps = JSON.parse(repository.Json);
        let workflowSteps: WorkflowStep[] = Object.keys(steps).map(function (i) { return steps[i]; });
        let workflowStepPositions: number[] = workflowSteps.map(step => step.Position);
        let maxStepPosition: number = Math.max(...workflowStepPositions);

        let workflowStepAuthType: string = this._cmbWorkflowStepAuthorizationType.get_value() == "" ? null : this._cmbWorkflowStepAuthorizationType.get_value();
        let workflowStepActivityType: string = this._cmbWorkflowStepActivityType.get_value() == "" ? null : this._cmbWorkflowStepActivityType.get_value();
        let workflowStepActivityAction: string = this._cmbWorkflowStepActivityAction.get_value() == "" ? null : this._cmbWorkflowStepActivityAction.get_value();
        let workflowStepActivityArea: string = this._cmbWorkflowStepActivityArea.get_value() == "" ? null : this._cmbWorkflowStepActivityArea.get_value();
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
            OutputArguments: new Array <WorkflowArgumentModel>()
        };
        if (this.actionPage == TbltWorkflowStepGes.ACTION_PAGE_EDIT) {
            let workflowStepForEdit: WorkflowStep = JSON.parse(sessionStorage[WorkflowTreeNodeType.Step]);
            workflowStep.Position = workflowStepForEdit.Position;
            workflowStep.EvaluationArguments = workflowStepForEdit.EvaluationArguments;
            workflowStep.InputArguments = workflowStepForEdit.InputArguments;
            workflowStep.OutputArguments = workflowStepForEdit.OutputArguments;
        }
        this.closeWindow(workflowStep, repository.UniqueId);
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

    _btnWorkflowStepSelectorCancel_onClick = (sender: any, args: Telerik.Web.UI.RadButtonEventArgs) => {
        this.clearFields();
    }

    private clearFields(): void {
        this._txtWorkflowStepName.clear();
        this._cmbWorkflowStepActivityAction.clearSelection();
        this._cmbWorkflowStepActivityArea.clearSelection();
        this._cmbWorkflowStepActivityType.clearSelection();
        this._cmbWorkflowStepAuthorizationType.clearSelection();
    }

    private populateWorkflowStepWindow() {
        this.workflowAuthorizationTypes = Object.keys(WorkflowAuthorizationType).filter(x => !(parseInt(x) >= -1));
        this.populateWorkflowStepComboFromEnum(this.workflowAuthorizationTypes, this._cmbWorkflowStepAuthorizationType, this._authorizationDataSource, WorkflowAuthorizationType);

        this.workflowStepActivityTypes = Object.keys(ActivityType).filter(x => !(parseInt(x) >= -1));
        this.populateWorkflowStepComboFromEnum(this.workflowStepActivityTypes, this._cmbWorkflowStepActivityType, this._activityDataSource, ActivityType);

        this.workflowStepActivityActions = Object.keys(ActivityAction).filter(x => !(parseInt(x) >= -1));
        this.populateWorkflowStepComboFromEnum(this.workflowStepActivityActions, this._cmbWorkflowStepActivityAction, this._actionDataSource, ActivityAction);

        this.workflowStepActivityAreas = Object.keys(ActivityArea).filter(x => !(parseInt(x) >= -1));
        this.populateWorkflowStepComboFromEnum(this.workflowStepActivityAreas, this._cmbWorkflowStepActivityArea, this._areaDataSource, ActivityArea);

        if (this.actionPage == TbltWorkflowStepGes.ACTION_PAGE_EDIT) {
            let workflowStepForEdit: WorkflowStep = JSON.parse(sessionStorage[WorkflowTreeNodeType.Step]);
            this._txtWorkflowStepName.set_value(workflowStepForEdit.Name);
            this._cmbWorkflowStepActivityAction.set_text(workflowStepForEdit.ActivityOperation.Action.toString());
            this._cmbWorkflowStepActivityAction.set_value(ActivityAction[workflowStepForEdit.ActivityOperation.Action]);
            this._cmbWorkflowStepActivityArea.set_text(workflowStepForEdit.ActivityOperation.Area.toString());
            this._cmbWorkflowStepActivityArea.set_value(ActivityArea[workflowStepForEdit.ActivityOperation.Area]);
            this._cmbWorkflowStepActivityType.set_text(workflowStepForEdit.ActivityType.toString());
            this._cmbWorkflowStepActivityType.set_value(ActivityType[workflowStepForEdit.ActivityType])
            this._cmbWorkflowStepAuthorizationType.set_text(workflowStepForEdit.AuthorizationType.toString());
            this._cmbWorkflowStepAuthorizationType.set_value(WorkflowAuthorizationType[workflowStepForEdit.AuthorizationType]);
        }
    }

    private populateWorkflowStepComboFromEnum(comboData: string[], cmbBox: Telerik.Web.UI.RadComboBox, dataSource: Telerik.Web.UI.RadClientDataSource, enumName: any) {
        cmbBox.get_items().clear();
        for (let data of comboData) {
            dataSource.add({ Name: data, Value: enumName[data] });
        }
    }
}

export = TbltWorkflowStepGes;