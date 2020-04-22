/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/DTOs/ExceptionDTO", "App/Mappers/Commons/ContactDSWModelMapper"], function (require, exports, ExceptionDTO, ContactDSWModelMapper) {
    var CommonSelContactRest = /** @class */ (function () {
        function CommonSelContactRest(serviceConfigurations) {
            var _this = this;
            /**
            *------------------------- Events -----------------------------
            */
            this.confirm_onClicked = function (sender, args) {
                _this._ajaxFlatLoadingPanel.show(_this.pnlButtonsId);
                _this.createContact()
                    .done(function (data) {
                    _this.returnValue(data[1], data[0], (sender.get_id() == _this.btnConfirmId));
                })
                    .fail(function (exception) { return _this.showNotificationException(exception); })
                    .always(function () { return _this._ajaxFlatLoadingPanel.hide(_this.pnlButtonsId); });
            };
            this._serviceConfigurations = serviceConfigurations;
            this._contactDSWModelMapper = new ContactDSWModelMapper();
        }
        /**
        *------------------------- Methods -----------------------------
        */
        CommonSelContactRest.prototype.initialize = function () {
            this._btnConfirm = $find(this.btnConfirmId);
            this._btnConfirm.add_clicked(this.confirm_onClicked);
            this._btnConfirmAndNew = $find(this.btnConfirmAndNewId);
            this._btnConfirmAndNew.add_clicked(this.confirm_onClicked);
            this._ajaxFlatLoadingPanel = $find(this.ajaxFlatLoadingPanelId);
        };
        CommonSelContactRest.prototype.createContact = function () {
            var promise = $.Deferred();
            var uscContactRest = $("#" + this.uscContactRestId).data();
            if (!jQuery.isEmptyObject(uscContactRest)) {
                uscContactRest.createContact()
                    .done(function (data) { return promise.resolve(data); })
                    .fail(function (exception) { return promise.reject(exception); });
            }
            return promise.promise();
        };
        CommonSelContactRest.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        CommonSelContactRest.prototype.returnValue = function (contact, contactType, toClose) {
            var contactDswModel = this._contactDSWModelMapper.Map(contact);
            var returnModel = { contactType: contactType, contact: JSON.stringify(contactDswModel) };
            if (toClose) {
                this.getRadWindow().close(returnModel);
            }
            else {
                this.getRadWindow().BrowserWindow[this.callerId + "_UpdateSmart"](returnModel.contactType, returnModel.contact);
                var uscContactRest_1 = $("#" + this.uscContactRestId).data();
                if (!jQuery.isEmptyObject(uscContactRest_1)) {
                    uscContactRest_1.clear();
                }
            }
        };
        CommonSelContactRest.prototype.showNotificationException = function (exception) {
            var uscNotification = $("#" + this.uscNotificationId).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                if (exception instanceof ExceptionDTO) {
                    uscNotification.showNotification(exception);
                    return;
                }
                uscNotification.showWarningMessage(exception);
            }
        };
        return CommonSelContactRest;
    }());
    return CommonSelContactRest;
});
//# sourceMappingURL=CommonSelContactRest.js.map