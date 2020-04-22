/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
define(["require", "exports", "App/Services/Workflows/WorkflowActivityService", "App/Helpers/ServiceConfigurationHelper", "App/Models/Workflows/WorkflowPropertyHelper", "App/Models/Fascicles/FascicleModel", "App/DTOs/ExceptionDTO", "App/Models/DocumentUnits/ChainType", "App/Services/Fascicles/FascicleDocumentService", "App/Services/Workflows/WorkflowAuthorizationService", "App/Services/UDS/UDSRepositoryService", "App/Services/Fascicles/FascicleFolderService"], function (require, exports, WorkflowActivityService, ServiceConfigurationHelper, WorkflowPropertyHelper, FascicleModel, ExceptionDTO, ChainType, FascicleDocumentService, WorkflowAuthorizationService, UDSRepositoryService, FascicleFolderService) {
    var WorkflowActivityManage = /** @class */ (function () {
        function WorkflowActivityManage(serviceConfigurations) {
            var _this = this;
            this._loadingDeferreds = [];
            /**
             *------------------------- Events -----------------------------
             */
            this.radioListButtonChanged = function () {
                var checkedChoice = _this.rblDocumentUnit.find('input:checked').val();
                _this.rfvUDSArchives.hide();
                _this.pnlArchives.hide();
                _this.pnlFascicleSelect.hide();
                _this.panelManage.hide();
                _this.panelDocumentUnitSelect.addClass(" t-col-12");
                _this.panelDocumentUnitSelect.removeClass(" t-col-4");
                _this._gridUD.get_masterTableView().showColumn(3);
                ValidatorEnable(document.getElementById(_this.rfvUDSArchivesId), false);
                switch (checkedChoice) {
                    case "Archivi": {
                        _this.panelManage.show();
                        _this.pnlArchives.show();
                        _this.rfvUDSArchives.show();
                        _this.panelDocumentUnitSelect.addClass(" t-col-4");
                        _this.panelDocumentUnitSelect.removeClass(" t-col-12");
                        ValidatorEnable(document.getElementById(_this.rfvUDSArchivesId), true);
                        break;
                    }
                    case "Fascicolo": {
                        _this.pnlFascicleSelect.show();
                        _this.panelManage.show();
                        _this.panelDocumentUnitSelect.addClass(" t-col-4");
                        _this.panelDocumentUnitSelect.removeClass(" t-col-12");
                        _this._gridUD.get_masterTableView().hideColumn(3);
                        break;
                    }
                }
                _this._ajaxManager = $find(_this.ajaxManagerId);
                _this._ajaxManager.ajaxRequestWithTarget(_this.rblDocumentUnitUniqueId, '');
            };
            this.initializeRequest = function (sender, args) {
                if (args.get_postBackElement().id.indexOf(_this.btnConfirmId) != -1) {
                    args.set_cancel(true);
                    sender._form.__EVENTTARGET.value = args.get_postBackElement().id.replace(/\_/g, "$");
                    sender._form.__EVENTARGUMENT.value;
                    sender._form.submit();
                    return;
                }
            };
            this.ddlUDSArchives_selectedIndexChanged = function (sender, args) {
                _this._ajaxManager = $find(_this.ajaxManagerId);
                _this._ajaxManager.ajaxRequestWithTarget(_this.ddlUDSArchivesUniqueId, '');
            };
            this.cmdFascMiscellaneaInsert_Click = function (sender, args) {
                var uscFascicleSearch = $("#" + _this.uscFascicleSearchId).data();
                if (!jQuery.isEmptyObject(uscFascicleSearch)) {
                    var selectedFascicle = uscFascicleSearch.getSelectedFascicle();
                    if (selectedFascicle) {
                        var ajaxModel = {};
                        ajaxModel.Value = new Array();
                        ajaxModel.Value.push(selectedFascicle.UniqueId);
                        ajaxModel.ActionName = WorkflowActivityManage.INSERT_MISCELLANEA;
                        $find(_this.ajaxManagerId).ajaxRequest(JSON.stringify(ajaxModel));
                    }
                    else {
                        _this._btnConfirm.enableAfterSingleClick();
                        _this.showNotificationException(_this.uscNotificationId, "Nessun fascicolo selezionato");
                    }
                }
            };
            this._serviceConfigurations = serviceConfigurations;
        }
        Object.defineProperty(WorkflowActivityManage.prototype, "pnlFascicleSelect", {
            get: function () {
                return $("#" + this.pnlFascicleSelectId);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(WorkflowActivityManage.prototype, "pnlArchives", {
            get: function () {
                return $("#" + this.pnlUDSID);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(WorkflowActivityManage.prototype, "lblProponente", {
            get: function () {
                return $("#" + this.lblProponenteId);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(WorkflowActivityManage.prototype, "lblDestinatario", {
            get: function () {
                return $("#" + this.lblDestinatarioId);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(WorkflowActivityManage.prototype, "lblRegistrationDate", {
            get: function () {
                return $("#" + this.lblRegistrationDateId);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(WorkflowActivityManage.prototype, "lblSubject", {
            get: function () {
                return $("#" + this.lblSubjectId);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(WorkflowActivityManage.prototype, "rblDocumentUnit", {
            get: function () {
                return $("#" + this.rblDocumentUnitId);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(WorkflowActivityManage.prototype, "rfvUDSArchives", {
            get: function () {
                return $("#" + this.rfvUDSArchivesId);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(WorkflowActivityManage.prototype, "panelDocumentUnitSelect", {
            get: function () {
                return $("#" + this.panelDocumentUnitSelectId);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(WorkflowActivityManage.prototype, "panelManage", {
            get: function () {
                return $("#" + this.panelManageId);
            },
            enumerable: true,
            configurable: true
        });
        /**
         *------------------------- Methods -----------------------------
         */
        WorkflowActivityManage.prototype.initialize = function () {
            var _this = this;
            Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(this.initializeRequest);
            var serviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowActivity");
            this._service = new WorkflowActivityService(serviceConfiguration);
            serviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowAuthorization");
            this._workflowAuthorizationService = new WorkflowAuthorizationService(serviceConfiguration);
            serviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "FascicleDocument");
            this._fascicleDocumentService = new FascicleDocumentService(serviceConfiguration);
            serviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "UDSRepository");
            this._udsRepositoryService = new UDSRepositoryService(serviceConfiguration);
            var fascicleFolderServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "FascicleFolder");
            this._fascicleFolderService = new FascicleFolderService(fascicleFolderServiceConfiguration);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._ddlUDSArchives = $find(this.ddlUDSArchivesId);
            this._ddlUDSArchives.add_selectedIndexChanged(this.ddlUDSArchives_selectedIndexChanged);
            this._btnConfirm = $find(this.btnConfirmId);
            this._gridUD = $find(this.grdUDId);
            this.rblDocumentUnit.find("input:first").prop("checked", true);
            this.rblDocumentUnit.on('change', this.radioListButtonChanged);
            this._loadingPanel.show(this.currentPageId);
            this.checkUserRights()
                .done(function (isValid) {
                if (!isValid) {
                    _this.showNotificationException(_this.uscNotificationId, "Non è possibile gestire l'attività richiesta. Verificare se si dispone di sufficienti autorizzazioni");
                    return;
                }
                _this.initializeArchivePanel()
                    .done(function () { return _this.loadData(_this.uniqueId)
                    .fail(function (exception) { return _this.showNotificationException(_this.uscNotificationId, exception); })
                    .always(function () { return _this._loadingPanel.hide(_this.currentPageId); }); })
                    .fail(function (exception) {
                    _this._loadingPanel.hide(_this.currentPageId);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            })
                .fail(function (exception) {
                _this._loadingPanel.hide(_this.currentPageId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        WorkflowActivityManage.prototype.initializeArchivePanel = function () {
            var _this = this;
            var promise = $.Deferred();
            this.pnlArchives.hide();
            this.rfvUDSArchives.hide();
            ValidatorEnable(document.getElementById(this.rfvUDSArchivesId), false);
            this._udsRepositoryService.getInsertableRepositoriesByTypology(this.currentUser.split("\\")[1], this.currentUser.split("\\")[0], null, false, function (data) {
                if (!data) {
                    return promise.resolve();
                }
                var repositories = data;
                var comboItem;
                for (var _i = 0, repositories_1 = repositories; _i < repositories_1.length; _i++) {
                    var repository = repositories_1[_i];
                    comboItem = new Telerik.Web.UI.RadComboBoxItem();
                    comboItem.set_text(repository.Name);
                    comboItem.set_value(repository.UniqueId);
                    _this._ddlUDSArchives.get_items().add(comboItem);
                }
                _this._ddlUDSArchives.clearSelection();
                if (repositories.length == 1) {
                    _this._ddlUDSArchives.set_selectedIndex(0);
                }
                else {
                    var emptyComboItem = new Telerik.Web.UI.RadComboBoxItem();
                    emptyComboItem.set_text("");
                    emptyComboItem.set_value("");
                    _this._ddlUDSArchives.get_items().insert(0, emptyComboItem);
                }
                promise.resolve();
            }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        WorkflowActivityManage.prototype.checkUserRights = function () {
            var promise = $.Deferred();
            this._workflowAuthorizationService.isUserAuthorized(this.currentUser, this.uniqueId, function (data) {
                if (data == undefined) {
                    return promise.reject("Errore nella ricerca delle autorizzazioni utente");
                }
                promise.resolve(data);
            }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        WorkflowActivityManage.prototype.loadData = function (uniqueId) {
            var _this = this;
            var promise = $.Deferred();
            this._service.getWorkflowActivityById(uniqueId, function (data) {
                if (!data) {
                    return promise.reject("Nessuna attivit\u00E0 di workflow trovata con ID " + uniqueId);
                }
                try {
                    var workflowActivity = data;
                    var workflowProponenteJson = workflowActivity.WorkflowProperties.filter(function (x) { return x.Name === WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_USER; })[0];
                    var workflowProponente = JSON.parse(workflowProponenteJson.ValueString);
                    _this.lblProponente.html(workflowProponente.DisplayName);
                    var workflowNoteJson = workflowActivity.WorkflowProperties.filter(function (x) { return x.Name === WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_MOTIVATION; })[0];
                    if (workflowNoteJson) {
                        _this.lblSubject.html(workflowNoteJson.ValueString);
                    }
                    _this.lblDestinatario.html(_this.currentUser);
                    _this.lblRegistrationDate.html(moment(workflowActivity.RegistrationDate).format("DD/MM/YYYY"));
                    var ajaxModel = {};
                    ajaxModel.Value = new Array();
                    ajaxModel.Value.push(JSON.stringify(workflowActivity.IdArchiveChain));
                    ajaxModel.ActionName = "LoadWorkFlowDocument";
                    _this._loadingDeferreds.push(promise);
                    _this._ajaxManager = $find(_this.ajaxManagerId);
                    _this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
                }
                catch (error) {
                    console.error(JSON.stringify(error));
                    promise.reject("E' avvenuto un errore durante il caricamento delle informazioni dell'attività");
                }
            }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        WorkflowActivityManage.prototype.loadCallback = function (errorMessage) {
            if (errorMessage) {
                this._loadingDeferreds.forEach(function (promise) {
                    var errorDto = new ExceptionDTO();
                    errorDto.statusText = errorMessage;
                    promise.reject(errorDto);
                });
                return;
            }
            this._loadingDeferreds.forEach(function (promise) { return promise.resolve(); });
        };
        WorkflowActivityManage.prototype.showNotificationException = function (uscNotificationId, exception) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                if (exception && exception instanceof ExceptionDTO) {
                    uscNotification.showNotification(exception);
                }
                else {
                    uscNotification.showNotificationMessage(exception);
                }
            }
        };
        WorkflowActivityManage.prototype.hasSelectedFascicle = function () {
            var uscFascicleSearch = $("#" + this.uscFascicleSearchId).data();
            if (!jQuery.isEmptyObject(uscFascicleSearch)) {
                var selectedFascicle = uscFascicleSearch.getSelectedFascicle();
                return selectedFascicle != null;
            }
        };
        WorkflowActivityManage.prototype.confirmCallback = function (idChain, idFascicle, isNewArchiveChain, errorMessage) {
            var _this = this;
            if (errorMessage) {
                this.showNotificationException(this.uscNotificationId, errorMessage);
                this._loadingPanel.hide(this.currentPageId);
                this._btnConfirm = $find(this.btnConfirmId);
                this._btnConfirm.enableAfterSingleClick();
                return;
            }
            if (isNewArchiveChain) {
                var fascicleDocumentModel_1 = {};
                fascicleDocumentModel_1.ChainType = ChainType.Miscellanea;
                fascicleDocumentModel_1.IdArchiveChain = idChain;
                fascicleDocumentModel_1.Fascicle = new FascicleModel();
                fascicleDocumentModel_1.Fascicle.UniqueId = idFascicle;
                this._fascicleFolderService.getDefaultFascicleFolder(idFascicle, function (data) {
                    if (!data) {
                        _this._loadingPanel.hide(_this.currentPageId);
                        _this.showNotificationException(_this.uscNotificationId, "E' avvenuto un errore durante il processo di fascicolazione");
                        return;
                    }
                    fascicleDocumentModel_1.FascicleFolder = data;
                    _this._fascicleDocumentService.insertFascicleDocument(fascicleDocumentModel_1, function (data) { return window.location.href = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=" + idFascicle; }, function (exception) {
                        _this._loadingPanel.hide(_this.currentPageId);
                        _this.showNotificationException(_this.uscNotificationId, exception);
                    });
                }, function (exception) {
                    _this._loadingPanel.hide(_this.currentPageId);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            }
            else {
                window.location.href = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=" + idFascicle;
            }
        };
        WorkflowActivityManage.INSERT_MISCELLANEA = "InsertMiscellanea";
        return WorkflowActivityManage;
    }());
    return WorkflowActivityManage;
});
//# sourceMappingURL=WorkflowActivityManage.js.map