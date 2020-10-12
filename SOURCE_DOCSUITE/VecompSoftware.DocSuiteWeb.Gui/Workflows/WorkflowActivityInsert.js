/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "Workflows/WorkflowActivityInsertBase", "App/Models/Workflows/WorkflowDSWEnvironmentType", "App/Models/Workflows/WorkflowPropertyHelper", "UserControl/uscStartWorkflow", "App/Models/Workflows/ArgumentType", "App/Models/Environment", "App/Models/Workflows/WorkflowReferenceType", "App/Models/DocumentUnits/ChainType"], function (require, exports, WorkflowActivityInsertBase, WorkflowDSWEnvironmentType, WorkflowPropertyHelper, UscStartWorkflow, ArgumentType, DSWEnvironment, WorkflowReferenceType, ChainType) {
    var WorkflowActivityInsert = /** @class */ (function (_super) {
        __extends(WorkflowActivityInsert, _super);
        function WorkflowActivityInsert(serviceConfigurations) {
            var _this = _super.call(this, serviceConfigurations) || this;
            _this._btnConfirm_onClick = function () {
                if (!Page_IsValid) {
                    return;
                }
                _this._btnConfirm.set_enabled(false);
                _this._loadingPanel.show(_this.pnlWorkflowActivityInsertId);
                var selectedWorkflowRepository = _this._ddlWorkflowActivity.get_selectedItem();
                _this._workflowStartModel.WorkflowName = selectedWorkflowRepository.get_text();
                _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME] = _this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, ArgumentType.PropertyString, _this.tenantName);
                _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID] = _this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, ArgumentType.PropertyGuid, _this.tenantId);
                //Documents
                _this.addDocuments();
                //Contacts
                _this.addContact();
                //Priority
                _this.rblPriorityVal = $("#".concat(_this.rblPriorityId).concat(" input:checked")).val();
                _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_PRIORITY] = _this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_PRIORITY, ArgumentType.PropertyInt, _this.rblPriorityVal);
                //Date
                var dataScadentaVal = $("#".concat(_this.dataScadentaId)).val();
                _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_DUEDATE] = _this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_DUEDATE, ArgumentType.PropertyDate, dataScadentaVal);
                //Motivation
                _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_MOTIVATION] = _this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_MOTIVATION, ArgumentType.PropertyString, _this._txtNote.get_textBoxValue());
                _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT] = _this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT, ArgumentType.PropertyString, _this._txtNote.get_textBoxValue());
                //Proposer User
                var workflowAccountModel = {
                    AccountName: _this.fullUserName,
                    DisplayName: _this.fullName,
                    EmailAddress: _this.email,
                    Required: false
                };
                _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_USER] = _this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_USER, ArgumentType.Json, JSON.stringify(workflowAccountModel));
                _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_FIELD_PRODUCT_NAME] = _this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_FIELD_PRODUCT_NAME, ArgumentType.PropertyString, "DocSuite");
                _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_FIELD_PRODUCT_VERSION] = _this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_FIELD_PRODUCT_VERSION, ArgumentType.PropertyString, _this.docSuiteVersion);
                //TenantAOO
                _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID] = _this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, ArgumentType.PropertyGuid, _this.idTenantAOO);
                _this.workflowStartService.startWorkflow(_this._workflowStartModel, function (data) {
                    _this._btnConfirm.set_enabled(true);
                    window.location.href = "../User/UserWorkflow.aspx?Type=Comm";
                }, function (exception) {
                    _this._btnConfirm.set_enabled(true);
                    alert("Anomalia nel avvio dell'attività. Contattare l'assistenza");
                    location.reload();
                });
            };
            _this.getUscDocument = function () {
                var workflowDocuments = new Array();
                if (!jQuery.isEmptyObject(_this._uscUploadDocumentRest)) {
                    var source = JSON.parse(_this._uscUploadDocumentRest.getDocument());
                    if (source != null) {
                        for (var _i = 0, source_1 = source; _i < source_1.length; _i++) {
                            var s = source_1[_i];
                            var document_1 = {};
                            document_1.FileName = s.FileName;
                            document_1.ContentStream = s.ContentStream;
                            workflowDocuments.push(document_1);
                        }
                    }
                }
                return workflowDocuments;
            };
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        WorkflowActivityInsert.prototype.initialize = function () {
            sessionStorage.clear(); //clear session of contacts
            _super.prototype.initialize.call(this);
            this._manager = $find(this.radWindowManagerId);
            this._ddlWorkflowActivity = $find(this.ddlWorkflowActivityId);
            this._btnConfirm = $find(this.btnConfirmId);
            this._btnConfirm.add_clicking(this._btnConfirm_onClick);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._uscUploadDocumentRest = $("#".concat(this.uscDocumentId)).data();
            this._txtNote = $find(this.txtNoteId);
            this._uscStartWorkflow = new UscStartWorkflow(this._serviceConfigurations);
            this._workflowStartModel = {};
            this._workflowStartModel.Arguments = {};
            this._workflowStartModel.StartParameters = {};
            this.addComboboxValues();
        };
        WorkflowActivityInsert.prototype.addComboboxValues = function () {
            var _this = this;
            this._loadingPanel.show(this.ddlWorkflowActivityId);
            this.workflowRepositoryService.getRepositoryByEnvironment(WorkflowDSWEnvironmentType.Desk, function (data) {
                if (!data)
                    return;
                _this.workflowRepositoriesResult = data;
                for (var _i = 0, _a = _this.workflowRepositoriesResult; _i < _a.length; _i++) {
                    var workflowRepository = _a[_i];
                    var comboItem = new Telerik.Web.UI.DropDownListItem();
                    comboItem.set_text(workflowRepository.Name);
                    comboItem.set_value(workflowRepository.UniqueId);
                    if (_this.workflowRepositoriesResult.length == 1) {
                        _this._ddlWorkflowActivity.get_items().insert(0, comboItem);
                        _this._ddlWorkflowActivity.findItemByValue(comboItem.get_value()).select();
                        _this._ddlWorkflowActivity.set_enabled(false);
                    }
                    else {
                        _this._ddlWorkflowActivity.get_items().add(comboItem);
                    }
                }
                _this._loadingPanel.hide(_this.ddlWorkflowActivityId);
            }, function (exception) {
                _this._loadingPanel.hide(_this.ddlWorkflowActivityId);
            });
        };
        WorkflowActivityInsert.prototype.addDocuments = function () {
            var referenceModel = {};
            referenceModel.ReferenceType = DSWEnvironment.Desk;
            referenceModel.WorkflowReferenceType = WorkflowReferenceType.Json;
            var sessionDocuments = this.getUscDocument();
            var workflowDocumentModel = {};
            workflowDocumentModel.Documents = [];
            //iterate through session documents to assign the key value of type Miscellanea
            for (var _i = 0, sessionDocuments_1 = sessionDocuments; _i < sessionDocuments_1.length; _i++) {
                var doc = sessionDocuments_1[_i];
                var obj = {
                    Key: ChainType[ChainType.Miscellanea],
                    Value: doc
                };
                workflowDocumentModel.Documents.push(obj);
            }
            referenceModel.ReferenceModel = JSON.stringify(workflowDocumentModel);
            var document = JSON.parse(referenceModel.ReferenceModel);
            if (document.Documents.length != 0) {
                this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_REFERENCE_MODEL] = this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_REFERENCE_MODEL, ArgumentType.Json, JSON.stringify(referenceModel));
            }
        };
        WorkflowActivityInsert.prototype.addContact = function () {
            var uscContacts = $("#".concat(this.uscSettoriId)).data();
            var workflowAccounts = [];
            var contactsModel = JSON.parse(uscContacts.getContacts());
            for (var _i = 0, contactsModel_1 = contactsModel; _i < contactsModel_1.length; _i++) {
                var contactModel = contactsModel_1[_i];
                var workflowAccount = {
                    AccountName: contactModel.Code,
                    DisplayName: contactModel.Description,
                    EmailAddress: contactModel.EmailAddress,
                    Required: false
                };
                workflowAccounts.push(workflowAccount);
            }
            this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS] = this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS, ArgumentType.Json, JSON.stringify(workflowAccounts));
            this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_ACTION_PARALLEL_ACTIVITY] = this._uscStartWorkflow.buildWorkflowArgument(WorkflowPropertyHelper.DSW_ACTION_PARALLEL_ACTIVITY, ArgumentType.PropertyBoolean, true);
        };
        return WorkflowActivityInsert;
    }(WorkflowActivityInsertBase));
    return WorkflowActivityInsert;
});
//# sourceMappingURL=WorkflowActivityInsert.js.map