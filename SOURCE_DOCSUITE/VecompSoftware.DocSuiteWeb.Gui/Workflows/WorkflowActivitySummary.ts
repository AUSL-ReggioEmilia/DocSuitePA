﻿import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import WorkflowActivityService = require('App/Services/Workflows/WorkflowActivityService');
import EnumHelper = require("App/Helpers/EnumHelper");
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import WorkflowActivityModel = require("App/Models/Workflows/WorkflowActivityModel");
import WorkflowPriorityType = require('App/Models/Workflows/WorkflowPriorityType');
import WorkflowPropertyHelper = require('App/Models/Workflows/WorkflowPropertyHelper');
import WorkflowAccountModel = require('App/Models/Workflows/WorkflowAccountModel');
import WorkflowAccountAngularModel = require('App/Models/Workflows/WorkflowAccountAngularModel');
import WorkflowReferenceModel = require('App/Models/Workflows/WorkflowReferenceModel');
import WorkflowDocumentModel = require('App/Models/Commons/WorkflowDocumentModel');
import ChainType = require('App/Models/DocumentUnits/ChainType');
import DocumentModel = require('App/Models/Commons/DocumentModel');
import WorkflowProperty = require('App/Models/Workflows/WorkflowProperty');
import WorkflowStatus = require('App/Models/Workflows/WorkflowStatus');
import ActivityAction = require('App/Models/Workflows/ActivityAction');
import WorkflowAcceptanceModel = require('App/Models/Workflows/WorkflowAcceptanceModel');
import AcceptanceStatus = require('App/Models/Workflows/AcceptanceStatus');
import RoleModel = require('App/Models/Commons/RoleModel');
import WorkflowPropertyModel = require('App/Models/Workflows/WorkflowProperty');
import RequestOpinionModel = require('App/Models/Workflows/RequestOpinionModel');
import WorkflowNotifyService = require('App/Services/Workflows/WorkflowNotifyService');
import WorkflowNotifyModel = require('App/Models/Workflows/WorkflowNotifyModel');
import ArgumentType = require('App/Models/Workflows/ArgumentType');
import Environment = require('App/Models/Environment');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import WorkflowActivityLogService = require('App/Services/Workflows/WorkflowActivityLogService');
import UscUploadDocumentRest = require('UserControl/uscUploadDocumentRest');
import WorkflowArgumentModel = require('App/Models/Workflows/WorkflowArgumentModel');

class WorkflowActivitySummary {
    ddlNameWorkflowId: string;
    tblFilterStateId: string;
    uscProponenteId: string;
    uscDestinatariId: string;
    tdpDateId: string;
    rtbNoteId: string;
    rtbParereId: string;
    lblActivityDateId: string;
    cmdDocumentsId: string;
    cmdManageActivityId: string;
    documentContainerId: string;
    cmdRefuseId: string;
    cmdApproveId: string;
    cmdSignId: string;
    cmdCompleteActivityId: string;
    uscNotificationId: string;

    activityDateId: string;
    tlrDateId: string;
    managerWindowsId: string;
    uscUploadDocumentiId: string;
    uscAddDocumentsRestId: string;
    bindDocument: string;

    mainContainerId: string;
    currentUser: string;
    treeDocumentsId: string;
    treeProponenteId: string;
    treeDestinatariId: string;
    logTreeId: string;

    ajaxLoadingPanelId: string;

