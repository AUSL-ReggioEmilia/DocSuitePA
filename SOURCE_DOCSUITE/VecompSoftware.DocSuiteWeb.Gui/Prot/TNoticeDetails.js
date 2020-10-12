define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Helpers/GenericHelper", "App/Services/PosteWeb/TNoticeService", "App/Helpers/NullSafe", "App/Models/PosteWeb/StatusColor"], function (require, exports, ServiceConfigurationHelper, GenericHelper, TNoticeService, NullSafe, StatusColor) {
    var TNoticeDetails = /** @class */ (function () {
        function TNoticeDetails(serviceConfigurations) {
            var _this_1 = this;
            this.btnDocuments_OnClicked = function (sender, eventArgs) {
                if (!!_this_1.request) {
                    window.location.href = "../Viewers/PosteWebActivityViewer.aspx?Title=" + _this_1.request.DocumentName + "&IdArchiveChain=" + _this_1.request.IdArchiveChainPoste;
                }
            };
            this._serviceConfigurations = serviceConfigurations;
        }
        TNoticeDetails.prototype.initialize = function () {
            var configuration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TNoticeDetails.SERVICE_CONFIGURATION_NAME);
            this._tNoticeService = new TNoticeService(configuration);
            this._dateId = $("#" + this.dateId);
            this._senderId = $("#" + this.senderId);
            this._statusId = $("#" + this.statusId);
            this._urlAcceptId = $("#" + this.urlAcceptId);
            this._urlCpfId = $("#" + this.urlCpfId);
            this._btnDocuments = $find(this.cmdDocumentsId);
            this._btnDocuments.add_clicked(this.btnDocuments_OnClicked);
            var entityId = GenericHelper.getUrlParams(window.location.href, "RequestId");
            this.$redStatusImg = this._statusId.find("img[data-name=statusRed]");
            this.$yelStatusImg = this._statusId.find("img[data-name=statusYellow]");
            this.$greStatusImg = this._statusId.find("img[data-name=statusGreen]");
            this.$bluStatusImg = this._statusId.find("img[data-name=statusBlue]");
            this.$statusText = this._statusId.find("#statusTextId");
            this.loadOutGoingData(entityId);
        };
        TNoticeDetails.prototype.loadOutGoingData = function (entityId) {
            var _this_1 = this;
            var _this = this;
            this._tNoticeService.getRequestByRequestId(entityId, function (data) {
                if (!data)
                    return;
                var summary = data;
                _this.request = data;
                //created at
                _this_1._dateId.text(summary.RegistrationDate);
                //sender
                _this_1._senderId.text(NullSafe.Do(function () {
                    var fullName = "";
                    if (!summary.ExtendedPropertiesDeserialized) {
                        return null;
                    }
                    if (!summary.ExtendedPropertiesDeserialized.MetaData) {
                        return null;
                    }
                    var polAccountName = summary.ExtendedPropertiesDeserialized.MetaData.PolAccountName;
                    var senderName = summary.ExtendedPropertiesDeserialized.MetaData.PolRequestContactName;
                    if (polAccountName) {
                        fullName = polAccountName;
                    }
                    if (fullName) {
                        if (fullName && fullName !== "") {
                            fullName = fullName + " - " + senderName;
                        }
                        else {
                            fullName = "" + senderName;
                        }
                    }
                    return fullName;
                }, ""));
                //status
                switch (summary.DisplayColor) {
                    case StatusColor.Yellow:
                        TNoticeDetails.ShowElement(_this_1.$yelStatusImg);
                        break;
                    case StatusColor.Green:
                        TNoticeDetails.ShowElement(_this_1.$greStatusImg);
                        break;
                    case StatusColor.Blue:
                        TNoticeDetails.ShowElement(_this_1.$bluStatusImg);
                        break;
                    default:
                        TNoticeDetails.ShowElement(_this_1.$redStatusImg);
                        break;
                }
                _this_1.$statusText.text(NullSafe.Do(function () { return summary.ExtendedPropertiesDeserialized.GetStatus.StatusDescription; }, summary.StatusDescription));
                //urlaccept
                var _urlAcceptUrl = NullSafe.Do(function () { return summary.ExtendedPropertiesDeserialized.GetStatus.UrlAccept; }, "");
                TNoticeDetails.ToggleLinkContent(_this_1._urlAcceptId, _urlAcceptUrl);
                //urlcpf
                var _urlCpfUrl = NullSafe.Do(function () { return summary.ExtendedPropertiesDeserialized.GetStatus.UrlCPF; }, "");
                TNoticeDetails.ToggleLinkContent(_this_1._urlCpfId, _urlCpfUrl);
                if (_this.request.IdArchiveChainPoste) {
                    _this_1._btnDocuments.set_enabled(true);
                }
            });
        };
        TNoticeDetails.ToggleLinkContent = function (anchorContainer, href) {
            var $anchor = anchorContainer.find("a:first");
            var $noLinkSpan = anchorContainer.find("span:first");
            if (!this.IsNullOrEmpty(href)) {
                this.ChangeAnchorHref($anchor, href);
                this.ShowElement($anchor);
                this.HideElement($noLinkSpan);
            }
            else {
                this.ShowElement($noLinkSpan);
                this.HideElement($anchor);
            }
        };
        TNoticeDetails.IsNullOrEmpty = function (str) {
            return !str;
        };
        TNoticeDetails.ShowElement = function (element) {
            element.css({ 'display': 'inline-block' });
        };
        TNoticeDetails.HideElement = function (element) {
            element.css({ 'display': 'none' });
        };
        TNoticeDetails.ChangeAnchorHref = function (element, href) {
            element.attr("href", href);
        };
        TNoticeDetails.SERVICE_CONFIGURATION_NAME = "POLRequest";
        return TNoticeDetails;
    }());
    return TNoticeDetails;
});
//# sourceMappingURL=TNoticeDetails.js.map