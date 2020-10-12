define(["require", "exports", "App/Services/Dossiers/DossierService", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO", "App/Helpers/EnumHelper"], function (require, exports, DossierService, ServiceConfigurationHelper, ExceptionDTO, EnumHelper) {
    var uscDossierSummary = /** @class */ (function () {
        function uscDossierSummary(serviceConfigurations) {
            this._serviceConfigurations = serviceConfigurations;
            this._enumHelper = new EnumHelper();
        }
        uscDossierSummary.prototype.initialize = function () {
            this._lblDossierSubject = $("#".concat(this.lblDossierSubjectId));
            this._lblStartDate = $("#".concat(this.lblStartDateId));
            this._lblDossierNote = $("#".concat(this.lblDossierNoteId));
            this._lblYear = $("#".concat(this.lblYearId));
            this._lblNumber = $("#".concat(this.lblNumberId));
            this._lblDossierType = $("#" + this.lblDossierTypeId);
            this._lblDossierStatus = $("#" + this.lblDossierStatusId);
            var serviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Dossier");
            this._dossierService = new DossierService(serviceConfiguration);
            $("#" + this.pageId).data(this);
        };
        uscDossierSummary.prototype.loadDossierSummary = function (dossierId) {
            var _this = this;
            var promise = $.Deferred();
            this._dossierService.getDossier(dossierId, function (data) {
                _this._DossierModel = data;
                uscDossierSummary.DOSSIER_TITLE = _this._DossierModel.Year + "/" + _this.pad(+_this._DossierModel.Number, 7);
                _this._lblDossierSubject.html(_this._DossierModel.Subject);
                _this._lblDossierNote.html(_this._DossierModel.Note);
                _this._lblYear.html(_this._DossierModel.Year.toString());
                _this._lblNumber.html(_this._DossierModel.Number);
                _this._lblStartDate.html(_this._DossierModel.FormattedStartDate);
                _this._lblDossierType.html(_this._enumHelper.getDossierTypeDescription(_this._DossierModel.DossierType));
                _this._lblDossierStatus.html(_this._enumHelper.getDossierStatusDescription(_this._DossierModel.Status));
                promise.resolve();
            }, function (exception) {
                _this.showNotificationException(_this.uscNotificationId, exception);
                promise.reject(exception);
            });
            return promise.promise();
        };
        uscDossierSummary.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
            if (exception && exception instanceof ExceptionDTO) {
                var uscNotification = $("#" + uscNotificationId).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
            }
            else {
                this.showNotificationMessage(uscNotificationId, customMessage);
            }
        };
        uscDossierSummary.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#" + uscNotificationId).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        uscDossierSummary.prototype.pad = function (currentNumber, paddingSize) {
            var s = currentNumber + "";
            while (s.length < paddingSize) {
                s = "0" + s;
            }
            return s;
        };
        return uscDossierSummary;
    }());
    return uscDossierSummary;
});
//# sourceMappingURL=uscDossierSummary.js.map