    documentSection: HTMLElement;
    addDocumentsSection: HTMLElement;

    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _ddlNameWorkflowId: Telerik.Web.UI.RadComboBox;
    private _tblFilterStateId: JQuery;
    private _uscProponenteId: JQuery;
    private _uscDestinatariId: JQuery;
    private _tdpDateId: Telerik.Web.UI.RadDatePicker;
    private _rtbNoteId: Telerik.Web.UI.RadTextBox;
    private _rtbParereId: Telerik.Web.UI.RadTextBox;
    private _treeDocumentsId: Telerik.Web.UI.RadTreeView;
    private _treeProponenteId: Telerik.Web.UI.RadTreeView;
    private _treeDestinatariId: Telerik.Web.UI.RadTreeView;
    private _lblActivityDate: JQuery;
    private _btnDocuments: Telerik.Web.UI.RadButton;
    private _btnRefuse: Telerik.Web.UI.RadButton;
    private _btnApprove: Telerik.Web.UI.RadButton;
    private _btnSign: Telerik.Web.UI.RadButton;
    private _btnCompleteActivity: Telerik.Web.UI.RadButton;
    private _btnManageActivity: Telerik.Web.UI.RadButton;
    private _logTree: Telerik.Web.UI.RadTreeView;
    private _uscAddDocumentsRest: UscUploadDocumentRest;

    private _activityDateId: JQuery;
    private _tlrDateId: JQuery;

    private _serviceConfigurations: ServiceConfiguration[];
    private _service: WorkflowActivityService;
    private _enumHelper: EnumHelper;
    private _workflowNotifyService: WorkflowNotifyService;
    private _workflowActivityLog: WorkflowActivityLogService;

    private static WORKFLOW_ACTIVITY_EXPAND_PROPERTIES: string[] =
        [
            "WorkflowProperties", "WorkflowAuthorizations", "WorkflowInstance($expand=WorkflowRepository)"
        ];

