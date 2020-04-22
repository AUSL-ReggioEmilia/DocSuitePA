/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Services/UDS/UDSTypologyService", "App/Models/UDS/UDSTypologyModel", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO"], function (require, exports, UDSTypologyService, UDSTypologyModel, ServiceConfigurationHelper, ExceptionDTO) {
    var TbltUDSTypologyGes = /** @class */ (function () {
        /**
       * Costruttore
       * @param serviceConfigurations
       */
        function TbltUDSTypologyGes(serviceConfigurations) {
            var _this = this;
            /**
            *------------------------- Events -----------------------------
            */
            this.btnConfirm_OnClick = function (sender, eventArgs) {
                if (!_this._txtName.get_value()) {
                    return;
                }
                switch (_this.action) {
                    case "Add":
                        var typology = new UDSTypologyModel();
                        typology.Name = _this._txtName.get_value();
                        _this._udsTypologyService.insertUDSTypology(typology, function (data) {
                            if (data) {
                                var ajaxModel = {};
                                ajaxModel.ActionName = 'Add';
                                ajaxModel.Value = [JSON.stringify(data)];
                                _this.closeWindow(ajaxModel);
                            }
                        }, function (exception) {
                            _this.showNotificationException(_this.uscNotificationId, exception);
                        });
                        break;
                    case "Edit":
                        _this._currentUDSTypologyModel.Name = _this._txtName.get_value();
                        _this._udsTypologyService.updateUDSTypology(_this._currentUDSTypologyModel, function (data) {
                            if (data) {
                                var ajaxModel = {};
                                ajaxModel.ActionName = 'Edit';
                                ajaxModel.Value = [JSON.stringify(data)];
                                _this.closeWindow(ajaxModel);
                            }
                        }, function (exception) {
                            _this.showNotificationException(_this.uscNotificationId, exception);
                        });
                        break;
                }
            };
            var serviceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "UDSTypology");
            this._udsTypologyService = new UDSTypologyService(serviceConfiguration);
        }
        /**
       * Inizializzazione classe
       */
        TbltUDSTypologyGes.prototype.initialize = function () {
            this._btnConfirm = $find(this.btnConfirmId);
            this._txtName = $find(this.txtNameId);
            this._txtOldName = $find(this.txtOldNameId);
            this._btnConfirm.add_clicked(this.btnConfirm_OnClick);
            this._txtOldName.set_visible(false);
            $("#".concat(this.rowOldNameId)).hide();
            if (this.action === "Edit") {
                $("#".concat(this.rowOldNameId)).show();
                this._txtOldName.set_visible(true);
                this._currentUDSTypologyModel = JSON.parse(sessionStorage[this.currentUDSTypologyId]);
                this._txtOldName.set_value(this._currentUDSTypologyModel.Name);
                this._txtOldName.set_textBoxValue(this._currentUDSTypologyModel.Name);
            }
        };
        /**
         *------------------------- Methods -----------------------------
         */
        TbltUDSTypologyGes.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                if (exception && exception instanceof ExceptionDTO) {
                    uscNotification.showNotification(exception);
                }
                else {
                    uscNotification.showNotificationMessage(customMessage);
                }
            }
        };
        TbltUDSTypologyGes.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        TbltUDSTypologyGes.prototype.closeWindow = function (message) {
            var wnd = this.getRadWindow();
            wnd.close(message);
        };
        return TbltUDSTypologyGes;
    }());
    return TbltUDSTypologyGes;
});
//# sourceMappingURL=TbltUDSTypologyGes.js.map