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
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "Monitors/TransparentAdministrationMonitorLogBase", "App/Services/Securities/DomainUserService", "App/Models/DocumentUnits/DocumentUnitModel"], function (require, exports, ServiceConfigurationHelper, TransparentAdministrationMonitorLogBase, DomainUserService, DocumentUnitModel) {
    var uscAmmTraspMonitorLog = /** @class */ (function (_super) {
        __extends(uscAmmTraspMonitorLog, _super);
        function uscAmmTraspMonitorLog(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, TransparentAdministrationMonitorLogBase.TransparentAdministrationMonitorLog_TYPE_NAME)) || this;
            _this.actionType = "Insert";
            _this.openMonitor = function (sender, args) {
                _this.actionType = "Edit";
                _this.canClearData = true;
                _this.loadLastLogData();
                _this._rwAmmTraspMonitorLog.show();
            };
            _this.formClosed = function (sender, args) {
                _this.clearWindowControls();
            };
            _this.updateCallback = function (model) {
                _this.service.updateTransparentAdministrationMonitorLog(model, function (data) {
                    if (data == null)
                        return;
                    var model = data;
                    if (model.LastChangedUser === null)
                        _this.displayDetailsAfterInsertOrUpdate(model, model.RegistrationUser);
                    else
                        _this.displayDetailsAfterInsertOrUpdate(model, model.LastChangedUser);
                    _this.actionType = "Insert";
                }, function (exception) {
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            };
            _this.Save_Clicked = function (sender, args) {
                if (_this.getUscRole(_this.uscOwnerRoleId) === null) {
                    alert('Campo Settori Obbligatorio');
                    return;
                }
                if (!Page_IsValid) {
                    return;
                }
                var ratingStringResult = "";
                for (var i = 0; i < _this._cmbAmmTraspMonitorLogRating.get_checkedItems().length; i++) {
                    var item = _this._cmbAmmTraspMonitorLogRating.get_checkedItems()[i];
                    ratingStringResult += item.get_text() + "|";
                }
                ratingStringResult = ratingStringResult.slice(0, -1);
                var stringDate = _this._dpAmmTraspMonitorLogDate.get_textBoxValue();
                var dateFormat = new Date(+stringDate.slice(6, 10), +stringDate.slice(3, 5) - 1, +stringDate.slice(0, 2), +stringDate.slice(11, 13), +stringDate.slice(14, 16), 0, 0);
                var documentUnit = new DocumentUnitModel();
                documentUnit.UniqueId = _this.txtAmmTraspMonitorLogDocumentUnitIdValue;
                var role = _this.getUscRole(_this.uscOwnerRoleId);
                var transparentAdministrationMonitorLogModel = {
                    UniqueId: "",
                    DocumentUnit: documentUnit,
                    DocumentUnitName: _this.txtAmmTraspMonitorLogDocumentUnitNameValue,
                    Date: dateFormat,
                    Note: _this._txtAmmTraspMonitorLogNote.get_textBoxValue(),
                    RegistrationUser: _this._txtAmmTraspMonitorLogName.get_textBoxValue(),
                    RegistrationDate: new Date(),
                    Rating: ratingStringResult,
                    Role: role
                };
                if (_this.actionType === "Edit") {
                    _this.updateLastLogData();
                }
                else if (_this.actionType === "Insert") {
                    _this.insertCallback(transparentAdministrationMonitorLogModel);
                }
            };
            _this.getUscRole = function (uscSettoreId) {
                var roles = new Array();
                var uscRoles = $("#".concat(uscSettoreId)).data();
                if (!jQuery.isEmptyObject(uscRoles)) {
                    var source = JSON.parse(uscRoles.getRoles());
                    if (source != null) {
                        for (var _i = 0, source_1 = source; _i < source_1.length; _i++) {
                            var s = source_1[_i];
                            roles.push(s);
                        }
                    }
                }
                if (roles.length > 0) {
                    return roles[0];
                }
                return null;
            };
            var metadataRepositoryConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "DomainUserModel");
            _this._domainUserService = new DomainUserService(metadataRepositoryConfiguration);
            $(document).ready(function () {
            });
            return _this;
        }
        uscAmmTraspMonitorLog.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            this._rwAmmTraspMonitorLog = $find(this.rwAmmTraspMonitorLogId);
            this._txtAmmTraspMonitorLogName = $find(this.txtAmmTraspMonitorLogNameId);
            this._dpAmmTraspMonitorLogDate = $find(this.dpAmmTraspMonitorLogDateId);
            this._txtAmmTraspMonitorLogNote = $find(this.txtAmmTraspMonitorLogNoteId);
            this._cmbAmmTraspMonitorLogRating = $find(this.cmbAmmTraspMonitorLogRatingId);
            this._cmdAmmTraspMonitorLogSave = $find(this.cmdAmmTraspMonitorLogSaveId);
            this._cmdAmmTraspMonitorLogSave.add_clicking(this.Save_Clicked);
            this._uscMonitoringEditButton = $find(this.uscMonitoringEditButtonId);
            this._rwAmmTraspMonitorLog.add_close(this.formClosed);
            if (this._uscMonitoringEditButton) {
                this._uscMonitoringEditButton.add_clicked(this.openMonitor);
            }
            $("#".concat(this.uscMonitoraggioContentId)).hide();
            for (var i = 0; i < this.ratingValues.length; i++) {
                var cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                cmbItem.set_text(this.ratingValues[i]);
                this._cmbAmmTraspMonitorLogRating.get_items().add(cmbItem);
            }
            if (this.txtAmmTraspMonitorLogDocumentUnitIdValue) {
                this.loadMonitorLog(this.txtAmmTraspMonitorLogDocumentUnitIdValue);
            }
            this.loadOwnerRole();
            this.bindLoaded();
        };
        uscAmmTraspMonitorLog.prototype.clearWindowControls = function () {
            if (this.canClearData) {
                this.canClearData = false;
                this._cmbAmmTraspMonitorLogRating.get_items().forEach(function (item) {
                    item.uncheck();
                });
                this._txtAmmTraspMonitorLogNote.clear();
                this._txtAmmTraspMonitorLogName.set_value(this.currentDisplayName);
                this._dpAmmTraspMonitorLogDate.set_value(moment(new Date()).format("DD/MM/YYYY hh:mm"));
                this.loadOwnerRole();
            }
        };
        uscAmmTraspMonitorLog.prototype.loadLastLogData = function () {
            var _this = this;
            this.service.getLatestMonitorLogByDocumentUnit(this.txtAmmTraspMonitorLogDocumentUnitIdValue, function (data) {
                _this.uniqueId = data.UniqueId;
                _this.documentUnit = data.DocumentUnit;
                _this.documentUnitName = data.DocumentUnitName;
                _this.getLastChangingUser(data);
                _this.txtAmmTraspMonitorLogDocumentUnitNameValue = data.DocumentUnitName;
                _this._txtAmmTraspMonitorLogNote.set_value(data.Note);
                _this._dpAmmTraspMonitorLogDate.set_textBoxValue(moment(data.RegistrationDate).format("YYYY/MM/DD HH:mm"));
                _this.getLastCheckedRatings(data);
                var ajaxModel = {};
                ajaxModel.Value = new Array();
                ajaxModel.Value.push(data.Role.EntityShortId.toString());
                ajaxModel.ActionName = uscAmmTraspMonitorLog.LOAD_ROLE_ID;
                $find(_this.ajaxManagerId).ajaxRequest(JSON.stringify(ajaxModel));
            }, function (exception) {
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        uscAmmTraspMonitorLog.prototype.updateLastLogData = function () {
            var role = this.getUscRole(this.uscOwnerRoleId);
            var ratingStringResult = "";
            for (var i = 0; i < this._cmbAmmTraspMonitorLogRating.get_checkedItems().length; i++) {
                var item = this._cmbAmmTraspMonitorLogRating.get_checkedItems()[i];
                ratingStringResult += item.get_text() + "|";
            }
            ratingStringResult = ratingStringResult.slice(0, -1);
            var transparentAdministrationMonitorLogModel = {
                UniqueId: this.uniqueId,
                DocumentUnit: this.documentUnit,
                DocumentUnitName: this.txtAmmTraspMonitorLogDocumentUnitNameValue,
                Date: new Date(),
                Note: this._txtAmmTraspMonitorLogNote.get_textBoxValue(),
                RegistrationUser: this._txtAmmTraspMonitorLogName.get_value(),
                RegistrationDate: new Date(this._dpAmmTraspMonitorLogDate.get_textBoxValue()),
                Rating: ratingStringResult,
                Role: role
            };
            this.updateCallback(transparentAdministrationMonitorLogModel);
        };
        uscAmmTraspMonitorLog.prototype.loadOwnerRole = function () {
            var uscRoles = $("#".concat(this.uscOwnerRoleId)).data();
            if (!jQuery.isEmptyObject(uscRoles)) {
                var ajaxModel = {};
                ajaxModel.Value = new Array();
                ajaxModel.ActionName = uscAmmTraspMonitorLog.LOAD_OWNER_ROLE;
                $find(this.ajaxManagerId).ajaxRequest(JSON.stringify(ajaxModel));
            }
        };
        uscAmmTraspMonitorLog.prototype.bindLoaded = function () {
            $("#".concat(this.pageContentId)).data(this);
        };
        uscAmmTraspMonitorLog.prototype.insertCallback = function (transparentAdministrationMonitorLogModel) {
            var _this = this;
            this.service.insertTransparentAdministrationMonitorLog(transparentAdministrationMonitorLogModel, function (data) {
                if (data == null)
                    return;
                var model = data;
                if (model.LastChangedUser === null)
                    _this.displayDetailsAfterInsertOrUpdate(model, model.RegistrationUser);
                else
                    _this.displayDetailsAfterInsertOrUpdate(model, model.LastChangedUser);
            }, function (exception) {
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        uscAmmTraspMonitorLog.prototype.loadUser = function (accountUser) {
            var promise = $.Deferred();
            this._domainUserService.getUser(accountUser, function (data) {
                if (!data) {
                    return promise.resolve({});
                }
                ;
                var user = data;
                return promise.resolve(user);
            }, function (exception) {
                return promise.reject(exception);
            });
            return promise.promise();
        };
        uscAmmTraspMonitorLog.prototype.loadMonitorLog = function (documentUnitId) {
            var _this = this;
            this.service.getLatestMonitorLogByDocumentUnit(documentUnitId, function (data) {
                if (!data) {
                    return;
                }
                $("#".concat(_this.uscMonitoraggioContentId)).show();
                var model = data;
                if (model.LastChangedUser === null)
                    _this.displayDetailsAfterInsertOrUpdate(model, model.RegistrationUser);
                else
                    _this.displayDetailsAfterInsertOrUpdate(model, model.LastChangedUser);
            }, function (exception) {
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        uscAmmTraspMonitorLog.prototype.getLastChangingUser = function (data) {
            var _this = this;
            if (data.LastChangedUser === null) {
                this.loadUser(data.RegistrationUser)
                    .done(function (domainUser) {
                    _this._txtAmmTraspMonitorLogName.set_textBoxValue(domainUser.DisplayName);
                    _this._txtAmmTraspMonitorLogName.set_value(data.RegistrationUser);
                })
                    .fail(function (exception) {
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            }
            else {
                this.loadUser(data.LastChangedUser)
                    .done(function (domainUser) {
                    _this._txtAmmTraspMonitorLogName.set_textBoxValue(domainUser.DisplayName);
                    _this._txtAmmTraspMonitorLogName.set_value(data.LastChangedUser);
                })
                    .fail(function (exception) {
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            }
        };
        uscAmmTraspMonitorLog.prototype.getLastCheckedRatings = function (data) {
            var ratings = data.Rating.split('|');
            this._cmbAmmTraspMonitorLogRating.get_items().forEach(function (item) {
                for (var _i = 0, ratings_1 = ratings; _i < ratings_1.length; _i++) {
                    var rating = ratings_1[_i];
                    if (item.get_text() === rating) {
                        item.check();
                    }
                }
            });
        };
        uscAmmTraspMonitorLog.prototype.displayDetailsAfterInsertOrUpdate = function (model, user) {
            var _this = this;
            this.loadUser(user)
                .done(function (domainUser) {
                $("#".concat(_this.lblMonitoringId)).html("Monitorato da ".concat(domainUser.DisplayName, " in data ", moment(model.Date).format("L").concat(" ", moment(model.Date).format("LTS"))));
                $("#".concat(_this.lblArchiveId)).html(model.DocumentUnitName);
                $("#".concat(_this.lblCreatedById)).html(domainUser.DisplayName + " " + moment(model.RegistrationDate).format("L"));
                $("#".concat(_this.uscMonitoraggioContentId)).show();
                _this._rwAmmTraspMonitorLog.close();
            })
                .fail(function (exception) {
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        uscAmmTraspMonitorLog.LOAD_OWNER_ROLE = "LoadOwnerRole";
        uscAmmTraspMonitorLog.LOAD_ROLE_ID = "LoadRoleId";
        return uscAmmTraspMonitorLog;
    }(TransparentAdministrationMonitorLogBase));
    return uscAmmTraspMonitorLog;
});
//# sourceMappingURL=uscAmmTraspMonitorLog.js.map