    workflowActivity: WorkflowActivityModel;
    workflowActivityDoneLog: WorkflowActivityModel;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._enumHelper = new EnumHelper();
        $(document).ready(() => {
        });
    }

    initialize(): void {
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);

        let serviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowActivity");
        this._service = new WorkflowActivityService(serviceConfiguration);

        let workflowActivityLogserviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowActivityLog");
        this._workflowActivityLog = new WorkflowActivityLogService(workflowActivityLogserviceConfiguration);


        let workflowNotifyConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, 'WorkflowNotify');
        this._workflowNotifyService = new WorkflowNotifyService(workflowNotifyConfiguration);

        this._btnDocuments = <Telerik.Web.UI.RadButton>$find(this.cmdDocumentsId);
        this._btnDocuments.add_clicked(this.btnDocuments_OnClicked);

        this._btnRefuse = <Telerik.Web.UI.RadButton>$find(this.cmdRefuseId);
        if (this._btnRefuse) {
            this._btnRefuse.add_clicked(this.btnRefuse_OnClicked);
        }

        this._btnApprove = <Telerik.Web.UI.RadButton>$find(this.cmdApproveId);
        if (this._btnApprove) {
            this._btnApprove.add_clicked(this.btnApprove_OnClicked);
        }

        this._btnSign = <Telerik.Web.UI.RadButton>$find(this.cmdSignId);
        if (this._btnSign) {
            this._btnSign.add_clicked(this.btnSign_OnClicked);
        }

        this._btnCompleteActivity = <Telerik.Web.UI.RadButton>$find(this.cmdCompleteActivityId);
        if (this._btnCompleteActivity) {
            this._btnCompleteActivity.add_clicked(this.btnCompleteActivity_OnClicked);
        }

        this._btnManageActivity = <Telerik.Web.UI.RadButton>$find(this.cmdManageActivityId);
        this._btnManageActivity.add_clicked(this.btnManageActivity_OnClicked);
        this._ddlNameWorkflowId = <Telerik.Web.UI.RadComboBox>$find(this.ddlNameWorkflowId);

        this._tblFilterStateId = <JQuery>$(`#${this.tblFilterStateId}`);
        this._lblActivityDate = <JQuery>$(`#${this.lblActivityDateId}`);
        this._uscProponenteId = <JQuery>$(`#${this.uscProponenteId}`);
        this._uscDestinatariId = <JQuery>$(`#${this.uscDestinatariId}`);
        this._logTree = <Telerik.Web.UI.RadTreeView>$find(this.logTreeId);

        this._tdpDateId = <Telerik.Web.UI.RadDatePicker>$find(this.tdpDateId);

        this._rtbNoteId = <Telerik.Web.UI.RadTextBox>$find(this.rtbNoteId);
        this._rtbParereId = <Telerik.Web.UI.RadTextBox>$find(this.rtbParereId);

        let uniqueId = this.getUrlParams(window.location.href);

        this._treeDocumentsId = <Telerik.Web.UI.RadTreeView>$find(this.treeDocumentsId);
        this._treeProponenteId = <Telerik.Web.UI.RadTreeView>$find(this.treeProponenteId);
        this._treeDestinatariId = <Telerik.Web.UI.RadTreeView>$find(this.treeDestinatariId);

        this._activityDateId = <JQuery>$(`#${this.activityDateId}`);
        this._tlrDateId = <JQuery>$(`#${this.tlrDateId}`);

        this.documentSection.hidden = true;
        this._uscAddDocumentsRest = <UscUploadDocumentRest>$("#".concat(this.uscAddDocumentsRestId)).data();

        this.createNode("Attività gestita in", () => this.loadWorkflowActivitiesDoneLogCount(uniqueId, WorkflowStatus.Done));
        this.loadData(uniqueId, WorkflowStatus.Done);
    }

    private getUrlParams(URL: string): string {
        var vars = URL.split("&");
        var value = vars[2].split("=")[1];

        return value;
    }

    private loadWorkflowActivitiesDoneLogCount(uniqueId: string, workflowStatus: WorkflowStatus) {
        this._workflowActivityLog.countWorkflowActivityByLogType(uniqueId, workflowStatus, (data) => {
            let parentNode = this._logTree.get_nodes().getNode(0);
            parentNode.set_text(`Attività gestita in (${data})`);
        });
    }

    private loadData(uniqueId: string, workflowStatus: WorkflowStatus): void {
        this._loadingPanel.show(this.mainContainerId);
        this._service.getWorkflowActivityById(uniqueId, (data) => {
            if (!data) return;
            this.workflowActivity = data;

            let approveAction: number = ActivityAction.ToApprove;
            let signAction: number = ActivityAction.ToSign;
            let creatAction: number = ActivityAction.Create;

            let cmbItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
            let cmbValue = this.workflowActivity.WorkflowInstance.WorkflowRepository.Name;
            cmbItem.set_text(cmbValue);
            cmbItem.set_value(this.workflowActivity.WorkflowInstance.WorkflowRepository.UniqueId);
            this._ddlNameWorkflowId.get_items().add(cmbItem);
            this._ddlNameWorkflowId.set_emptyMessage(cmbValue);
            this._ddlNameWorkflowId.disable();

            this._btnDocuments.set_enabled(false);
            if (this.workflowActivity.IdArchiveChain && this.workflowActivity.IdArchiveChain != "") {
                this._btnDocuments.set_enabled(true);
            }

            let isCurrentUser = this.workflowActivity.WorkflowAuthorizations.filter(x => x.Account == this.currentUser).length == 1;
            let workflowEndReferenceModelJson = this.workflowActivity.WorkflowProperties.filter(x => x.Name === WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_END_REFERENCE_MODEL)[0];
            let isManageable: boolean = false;
            let workflowEndReferenceModel: WorkflowArgumentModel = null;
            if (workflowEndReferenceModelJson && workflowEndReferenceModelJson.ValueString) {
                workflowEndReferenceModel = JSON.parse(workflowEndReferenceModelJson.ValueString);
                if (workflowEndReferenceModel) {
                    isManageable = true;
                }
            }

            this._btnManageActivity.set_visible(isCurrentUser && isManageable && this._btnDocuments.get_enabled());

            var list = document.getElementById(`${this.tblFilterStateId}`);
            var inputs = list.getElementsByTagName("input");

            switch (this.workflowActivity.Priority) {
                case "Low": WorkflowPriorityType.Normale;
                    inputs[0].checked = true;
                    break;
                case "Medium": WorkflowPriorityType.Bassa;
                    inputs[1].checked = true;
                    break;
                case "High": WorkflowPriorityType.Alta;
                    inputs[2].checked = true;
                    break;
                default:
                    inputs[0].checked = true;
                    break;
            }

            let workflowProponenteJson = this.workflowActivity.WorkflowProperties.filter(x => x.Name === WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_USER)[0];
            let workflowProponente: WorkflowAccountModel = null;
            if (workflowProponenteJson && workflowProponenteJson.ValueString) {
                workflowProponente = JSON.parse(workflowProponenteJson.ValueString);
            }

            var node = new Telerik.Web.UI.RadTreeNode();
            if (workflowProponente) {
                node.set_text(workflowProponente.DisplayName);
            }
            this._treeProponenteId.get_nodes().getNode(0).get_nodes().add(node);
            this._treeProponenteId.get_nodes().getNode(0).expand();
            node.set_imageUrl("../Comm/Images/Interop/AdAm.gif");
            node.disable();
            this._treeProponenteId.commitChanges()

            let workflowDestinatariJson = this.workflowActivity.WorkflowProperties.filter(x => x.Name === WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS)[0];
            let workflowDestinatari: WorkflowAccountModel[] = [];
            let workflowDestinatariAngular: WorkflowAccountAngularModel[] = [];
            if (workflowDestinatariJson && workflowDestinatariJson.ValueString) {
                workflowDestinatari = JSON.parse(workflowDestinatariJson.ValueString);
                workflowDestinatariAngular = JSON.parse(workflowDestinatariJson.ValueString);
            }

            var node = new Telerik.Web.UI.RadTreeNode();
            //workaround to angual MyDocSuite model
            if (workflowDestinatari && workflowDestinatari.length > 0 && workflowDestinatari[0].DisplayName) {
                node.set_text(workflowDestinatari[0].DisplayName);

            }
            if (workflowDestinatariAngular && workflowDestinatariAngular.length > 0 && workflowDestinatariAngular[0].displayName) {
                node.set_text(workflowDestinatariAngular[0].displayName);

            }
            this._treeDestinatariId.get_nodes().getNode(0).get_nodes().add(node);
            this._treeDestinatariId.get_nodes().getNode(0).expand();
            node.disable();
            node.set_imageUrl("../Comm/Images/Interop/AdAm.gif");
            this._treeDestinatariId.commitChanges();

            let workflowRepositoryDate = this.workflowActivity.DueDate;
            let date;
            if (workflowRepositoryDate) {
                date = new Date(workflowRepositoryDate);
                this._tdpDateId.set_selectedDate(date);
            }

            let workflowNoteJson = this.workflowActivity.WorkflowProperties.filter(x => x.Name === WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_MOTIVATION)[0];
            if (workflowNoteJson) {
                this._rtbNoteId.set_textBoxValue(workflowNoteJson.ValueString);
                this._rtbNoteId.disable();
            }

            let workflowParere: WorkflowProperty = this.workflowActivity.WorkflowProperties.filter(x => x.Name === WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_END_MOTIVATION)[0];
            this._rtbParereId.set_textBoxValue("");
            if (workflowParere) {
                this._rtbParereId.set_textBoxValue(workflowParere.ValueString);
                this._lblActivityDate.html(moment(workflowParere.RegistrationDate).format("DD/MM/YYYY"));
                this._rtbParereId.disable();
                this._btnCompleteActivity.set_enabled(WorkflowStatus[this.workflowActivity.Status.toString()] !== WorkflowStatus.Done);
                this.addDocumentsSection.hidden = true;
            }
            else {
                this._rtbParereId.enable();
            }

            this._loadingPanel.hide(this.mainContainerId);

            let workflowAcceptanceJson = this.workflowActivity.WorkflowProperties.filter(x => x.Name === WorkflowPropertyHelper.DSW_FIELD_ACCEPTANCE)[0];
            if (ActivityAction[this.workflowActivity.ActivityAction.toString()] == approveAction) {
                this._activityDateId.hide();
                if (!workflowAcceptanceJson) {
                    $(`#${this.cmdApproveId}`).show();
                    $(`#${this.cmdRefuseId}`).show();
                }
            }

            if (ActivityAction[this.workflowActivity.ActivityAction.toString()] == signAction) {
                $(`#${this.cmdSignId}`).show();
                this._tlrDateId.hide();
                if (this._btnDocuments.get_enabled()) {
                    this._btnSign.set_enabled(true);
                }
                else {
                    this._btnSign.set_enabled(false);
                }
            }

            if (ActivityAction[this.workflowActivity.ActivityAction.toString()] == creatAction) {
                this.documentSection.hidden = false;
                $(`#${this.cmdCompleteActivityId}`).show();

                let enableButtons: boolean = this.workflowActivity.WorkflowAuthorizations.filter(x => this.currentUser.toLocaleLowerCase() == x.Account.toLocaleLowerCase()).length == 1;

                if (!enableButtons) {
                    this._btnCompleteActivity.set_enabled(false);
                    this._rtbParereId.disable();
                }
            }
        }, this._handleException, WorkflowActivitySummary.WORKFLOW_ACTIVITY_EXPAND_PROPERTIES);

        this._service.getWorkflowActivityByLogType(uniqueId, workflowStatus, (data) => {
            if (!data) return;
            this.workflowActivityDoneLog = data;

            for (let wfaDoneLog of this.workflowActivityDoneLog.WorkflowActivityLogs) {
                let wtfDoneLogName: string = `Attività gestita da ${wfaDoneLog.RegistrationUser} in ${moment(wfaDoneLog.RegistrationDate).format("DD/MM/YYYY")} in ${wfaDoneLog.LogDescription}`;
                let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
                node.set_text(wtfDoneLogName);
                this._logTree.get_nodes().getNode(0).set_expanded(true);
                this._logTree.get_nodes().getNode(0).get_nodes().add(node);

                this._logTree.commitChanges();
            }

        });
    }

    private _handleException = (exception: ExceptionDTO): void => {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotification(exception);
        }
    }

    private createNode(nodeText: string, callback: any) {
        let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        node.set_text(nodeText);
        node.set_cssClass("font_node");
        this._logTree.get_nodes().add(node);
        callback();
    }

    private populateAcceptanceModel(acceptanceStatus: AcceptanceStatus) {
        let workflowNotifyModel: WorkflowNotifyModel = <WorkflowNotifyModel>{};

        let acceptanceModel: WorkflowAcceptanceModel = <WorkflowAcceptanceModel>{};
        acceptanceModel.Status = acceptanceStatus;
        acceptanceModel.Owner = this.currentUser;
        acceptanceModel.Executor = this.currentUser;

        workflowNotifyModel.WorkflowName = this.workflowActivity.Name;
        workflowNotifyModel.WorkflowActivityId = this.workflowActivity.UniqueId;

        let txt = <Telerik.Web.UI.RadTextBox>$find(this.rtbParereId);
        acceptanceModel.AcceptanceReason = txt.get_textBoxValue();

        let proposerRole: string;

        this.workflowActivity.WorkflowProperties.forEach(function (item) {
            if (item.Name === WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_ROLE) {
                proposerRole = item.ValueString;
                return;
            }
        });

        let proposerProperty;
        if (proposerRole) {
            proposerProperty = JSON.parse(proposerRole);
        } else {
            proposerProperty = "";
        }

        acceptanceModel.ProposedRole = <RoleModel>proposerProperty;
        acceptanceModel.AcceptanceDate = new Date();

        let propertyToUpdate: WorkflowPropertyModel = <WorkflowPropertyModel>{};
        propertyToUpdate.WorkflowActivity = this.workflowActivity;

        this.workflowActivity.WorkflowProperties.forEach(function (item: WorkflowPropertyModel) {
            if (item.Name === WorkflowPropertyHelper.DSW_FIELD_ACCEPTANCE) {
                item.ValueString = JSON.stringify(acceptanceModel);
                propertyToUpdate = item;
                return;
            }
        });

        let _dsw_e_Acceptance: WorkflowProperty = <WorkflowProperty>{};
        _dsw_e_Acceptance.Name = WorkflowPropertyHelper.DSW_FIELD_ACCEPTANCE;
        _dsw_e_Acceptance.PropertyType = ArgumentType.PropertyString;
        _dsw_e_Acceptance.ValueString = JSON.stringify(acceptanceModel);

        workflowNotifyModel.OutputArguments = {};
        workflowNotifyModel.OutputArguments[WorkflowPropertyHelper.DSW_FIELD_ACCEPTANCE] = _dsw_e_Acceptance;

        this._workflowNotifyService.notifyWorkflow(workflowNotifyModel, (data) => {
            if (acceptanceStatus == AcceptanceStatus.Accepted) {
                alert("Attività approvata con successo");
                window.location.href = "../User/UserWorkflow.aspx?Type=Comm";
            } else {
                alert("Attività rifiutata con successo");
                window.location.href = "../User/UserWorkflow.aspx?Type=Comm";
            }
        });
    }

    btnDocuments_OnClicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        window.location.href = `../Viewers/WorkflowActivityViewer.aspx?Title=${this.workflowActivity.WorkflowInstance.WorkflowRepository.Name}&IdWorkflowActivity=${this.workflowActivity.UniqueId}&IdArchiveChain=${this.workflowActivity.IdArchiveChain}`;
    }

    btnRefuse_OnClicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        this.populateAcceptanceModel(AcceptanceStatus.Refused);
    }

    btnApprove_OnClicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        this.populateAcceptanceModel(AcceptanceStatus.Accepted);
    }

    btnSign_OnClicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        let url: string = `../Comm/SingleSignRest.aspx?IdChain=${this.workflowActivity.IdArchiveChain}`;
        this.openWindow(url, "singleSign", 750, 300);
    }

    private openWindow(url: string, name: string, width: number, height: number): boolean {
        let manager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.managerWindowsId);
        let wnd: Telerik.Web.UI.RadWindow = manager.open(url, name, null);
        wnd.setSize(width, height);
        wnd.set_modal(true);
        wnd.center();
        return false;
    }

    btnCompleteActivity_OnClicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        if (this._rtbParereId.get_textBoxValue() == "") {
            alert("Il campo rispondi e obbligatorio");
            return;
        }
        this._loadingPanel.show(this.mainContainerId);
        const workflowNotify: WorkflowNotifyModel = {} as WorkflowNotifyModel;
        workflowNotify.WorkflowName = this.workflowActivity.Name;
        workflowNotify.WorkflowActivityId = this.workflowActivity.UniqueId;
        workflowNotify.InstanceId = this.workflowActivity.WorkflowInstance.UniqueId;
        workflowNotify.OutputArguments = {};        

        let dsw_e_ActivityEndMotivation: WorkflowPropertyModel = <WorkflowPropertyModel>{};
        dsw_e_ActivityEndMotivation.Name = WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_END_MOTIVATION;
        dsw_e_ActivityEndMotivation.PropertyType = ArgumentType.PropertyString;
        dsw_e_ActivityEndMotivation.ValueString = this._rtbParereId.get_textBoxValue();
        workflowNotify.OutputArguments[WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_END_MOTIVATION] = dsw_e_ActivityEndMotivation;

        let sessionDocuments: Array<DocumentModel> = this.getUscDocument();
        const workflowDocumentModel: WorkflowDocumentModel = <WorkflowDocumentModel>{}
        workflowDocumentModel.Documents = [];

        if (sessionDocuments && sessionDocuments.length > 0) {
            for (let i = 0; i < sessionDocuments.length; i++) {
                const documentModel: DocumentModel = <DocumentModel>{};
                documentModel.FileName = sessionDocuments[i].FileName;
                documentModel.ContentStream = sessionDocuments[i].ContentStream;
                const encodedDocument: any = {
                    Key: ChainType[ChainType.Miscellanea],
                    Value: documentModel
                }
                workflowDocumentModel.Documents.push(encodedDocument);
            }
        }

        const encodedDocuments: string = JSON.stringify(workflowDocumentModel);
        const documentReferenceModel: WorkflowReferenceModel = <WorkflowReferenceModel>{};
        documentReferenceModel.ReferenceType = 3;
        documentReferenceModel.ReferenceModel = encodedDocuments;
        const encodedDocumentReferenceModel: string = JSON.stringify(documentReferenceModel);
        const propActivityEndReferenceModel: WorkflowArgumentModel = {} as WorkflowArgumentModel;
        propActivityEndReferenceModel.Name = WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_END_REFERENCE_MODEL;
        propActivityEndReferenceModel.PropertyType = ArgumentType.Json;
        propActivityEndReferenceModel.ValueString = encodedDocumentReferenceModel;
        workflowNotify.OutputArguments[WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_END_REFERENCE_MODEL] = propActivityEndReferenceModel;

        const requestorUsername: string = this.workflowActivity.WorkflowProperties.filter(x => x.Name === WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_USER)[0].ValueString;
        const workflowAccountModels: Array<WorkflowAccountModel> = [];
        workflowAccountModels.push(JSON.parse(requestorUsername));
        const dswpAccounts = {} as WorkflowArgumentModel;
        dswpAccounts.Name = WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS;
        dswpAccounts.PropertyType = ArgumentType.Json;
        dswpAccounts.ValueString = JSON.stringify(workflowAccountModels);
        workflowNotify.OutputArguments[WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS] = dswpAccounts;

        //DSW_PROPERTY_PROPOSER_USER
        const workflowProposerAccountModel: any = {
            AccountName: this.currentUser,
            DisplayName: this.currentUser,
            Required: false
        };

        const dswpProposerUser = {} as WorkflowArgumentModel;
        dswpProposerUser.Name = WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_USER;
        dswpProposerUser.PropertyType = ArgumentType.Json;
        dswpProposerUser.ValueString = JSON.stringify(workflowProposerAccountModel);
        workflowNotify.OutputArguments[WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_USER] = dswpProposerUser;

        //DSW_FIELD_IDENTITY
        const identityContextModel: any = {
            User: this.currentUser
        };
        const dswpIdentityContext = {} as WorkflowArgumentModel;
        dswpIdentityContext.Name = WorkflowPropertyHelper.DSW_FIELD_IDENTITY;
        dswpIdentityContext.PropertyType = ArgumentType.Json;
        dswpIdentityContext.ValueString = JSON.stringify(identityContextModel);
        workflowNotify.OutputArguments[WorkflowPropertyHelper.DSW_FIELD_IDENTITY] = dswpIdentityContext;


        const propManualComplete: WorkflowArgumentModel = {} as WorkflowArgumentModel;
        propManualComplete.Name = WorkflowPropertyHelper.DSW_ACTION_ACTIVITY_MANUAL_COMPLETE;
        propManualComplete.PropertyType = ArgumentType.PropertyBoolean;
        propManualComplete.ValueBoolean = true;
        workflowNotify.OutputArguments[WorkflowPropertyHelper.DSW_ACTION_ACTIVITY_MANUAL_COMPLETE] = propManualComplete;

        this._workflowNotifyService.notifyWorkflow(workflowNotify,
            (data: any) => {
                this._loadingPanel.hide(this.mainContainerId);
                window.location.href = "../User/UserWorkflow.aspx?Type=Comm";
            }, this._handleException);
    }

    btnManageActivity_OnClicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        window.location.href = `../Workflows/WorkflowActivityManage.aspx?&IdWorkflowActivity=${this.workflowActivity.UniqueId}&IdChain=${this.workflowActivity.IdArchiveChain}`;
    }
    getUscDocument = () => {
        let workflowDocuments: Array<DocumentModel> = new Array<DocumentModel>();
        if (!jQuery.isEmptyObject(this._uscAddDocumentsRest)) {
            let source: any = JSON.parse(this._uscAddDocumentsRest.getDocument());
            if (source != null) {
                for (let s of source) {
                    let document: DocumentModel = <DocumentModel>{};
                    document.FileName = s.FileName;
                    document.ContentStream = s.ContentStream;
                    workflowDocuments.push(document);
                }
            }
        }
        return workflowDocuments;
    }
}

export = WorkflowActivitySummary;