/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
define(["require", "exports", "App/Services/Commons/ConservationService", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO", "App/Models/Commons/ConservationStatusType", "App/Services/DocumentUnits/DocumentUnitService"], function (require, exports, ConservationService, ServiceConfigurationHelper, ExceptionDTO, ConservationStatusType, DocumentUnitService) {
    var uscDocumentUnitConservationRest = /** @class */ (function () {
        function uscDocumentUnitConservationRest(serviceConfigurations) {
            var _this = this;
            /**
            *------------------------- Events -----------------------------
            */
            this.openConservationDetails = function (sender) {
                _this._windowConservationDetails.show();
                _this._windowConservationDetails.center();
            };
            var serviceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Conservation");
            this._service = new ConservationService(serviceConfiguration);
            var documentUnitServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "DocumentUnit");
            this._documentUnitService = new DocumentUnitService(documentUnitServiceConfiguration);
        }
        uscDocumentUnitConservationRest.prototype._imgConservationStatus = function () {
            return $("#" + this.imgConservationId);
        };
        uscDocumentUnitConservationRest.prototype._lblConservationDescription = function () {
            return $("#" + this.lblConservationDescriptionId);
        };
        uscDocumentUnitConservationRest.prototype._lblArchivedDate = function () {
            return $("#" + this.lblArchivedDateId);
        };
        uscDocumentUnitConservationRest.prototype._lblParerUri = function () {
            return $("#" + this.lblParerUriId);
        };
        uscDocumentUnitConservationRest.prototype._lblHasError = function () {
            return $("#" + this.lblHasErrorId);
        };
        uscDocumentUnitConservationRest.prototype._lblLastError = function () {
            return $("#" + this.lblLastErrorId);
        };
        uscDocumentUnitConservationRest.prototype._imgConservationInfo = function () {
            return $("#" + this.imgConservationInfoId);
        };
        /**
        *------------------------- Methods -----------------------------
        */
        uscDocumentUnitConservationRest.prototype.initialize = function () {
            var _this = this;
            this._windowConservationDetails = $find(this.windowConservationDetailsId);
            this.loadConservationInfos()
                .fail(function (exception) {
                _this.showNotificationException(exception);
            })
                .always(function () { return _this.bindLoaded(); });
        };
        uscDocumentUnitConservationRest.prototype.bindLoaded = function () {
            $("#" + this.pnlMainContentId).data(this);
            $("#" + this.pnlMainContentId).triggerHandler(uscDocumentUnitConservationRest.LOADED_EVENT);
        };
        uscDocumentUnitConservationRest.prototype.loadConservationInfos = function () {
            var _this = this;
            var promise = $.Deferred();
            this._service.getById(this.idDocumentUnit, function (data) {
                var conservation = data;
                if (!conservation) {
                    _this._imgConservationStatus().attr("src", "../Comm/images/parer/lightgray.png");
                    _this._lblConservationDescription().text("Documento non soggetto a versamento.");
                    _this._imgConservationInfo().hide();
                    return promise.resolve();
                }
                _this._documentUnitService.getDocumentUnitById(_this.idDocumentUnit, function (data) {
                    try {
                        var documentUnit = data;
                        _this.setConservationStatus(conservation);
                        _this.setWindowDetails(conservation, documentUnit);
                        promise.resolve();
                    }
                    catch (error) {
                        console.error(error);
                        var ex = new ExceptionDTO();
                        ex.statusText = "E' avvenuto un errore durante il recupero delle informazioni di conservazione per l'unità documentaria corrente.";
                        promise.reject(ex);
                    }
                }, function (exception) { return promise.reject(exception); });
            }, function (exception) {
                promise.reject(exception);
            });
            return promise.promise();
        };
        uscDocumentUnitConservationRest.prototype.setConservationStatus = function (conservation) {
            if (conservation.Status == ConservationStatusType.Conservated && !conservation.Message) {
                this._imgConservationStatus().attr("src", "../Comm/images/parer/green.png");
                this._lblConservationDescription().html("Conservazione corretta.");
                return;
            }
            if (conservation.Status == ConservationStatusType.Conservated && conservation.Message) {
                this._imgConservationStatus().attr("src", "../Comm/images/parer/yellow.png");
                this._lblConservationDescription().html("Conservazione con avviso.");
                return;
            }
            this._imgConservationStatus().attr("src", "../Comm/images/parer/red.png");
            this._lblConservationDescription().html("Conservazione con errori.");
        };
        uscDocumentUnitConservationRest.prototype.setWindowDetails = function (conservation, documentUnit) {
            this._windowConservationDetails.set_title("Dettaglio conservazione " + documentUnit.DocumentUnitName + " " + documentUnit.Title);
            if (conservation.SendDate) {
                this._lblArchivedDate().html(moment(conservation.SendDate).format("L") + " " + moment(conservation.SendDate).format("LTS"));
            }
            this._lblParerUri().html(conservation.Uri);
            this._lblHasError().html(conservation.Status == ConservationStatusType.Error ? "sì" : "no");
            this._lblLastError().html(conservation.Message);
        };
        uscDocumentUnitConservationRest.prototype.showNotificationException = function (exception, customMessage) {
            var uscNotification = $("#" + this.uscNotificationId).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                if (exception) {
                    uscNotification.showNotification(exception);
                    return;
                }
                uscNotification.showWarningMessage(customMessage);
            }
        };
        uscDocumentUnitConservationRest.LOADED_EVENT = "onLoaded";
        return uscDocumentUnitConservationRest;
    }());
    return uscDocumentUnitConservationRest;
});
//# sourceMappingURL=uscDocumentUnitConservationRest.js.map