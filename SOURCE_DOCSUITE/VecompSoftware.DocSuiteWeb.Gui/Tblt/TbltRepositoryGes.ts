import DSWEnvironmentType = require("../App/Models/Workflows/WorkflowDSWEnvironmentType");
import WorkflowRepositoryModel = require("../App/Models/Workflows/WorkflowRepositoryModel");
import WorkflowTreeNodeType = require("../App/Models/Workflows/WorkflowTreeNodeType");
import WorkflowStatus = require("../App/Models/Workflows/WorkflowStatus");
import WorkflowEvaluationProperty = require("../App/Models/Workflows/WorkflowEvaluationProperty");
import WorkflowRoleMappingModel = require("../App/Models/Workflows/WorkflowRoleMappingModel");
import Guid = require('App/Helpers/GuidHelper');
import RoleModel = require("../App/Models/Commons/RoleModel");

class TbltRepositoryGes {
    txtWorkflowNameId: string;
    rntbVersionValueId: string;
    rdpValueDateId: string;
    cmbEnvironmentId: string;
    cmbStatusId: string;
    btnWorkflowSelectorOkId: string;
    btnWorkflowSelectorCancelId: string;
    actionPage: string;
    environmentDataSourceId: string;
    statusDataSourceId: string;
    workflowEnvironmentType: string[];

    private static ACTION_PAGE_EDIT = "Edit";

    private _txtWorkflowName: Telerik.Web.UI.RadTextBox;
    private _rntbVersionValue: Telerik.Web.UI.RadNumericTextBox;
    private _rdpValueDate: Telerik.Web.UI.RadDatePicker;
    private _btnWorkflowSelectorOk: Telerik.Web.UI.RadButton;
    private _btnWorkflowSelectorCancel: Telerik.Web.UI.RadButton;
    private _environmentDataSource: Telerik.Web.UI.RadClientDataSource;
    private _cmbEnvironment: Telerik.Web.UI.RadComboBox;

    constructor(){

    }

    initialize(): void {
        this._txtWorkflowName = <Telerik.Web.UI.RadTextBox>$find(this.txtWorkflowNameId);
        this._rntbVersionValue = <Telerik.Web.UI.RadNumericTextBox>$find(this.rntbVersionValueId);
        this._rdpValueDate = <Telerik.Web.UI.RadDatePicker>$find(this.rdpValueDateId);
        this._environmentDataSource = <Telerik.Web.UI.RadClientDataSource>$find(this.environmentDataSourceId);
        this._cmbEnvironment = <Telerik.Web.UI.RadComboBox>$find(this.cmbEnvironmentId);
        this._btnWorkflowSelectorOk = <Telerik.Web.UI.RadButton>$find(this.btnWorkflowSelectorOkId);
        this._btnWorkflowSelectorOk.add_clicked(this._btnWorkflowSelectorOk_onClick);
        this._btnWorkflowSelectorCancel = <Telerik.Web.UI.RadButton>$find(this.btnWorkflowSelectorCancelId);
        this._btnWorkflowSelectorCancel.add_clicked(this._btnWorkflowSelectorCancel_onClick);

        this.populateWorkflowWindow();
    }

    private populateWorkflowWindow() {
        this._rntbVersionValue.set_value('1');
        this._rdpValueDate.set_selectedDate(new Date());
        this.workflowEnvironmentType = Object.keys(DSWEnvironmentType).filter(x => !(parseInt(x) >= -1));
        this._cmbEnvironment.get_items().clear();
        for (let data of this.workflowEnvironmentType) {
            this._environmentDataSource.add({ Name: data, Value: DSWEnvironmentType[data] });
        }

        if (this.actionPage == TbltRepositoryGes.ACTION_PAGE_EDIT) {
            let workflowRepositoryForEdit: WorkflowRepositoryModel = JSON.parse(sessionStorage[WorkflowTreeNodeType.Workflow]);
            this._txtWorkflowName.set_value(workflowRepositoryForEdit.Name);
            this._rntbVersionValue.set_value(workflowRepositoryForEdit.Version);
            this._cmbEnvironment.set_text(workflowRepositoryForEdit.DSWEnvironment.toString());
            this._cmbEnvironment.set_value(DSWEnvironmentType[workflowRepositoryForEdit.DSWEnvironment]);
            this._rdpValueDate.set_selectedDate(new Date(workflowRepositoryForEdit.ActiveFrom));
            this._rdpValueDate.set_enabled(false);
        }
    }

    private closeWindow(workflowRepository: WorkflowRepositoryModel): void {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        let obj = {
            WorkflowRepository: workflowRepository,
            Action: this.actionPage
        }
        wnd.close(obj);
        this.clearFields();
    }

    private getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return wnd;
    }

    private clearFields() {
        this._txtWorkflowName.clear();
        this._rntbVersionValue.set_value('1');
        this._rdpValueDate.set_selectedDate(new Date());
        this._cmbEnvironment.clearSelection();
    }

    _btnWorkflowSelectorOk_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {
        let workflowName: string = this._txtWorkflowName.get_textBoxValue();
        let workflowVersion: string = this._rntbVersionValue.get_value();
        let workflowActiveFromDate: Date = this._rdpValueDate.get_selectedDate();
        let workflowEnvironment: number = this._cmbEnvironment.get_value() == "" ? null : parseInt(this._cmbEnvironment.get_value());

        let workflowRepository: WorkflowRepositoryModel = {
            UniqueId: Guid.newGuid(),
            Name: workflowName,
            Version: workflowVersion,
            ActiveFrom: workflowActiveFromDate,
            ActiveTo: null,
            DSWEnvironment: workflowEnvironment,
            Status: WorkflowStatus.Todo,
            Json: '{}',
            CustomActivities: null,
            Xaml: null,
            WorkflowEvaluationProperties: new Array<WorkflowEvaluationProperty>(),
            WorkflowRoleMappings: new Array<WorkflowRoleMappingModel>(),
            Roles: new Array<RoleModel>()
        };
        if (this.actionPage === TbltRepositoryGes.ACTION_PAGE_EDIT) {
            let workflowRepositoryForEdit: WorkflowRepositoryModel = JSON.parse(sessionStorage[WorkflowTreeNodeType.Workflow]);

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
        this.closeWindow(workflowRepository);
    }

    _btnWorkflowSelectorCancel_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {
        this.clearFields();
    }
}
export = TbltRepositoryGes;