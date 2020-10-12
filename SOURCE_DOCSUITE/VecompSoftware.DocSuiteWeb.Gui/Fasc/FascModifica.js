/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
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
define(["require", "exports", "App/Models/Fascicles/FascicleType", "App/Helpers/ServiceConfigurationHelper", "Fasc/FascBase", "UserControl/uscFascicolo", "App/Models/DocumentUnits/ChainType", "App/Services/Processes/ProcessService", "App/Services/Dossiers/DossierFolderService", "App/Mappers/Dossiers/DossierFolderModelMapper", "App/Models/UpdateActionType", "UserControl/uscSetiContactSel", "UserControl/uscMetadataRepositorySel", "App/Models/Environment", "App/Services/Commons/RoleService", "App/Helpers/PageClassHelper"], function (require, exports, FascicleType, ServiceConfigurationHelper, FascicleBase, UscFascicolo, ChainType, ProcessService, DossierFolderService, DossierFolderModelMapper, UpdateActionType, uscSetiContactSel, UscMetadataRepositorySel, Environment, RoleService, PageClassHelper) {
    var FascModifica = /** @class */ (function (_super) {
        __extends(FascModifica, _super);
        /**
         * Costruttore
         * @param serviceConfigurations
         */
        function FascModifica(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME)) || this;
            /**
             *------------------------- Events -----------------------------
             */
            /**
             * Evento scatenato al click del pulsante di inserimento
             * @param sender
             * @param args
             */
            _this.btnConferma_OnClick = function (sender, args) {
                args.set_cancel(true);
                if (!Page_IsValid) {
                    return;
                }
                _this._loadingPanel.show(_this.pageContentId);
                _this._btnConfirm.set_enabled(false);
                if (_this.isPageValid()) {
                    var insertsArchiveChain = _this.getInsertsArchiveChain();
                    var ajaxModel = {};
                    ajaxModel.Value = new Array();
                    ajaxModel.ActionName = "Update";
                    ajaxModel.Value = new Array();
                    ajaxModel.Value.push(insertsArchiveChain);
                    _this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
                    return;
                }
                _this._loadingPanel.hide(_this.pageContentId);
                _this._btnConfirm.set_enabled(true);
            };
            _this.rtvProcessFolders_OnNodeClicked = function (sender, args) {
                _this._selectedDossierFolderId = args.get_node().get_value();
            };
            _this.ddlProcess_OnClientSelectedIndexChanged = function (sender, args) {
                var selectedProcess = args.get_item();
                if (!selectedProcess || !selectedProcess.get_value()) {
                    _this._rtvProcessFoldersRootNode().get_nodes().clear();
                    return;
                }
                _this.loadProcessDossierFolders(_this._ddlProcess.get_value());
            };
            _this._selectedProcessDossierFolders = [];
            _this._serviceConfigurations = serviceConfigurations;
            return _this;
        }
        FascModifica.prototype._rtvProcessFoldersRootNode = function () {
            return this._rtvProcessFolders.get_nodes().getNode(0);
        };
        /**
         * Initialize
         */
        FascModifica.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            var processConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascModifica.PROCESS_TYPE_NAME);
            this._processService = new ProcessService(processConfiguration);
            var dossierFolderConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascModifica.DOSSIER_FOLDER_TYPE_NAME);
            this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);
            var roleConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascModifica.ROLE_FOLDER_TYPE);
            this._roleService = new RoleService(roleConfiguration);
            this._txtName = $find(this.txtNameId);
            this._txtRack = $find(this.txtRackId);
            this._txtNote = $find(this.txtNoteId);
            this._txtManager = $find(this.txtManagerId);
            this._txtObject = $find(this.txtObjectId);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._btnConfirm = $find(this.btnConfermaId);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._manager = $find(this.radWindowManagerId);
            this._ddlProcess = $find(this.ddlProcessId);
            this._ddlProcess.add_selectedIndexChanged(this.ddlProcess_OnClientSelectedIndexChanged);
            this._rtvProcessFolders = $find(this.rtvProcessFoldersId);
            this._rtvProcessFolders.add_nodeClicked(this.rtvProcessFolders_OnNodeClicked);
            this._rowName = $("#".concat(this.rowNameId));
            this._rowRacks = $("#".concat(this.rowRackId));
            this._rowDynamicMetadata = $("#".concat(this.rowDynamicMetadataId));
            this._btnConfirm.add_clicking(this.btnConferma_OnClick);
            this._btnConfirm.set_enabled(false);
            this._rowDynamicMetadata.hide();
            this._uscSetiContact = $("#" + this.uscSetiContactId).data();
            $("#" + this.chkTransformIntoProcessFascicleId).on("click", function () {
                $("#" + _this.uscContactDivId).toggle();
            });
            this._processPanel = $("#" + this.processPanelId);
            this._processPanel.hide();
            this.initializeFascicle();
            this.initializeDossierFoldersTree();
            $("#".concat(this.uscDynamicMetadataId)).on(UscMetadataRepositorySel.SELECTED_SETI_CONTACT_EVENT, function (sender, args) {
                var uscDynamicMetadataRest = $("#".concat(_this.uscDynamicMetadataId)).data();
                uscDynamicMetadataRest.populateMetadataRepository(args, _this._fascicleModel.MetadataDesigner);
            });
            this.fascicleContacts = [];
            this.registerUscContactRestEventHandlers();
        };
        /**
         *------------------------- Methods -----------------------------
         */
        FascModifica.prototype.selectDefaultDossierFolder = function (defaultNode) {
            var currentNodeParent = defaultNode.get_parent();
            currentNodeParent.set_expanded(true);
            if (!!currentNodeParent.get_value()) {
                this.selectDefaultDossierFolder(currentNodeParent);
            }
        };
        FascModifica.prototype.loadProcessDossierFolders = function (processId, defaultDossierFolderId) {
            var _this = this;
            if (this._rtvProcessFoldersRootNode().get_nodes().get_count() > 0) {
                this._rtvProcessFoldersRootNode().get_nodes().clear();
            }
            this._rtvProcessFoldersRootNode().showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
            this._dossierFolderService.getProcessFolders(null, processId, false, true, function (dossierFolders) {
                var dossierFolderModelMapper = new DossierFolderModelMapper();
                _this._selectedProcessDossierFolders = dossierFolderModelMapper.MapCollection(dossierFolders);
                _this.populateDossierFolderTreeRecursive(_this._selectedProcessDossierFolders, _this._rtvProcessFoldersRootNode());
                if (defaultDossierFolderId) {
                    var defaultNode = _this._rtvProcessFoldersRootNode().get_allNodes().filter(function (node) { return node.get_value() === defaultDossierFolderId; })[0];
                    if (defaultNode) {
                        defaultNode.set_selected(true);
                        _this.selectDefaultDossierFolder(defaultNode);
                    }
                }
                _this._rtvProcessFoldersRootNode().hideLoadingStatus();
            }, function (exception) {
                _this._rtvProcessFoldersRootNode().hideLoadingStatus();
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        FascModifica.prototype.loadProcesses = function (defaultProcessId) {
            var _this = this;
            this._loadingPanel.show(this.ddlProcessId);
            var fascicleCategoryId = this._fascicleModel.Category.EntityShortId;
            this._processService.getAvailableProcesses(null, true, fascicleCategoryId, null, function (processes) {
                var today = new Date();
                processes = processes.filter(function (x) { return new Date(x.StartDate) < today && (x.EndDate === null || new Date(x.EndDate) > today); });
                var processesCbItems = processes.map(function (process) { return _this._createComboboxItem(process.Name, process.UniqueId, FascModifica.PROCESS_IMGURL); });
                var defaultSelectedItem = processes.length === 1 ? processesCbItems[0] : (!defaultProcessId ? undefined : processesCbItems.filter(function (item) { return item.get_value() === defaultProcessId; })[0]);
                _this.populateCombobox(processesCbItems, _this._ddlProcess, defaultSelectedItem);
                _this._loadingPanel.hide(_this.ddlProcessId);
            }, function (exception) {
                _this._loadingPanel.hide(_this.ddlProcessId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        FascModifica.prototype.initializeDossierFoldersTree = function () {
            var rootNode = new Telerik.Web.UI.RadTreeNode();
            rootNode.set_text("Tutti i volumi");
            rootNode.set_expanded(true);
            rootNode.set_checkable(false);
            rootNode.disable();
            this._rtvProcessFolders.get_nodes().add(rootNode);
        };
        FascModifica.prototype.populateDossierFolderTreeRecursive = function (dossierFolders, parentNode) {
            var _this = this;
            dossierFolders.forEach(function (dossierFolder) {
                if (dossierFolder.Status.toString() === "Fascicle") {
                    return;
                }
                var currentNode = new Telerik.Web.UI.RadTreeNode();
                currentNode.set_text(dossierFolder.Name);
                currentNode.set_value(dossierFolder.UniqueId);
                currentNode.set_imageUrl(FascModifica.DOSSIERFOLDER_IMGURL);
                currentNode.set_expandedImageUrl(FascModifica.DOSSIERFOLDER_EXPANDED_IMGURL);
                if (parentNode) {
                    parentNode.get_nodes().add(currentNode);
                }
                else {
                    _this._rtvProcessFolders.get_nodes().add(currentNode);
                }
                if (dossierFolder.DossierFolders.length) {
                    _this.populateDossierFolderTreeRecursive(dossierFolder.DossierFolders, currentNode);
                }
            });
        };
        FascModifica.prototype.initializeFascicle = function () {
            var _this = this;
            this._loadingPanel.show(this.pageContentId);
            this.service.getFascicle(this.currentFascicleId, function (data) {
                if (data == null)
                    return;
                _this._fascicleModel = data;
                _this.checkFascicleRight(_this.currentFascicleId)
                    .done(function (isEditable) {
                    if (!isEditable) {
                        _this._loadingPanel.hide(_this.pageContentId);
                        _this.showNotificationMessage(_this.uscNotificationId, "Fascicolo n. " + _this._fascicleModel.Title + ". Mancano diritti di modifica.");
                        $("#".concat(_this.pageContentId)).hide();
                        return;
                    }
                    _this.bindPageFromModel(_this._fascicleModel);
                    if (_this.metadataRepositoryEnabled) {
                        _this.loadMetadata(_this._fascicleModel.MetadataDesigner, _this._fascicleModel.MetadataValues);
                    }
                    if (_this._fascicleModel && _this._fascicleModel.MetadataDesigner) {
                        var metadata = JSON.parse(_this._fascicleModel.MetadataDesigner);
                        if (metadata && metadata.SETIFieldEnabled) {
                            $("#".concat(_this.uscSetiContactId)).triggerHandler(uscSetiContactSel.SHOW_SETI_CONTACT_BUTTON, metadata.SETIFieldEnabled && _this.setiContactEnabledId);
                        }
                    }
                    var jsonFascicle = JSON.stringify(_this._fascicleModel);
                    var ajaxModel = {};
                    ajaxModel.Value = new Array();
                    ajaxModel.ActionName = "Initialize";
                    ajaxModel.Value = new Array();
                    ajaxModel.Value.push(jsonFascicle);
                    _this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
                })
                    .fail(function (exception) {
                    _this._loadingPanel.hide(_this.pageContentId);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageContentId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        FascModifica.prototype.initializeProcessPanel = function () {
            var _this = this;
            var fascicleDossierFolder = this._fascicleModel.DossierFolders[0];
            if (!fascicleDossierFolder) {
                this.loadProcesses();
                return;
            }
            this._processService.getProcessByDossierFolderId(this._fascicleModel.DossierFolders[0].UniqueId, function (process) {
                if (process) {
                    _this.loadProcesses(process.UniqueId);
                    _this.loadProcessDossierFolders(process.UniqueId, _this._fascicleModel.DossierFolders[0].UniqueId);
                }
                else {
                    _this.loadProcesses();
                }
            }, function (exception) {
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        FascModifica.prototype.populateCombobox = function (comboboxItems, combobox, defaultSelectedItem) {
            comboboxItems.forEach(function (cbItem) {
                combobox.get_items().add(cbItem);
            });
            if (comboboxItems.length !== 1) {
                var emptyComboboxItem = new Telerik.Web.UI.RadComboBoxItem();
                emptyComboboxItem.set_text("");
                emptyComboboxItem.set_value("");
                combobox.get_items().insert(0, emptyComboboxItem);
            }
            if (defaultSelectedItem) {
                combobox.set_selectedItem(defaultSelectedItem);
                combobox.set_text(defaultSelectedItem.get_text());
                combobox.set_value(defaultSelectedItem.get_value());
            }
        };
        FascModifica.prototype.checkFascicleRight = function (idFascicle) {
            var promise = $.Deferred();
            this.service.hasManageableRight(idFascicle, function (data) { return promise.resolve(!!data); }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        FascModifica.prototype.bindPageFromModel = function (fascicle) {
            this._txtObject.set_value(fascicle.FascicleObject);
            this._txtNote.set_value(fascicle.Note);
            this._txtManager.set_value(fascicle.Manager);
            if (!this.isEditPage && !(this.fasciclesPanelVisibilities["FascicleNameVisibility"])) {
                $("#" + this.rowNameId).hide();
            }
            if (this._txtName) {
                this._txtName.set_value(fascicle.Name);
            }
            if (!this.isEditPage && !(this.fasciclesPanelVisibilities["FascicleRacksVisibility"])) {
                $("#" + this.rowRackId).hide();
            }
            this._txtRack.set_value(fascicle.Rack);
            if (fascicle.FascicleType != FascicleType[FascicleType.Legacy]) {
                $("#" + this.rowLegacyManagerId).remove();
            }
            if (fascicle.FascicleType == FascicleType[FascicleType.Activity]) {
                $("#" + this.rowManagerId).hide();
            }
            if (this.processEnabled) {
                this._processPanel.show();
                this.initializeProcessPanel();
            }
            this.setTransformIntoProcessFascicleVisibility(fascicle);
        };
        /**
         * Inizializza lo user control del sommario di fascicolo
         */
        FascModifica.prototype.loadFascicoloSummary = function () {
            var _this = this;
            PageClassHelper.callUserControlFunctionSafe(this.uscFascicoloId)
                .done(function (instance) {
                $("#".concat(_this.uscFascicoloId)).bind(UscFascicolo.DATA_LOADED_EVENT, function (args) {
                    _this._loadingPanel.hide(_this.pageContentId);
                    _this._btnConfirm.set_enabled(true);
                });
                instance.loadDataWithoutFolders(_this._fascicleModel);
            });
        };
        FascModifica.prototype._createComboboxItem = function (itemText, itemValue, itemImgUrl) {
            var cmbItem = new Telerik.Web.UI.RadComboBoxItem();
            cmbItem.set_text(itemText);
            cmbItem.set_value(itemValue);
            if (itemImgUrl) {
                cmbItem.set_imageUrl(itemImgUrl);
            }
            return cmbItem;
        };
        /**
         * Callback inizializzazione pagina
         */
        FascModifica.prototype.initializeCallback = function () {
            this.loadFascicoloSummary();
        };
        /**
         * Metodo per la verifica della validità della pagina
         */
        FascModifica.prototype.isPageValid = function () {
            var txtObject = $find(this.txtObjectId);
            if (txtObject.get_maxLength() != 0 && txtObject.get_textBoxValue().length > txtObject.get_maxLength()) {
                this.showNotificationMessage(this.uscNotificationId, "Impossibile salvare.\nIl campo Oggetto ha superato i caratteri disponibili.\n(Caratteri ".concat(txtObject.get_textBoxValue().length.toString(), " Disponibili ", txtObject.get_maxLength().toString()));
                return false;
            }
            return true;
        };
        /**
         * Callback di modifica fascicolo
         * @param contact
         */
        FascModifica.prototype.updateCallback = function (contact) {
            var _this = this;
            if (this._fascicleModel == null) {
                this._loadingPanel.hide(this.pageContentId);
                this._btnConfirm.set_enabled(true);
                this.showWarningMessage(this.uscNotificationId, "Nessun fascicolo definito per la modifica");
                return;
            }
            var txtObject = $find(this.txtObjectId);
            if (this._txtName) {
                this._fascicleModel.Name = this._txtName.get_value();
            }
            this._fascicleModel.Rack = this._txtRack.get_value();
            this._fascicleModel.Note = this._txtNote.get_value();
            this._fascicleModel.FascicleObject = txtObject.get_value();
            if (this._fascicleModel.FascicleType == FascicleType.Legacy) {
                this._fascicleModel.Manager = this._txtManager.get_value();
            }
            if (this._selectedDossierFolderId) {
                var processDossierFolder = {};
                processDossierFolder.UniqueId = this._selectedDossierFolderId;
                this._fascicleModel.DossierFolders = [processDossierFolder];
            }
            if (this._fascicleModel.FascicleType != FascicleType.Activity && contact != null && contact != 0) {
                var contactModel = {};
                contactModel.EntityId = contact;
                this._fascicleModel.Contacts.splice(0, this._fascicleModel.Contacts.length);
                this._fascicleModel.Contacts.push(contactModel);
            }
            if (this.metadataRepositoryEnabled) {
                var uscDynamicMetadataRest = $("#".concat(this.uscDynamicMetadataId)).data();
                if (!jQuery.isEmptyObject(uscDynamicMetadataRest)) {
                    var metadata = JSON.parse(this._fascicleModel.MetadataDesigner);
                    if (metadata) {
                        var setiIntegrationField = metadata.SETIFieldEnabled;
                        var result = uscDynamicMetadataRest.bindModelFormPage(setiIntegrationField);
                        if (!result) {
                            this._btnConfirm = $find(this.btnConfermaId);
                            this._btnConfirm.set_enabled(true);
                            return;
                        }
                        this._fascicleModel.MetadataDesigner = result[0];
                        this._fascicleModel.MetadataValues = result[1];
                    }
                }
            }
            var actionType = UpdateActionType.AssociatedProcessDossierFolderToFascicle;
            if ($("#" + this.chkTransformIntoProcessFascicleId).is(":checked")) {
                if (!this.fascicleContacts || this.fascicleContacts.length === 0) {
                    this.showWarningMessage(this.uscNotificationId, "Responsabile di procedimento non può essere vuoto.");
                    this._loadingPanel.hide(this.pageContentId);
                    this._btnConfirm.set_enabled(true);
                    return;
                }
                this._fascicleModel.Contacts = this.fascicleContacts;
                actionType = UpdateActionType.ChangeFascicleType;
            }
            this.service.updateFascicle(this._fascicleModel, actionType, function (data) {
                window.location.href = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=".concat(_this._fascicleModel.UniqueId);
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageContentId);
                _this._btnConfirm.set_enabled(true);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        /**
         * Recupera il record relativo agli Inserti in FascicleDocuments
         */
        FascModifica.prototype.getInsertsArchiveChain = function () {
            var insertsArchiveChain = "";
            var inserts = $.grep(this._fascicleModel.FascicleDocuments, function (x) { return ChainType[x.ChainType.toString()] == ChainType.Miscellanea; })[0];
            if (inserts != undefined) {
                insertsArchiveChain = inserts.IdArchiveChain;
            }
            return insertsArchiveChain;
        };
        FascModifica.prototype.loadMetadata = function (metadatas, metadataValues) {
            if (metadatas) {
                this._rowDynamicMetadata.show();
                var uscDynamicMetadataRest = $("#".concat(this.uscDynamicMetadataId)).data();
                if (!jQuery.isEmptyObject(uscDynamicMetadataRest)) {
                    uscDynamicMetadataRest.loadPageItems(metadatas, metadataValues);
                }
            }
        };
        FascModifica.prototype.setTransformIntoProcessFascicleVisibility = function (fascicle) {
            var _this = this;
            $("#" + this.rowTransformIntoProcessFascicleId).hide();
            $("#" + this.uscContactDivId).hide();
            this._roleService.hasCategoryFascicleRole(fascicle.Category.EntityShortId, function (userBelongsToCategoryRole) {
                var isQualifiedForTransforming = fascicle.Category.CategoryFascicles
                    .some(function (x) { return x.Environment === Environment.Any; });
                if (fascicle.FascicleType.toString() === FascicleType[FascicleType.Activity] && isQualifiedForTransforming && userBelongsToCategoryRole) {
                    $("#" + _this.rowTransformIntoProcessFascicleId).show();
                }
            }, function (exception) {
                console.log(exception);
            });
        };
        FascModifica.prototype.registerUscContactRestEventHandlers = function () {
            var _this = this;
            PageClassHelper.callUserControlFunctionSafe(this.uscContactId)
                .done(function (instance) {
                instance.registerEventHandler(instance.uscContattiSelRestEvents.ContactDeleted, function (contactIdToDelete) {
                    _this.deleteContactFromModel(contactIdToDelete);
                    return $.Deferred().resolve();
                });
                instance.registerEventHandler(instance.uscContattiSelRestEvents.NewContactsAdded, function (newAddedContact) {
                    _this.addContactToModel(newAddedContact);
                    return $.Deferred().resolve();
                });
            });
        };
        FascModifica.prototype.deleteContactFromModel = function (contactIdToDelete) {
            if (!contactIdToDelete)
                return;
            this.fascicleContacts = this.fascicleContacts.filter(function (x) { return x.EntityId !== contactIdToDelete; });
        };
        FascModifica.prototype.addContactToModel = function (newAddedContact) {
            if (!newAddedContact)
                return;
            this.fascicleContacts.push(newAddedContact);
        };
        FascModifica.PROCESS_TYPE_NAME = "Process";
        FascModifica.DOSSIER_FOLDER_TYPE_NAME = "DossierFolder";
        FascModifica.ROLE_FOLDER_TYPE = "Role";
        FascModifica.DOSSIERFOLDER_IMGURL = "../App_Themes/DocSuite2008/imgset16/folder_closed.png";
        FascModifica.DOSSIERFOLDER_EXPANDED_IMGURL = "../App_Themes/DocSuite2008/imgset16/folder_open.png";
        FascModifica.PROCESS_IMGURL = "../App_Themes/DocSuite2008/imgset16/process.png";
        return FascModifica;
    }(FascicleBase));
    return FascModifica;
});
//# sourceMappingURL=FascModifica.js.map