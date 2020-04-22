/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Services/Fascicles/FascicleService", "App/Models/Fascicles/FascicleType", "App/Services/Securities/DomainUserService", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO", "App/Models/Workflows/WorkflowPropertyHelper", "App/Services/Workflows/WorkflowActivityService"], function (require, exports, FascicleService, FascicleType, DomainUserService, ServiceConfigurationHelper, ExceptionDTO, WorkflowPropertyHelper, WorkflowActivityService) {
    var uscFascSummary = /** @class */ (function () {
        /**
         * Costruttore
         * @param webApiConfiguration
         */
        function uscFascSummary(serviceConfigurations) {
            var _this = this;
            /**
             *------------------------- Events -----------------------------
             */
            /**
            * Evento al click del pulsante per la espandere o comprimere il sommario
            */
            this.btnExpandFascInfo_OnClick = function (sender, args) {
                args.set_cancel(true);
                if (_this._isFascInfoOpen) {
                    _this._isFascInfoOpen = false;
                    _this._fascInfoContent.hide();
                    _this._btnExpandFascInfo.removeCssClass("dsw-arrow-down");
                    _this._btnExpandFascInfo.addCssClass("dsw-arrow-up");
                }
                else {
                    _this._isFascInfoOpen = true;
                    _this._fascInfoContent.show();
                    _this._btnExpandFascInfo.removeCssClass("dsw-arrow-up");
                    _this._btnExpandFascInfo.addCssClass("dsw-arrow-down");
                }
            };
            this._service = new FascicleService(ServiceConfigurationHelper.getService(serviceConfigurations, "Fascicle"));
            this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
        }
        Object.defineProperty(uscFascSummary.prototype, "lblContainer", {
            get: function () {
                return $("#" + this.lblContainerId);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(uscFascSummary.prototype, "containerRow", {
            get: function () {
                return $("#" + this.containerRowId);
            },
            enumerable: true,
            configurable: true
        });
        /**
         * Inizializzazione
         */
        uscFascSummary.prototype.initialize = function () {
            this._lblTitle = $("#".concat(this.lblTitleId));
            this._lblFascicleType = $("#".concat(this.lblFascicleTypeId));
            this._lblFascicleObject = $("#".concat(this.lblFascicleObjectId));
            this._lblStartDate = $("#".concat(this.lblStartDateId));
            this._lblEndDate = $("#".concat(this.lblEndDateId));
            this._lblFascicleNote = $("#".concat(this.lblFascicleNoteId));
            this._lblRegistrationUser = $("#".concat(this.lblRegistrationUserId));
            this._lblLastChangedDate = $("#".concat(this.lblLastChangedDateId));
            this._lblLastChangedUser = $("#".concat(this.lblLastChangedUserId));
            this._lblRegistrationDate = $("#".concat(this.lblRegistrationDateId));
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._btnExpandFascInfo = $find(this.btnExpandFascInfoId);
            this._btnExpandFascInfo.addCssClass("dsw-arrow-down");
            this._btnExpandFascInfo.add_clicking(this.btnExpandFascInfo_OnClick);
            this._isFascInfoOpen = true;
            this._fascInfoContent = $("#".concat(this.fascInfoId));
            this._fascInfoContent.show();
            this._lblViewFascicle = $("#".concat(this.lblViewFascicleId));
            var domainUserConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DomainUserModel");
            this._domainUserService = new DomainUserService(domainUserConfiguration);
            var workflowActivityConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, 'WorkflowActivity');
            this._workflowActivityService = new WorkflowActivityService(workflowActivityConfiguration);
            if (this.isSummaryLink) {
                $("#".concat(this.fascCaptionId)).hide();
            }
            this.bindLoaded();
        };
        /**
         *------------------------- Methods -----------------------------
         */
        /**
         * Scatena l'evento di "load completed" del controllo
         */
        uscFascSummary.prototype.bindLoaded = function () {
            $("#".concat(this.pageId)).data(this);
            $("#".concat(this.pageId)).triggerHandler(uscFascSummary.LOADED_EVENT);
        };
        /**
         * Carica i dati dello user control
         */
        uscFascSummary.prototype.loadData = function (fascicle) {
            var _this = this;
            var promise = $.Deferred();
            if (fascicle == null) {
                return promise.resolve();
            }
            $.when(this.getFascicleUserDisplayName(fascicle.RegistrationUser), this.getFascicleUserDisplayName(fascicle.LastChangedUser))
                .done(function (registrationUser, lastChangedUser) {
                fascicle.RegistrationUser = registrationUser;
                fascicle.LastChangedUser = lastChangedUser;
                _this.setSummaryData(fascicle)
                    .done(function () { return promise.resolve(); })
                    .fail(function (exception) {
                    _this._uscNotification = $("#".concat(_this.uscNotificationId)).data();
                    if (!jQuery.isEmptyObject(_this._uscNotification)) {
                        if (exception instanceof ExceptionDTO) {
                            _this._uscNotification.showNotification(exception);
                        }
                        else {
                            _this._uscNotification.showNotificationMessage("E' avvenuto un errore durante il caricamento delle informazioni del Fascicolo: " + exception.message);
                        }
                    }
                });
            });
            return promise.promise();
        };
        uscFascSummary.prototype.getFascicleUserDisplayName = function (account) {
            var promise = $.Deferred();
            if (!account) {
                return promise.resolve(account);
            }
            this._domainUserService.getUser(account, function (user) {
                if (user) {
                    return promise.resolve(user.DisplayName);
                }
                return promise.resolve(account);
            }, function (exception) {
                console.warn("E' avvenuto un errore durante la ricerca dell'utente " + account + ". Viene restituito l'account name.");
                return promise.resolve(account);
            });
            return promise.promise();
        };
        /**
         * Imposta i dati nel sommario
         * @param fascicle
         */
        uscFascSummary.prototype.setSummaryData = function (fascicle) {
            var _this = this;
            var promise = $.Deferred();
            try {
                this._lblViewFascicle.hide();
                var title = fascicle.Title + " - " + fascicle.Category.Name;
                this._lblTitle.html(title);
                if (this.isSummaryLink) {
                    this._lblTitle.hide();
                    this._lblViewFascicle.show();
                    this._lblViewFascicle.html(title);
                    this._lblViewFascicle.attr("href", "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=".concat(fascicle.UniqueId));
                }
                this._lblFascicleObject.html(fascicle.FascicleObject);
                if ($.type(fascicle.FascicleType) === "string") {
                    fascicle.FascicleType = FascicleType[fascicle.FascicleType.toString()];
                }
                var fascicleTypeName = "";
                switch (fascicle.FascicleType) {
                    case FascicleType.Procedure:
                        fascicleTypeName = "Fascicolo di procedimento";
                        break;
                    case FascicleType.Period:
                        fascicleTypeName = "Fascicolo periodico";
                        break;
                    case FascicleType.Legacy:
                        fascicleTypeName = "Fascicolo non a norma";
                        break;
                    case FascicleType.Activity:
                        fascicleTypeName = "Fascicolo di attività";
                        break;
                }
                this._lblFascicleType.html(fascicleTypeName);
                this._lblStartDate.html(moment(fascicle.StartDate).format("DD/MM/YYYY"));
                this._lblEndDate.html("");
                if (fascicle.EndDate) {
                    this._lblEndDate.html(moment(fascicle.EndDate).format("DD/MM/YYYY"));
                }
                this._lblFascicleNote.html(fascicle.Note);
                this._lblLastChangedDate.html("");
                if (fascicle.LastChangedDate) {
                    this._lblLastChangedDate.html(moment(fascicle.LastChangedDate).format("DD/MM/YYYY"));
                }
                this._lblLastChangedUser.html("");
                if (fascicle.LastChangedUser) {
                    this._lblLastChangedUser.html(fascicle.LastChangedUser);
                }
                this._lblRegistrationDate.html(moment(fascicle.RegistrationDate).format("DD/MM/YYYY"));
                this._lblRegistrationUser.html(fascicle.RegistrationUser);
                this.containerRow.hide();
                if (this.fascicleContainerEnabled && (fascicle.FascicleType == FascicleType.Period
                    || fascicle.FascicleType == FascicleType.Procedure)
                    && fascicle.Container) {
                    this.containerRow.show();
                    this.lblContainer.html(fascicle.Container.Name);
                }
                if (!this.workflowActivityId) {
                    return promise.resolve();
                }
                this._workflowActivityService.getWorkflowActivity(this.workflowActivityId, function (data) {
                    if (data == null)
                        return;
                    _this._workflowActivity = data;
                    var subject;
                    if (_this._workflowActivity.WorkflowProperties != null) {
                        subject = _this._workflowActivity.WorkflowProperties.filter(function (item) { if (item.Name == WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT)
                            return item; })[0].ValueString;
                        _this._lblFascicleNote.html(subject);
                    }
                    promise.resolve();
                }, function (exception) {
                    promise.reject(exception);
                });
                return promise.promise();
            }
            catch (exception) {
                return promise.reject(exception);
            }
        };
        uscFascSummary.LOADED_EVENT = "onLoaded";
        uscFascSummary.DATA_LOADED_EVENT = "onDataLoaded";
        uscFascSummary.REBIND_EVENT = "onRebind";
        return uscFascSummary;
    }());
    return uscFascSummary;
});
//# sourceMappingURL=uscFascSummary.js.map