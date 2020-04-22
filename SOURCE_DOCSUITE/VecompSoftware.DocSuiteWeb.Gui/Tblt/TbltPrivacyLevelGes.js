/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Services/Commons/PrivacyLevelService", "App/Models/Commons/PrivacyLevelModel", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO"], function (require, exports, PrivacyLevelService, PrivacyLevelModel, ServiceConfigurationHelper, ExceptionDTO) {
    var TbltPrivacyLevelGes = /** @class */ (function () {
        /**
       * Costruttore
       * @param serviceConfigurations
       */
        function TbltPrivacyLevelGes(serviceConfigurations) {
            var _this = this;
            /**
            *------------------------- Events -----------------------------
            */
            this.btnConfirm_OnClick = function (sender, eventArgs) {
                if (!Page_IsValid) {
                    return;
                }
                var levelValue = Number(_this._txtLevel.get_value());
                if (isNaN(levelValue)) {
                    _this.showNotificationException(_this.uscNotificationId, null, "Il livello dev'essere un valore numerico");
                    return;
                }
                switch (_this.action) {
                    case "Add":
                        var level = new PrivacyLevelModel();
                        level.Level = levelValue;
                        level.Description = _this._txtDescription.get_value();
                        if (_this._rcpColor.get_selectedColor() && _this._rcpColor.get_selectedColor().toLowerCase() != '#ffffff') {
                            level.Colour = _this._rcpColor.get_selectedColor();
                        }
                        level.IsActive = true;
                        _this._privacyLevelService.insertPrivacyLevel(level, function (data) {
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
                        _this._currentPrivacyLevelModel.Level = levelValue;
                        _this._currentPrivacyLevelModel.Description = _this._txtDescription.get_value();
                        if (_this._rcpColor.get_selectedColor() && _this._rcpColor.get_selectedColor().toLowerCase() != '#ffffff') {
                            _this._currentPrivacyLevelModel.Colour = _this._rcpColor.get_selectedColor();
                        }
                        _this._privacyLevelService.updatePrivacyLevel(_this._currentPrivacyLevelModel, function (data) {
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
            var serviceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "PrivacyLevel");
            this._privacyLevelService = new PrivacyLevelService(serviceConfiguration);
        }
        /**
       * Inizializzazione classe
       */
        TbltPrivacyLevelGes.prototype.initialize = function () {
            this._btnConfirm = $find(this.btnConfirmId);
            this._txtLevel = $find(this.txtLevelId);
            this._txtDescription = $find(this.txtDescriptionId);
            this._rcpColor = $find(this.rcpColorId);
            this._btnConfirm.add_clicked(this.btnConfirm_OnClick);
            if (this.action === "Edit") {
                this._currentPrivacyLevelModel = JSON.parse(sessionStorage[this.currentPrivacyLevelId]);
                var descr = this._currentPrivacyLevelModel.Description ? this._currentPrivacyLevelModel.Description : '';
                this._txtLevel.set_value(this._currentPrivacyLevelModel.Level.toString());
                this._txtLevel.set_textBoxValue(this._currentPrivacyLevelModel.Level.toString());
                this._txtDescription.set_value(this._currentPrivacyLevelModel.Description);
                this._txtDescription.set_textBoxValue(this._currentPrivacyLevelModel.Description);
                this._rcpColor.set_selectedColor(this._currentPrivacyLevelModel.Colour, false);
            }
        };
        /**
         *------------------------- Methods -----------------------------
         */
        TbltPrivacyLevelGes.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
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
        TbltPrivacyLevelGes.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        TbltPrivacyLevelGes.prototype.closeWindow = function (message) {
            var wnd = this.getRadWindow();
            wnd.close(message);
        };
        return TbltPrivacyLevelGes;
    }());
    return TbltPrivacyLevelGes;
});
//# sourceMappingURL=TbltPrivacyLevelGes.js.map