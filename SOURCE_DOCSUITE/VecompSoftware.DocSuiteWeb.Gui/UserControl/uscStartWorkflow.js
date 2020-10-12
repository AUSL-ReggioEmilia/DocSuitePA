/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />
define(["require", "exports", "App/Services/Workflows/WorkflowRepositoryService", "App/Services/Workflows/WorkflowStartService", "App/Helpers/ServiceConfigurationHelper", "App/Models/Environment", "App/Models/Workflows/WorkflowPropertyHelper", "App/DTOs/ExceptionDTO", "App/Models/Workflows/WorkflowReferenceType", "App/Models/Workflows/ArgumentType", "App/Models/Fascicles/FascicleModel", "App/Services/Templates/TemplateCollaborationService", "App/Helpers/EnumHelper", "./uscTenantsSelRest", "App/Helpers/PageClassHelper", "App/Models/Commons/UscRoleRestEventType", "App/Models/Workflows/WorkflowDSWEnvironmentType", "App/Helpers/SessionStorageKeysHelper", "App/Helpers/GuidHelper"], function (require, exports, WorkflowRepositoryService, WorkflowStartService, ServiceConfigurationHelper, DSWEnvironment, WorkflowPropertyHelper, ExceptionDTO, WorkflowReferenceType, ArgumentType, FascicleModel, TemplateCollaborationService, EnumHelper, uscTenantsSelRest, PageClassHelper, UscRoleRestEventType, DSWEnvironmentType, SessionStorageKeysHelper, Guid) {
    var uscStartWorkflow = /** @class */ (function () {
        /**
        * Costruttore
             * @param serviceConfiguration
        */
        function uscStartWorkflow(serviceConfigurations) {
            var _this = this;
            /**
            *------------------------- Events -----------------------------
            */
            /**
            * Evento scatenato al click del pulsante ConfermaModifica
                * @method
                * @param sender
                * @param eventArgs
                * @returns
            */
            this.btnConfirm_OnClick = function (sender, args) {
                _this._loadingPanel.show(_this.contentId);
                _this._btnConfirm.set_enabled(false);
                var selectedWorkflowRepository = _this._rdlWorkflowRepository.get_selectedItem();
                _this.setRecipientValidation();
                _this.setProposerValidation();
                _this.setUscWorkflowFolderValidation();
                var isValid = Page_ClientValidate('');
                var documentTypeRequired = _this.documentHasSelectedType();
                var uscTenantSelRest = $("#" + _this.uscTenantsSelRestId).data();
                if (!isValid || selectedWorkflowRepository == null
                    || String.isNullOrEmpty(selectedWorkflowRepository.get_value())
                    || documentTypeRequired === false
                    || (_this.workflowStartTenantRequired && !uscTenantSelRest.hasValue())) {
                    args.set_cancel(true);
                    if (selectedWorkflowRepository == null && !String.isNullOrEmpty(_this._rdlWorkflowRepository.get_text())) {
                        _this.onError("Selezionare una attività valida");
                    }
                    if (documentTypeRequired === false) {
                        _this.onError("E' necessario specificare il tipo di documento per tutti i documenti");
                    }
                    if (_this.workflowStartTenantRequired && !uscTenantSelRest.hasValue()) {
                        _this.onError("E' necessario selezionare una UO");
                    }
                    _this._loadingPanel.hide(_this.contentId);
                    _this._btnConfirm.set_enabled(true);
                    args.set_cancel(true);
                    return;
                }
                _this.startWorkflow();
            };
            /**
            * Caricamento dei workflow repository disponibili
            */
            this.loadWorkflowRepository = function (sender, args) {
                _this._loadingPanel.show(_this.contentId);
                var env = parseInt(_this.dswEnvironment);
                if (isNaN(env)) {
                    env = DSWEnvironment[_this.dswEnvironment];
                }
                var onlyDocumentWorkflows = false;
                var sessionStorageValue = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_DOCUMENTS_REFERENCE_MODEL);
                if (sessionStorageValue && sessionStorageValue !== "[]") {
                    onlyDocumentWorkflows = true;
                }
                //il false dovrà essere gestito da un checkbox
                _this._workflowRepositoryService.getByEnvironment(env, args.get_text(), false, onlyDocumentWorkflows, _this.showOnlyNoInstanceWorkflows, _this.showOnlyHasIsFascicleClosedRequired, function (data) {
                    _this._rdlWorkflowRepository.clearItems();
                    var repositories = data;
                    _this.addWorkflowRepositories(repositories, _this._rdlWorkflowRepository);
                    _this._loadingPanel.hide(_this.contentId);
                }, function (exception) {
                    _this.showNotificationException(_this.uscNotificationId, exception, "Anomalia nel recupero dei WorkflowRepositories autorizzati all'utente.");
                });
                _this._loadingPanel.hide(_this.contentId);
            };
            this.loadTemplateCollaborations = function () {
                _this._templateCollaborationService.getTemplates(function (data) {
                    var templateCollaborations = data;
                    var item;
                    var defaultTemplate = _this._workflowEvaluationProperties.filter(function (item) {
                        return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_TEMPLATE_COLLABORATION_DEFAULT;
                    });
                    var readonlyTemplate = _this._workflowEvaluationProperties.filter(function (item) {
                        return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_TEMPLATE_COLLABORATION_READONLY;
                    });
                    for (var _i = 0, templateCollaborations_1 = templateCollaborations; _i < templateCollaborations_1.length; _i++) {
                        var templateCollaboration = templateCollaborations_1[_i];
                        item = new Telerik.Web.UI.RadComboBoxItem();
                        item.set_text(templateCollaboration.Name);
                        item.set_value(templateCollaboration.UniqueId.toString());
                        _this._ddlTemplateCollaboration.get_items().add(item);
                        if (defaultTemplate && defaultTemplate.length > 0) {
                            if (templateCollaboration.UniqueId === defaultTemplate[0].ValueGuid) {
                                var selectedItem = _this._ddlTemplateCollaboration.findItemByValue(templateCollaboration.UniqueId);
                                selectedItem.select();
                            }
                        }
                    }
                    _this._ddlTemplateCollaboration.enable();
                    if (readonlyTemplate && readonlyTemplate.length > 0
                        && readonlyTemplate[0].ValueBoolean && _this._ddlTemplateCollaboration.get_selectedItem()) {
                        _this._ddlTemplateCollaboration.disable();
                    }
                });
            };
            this.onRdlWorkflowRepository_SelectedIndexChanged = function () {
                if (_this._rdlWorkflowRepository.get_selectedItem() == null) {
                    return;
                }
                _this._loadingPanel.show(_this.contentId);
                _this.setPageVisibilities();
                _this.clearSessionContacts();
            };
            this.setPageVisibilities = function () {
                _this._loadingPanel.show(_this.contentId);
                _this._repository = null;
                _this._workflowRepositoryService.getById(_this._rdlWorkflowRepository.get_selectedItem().get_value(), function (data) {
                    _this._repository = data;
                    _this.dswEnvironment = DSWEnvironment[_this._repository.DSWEnvironment.toString()] || _this._repository.DSWEnvironment.toString();
                    _this._workflowEvaluationProperties = _this._repository.WorkflowEvaluationProperties;
                    if (_this._workflowEvaluationProperties == null) {
                        _this._workflowEvaluationProperties = [];
                    }
                    var varStr = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL);
                    var fascicle = new FascicleModel();
                    if (varStr) {
                        fascicle = JSON.parse(varStr);
                    }
                    _this.checkWorkflowEvaluationPropertyValues();
                    if (_this.redirectToFascicleSingDocument) {
                        if (fascicle) {
                            varStr = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_DOCUMENTS_REFERENCE_MODEL);
                            if (varStr) {
                                var documents = JSON.parse(varStr);
                                window.location.href = "../Fasc/FascDocumentsInsert.aspx?Type=Fasc&IdFascicle=" + fascicle.UniqueId + "&OnlySignEnabled=true&FilterByArchiveDocumentId=" + documents[0].ArchiveDocumentId;
                            }
                        }
                    }
                    _this.setRecipientProperties();
                    _this.setProposerProperties();
                    if (_this.templateCollaborationRequired) {
                        _this.loadTemplateCollaborations();
                    }
                    _this.setUscWorkflowFolderProperties(fascicle.UniqueId);
                    _this._loadingPanel.hide(_this.contentId);
                }, function (exception) {
                    _this._loadingPanel.hide(_this.contentId);
                    _this.showNotificationException(_this.uscNotificationId, exception, "Anomalia nel recupero della definizione dell'attività.");
                });
            };
            this.checkWorkflowEvaluationPropertyValues = function () {
                var results;
                var proposerRoleRow = $('#'.concat(_this.proposerRoleRowId));
                var proposerContactRow = $('#'.concat(_this.proposerContactRowId));
                var lblProposerContact = $('#'.concat(_this.proposerContactRowLabelId));
                var recipientRoleRow = $('#'.concat(_this.recipientRoleRowId));
                var recipientContactRow = $('#'.concat(_this.recipientContactRowId));
                var lblRecipientContact = $('#'.concat(_this.recipientContactRowLabelId));
                var lrUscWorkflowFolderSelRest = $("#" + _this.lrUscWorkflowFolderSelRestId);
                var lblDossierTitle = $("#" + _this.lblDossierTitleId);
                //motivazione avvia workflow obbligatorietà
                var startMotivationRequired = false;
                results = _this._workflowEvaluationProperties.filter(function (item) {
                    return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_START_MOTIVATION_REQUIRED;
                });
                if (results && results.length > 0) {
                    startMotivationRequired = results[0].ValueBoolean;
                }
                results = _this._workflowEvaluationProperties.filter(function (item) {
                    return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_START_TEMPLATECOLLABORATION_REQUIRED;
                });
                _this.templateCollaborationRequired = false;
                if (results && results.length > 0) {
                    _this.templateCollaborationRequired = results[0].ValueBoolean;
                }
                results = _this._workflowEvaluationProperties.filter(function (item) {
                    return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_DOCUMENT_ORIGINAL_TYPE_SELECTION;
                });
                _this.documentOriginalTypeRequired = false;
                if (results && results.length > 0) {
                    _this.documentOriginalTypeRequired = results[0].ValueBoolean;
                }
                results = _this._workflowEvaluationProperties.filter(function (item) {
                    return item.Name == WorkflowPropertyHelper.DSW_ACTION_REDIRECT_TO_COLLABORATION;
                });
                _this.redirectToCollaboration = false;
                if (results && results.length > 0) {
                    _this.redirectToCollaboration = results[0].ValueBoolean;
                }
                results = _this._workflowEvaluationProperties.filter(function (item) {
                    return item.Name == WorkflowPropertyHelper.DSW_ACTION_REDIRECT_TO_PROTOCOL;
                });
                _this.redirectToProtocol = false;
                if (results && results.length > 0) {
                    _this.redirectToProtocol = results[0].ValueBoolean;
                }
                results = _this._workflowEvaluationProperties.filter(function (item) {
                    return item.Name == WorkflowPropertyHelper.DSW_ACTION_REDIRECT_TO_FASICLE_SIGN_DOCUMENT;
                });
                _this.redirectToFascicleSingDocument = false;
                if (results && results.length > 0) {
                    _this.redirectToFascicleSingDocument = results[0].ValueBoolean;
                }
                results = _this._workflowEvaluationProperties.filter(function (item) {
                    return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_START_DOCUMENT;
                });
                if (results && results.length > 0) {
                    if (results[0].ValueBoolean == true) {
                        $('#'.concat(_this.uploadDocumentId)).show();
                        $('#'.concat(_this.lblUploadDocumentId)).show();
                    }
                }
                var isRecipientContactType = _this.hasCurrentWorkflowRecipientContact();
                var isRecipientRoleType = _this.hasCurrentWorkflowRecipientRole();
                var isProposerContactType = _this.hasCurrentWorkflowProposerContact();
                var isProposerRoleType = _this.hasCurrentWorkflowProposerRole();
                if (proposerRoleRow) {
                    proposerRoleRow.hide();
                }
                if (proposerContactRow) {
                    proposerContactRow.hide();
                }
                if (lblProposerContact) {
                    lblProposerContact.hide();
                }
                if (recipientRoleRow) {
                    recipientRoleRow.hide();
                }
                if (recipientContactRow) {
                    recipientContactRow.hide();
                }
                if (lblRecipientContact) {
                    lblRecipientContact.hide();
                }
                if (lrUscWorkflowFolderSelRest) {
                    lrUscWorkflowFolderSelRest.hide();
                    lblDossierTitle.hide();
                }
                if (isProposerRoleType && proposerRoleRow) {
                    proposerRoleRow.show();
                }
                if (isProposerContactType && proposerContactRow && lblProposerContact) {
                    proposerContactRow.show();
                    lblProposerContact.show();
                }
                if (isRecipientRoleType && recipientRoleRow) {
                    recipientRoleRow.show();
                }
                if (isRecipientContactType && recipientContactRow && lblRecipientContact) {
                    recipientContactRow.show();
                    lblRecipientContact.show();
                }
                $('#'.concat(_this.uploadDocumentId)).hide();
                $('#'.concat(_this.lblUploadDocumentId)).hide();
                $('#'.concat(_this.chainTypeRowId)).hide();
                $('#'.concat(_this.lblChainTypeRowId)).hide();
                $('#'.concat(_this.lblTemplateCollaborationRowId)).hide();
                $('#'.concat(_this.ddlTemplateCollaborationRowId)).hide();
                if (_this.templateCollaborationRequired) {
                    $('#'.concat(_this.lblTemplateCollaborationRowId)).show();
                    $('#'.concat(_this.ddlTemplateCollaborationRowId)).show();
                }
                results = _this._workflowEvaluationProperties.filter(function (item) {
                    return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_DOCUMENT_CHAIN_TYPE_SELECTION;
                });
                _this.chainTypeRequired = false;
                var documents = JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_DOCUMENTS_REFERENCE_MODEL));
                if (results && results.length > 0 && documents) {
                    _this._rgvDocumentMasterTableView.set_dataSource(documents);
                    _this._rgvDocumentMasterTableView.set_virtualItemCount(documents.length);
                    _this._rgvDocumentMasterTableView.dataBind();
                    for (var i = 0; i < documents.length; i++) {
                        _this.createChangeEvent(documents[i].ArchiveDocumentId + "_chainTypes", documents);
                    }
                    $('#'.concat(_this.lblChainTypeRowId)).show();
                    $('#'.concat(_this.chainTypeRowId)).show();
                    _this.chainTypeRequired = true;
                }
                if (_this.documentOriginalTypeRequired) {
                    $('#'.concat(_this.copiaConformeRowId)).show();
                }
                results = _this._workflowEvaluationProperties.filter(function (item) {
                    return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_START_TENANT_REQUIRED;
                });
                _this.workflowStartTenantRequired = false;
                if (results && results.length > 0) {
                    _this.workflowStartTenantRequired = results[0].ValueBoolean;
                }
                if (_this.workflowStartTenantRequired) {
                    $('#'.concat(_this.tenantRowId)).show();
                }
                else {
                    $('#'.concat(_this.tenantRowId)).hide();
                }
            };
            this.setProposerProperties = function () {
                var isProposerContact = _this.hasCurrentWorkflowProposerContact();
                var isProposerRole = _this.hasCurrentWorkflowProposerRole();
                var proposerDisabled = false;
                var startRecipient = _this._workflowEvaluationProperties.filter(function (item) {
                    return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_START_RECIPIENT;
                });
                var proposerReadonly = _this._workflowEvaluationProperties.filter(function (item) {
                    return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_PROPOSER_AUTHORIZATION_READONLY;
                });
                if (proposerReadonly && proposerReadonly.length > 0) {
                    proposerDisabled = proposerReadonly[0].ValueBoolean;
                }
                //richiedenti workflow
                var proposerDefault = _this._workflowEvaluationProperties.filter(function (item) {
                    return (item.Name == WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_DEFAULT);
                });
                var proposerType = uscStartWorkflow.USC_PROPOSER_ROLE;
                var proposerModel = [];
                if (isProposerContact) {
                    proposerType = uscStartWorkflow.USC_PROPOSER_ACCOUNT;
                    var uscProposerDomainUserContacts = $("#".concat(_this.uscProposerContactRestId)).data();
                    uscProposerDomainUserContacts.setImageButtonsVisibility(!proposerDisabled);
                    proposerModel = new Array();
                    var contactsModel = uscProposerDomainUserContacts.getContacts();
                    for (var _i = 0, contactsModel_1 = contactsModel; _i < contactsModel_1.length; _i++) {
                        var contactModel = contactsModel_1[_i];
                        var workflowAccount = {
                            AccountName: contactModel.Code,
                            DisplayName: contactModel.Description,
                            EmailAddress: contactModel.EmailAddress,
                            Required: false
                        };
                        proposerModel.push(workflowAccount);
                    }
                }
                if (isProposerRole) {
                    proposerModel = _this._masterRoles;
                    if (proposerDefault != null && proposerDefault.length > 0) {
                        proposerModel = new Array();
                        var role = _this.buildRoleDefault(proposerDefault[0].ValueString);
                        proposerModel.push(role);
                    }
                }
                var roles = proposerModel;
                _this.sourceProposerRoles = roles;
                _this._uscRoleProposerRest.renderRolesTree(roles);
                _this.registerUscProposerRoleRestEventHandlers();
                if (isProposerContact || isProposerRole) {
                    _this.setToolbarRoleVisibility(!proposerDisabled, _this.uscRoleProposerRestId);
                    _this.setWorkflowRecipient();
                }
                _this._loadingPanel.hide(_this.contentId);
            };
            this.buildContactDefault = function (sourceContact) {
                var account = {};
                var workflowAccount = JSON.parse(sourceContact);
                account.AccountName = workflowAccount.Account.AccountName;
                account.DisplayName = workflowAccount.Account.DisplayName;
                account.EmailAddress = workflowAccount.Account.EmailAddress;
                account.Required = workflowAccount.Account.Required;
                return account;
            };
            this.buildRoleDefault = function (sourceRole) {
                var role = {};
                var workflowRole = JSON.parse(sourceRole);
                role.EntityShortId = workflowRole.Role.IdRole;
                role.IdRole = workflowRole.Role.IdRole;
                role.Name = workflowRole.Role.Name;
                role.TenantId = workflowRole.Role.TenantId;
                role.IdRoleTenant = workflowRole.Role.IdRole;
                role.UniqueId = workflowRole.Role.UniqueId;
                return role;
            };
            this.setWorkflowRecipient = function () {
                _this._loadingPanel.show(_this.contentId);
                //destinatari workflow
                var recipientDisabled = false;
                var recipientReadonly = _this._workflowEvaluationProperties.filter(function (item) {
                    return (item.Name == WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_RECIPIENT_AUTHORIZATION_READONLY);
                });
                if (recipientReadonly && recipientReadonly.length > 0) {
                    recipientDisabled = recipientReadonly[0].ValueBoolean;
                }
                var workflowRecipients = [];
                var ajaxModel = {};
                ajaxModel.Value = new Array();
                ajaxModel.Value.push(JSON.stringify(recipientDisabled));
                ajaxModel.ActionName = uscStartWorkflow.LOAD_EXTERNAL_DATA;
                var isRecipientContact = _this.hasCurrentWorkflowRecipientContact();
                var isRecipientRole = _this.hasCurrentWorkflowRecipientRole();
                if (isRecipientContact) {
                    var uscDomainUserContacts = $("#".concat(_this.uscRecipientContactRestId)).data();
                    uscDomainUserContacts.setImageButtonsVisibility(!recipientDisabled);
                    var accountRecipient = _this._workflowEvaluationProperties.filter(function (item) {
                        return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_RECIPIENT_DEFAULT;
                    });
                    //me ne aspetto uno solo
                    workflowRecipients = new Array();
                    if (accountRecipient != null && accountRecipient[0] != null) {
                        var account = _this.buildContactDefault(accountRecipient[0].ValueString);
                        var contacts = [];
                        var contactModel = {
                            Description: account.DisplayName,
                            Code: account.AccountName.split("\\")[1],
                            EmailAddress: account.EmailAddress,
                        };
                        contacts.push(contactModel);
                        var uscDomainUserContacts_1 = $("#".concat(_this.uscRecipientContactRestId)).data();
                        uscDomainUserContacts_1.createDomainUsersContactsTree(contacts);
                        workflowRecipients.push(account);
                    }
                }
                if (isRecipientRole) {
                    _this.setToolbarRoleVisibility(!recipientDisabled, _this.uscRoleRecipientRestId);
                    var rolesProp = _this._workflowEvaluationProperties.filter(function (item) {
                        return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_RECIPIENT_DEFAULT;
                    });
                    //me ne aspetto uno solo
                    workflowRecipients = new Array();
                    if (rolesProp != null && rolesProp[0] != null) {
                        var role = _this.buildRoleDefault(rolesProp[0].ValueString);
                        workflowRecipients.push(role);
                    }
                }
                var roles = workflowRecipients;
                _this.sourceRecipientRoles = roles;
                _this._uscRoleRecipientRest.renderRolesTree(roles);
                _this.registerUscRecipientRoleRestEventHandlers();
                ajaxModel.Value.push(JSON.stringify(workflowRecipients));
                $find(_this.ajaxManagerId).ajaxRequest(JSON.stringify(ajaxModel));
            };
            /**
            * Callback
            */
            this.updateCallback = function () {
                _this._loadingPanel.hide(_this.contentId);
            };
            this.setRecipientValidation = function () {
                var isRecipientContactType = _this.hasCurrentWorkflowRecipientContact();
                var isRecipientRoleType = _this.hasCurrentWorkflowRecipientRole();
                PageClassHelper.callUserControlFunctionSafe(_this.uscRoleRecipientRestId)
                    .done(function (instance) {
                    instance.forceBehaviourValidationState(isRecipientRoleType);
                    instance.enableValidators(isRecipientRoleType);
                });
            };
            this.setProposerValidation = function () {
                var isProposerContactType = _this.hasCurrentWorkflowProposerContact();
                var isProposerRoleType = _this.hasCurrentWorkflowProposerRole();
                PageClassHelper.callUserControlFunctionSafe(_this.uscRoleProposerRestId)
                    .done(function (instance) {
                    instance.forceBehaviourValidationState(isProposerRoleType);
                    instance.enableValidators(isProposerRoleType);
                });
            };
            this.setUscWorkflowFolderValidation = function () {
                PageClassHelper.callUserControlFunctionSafe(_this.uscWorkflowFolderSelRestId)
                    .done(function (instance) {
                    instance.enableValidator($("#" + _this.uscWorkflowFolderSelRestId).is(":visible"));
                    return;
                });
                var nodeSelectedFromUscWorkflowFolder;
                PageClassHelper.callUserControlFunctionSafe(_this.uscWorkflowFolderSelRestId)
                    .done(function (instance) {
                    nodeSelectedFromUscWorkflowFolder = instance.getSelectedNode();
                    instance.enableTemplateValidator(!nodeSelectedFromUscWorkflowFolder);
                });
                if ($("#" + _this.uscWorkflowFolderSelRestId).is(":visible") && !nodeSelectedFromUscWorkflowFolder) {
                    _this._loadingPanel.hide(_this.contentId);
                    _this._btnConfirm.set_enabled(true);
                    return;
                }
            };
            this.hasCurrentWorkflowPropValueInt = function (propName, intValue) {
                var isProperty = false;
                if (_this._workflowEvaluationProperties) {
                    var property = _this._workflowEvaluationProperties.filter(function (item) {
                        return item.Name === propName;
                    });
                    if (property && property.length > 0 && property[0].ValueInt === intValue) {
                        isProperty = true;
                    }
                }
                return isProperty;
            };
            this.hasCurrentWorkflowPropValueBool = function (propName) {
                var isProperty = false;
                if (_this._workflowEvaluationProperties) {
                    var property = _this._workflowEvaluationProperties.filter(function (item) {
                        return item.Name === propName;
                    });
                    if (property && property.length > 0 && property[0].ValueBoolean === true) {
                        isProperty = true;
                    }
                }
                return isProperty;
            };
            /**
            * Metodo che determina se il workflow ha il destinatario di tipo "contatto"
            */
            this.hasCurrentWorkflowRecipientContact = function () {
                return _this.hasCurrentWorkflowPropValueInt(WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_START_RECIPIENT, 1);
            };
            /**
           * Metodo che determina se il workflow ha il proponente di tipo "contatto"
           */
            this.hasCurrentWorkflowProposerContact = function () {
                return _this.hasCurrentWorkflowPropValueInt(WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_START_PROPOSER, 1);
            };
            /**
            * Metodo che determina se il workflow ha il destinatario di tipo "contatto"
            */
            this.hasCurrentWorkflowRecipientRole = function () {
                return _this.hasCurrentWorkflowPropValueInt(WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_START_RECIPIENT, 0);
            };
            /**
            * Metodo che determina se il workflow ha il destinatario di tipo "contatto"
            */
            this.hasCurrentWorkflowProposerRole = function () {
                return _this.hasCurrentWorkflowPropValueInt(WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_START_PROPOSER, 0);
            };
            /**
            * Metodo che determina se il workflow richiede la costruzione del FascicleBuildModel
            */
            this.hasCurrentWorkflowFascicleBuildModel = function () {
                return _this.hasCurrentWorkflowPropValueBool(WorkflowPropertyHelper.DSW_PROPERTY_BUILD_MODEL_CREATE);
            };
            /**
            * Metodo che completa il modello per avviare un workflow e spedisce il comando di avvio
            */
            this.startWorkflow = function () {
                var isRecipientContact = _this.hasCurrentWorkflowRecipientContact();
                var isRecipientRole = _this.hasCurrentWorkflowRecipientRole();
                var isProposerContact = _this.hasCurrentWorkflowProposerContact();
                var isProposerRole = _this.hasCurrentWorkflowProposerRole();
                _this._workflowStartModel = {};
                _this._workflowStartModel.Arguments = {};
                _this._workflowStartModel.StartParameters = {};
                var currentTenantModelSelection = JSON.parse(sessionStorage.getItem(uscTenantsSelRest.SESSION_TENANT_SELECTION_MODEL));
                var selectedWorkflowRepository = _this._rdlWorkflowRepository.get_selectedItem();
                if (!currentTenantModelSelection || currentTenantModelSelection.length === 0) {
                    _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME] = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, ArgumentType.PropertyString, _this.tenantName);
                    _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID] = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, ArgumentType.PropertyGuid, _this.tenantId);
                    _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID] = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, ArgumentType.PropertyGuid, _this.tenantAOOId);
                }
                else {
                    for (var i = 0; i < currentTenantModelSelection.length; i++) {
                        _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME] = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, ArgumentType.PropertyString, currentTenantModelSelection[i].TenantName);
                        _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID] = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, ArgumentType.PropertyGuid, currentTenantModelSelection[i].IdTenant);
                        _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID] = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, ArgumentType.PropertyGuid, currentTenantModelSelection[i].IdTenantAOO);
                    }
                }
                _this._workflowStartModel.WorkflowName = selectedWorkflowRepository.get_text();
                _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_FIELD_PRODUCT_NAME] = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_FIELD_PRODUCT_NAME, ArgumentType.PropertyString, "DocSuite");
                _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_FIELD_PRODUCT_VERSION] = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_FIELD_PRODUCT_VERSION, ArgumentType.PropertyString, _this.docSuiteVersion);
                _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_FIELD_REFERENCE_UNIQUEID] = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_FIELD_REFERENCE_UNIQUEID, ArgumentType.PropertyGuid, sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_UNIQUEID));
                _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_FIELD_REFERENCE_ENVIRONMENT] = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_FIELD_REFERENCE_ENVIRONMENT, ArgumentType.PropertyInt, _this.dswEnvironment);
                _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_ROLES] = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_ROLES, ArgumentType.Json, sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_RECIPIENT_ROLES));
                //Priority
                var rblPriorityVal = $("input:radio[name='" + _this.rblPriorityId + "']:checked").val();
                _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_PRIORITY] = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_PRIORITY, ArgumentType.PropertyInt, rblPriorityVal);
                //Date
                var dueDateVal = _this._dueDate.val();
                _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_DUEDATE] = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_DUEDATE, ArgumentType.PropertyDate, dueDateVal);
                if (sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_PROPOSER_ROLES)) {
                    var workflowProposerRoleModel = JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_PROPOSER_ROLES));
                    if (workflowProposerRoleModel) {
                        _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_ROLE] = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_ROLE, ArgumentType.Json, JSON.stringify(workflowProposerRoleModel));
                    }
                }
                _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_FOLDER_SELECTED] = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_FOLDER_SELECTED, ArgumentType.Json, sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_SELECTED_FASCICLE_FOLDER_ID));
                if (isProposerRole) {
                    var workflowProposerRole_1 = {};
                    var proposerRoleFromUscRole = new Array();
                    //settore proponente
                    proposerRoleFromUscRole = _this.getUscRoles(_this.sourceProposerRoles);
                    if (proposerRoleFromUscRole.length > 0) {
                        //ce ne sarà solo uno
                        proposerRoleFromUscRole.forEach(function (item) {
                            workflowProposerRole_1.TenantId = item.TenantId;
                            workflowProposerRole_1.IdRole = item.IdRole;
                        });
                        var argumentProposer = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_ROLE, ArgumentType.Json, JSON.stringify(workflowProposerRole_1));
                        _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_ROLE] = argumentProposer;
                    }
                }
                if (isProposerContact) {
                    var uscProposerDomainUserContacts = $("#".concat(_this.uscProposerContactRestId)).data();
                    var contactsModel = uscProposerDomainUserContacts.getContacts();
                    var accountName = "";
                    var displayName = "";
                    var emailAddress = "";
                    if (contactsModel && contactsModel.length > 0) {
                        var proposer = contactsModel[0];
                        accountName = proposer.Code;
                        displayName = proposer.Description;
                        emailAddress = proposer.EmailAddress;
                    }
                    var workflowAccountModel = {
                        AccountName: accountName,
                        DisplayName: displayName,
                        EmailAddress: emailAddress,
                        Required: false
                    };
                    var argumentProposer = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_USER, ArgumentType.Json, JSON.stringify(workflowAccountModel));
                    _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_USER] = argumentProposer;
                }
                if (isRecipientRole) {
                    _this.setStartWorkflowRecipientRoles();
                }
                if (isRecipientContact) {
                    _this.setStartWorkflowRecipientContacts();
                }
                if (_this.hasCurrentWorkflowFascicleBuildModel()) {
                    var fascicleBuildModel = {
                        WorkflowAutoComplete: true,
                        Fascicle: JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL)),
                        WorkflowName: selectedWorkflowRepository.get_text()
                    };
                    fascicleBuildModel.Fascicle.UniqueId = Guid.newGuid();
                    sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_ID, fascicleBuildModel.Fascicle.UniqueId);
                    if (fascicleBuildModel.Fascicle.Category) {
                        fascicleBuildModel.Fascicle.Category.IdCategory = fascicleBuildModel.Fascicle.Category.EntityShortId;
                    }
                    if (fascicleBuildModel.Fascicle.MetadataRepository) {
                        fascicleBuildModel.Fascicle.MetadataRepository.Id = fascicleBuildModel.Fascicle.MetadataRepository.UniqueId;
                    }
                    sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL, JSON.stringify(fascicleBuildModel));
                }
                var workflowReferenceModel = {};
                var env = parseInt(_this.dswEnvironment);
                workflowReferenceModel.ReferenceId = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_ID);
                workflowReferenceModel.ReferenceType = DSWEnvironment[_this.dswEnvironment];
                if (env >= 100) {
                    workflowReferenceModel.ReferenceType = DSWEnvironment.UDS;
                }
                workflowReferenceModel.ReferenceModel = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL);
                workflowReferenceModel.Documents = JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_DOCUMENTS_REFERENCE_MODEL));
                workflowReferenceModel.DocumentUnits = JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_DOCUMENT_UNITS_REFERENCE_MODEL));
                workflowReferenceModel.Title = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_TITLE);
                workflowReferenceModel.WorkflowReferenceType = WorkflowReferenceType.Json;
                var argumentReferenceModel = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, ArgumentType.Json, JSON.stringify(workflowReferenceModel));
                _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL] = argumentReferenceModel;
                //oggetto
                var argumentSubject = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT, ArgumentType.PropertyString, _this._txtObject.get_value());
                _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT] = argumentSubject;
                var startMotivation = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_MOTIVATION, ArgumentType.PropertyString, _this._txtObject.get_value());
                _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_MOTIVATION] = startMotivation;
                //evaluationProperties
                _this._workflowEvaluationProperties.forEach(function (item) {
                    this.Arguments[item.Name] = item;
                }, _this._workflowStartModel);
                if (env >= 100) {
                    var documentUnit = JSON.parse(workflowReferenceModel.ReferenceModel);
                    var idUDS = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_FIELD_UDS_ID, ArgumentType.PropertyGuid, documentUnit.UniqueId);
                    _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_FIELD_UDS_ID] = idUDS;
                    var idUDSRepository = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_FIELD_UDS_REPOSITORY_ID, ArgumentType.PropertyGuid, documentUnit.UDSRepository.UniqueId);
                    _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_FIELD_UDS_REPOSITORY_ID] = idUDSRepository;
                    var udsModel = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_UDS_MODEL);
                    if (udsModel) {
                        var dsw_p_Model = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_MODEL, ArgumentType.Json, udsModel);
                        _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_MODEL] = dsw_p_Model;
                    }
                }
                var documentMetadataValues = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_DOCUMENT_METADATAS);
                if (documentMetadataValues) {
                    var documentMetadatas = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_FIELD_GENERATE_DOCUMENT_METADATAS, ArgumentType.Json, documentMetadataValues);
                    _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_FIELD_GENERATE_DOCUMENT_METADATAS] = documentMetadatas;
                }
                if (_this.templateCollaborationRequired) {
                    var templateCollaboration = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_TEMPLATE_COLLABORATION, ArgumentType.PropertyGuid, _this._ddlTemplateCollaboration.get_selectedItem().get_value());
                    _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TEMPLATE_COLLABORATION] = templateCollaboration;
                }
                if (_this.documentOriginalTypeRequired) {
                    var documentOriginal = $("input:radio[name='" + _this.rdlCCDocumentId + "']:checked").val();
                    _this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_DOCUMENT_ORIGINAL_TYPE_SELECTION] = _this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_DOCUMENT_ORIGINAL_TYPE_SELECTION, ArgumentType.PropertyBoolean, (documentOriginal === "1"));
                }
                if (_this.chainTypeRequired) {
                    var enumHelper_1 = new EnumHelper();
                    workflowReferenceModel.Documents.forEach(function (item) {
                        var val = $("input:radio[name='" + item.ArchiveDocumentId + "_chainTypes']:checked").val();
                        item.ChainType = enumHelper_1.getChainType(val);
                    });
                }
                if (_this.redirectToCollaboration) {
                    sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_WORKFLOW_REFERENCE_MODEL, JSON.stringify(workflowReferenceModel));
                    sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_WORKFLOW_START_MODEL, JSON.stringify(_this._workflowStartModel));
                    var defaultTemplateId = void 0;
                    var defaultTemplate = _this._workflowEvaluationProperties.filter(function (item) {
                        return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_TEMPLATE_COLLABORATION_DEFAULT;
                    });
                    if (defaultTemplate && defaultTemplate.length > 0) {
                        defaultTemplateId = defaultTemplate[0].ValueGuid;
                    }
                    _this._loadingPanel.hide(_this.contentId);
                    var result = {};
                    result.ActionName = "redirect";
                    result.Value = new Array();
                    result.Value.push("../User/UserCollGestione.aspx?Titolo=Inserimento&Action=Add&Title2=Ins.%20alla%20visione/firma&Action2=CI&DefaultTemplateId=" + defaultTemplateId + "&Type=Prot&Action2=CI&FromWorkflowUI=True");
                    _this.closeWindow(result);
                    return;
                }
                if (_this.redirectToProtocol) {
                    sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_WORKFLOW_REFERENCE_MODEL, JSON.stringify(workflowReferenceModel));
                    sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_WORKFLOW_START_MODEL, JSON.stringify(_this._workflowStartModel));
                    _this._loadingPanel.hide(_this.contentId);
                    var result = {};
                    result.ActionName = "redirect";
                    result.Value = new Array();
                    result.Value.push("../Prot/ProtInserimento.aspx?Type=Prot&Action=FromWorkflowUI");
                    _this.closeWindow(result);
                    return;
                }
                _this._workflowStartService.startWorkflow(_this._workflowStartModel, function (data) {
                    _this._loadingPanel.hide(_this.contentId);
                    var result = {};
                    result.ActionName = "Attività avviata correttamente";
                    result.Value = new Array();
                    _this.closeWindow(result);
                }, function (exception) {
                    _this._loadingPanel.hide(_this.contentId);
                    _this.showNotificationException(_this.uscNotificationId, exception, "Anomalia nel avvio dell'attività.");
                    _this._btnConfirm.set_enabled(true);
                });
            };
            this.onError = function (message) {
                alert(message);
            };
            /**
            * Recupera una RadWindow dalla pagina
            */
            this.getRadWindow = function () {
                var wnd = null;
                if (window.radWindow)
                    wnd = window.radWindow;
                else if (window.frameElement.radWindow)
                    wnd = window.frameElement.radWindow;
                return wnd;
            };
            /**
             * Chiude la RadWindow
            */
            this.closeWindow = function (message) {
                var wnd = _this.getRadWindow();
                wnd.close(message);
            };
            this.getUscRoles = function (roles) {
                var workflowRoles = new Array();
                var source = roles;
                if (source != null) {
                    for (var _i = 0, source_1 = source; _i < source_1.length; _i++) {
                        var s = source_1[_i];
                        var role = {};
                        role.IdRole = s.EntityShortId;
                        role.TenantId = s.TenantId;
                        workflowRoles.push(role);
                    }
                }
                return workflowRoles;
            };
            this.getUscDocument = function (uscId) {
                var workflowDocuments = new Array();
                var uscDocuments = $("#".concat(uscId)).data();
                if (!jQuery.isEmptyObject(uscDocuments)) {
                    var source = JSON.parse(uscDocuments.getDocument());
                    if (source != null) {
                        for (var _i = 0, source_2 = source; _i < source_2.length; _i++) {
                            var s = source_2[_i];
                            var document_1 = {};
                            document_1.FileName = s.FileName;
                            document_1.ContentStream = s.ContentStream;
                            workflowDocuments.push(document_1);
                        }
                    }
                }
                return workflowDocuments;
            };
            this.buildWorkflowArgument = function (propertyName, propertyType, propertyValue) {
                var property = {};
                property.Name = propertyName;
                property.PropertyType = propertyType;
                property.ValueInt = null;
                property.ValueDate = null;
                property.ValueDouble = null;
                property.ValueBoolean = null;
                property.ValueGuid = null;
                property.ValueString = null;
                switch (propertyType) {
                    case ArgumentType.PropertyInt:
                        {
                            property.ValueInt = propertyValue;
                            break;
                        }
                    case ArgumentType.PropertyDate:
                        {
                            property.ValueDate = propertyValue;
                            break;
                        }
                    case ArgumentType.PropertyDouble:
                        {
                            property.ValueDouble = propertyValue;
                            break;
                        }
                    case ArgumentType.PropertyBoolean:
                        {
                            property.ValueBoolean = propertyValue;
                            break;
                        }
                    case ArgumentType.PropertyGuid:
                        {
                            property.ValueGuid = propertyValue;
                            break;
                        }
                    case ArgumentType.PropertyString:
                    case ArgumentType.Json:
                        {
                            property.ValueString = propertyValue;
                            break;
                        }
                }
                return property;
            };
            this._serviceConfigurations = serviceConfigurations;
            ;
            $(document).ready(function () {
            });
        }
        uscStartWorkflow.prototype.documentHasSelectedType = function () {
            if (this.chainTypeRequired) {
                var workflowReferenceModel = {};
                workflowReferenceModel.Documents = JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_DOCUMENTS_REFERENCE_MODEL));
                var documentTypeSelected_1;
                workflowReferenceModel.Documents.forEach(function (item) {
                    var val = $("input:radio[name='" + item.ArchiveDocumentId + "_chainTypes']:checked").val();
                    if (val === undefined) {
                        documentTypeSelected_1 = false;
                    }
                });
                return documentTypeSelected_1;
            }
            return true;
        };
        /**
        * ------------------------- Methods -----------------------------
        */
        /**
        * Initialize
        */
        uscStartWorkflow.prototype.initialize = function () {
            this._ajaxManager = $find(this.ajaxManagerId);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._rdlWorkflowRepository = $find(this.rdlWorkflowRepositoryId);
            this._rdlWorkflowRepository.add_itemsRequesting(this.loadWorkflowRepository);
            this._rdlWorkflowRepository.add_selectedIndexChanged(this.onRdlWorkflowRepository_SelectedIndexChanged);
            this._txtObject = $find(this.txtObjectId);
            this._dueDate = $("#".concat(this.dueDateId));
            var workflowRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowRepository");
            this._workflowRepositoryService = new WorkflowRepositoryService(workflowRepositoryConfiguration);
            var workflowStartConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, uscStartWorkflow.WORKFLOWSTART_TYPE_NAME);
            this._workflowStartService = new WorkflowStartService(workflowStartConfiguration);
            this._btnConfirm = $find(this.btnConfirmId);
            this._btnConfirm.add_clicking(this.btnConfirm_OnClick);
            this._ddlTemplateCollaboration = $find(this.ddlTemplateCollaborationId);
            this.clearSessionStorage();
            this._rgvDocumentLists = $find(this.rgvDocumentListsId);
            this._rgvDocumentMasterTableView = this._rgvDocumentLists.get_masterTableView();
            this._rgvDocumentMasterTableView.set_currentPageIndex(0);
            this._rgvDocumentMasterTableView.set_virtualItemCount(0);
            this._uscRoleProposerRest = $("#" + this.uscRoleProposerRestId).data();
            this._uscRoleRecipientRest = $("#" + this.uscRoleRecipientRestId).data();
            this._uscRecipientContactRest = $("#" + this.uscRecipientContactRestId).data();
            this._uscProposerContactRest = $("#" + this.uscProposerContactRestId).data();
            var templateCollaborationConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "TemplateCollaboration");
            this._templateCollaborationService = new TemplateCollaborationService(templateCollaborationConfiguration);
            this._loadingPanel.show(this.contentId);
            var ajaxModel = {};
            ajaxModel.Value = new Array();
            var env = parseInt(this.dswEnvironment);
            if (isNaN(env)) {
                env = DSWEnvironment[this.dswEnvironment];
            }
            var defaultReadOnlyProposer = false;
            switch (env) {
                case DSWEnvironment.Fascicle:
                case DSWEnvironment.Dossier:
                    defaultReadOnlyProposer = true;
                    break;
            }
            ajaxModel.Value.push(JSON.stringify(defaultReadOnlyProposer));
            ajaxModel.Value.push(WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_ROLE);
            this._masterRoles = new Array();
            switch (env) {
                case DSWEnvironment.Fascicle: {
                    var fascicle = JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL));
                    this._txtObject.set_value(fascicle.FascicleObject);
                    var fascicleMasterRoles = $.grep(fascicle.FascicleRoles, function (r) { return r.IsMaster; });
                    for (var _i = 0, fascicleMasterRoles_1 = fascicleMasterRoles; _i < fascicleMasterRoles_1.length; _i++) {
                        var role = fascicleMasterRoles_1[_i];
                        this._masterRoles.push(role.Role);
                    }
                    break;
                }
                case DSWEnvironment.Dossier: {
                    var dossier = JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL));
                    //per ora so che le api ritornano solo quello attivo (che è uno solo)
                    var dossierRoles = dossier.Roles;
                    for (var _a = 0, dossierRoles_1 = dossierRoles; _a < dossierRoles_1.length; _a++) {
                        var role = dossierRoles_1[_a];
                        var roleModel = {};
                        roleModel.EntityShortId = role.EntityShortId;
                        roleModel.IdRole = role.EntityShortId;
                        roleModel.Name = role.Name;
                        roleModel.IdRoleTenant = role.EntityShortId;
                        this._masterRoles.push(roleModel);
                    }
                    break;
                }
            }
            if (env >= 100) {
                var documentUnit = JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL));
                this._txtObject.set_value(documentUnit.Subject);
                var documentUnitRoles = documentUnit.Roles;
                for (var _b = 0, documentUnitRoles_1 = documentUnitRoles; _b < documentUnitRoles_1.length; _b++) {
                    var role = documentUnitRoles_1[_b];
                    var roleModel = {};
                    roleModel.EntityShortId = role.EntityShortId;
                    roleModel.IdRole = role.EntityShortId;
                    roleModel.Name = role.Name;
                    roleModel.IdRoleTenant = role.EntityShortId;
                    this._masterRoles.push(roleModel);
                }
            }
            ajaxModel.ActionName = uscStartWorkflow.LOAD_EXTERNAL_DATA;
            ajaxModel.Value.push(JSON.stringify(this._masterRoles));
            ajaxModel.Value.push(uscStartWorkflow.UPDATE_CALLBACK);
            $find(this.ajaxManagerId).ajaxRequest(JSON.stringify(ajaxModel));
            this.setRecipientValidation();
            this.setProposerValidation();
            this.bindLoaded();
        };
        /**
        * Caricamento dei dati dello user control
        */
        uscStartWorkflow.prototype.loadData = function () {
            this.loadWorkflowRepository(this._rdlWorkflowRepository, new Telerik.Web.UI.RadComboBoxRequestCancelEventArgs());
        };
        uscStartWorkflow.prototype.setToolbarRoleVisibility = function (isReadOnly, uscID) {
            PageClassHelper.callUserControlFunctionSafe(uscID)
                .done(function (instance) {
                instance.setToolbarRoleVisibility(isReadOnly);
            });
        };
        /**
        * Metodo che riempie la RadComboBox dei workflow repository
        */
        uscStartWorkflow.prototype.addWorkflowRepositories = function (repositories, rdlWorkflowRepository) {
            var item;
            for (var _i = 0, repositories_1 = repositories; _i < repositories_1.length; _i++) {
                var repository = repositories_1[_i];
                item = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(repository.Name);
                item.get_attributes().setAttribute(uscStartWorkflow.ENVIRONMENT, repository.DSWEnvironment.toString());
                item.set_value(repository.UniqueId.toString());
                rdlWorkflowRepository.get_items().add(item);
            }
            //set default if list contains only one repository
            if (repositories.length === 1) {
                rdlWorkflowRepository.get_items().getItem(0).select();
            }
            rdlWorkflowRepository.trackChanges();
        };
        /**
        * Scateno l'evento di "Load Completed" del controllo
        */
        uscStartWorkflow.prototype.bindLoaded = function () {
            $("#".concat(this.contentId)).data(this);
            $("#".concat(this.contentId)).triggerHandler(uscStartWorkflow.LOADED_EVENT);
            $("#".concat(this.contentId)).triggerHandler(uscStartWorkflow.DATA_LOADED_EVENT);
        };
        uscStartWorkflow.prototype.clearSessionContacts = function () {
            var uscRecipientDomainUserContacts = $("#".concat(this.uscRecipientContactRestId)).data();
            uscRecipientDomainUserContacts.clearDomainUsersContactsTree();
            var uscProposerDomainUserContacts = $("#".concat(this.uscRecipientContactRestId)).data();
            uscProposerDomainUserContacts.clearDomainUsersContactsTree();
        };
        uscStartWorkflow.prototype.setUscWorkflowFolderProperties = function (fascicleId) {
            this._uscWorkflowFolderSelRest = $("#" + this.uscWorkflowFolderSelRestId).data();
            var worfklowFolderProperties = {};
            var dossierFolderEnable = this._workflowEvaluationProperties.filter(function (item) {
                return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_DOSSIER_FOLDER;
            });
            var dossierType = this._workflowEvaluationProperties.filter(function (item) {
                return item.Name == WorkflowPropertyHelper.DSW_ACTION_DOSSIER_FILTER_TYPE;
            });
            var onlyFolderHasTemplate = this._workflowEvaluationProperties.filter(function (item) {
                return item.Name == WorkflowPropertyHelper.DSW_ACTION_DOSSIERFOLDER_FILTER_HASTEMPLATE;
            });
            var setResponsibleRole = this._workflowEvaluationProperties.filter(function (item) {
                return item.Name == WorkflowPropertyHelper.DSW_ACTION_FASCICLE_SET_RESPONSIBLE_ROLE;
            });
            if (this.dswEnvironment == DSWEnvironmentType.Fascicle.toString()) {
                worfklowFolderProperties.IdFascicle = fascicleId;
            }
            if (dossierFolderEnable.length > 0 && dossierFolderEnable[0].ValueBoolean) {
                worfklowFolderProperties.DossierEnable = true;
            }
            if (dossierType.length > 0 && dossierType[0].ValueInt) {
                worfklowFolderProperties.DossierType = Number(dossierType[0].ValueInt);
            }
            if (setResponsibleRole.length > 0 && setResponsibleRole[0].ValueBoolean) {
                worfklowFolderProperties.SetRecipientRole = true;
            }
            if (onlyFolderHasTemplate.length > 0 && onlyFolderHasTemplate[0].ValueBoolean) {
                worfklowFolderProperties.OnlyFolderHasTemplate = true;
            }
            if (worfklowFolderProperties.DossierEnable && worfklowFolderProperties.IdFascicle) {
                PageClassHelper.callUserControlFunctionSafe(this.uscWorkflowFolderSelRestId)
                    .done(function (instance) {
                    instance.populateTreeByProperties(worfklowFolderProperties);
                });
                $("#" + this.lrUscWorkflowFolderSelRestId).show();
                $("#" + this.lblDossierTitleId).show();
            }
        };
        uscStartWorkflow.prototype.createChangeEvent = function (chainTypeGroupName, documents) {
            var _this = this;
            $("input[type=radio][name=" + chainTypeGroupName + "]").click(function (ev) {
                return _this.allowOnlyOneMainDocumentToBeSelected(ev, documents);
            });
        };
        uscStartWorkflow.prototype.allowOnlyOneMainDocumentToBeSelected = function (ev, documents) {
            var selectedId = ev.target.id;
            var count = 0;
            for (var i = 0; i < documents.length; i++) {
                var mainDocument = $("#" + documents[i].ArchiveDocumentId + "_1");
                if (mainDocument.is(':checked') && selectedId.substring(selectedId.length - 2) === "_1") {
                    count++;
                }
                if (count === 2) {
                    alert("E' possibile selezionare un solo documento principale all volta. Cambiare prima la tipologia del documento principale e poi rifare l'operazione.");
                    count = 0;
                    return ev.preventDefault();
                }
            }
        };
        uscStartWorkflow.prototype.setRecipientProperties = function () {
            var ajaxModel = {};
            ajaxModel.Value = new Array();
            ajaxModel.ActionName = uscStartWorkflow.SET_RECIPIENT_PROPERTIES;
            var multiProp = this._repository.WorkflowEvaluationProperties.filter(function (item) {
                return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_RECIPIENT_MULTIPLE;
            });
            if (multiProp && multiProp.length > 0) {
                ajaxModel.Value.push(JSON.stringify(multiProp[0].ValueBoolean));
            }
            $find(this.ajaxManagerId).ajaxRequest(JSON.stringify(ajaxModel));
        };
        uscStartWorkflow.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
            if (exception && exception instanceof ExceptionDTO) {
                var uscNotification = $("#".concat(uscNotificationId)).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
            }
            else {
                this.showNotificationMessage(uscNotificationId, customMessage);
            }
        };
        uscStartWorkflow.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        uscStartWorkflow.prototype.setStartWorkflowRecipientRoles = function () {
            var workflowAuthorizedRoles = new Array();
            workflowAuthorizedRoles = this.getUscRoles(this.sourceRecipientRoles);
            var argumentRoles = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_ROLES, ArgumentType.Json, JSON.stringify(workflowAuthorizedRoles));
            this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_ROLES] = argumentRoles;
        };
        uscStartWorkflow.prototype.setStartWorkflowRecipientContacts = function () {
            var uscDomainUserContacts = $("#".concat(this.uscRecipientContactRestId)).data();
            var contactsModel = uscDomainUserContacts.getContacts();
            var workflowAccounts = [];
            for (var _i = 0, contactsModel_2 = contactsModel; _i < contactsModel_2.length; _i++) {
                var contactModel = contactsModel_2[_i];
                var workflowAccount = {
                    AccountName: contactModel.Code,
                    DisplayName: contactModel.Description,
                    EmailAddress: contactModel.EmailAddress,
                    Required: false
                };
                workflowAccounts.push(workflowAccount);
            }
            this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS] = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS, ArgumentType.Json, JSON.stringify(workflowAccounts));
        };
        uscStartWorkflow.prototype.clearSessionStorage = function () {
            sessionStorage.removeItem(uscTenantsSelRest.SESSION_TENANT_SELECTION_MODEL);
        };
        uscStartWorkflow.prototype.registerUscProposerRoleRestEventHandlers = function () {
            var _this = this;
            PageClassHelper.callUserControlFunctionSafe(this.uscRoleProposerRestId)
                .done(function (instance) {
                instance.registerEventHandler(UscRoleRestEventType.RoleDeleted, function (roleId) {
                    return $.Deferred().resolve();
                });
                instance.registerEventHandler(UscRoleRestEventType.NewRolesAdded, function (newAddedRoles) {
                    var existedRole;
                    _this.roleInsertId = [newAddedRoles[0].IdRole];
                    _this.sourceProposerRoles = newAddedRoles;
                    return $.Deferred().resolve(existedRole);
                });
            });
        };
        uscStartWorkflow.prototype.registerUscRecipientRoleRestEventHandlers = function () {
            var _this = this;
            PageClassHelper.callUserControlFunctionSafe(this.uscRoleRecipientRestId)
                .done(function (instance) {
                instance.registerEventHandler(UscRoleRestEventType.RoleDeleted, function (roleId) {
                    return $.Deferred().resolve();
                });
                instance.registerEventHandler(UscRoleRestEventType.NewRolesAdded, function (newAddedRoles) {
                    var existedRole;
                    _this.roleInsertId = [newAddedRoles[0].IdRole];
                    _this.sourceRecipientRoles = newAddedRoles;
                    return $.Deferred().resolve(existedRole);
                });
            });
        };
        uscStartWorkflow.LOADED_EVENT = "onLoaded";
        uscStartWorkflow.DATA_LOADED_EVENT = "onDataLoaded";
        uscStartWorkflow.WORKFLOWSTART_TYPE_NAME = "WorkflowStart";
        uscStartWorkflow.LOAD_EXTERNAL_DATA = "LoadExternalData";
        uscStartWorkflow.UPDATE_CALLBACK = "uscStartWorkflow.updateCallback()";
        uscStartWorkflow.SET_WORKFLOW_RECIPIENT = "uscStartWorkflow.setWorkflowRecipient()";
        uscStartWorkflow.SET_WORKFLOW_PROPOSER = "uscStartWorkflow.setWorkflowProposer()";
        uscStartWorkflow.SET_PAGE_VISIBILITIES = "uscStartWorkflow.setPageVisibilities()";
        uscStartWorkflow.USC_PROPOSER_ACCOUNT = "usc_proposer_account";
        uscStartWorkflow.USC_PROPOSER_ROLE = "usc_proposer_role";
        uscStartWorkflow.USC_RECIPIENT_ROLE = "usc_recipient_role";
        uscStartWorkflow.USC_RECIPIENT_ACCOUNT = "usc_recipient_account";
        uscStartWorkflow.ENVIRONMENT = "Environment";
        uscStartWorkflow.GET_RECIPIENT_CONTACT = "Get_Recipient_Contact";
        uscStartWorkflow.SET_RECIPIENT_PROPERTIES = "SetRecipientProperties";
        return uscStartWorkflow;
    }());
    return uscStartWorkflow;
});
//# sourceMappingURL=uscStartWorkflow.js.map