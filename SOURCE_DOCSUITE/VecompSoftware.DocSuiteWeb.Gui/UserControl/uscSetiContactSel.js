define(["require", "exports"], function (require, exports) {
    var uscSetiContactSel = /** @class */ (function () {
        function uscSetiContactSel(serviceConfigurations) {
            var _this = this;
            this.onSetiContactWindowClosed = function (sender, args) {
                if (args.get_argument()) {
                    $("#".concat(_this.metadataAddId)).triggerHandler(uscSetiContactSel.SELECTED_SETI_CONTACT_EVENT, args.get_argument());
                    $("#".concat(_this.metadataEditId)).triggerHandler(uscSetiContactSel.SELECTED_SETI_CONTACT_EVENT, args.get_argument());
                    $("#".concat(_this.fascicleInsertCommonIdEvent)).triggerHandler(uscSetiContactSel.SELECTED_SETI_CONTACT_EVENT, args.get_argument());
                    $("#".concat(_this.fascicleEditCommonIdEvent)).triggerHandler(uscSetiContactSel.SELECTED_SETI_CONTACT_EVENT, args.get_argument());
                }
            };
            this.showSetiContactWindow = function (sender, args) {
                _this._wndSetiContactSel.set_navigateUrl("../UserControl/CommonSetiContactSel.aspx");
                _this._wndSetiContactSel.set_width(760);
                _this._wndSetiContactSel.set_height(400);
                _this._wndSetiContactSel.show();
            };
            this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () { });
        }
        uscSetiContactSel.prototype.initialize = function () {
            var _this = this;
            this._btnOpenSetiContact = $find(this.btnOpenSetiContactId);
            this._wndSetiContactSel = $find(this.wndSetiContactSelId);
            this._wndSetiContactSel.add_close(this.onSetiContactWindowClosed);
            if (this._btnOpenSetiContact) {
                this._btnOpenSetiContact.add_clicked(this.showSetiContactWindow);
            }
            $("#".concat(this.btnOpenSetiContactId)).data(this.btnOpenSetiContactId, this);
            $("#".concat(this.btnOpenSetiContactId)).on(uscSetiContactSel.SHOW_SETI_CONTACT_BUTTON, function (sender, args) {
                _this._btnOpenSetiContact.set_visible(args);
            });
            $("#".concat(this.btnOpenSetiContactId)).data(this);
        };
        uscSetiContactSel.prototype.triggerButtonsVisibility = function (args) {
            this._btnOpenSetiContact.set_visible(args);
        };
        uscSetiContactSel.SHOW_SETI_CONTACT_BUTTON = "onShowSetiContactButton";
        uscSetiContactSel.SELECTED_SETI_CONTACT_EVENT = "onSelectedSetiContactEvent";
        return uscSetiContactSel;
    }());
    return uscSetiContactSel;
});
//# sourceMappingURL=uscSetiContactSel.js.map