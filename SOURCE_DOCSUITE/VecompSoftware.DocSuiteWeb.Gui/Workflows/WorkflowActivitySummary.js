define(["require", "exports", "App/Services/Workflows/WorkflowActivityService", "App/Helpers/EnumHelper", "App/Helpers/ServiceConfigurationHelper", "../App/Models/Workflows/WorkflowPriorityType", "App/Models/Workflows/WorkflowPropertyHelper", "App/Models/DocumentUnits/ChainType", "App/Models/Workflows/WorkflowStatus", "App/Models/Workflows/ActivityAction", "App/Models/Workflows/AcceptanceStatus", "App/Services/Workflows/WorkflowNotifyService", "App/Models/Workflows/ArgumentType", "App/Models/Environment"], function (require, exports, WorkflowActivityService, EnumHelper, ServiceConfigurationHelper, WorkflowPriorityType, WorkflowPropertyHelper, ChainType, WorkflowStatus, ActivityAction, AcceptanceStatus, WorkflowNotifyService, ArgumentType, Environment) {
    var WorkflowActivitySummary = /** @class */ (function () {
        function WorkflowActivitySummary(serviceConfigurations) {
            var _this = this;
            this.btnDocuments_OnClicked = function (sender, eventArgs) {
                window.location.href = "../Viewers/WorkflowActivityViewer.aspx?Title=" + _this.workflowActivity.WorkflowInstance.WorkflowRepository.Name + "&IdWorkflowActivity=" + _this.workflowActivity.UniqueId + "&IdArchiveChain=" + _this.workflowActivity.IdArchiveChain;
            };
            this.btnRefuse_OnClicked = function (sender, eventArgs) {
                _this.populateAcceptanceModel(AcceptanceStatus.Refused);
            };
            this.btnApprove_OnClicked = function (sender, eventArgs) {
                _this.populateAcceptanceModel(AcceptanceStatus.Accepted);
            };
            this.btnSign_OnClicked = function (sender, eventArgs) {
                var url = "../Comm/SingleSignRest.aspx?IdChain=" + _this.workflowActivity.IdArchiveChain;
                _this.openWindow(url, "singleSign", 750, 300);
            };
            this.btnCompleteActivity_OnClicked = function (sender, eventArgs) {
                if (_this._rtbParereId.get_textBoxValue() == "") {
                    alert("Il campo rispondi e obbligatorio");
                    return;
                }
                _this._loadingPanel.show(_this.mainContainerId);
                var workflowNotify = {};
                workflowNotify.WorkflowName = _this.workflowActivity.Name;
                workflowNotify.WorkflowActivityId = _this.workflowActivity.UniqueId;
                workflowNotify.InstanceId = _this.workflowActivity.WorkflowInstance.UniqueId;
                var dsw_e_ActivityEndMotivation = {};
                dsw_e_ActivityEndMotivation.Name = WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_END_MOTIVATION;
                dsw_e_ActivityEndMotivation.PropertyType = ArgumentType.PropertyString;
                dsw_e_ActivityEndMotivation.ValueString = _this._rtbParereId.get_textBoxValue();
                var requestOpinion = {};
                var txt = $find(_this.rtbParereId);
                requestOpinion.opinion = txt.get_textBoxValue();
                var requertorNameJson = _this.workflowActivity.WorkflowProperties.filter(function (x) { return x.Name === WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_USER; })[0].ValueString;
                requestOpinion.requestor = "[" + requertorNameJson + "]";
                var keys = Object.keys(sessionStorage);
                var documentKey;
                var documentFileName;
                var documentStream;
                for (var _i = 0, keys_1 = keys; _i < keys_1.length; _i++) {
                    var i = keys_1[_i];
                    if (i.startsWith(_this.bindDocument)) {
                        documentKey = i;
                        documentFileName = JSON.parse(sessionStorage.getItem(documentKey))[0].FileName;
                        documentStream = JSON.parse(sessionStorage.getItem(documentKey))[0].ContentStream;
                    }
                }
                if (documentFileName) {
                    requestOpinion.document = documentFileName;
                    requestOpinion.b64ContentStream = documentStream;
                }
                else {
                    requestOpinion.document = "";
                    requestOpinion.b64ContentStream = "";
                }
                var documentModel = {};
                documentModel.FileName = requestOpinion.document;
                documentModel.ContentStream = requestOpinion.b64ContentStream;
                var wrappedDocumentModel = {
                    Key: ChainType[ChainType.Miscellanea],
                    Value: documentModel
                };
                var workflowDocumentModel = {};
                workflowDocumentModel.Documents = [];
                workflowDocumentModel.Documents.push(wrappedDocumentModel);
                var encodedDocuments = JSON.stringify(workflowDocumentModel);
                var documentReferenceModel = {};
                documentReferenceModel.ReferenceType = Environment.Document;
                documentReferenceModel.ReferenceModel = encodedDocuments;
                var dsw_e_ActivityEndReferenceModel = {};
                dsw_e_ActivityEndReferenceModel.Name = WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_END_REFERENCE_MODEL;
                dsw_e_ActivityEndReferenceModel.PropertyType = ArgumentType.Json;
                dsw_e_ActivityEndReferenceModel.ValueString = JSON.stringify(documentReferenceModel);
                var dsw_p_Accounts = {};
                dsw_p_Accounts.Name = WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS;
                dsw_p_Accounts.PropertyType = ArgumentType.Json;
                dsw_p_Accounts.ValueString = requestOpinion.requestor;
                var dsw_p_Subject = {};
                dsw_p_Subject.Name = WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT;
                dsw_p_Subject.PropertyType = ArgumentType.PropertyString;
                dsw_p_Subject.ValueString = dsw_e_ActivityEndMotivation.ValueString;
                workflowNotify.OutputArguments = {};
                workflowNotify.OutputArguments[WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_END_MOTIVATION] = dsw_e_ActivityEndMotivation;
                workflowNotify.OutputArguments[WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_END_REFERENCE_MODEL] = dsw_e_ActivityEndReferenceModel;
                workflowNotify.OutputArguments[WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS] = dsw_p_Accounts;
                workflowNotify.OutputArguments[WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT] = dsw_p_Subject;
                _this._workflowNotifyService.notifyWorkflow(workflowNotify, function (data) {
                    alert("Attività completata con successo");
                    _this._loadingPanel.hide(_this.mainContainerId);
                    window.location.href = "../User/UserWorkflow.aspx?Type=Comm";
                }, function (error) {
                    _this._loadingPanel.hide(_this.mainContainerId);
                    console.log(error);
                });
            };
            this.btnManageActivity_OnClicked = function (sender, eventArgs) {
                window.location.href = "../Workflows/WorkflowActivityManage.aspx?&IdWorkflowActivity=" + _this.workflowActivity.UniqueId + "&IdChain=" + _this.workflowActivity.IdArchiveChain;
            };
            this._serviceConfigurations = serviceConfigurations;
            this._enumHelper = new EnumHelper();
            $(document).ready(function () {
            });
        }
        WorkflowActivitySummary.prototype.initialize = function () {
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            var serviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowActivity");
            this._service = new WorkflowActivityService(serviceConfiguration);
            var workflowNotifyConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, 'WorkflowNotify');
            this._workflowNotifyService = new WorkflowNotifyService(workflowNotifyConfiguration);
            this._btnDocuments = $find(this.cmdDocumentsId);
            this._btnDocuments.add_clicked(this.btnDocuments_OnClicked);
            this._btnRefuse = $find(this.cmdRefuseId);
            if (this._btnRefuse) {
                this._btnRefuse.add_clicked(this.btnRefuse_OnClicked);
            }
            this._btnApprove = $find(this.cmdApproveId);
            if (this._btnApprove) {
                this._btnApprove.add_clicked(this.btnApprove_OnClicked);
            }
            this._btnSign = $find(this.cmdSignId);
            if (this._btnSign) {
                this._btnSign.add_clicked(this.btnSign_OnClicked);
            }
            this._btnCompleteActivity = $find(this.cmdCompleteActivityId);
            if (this._btnCompleteActivity) {
                this._btnCompleteActivity.add_clicked(this.btnCompleteActivity_OnClicked);
            }
            this._btnManageActivity = $find(this.cmdManageActivityId);
            this._btnManageActivity.add_clicked(this.btnManageActivity_OnClicked);
            this._ddlNameWorkflowId = $find(this.ddlNameWorkflowId);
            this._tblFilterStateId = $("#" + this.tblFilterStateId);
            this._lblActivityDate = $("#" + this.lblActivityDateId);
            this._uscProponenteId = $("#" + this.uscProponenteId);
            this._uscDestinatariId = $("#" + this.uscDestinatariId);
            this._tdpDateId = $find(this.tdpDateId);
            this._rtbNoteId = $find(this.rtbNoteId);
            this._rtbParereId = $find(this.rtbParereId);
            var uniqueId = this.getUrlParams(window.location.href);
            this._treeDocumentsId = $find(this.treeDocumentsId);
            this._treeProponenteId = $find(this.treeProponenteId);
            this._treeDestinatariId = $find(this.treeDestinatariId);
            this._activityDateId = $("#" + this.activityDateId);
            this._tlrDateId = $("#" + this.tlrDateId);
            this.documentSection.hidden = true;
            this._btnCompleteActivity.set_visible(false);
            this._btnApprove.set_visible(false);
            this._btnRefuse.set_visible(false);
            this._btnSign.set_visible(false);
            this.loadData(uniqueId);
        };
        WorkflowActivitySummary.prototype.getUrlParams = function (URL) {
            var vars = URL.split("&");
            var value = vars[2].split("=")[1];
            return value;
        };
        WorkflowActivitySummary.prototype.loadData = function (uniqueId) {
            var _this = this;
            this._loadingPanel.show(this.mainContainerId);
            this._service.getWorkflowActivityById(uniqueId, function (data) {
                if (!data)
                    return;
                _this.workflowActivity = data;
                var approveAction = ActivityAction.ToApprove;
                var signAction = ActivityAction.ToSign;
                var creatAction = ActivityAction.Create;
                var cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                var cmbValue = _this.workflowActivity.WorkflowInstance.WorkflowRepository.Name;
                cmbItem.set_text(cmbValue);
                cmbItem.set_value(_this.workflowActivity.WorkflowInstance.WorkflowRepository.UniqueId);
                _this._ddlNameWorkflowId.get_items().add(cmbItem);
                _this._ddlNameWorkflowId.set_emptyMessage(cmbValue);
                _this._ddlNameWorkflowId.disable();
                _this._btnDocuments.set_enabled(false);
                if (_this.workflowActivity.IdArchiveChain && _this.workflowActivity.IdArchiveChain != "") {
                    _this._btnDocuments.set_enabled(true);
                }
                var isCurrentUser = _this.workflowActivity.WorkflowAuthorizations.filter(function (x) { return x.Account == _this.currentUser; }).length == 1;
                var isManageable = WorkflowStatus[_this.workflowActivity.Status.toString()] == WorkflowStatus.Todo || WorkflowStatus[_this.workflowActivity.Status.toString()] == WorkflowStatus.Progress;
                _this._btnManageActivity.set_visible(isCurrentUser && isManageable && !!_this.workflowActivity.IdArchiveChain);
                var list = document.getElementById("" + _this.tblFilterStateId);
                var inputs = list.getElementsByTagName("input");
                switch (_this.workflowActivity.Priority) {
                    case "Low":
                        WorkflowPriorityType.Normale;
                        inputs[0].checked = true;
                        break;
                    case "Medium":
                        WorkflowPriorityType.Bassa;
                        inputs[1].checked = true;
                        break;
                    case "High":
                        WorkflowPriorityType.Alta;
                        inputs[2].checked = true;
                        break;
                    default:
                        inputs[0].checked = true;
                        break;
                }
                var workflowProponenteJson = _this.workflowActivity.WorkflowProperties.filter(function (x) { return x.Name === WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_USER; })[0];
                var workflowProponente = null;
                if (workflowProponenteJson && workflowProponenteJson.ValueString) {
                    workflowProponente = JSON.parse(workflowProponenteJson.ValueString);
                }
                var node = new Telerik.Web.UI.RadTreeNode();
                if (workflowProponente) {
                    node.set_text(workflowProponente.DisplayName);
                }
                _this._treeProponenteId.get_nodes().getNode(0).get_nodes().add(node);
                _this._treeProponenteId.get_nodes().getNode(0).expand();
                node.set_imageUrl("../Comm/Images/Interop/AdAm.gif");
                node.disable();
                _this._treeProponenteId.commitChanges();
                var workflowDestinatariJson = _this.workflowActivity.WorkflowProperties.filter(function (x) { return x.Name === WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS; })[0];
                var workflowDestinatari = [];
                var workflowDestinatariAngular = [];
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
                _this._treeDestinatariId.get_nodes().getNode(0).get_nodes().add(node);
                _this._treeDestinatariId.get_nodes().getNode(0).expand();
                node.disable();
                node.set_imageUrl("../Comm/Images/Interop/AdAm.gif");
                _this._treeDestinatariId.commitChanges();
                var workflowRepositoryDate = _this.workflowActivity.DueDate;
                var date;
                if (workflowRepositoryDate) {
                    date = new Date(workflowRepositoryDate);
                    _this._tdpDateId.set_selectedDate(date);
                }
                var workflowNoteJson = _this.workflowActivity.WorkflowProperties.filter(function (x) { return x.Name === WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_MOTIVATION; })[0];
                if (workflowNoteJson) {
                    _this._rtbNoteId.set_textBoxValue(workflowNoteJson.ValueString);
                    _this._rtbNoteId.disable();
                }
                var workflowParere = _this.workflowActivity.WorkflowProperties.filter(function (x) { return x.Name === WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_END_MOTIVATION; })[0];
                _this._rtbParereId.set_textBoxValue("");
                if (workflowParere) {
                    _this._rtbParereId.set_textBoxValue(workflowParere.ValueString);
                    _this._lblActivityDate.html(moment(workflowParere.RegistrationDate).format("DD/MM/YYYY"));
                    _this._rtbParereId.disable();
                    _this._btnCompleteActivity.set_enabled(false);
                }
                else {
                    _this._rtbParereId.enable();
                }
                _this._loadingPanel.hide(_this.mainContainerId);
                var workflowAcceptanceJson = _this.workflowActivity.WorkflowProperties.filter(function (x) { return x.Name === WorkflowPropertyHelper.DSW_FIELD_ACCEPTANCE; })[0];
                if (ActivityAction[_this.workflowActivity.ActivityAction.toString()] == approveAction) {
                    _this._activityDateId.hide();
                    if (!workflowAcceptanceJson) {
                        _this._btnApprove.set_visible(true);
                        _this._btnRefuse.set_visible(true);
                    }
                }
                if (ActivityAction[_this.workflowActivity.ActivityAction.toString()] == signAction) {
                    _this._btnSign.set_visible(true);
                    _this._tlrDateId.hide();
                    if (_this._btnDocuments.get_enabled()) {
                        _this._btnSign.set_enabled(true);
                    }
                    else {
                        _this._btnSign.set_enabled(false);
                    }
                }
                if (ActivityAction[_this.workflowActivity.ActivityAction.toString()] == creatAction) {
                    _this.documentSection.hidden = false;
                    _this._btnCompleteActivity.set_visible(true);
                    var enableButtons = _this.workflowActivity.WorkflowAuthorizations.filter(function (x) { return _this.currentUser.toLocaleLowerCase() == x.Account.toLocaleLowerCase(); }).length == 1;
                    if (!enableButtons) {
                        _this._btnCompleteActivity.set_enabled(false);
                        _this._rtbParereId.disable();
                    }
                }
            });
        };
        WorkflowActivitySummary.prototype.populateAcceptanceModel = function (acceptanceStatus) {
            var workflowNotifyModel = {};
            var acceptanceModel = {};
            acceptanceModel.Status = acceptanceStatus;
            acceptanceModel.Owner = this.currentUser;
            acceptanceModel.Executor = this.currentUser;
            workflowNotifyModel.WorkflowName = this.workflowActivity.Name;
            workflowNotifyModel.WorkflowActivityId = this.workflowActivity.UniqueId;
            var txt = $find(this.rtbParereId);
            acceptanceModel.AcceptanceReason = txt.get_textBoxValue();
            var proposerRole;
            this.workflowActivity.WorkflowProperties.forEach(function (item) {
                if (item.Name === WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_ROLE) {
                    proposerRole = item.ValueString;
                    return;
                }
            });
            var proposerProperty;
            if (proposerRole) {
                proposerProperty = JSON.parse(proposerRole);
            }
            else {
                proposerProperty = "";
            }
            acceptanceModel.ProposedRole = proposerProperty;
            acceptanceModel.AcceptanceDate = new Date();
            var propertyToUpdate = {};
            propertyToUpdate.WorkflowActivity = this.workflowActivity;
            this.workflowActivity.WorkflowProperties.forEach(function (item) {
                if (item.Name === WorkflowPropertyHelper.DSW_FIELD_ACCEPTANCE) {
                    item.ValueString = JSON.stringify(acceptanceModel);
                    propertyToUpdate = item;
                    return;
                }
            });
            var _dsw_e_Acceptance = {};
            _dsw_e_Acceptance.Name = WorkflowPropertyHelper.DSW_FIELD_ACCEPTANCE;
            _dsw_e_Acceptance.PropertyType = ArgumentType.PropertyString;
            _dsw_e_Acceptance.ValueString = JSON.stringify(acceptanceModel);
            workflowNotifyModel.OutputArguments = {};
            workflowNotifyModel.OutputArguments[WorkflowPropertyHelper.DSW_FIELD_ACCEPTANCE] = _dsw_e_Acceptance;
            this._workflowNotifyService.notifyWorkflow(workflowNotifyModel, function (data) {
                if (acceptanceStatus == AcceptanceStatus.Accepted) {
                    alert("Attività approvata con successo");
                    window.location.href = "../User/UserWorkflow.aspx?Type=Comm";
                }
                else {
                    alert("Attività rifiutata con successo");
                    window.location.href = "../User/UserWorkflow.aspx?Type=Comm";
                }
            });
        };
        WorkflowActivitySummary.prototype.openWindow = function (url, name, width, height) {
            var manager = $find(this.managerWindowsId);
            var wnd = manager.open(url, name, null);
            wnd.setSize(width, height);
            wnd.set_modal(true);
            wnd.center();
            return false;
        };
        return WorkflowActivitySummary;
    }());
    return WorkflowActivitySummary;
});
//# sourceMappingURL=WorkflowActivitySummary.js.map