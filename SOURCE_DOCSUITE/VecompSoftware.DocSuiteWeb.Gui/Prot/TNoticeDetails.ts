import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import GenericHelper = require("App/Helpers/GenericHelper");
import TNoticeService = require("App/Services/PosteWeb/TNoticeService");
import POLRequest = require("App/Models/PosteWeb/POLRequest");
import NullSafe = require("App/Helpers/NullSafe");
import StatusColor = require("App/Models/PosteWeb/StatusColor");

class TNoticeDetails {
    dateId: string;
    senderId: string;
    statusId: string;
    urlAcceptId: string;
    urlCpfId: string;
    cmdDocumentsId: string;

    private _serviceConfigurations: ServiceConfiguration[];
    private _tNoticeService: TNoticeService;
    //id's linked from aspx
    private _dateId: JQuery;
    private _senderId: JQuery;
    private _statusId: JQuery;
    private _urlAcceptId: JQuery;
    private _urlCpfId: JQuery;
    private _btnDocuments: Telerik.Web.UI.RadButton;

    //id's found through jQuery
    private $statusText: JQuery;
    private $redStatusImg: JQuery;
    private $yelStatusImg: JQuery;
    private $greStatusImg: JQuery;
    private $bluStatusImg: JQuery;

    private request: POLRequest;

    protected static SERVICE_CONFIGURATION_NAME = "POLRequest";

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;

    }

    initialize(): void {
        let configuration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TNoticeDetails.SERVICE_CONFIGURATION_NAME);
        this._tNoticeService = new TNoticeService(configuration);

        this._dateId = <JQuery>$(`#${this.dateId}`);
        this._senderId = <JQuery>$(`#${this.senderId}`);
        this._statusId = <JQuery>$(`#${this.statusId}`);
        this._urlAcceptId = <JQuery>$(`#${this.urlAcceptId}`);
        this._urlCpfId = <JQuery>$(`#${this.urlCpfId}`);
        this._btnDocuments = <Telerik.Web.UI.RadButton>$find(this.cmdDocumentsId);
        this._btnDocuments.add_clicked(this.btnDocuments_OnClicked);

        let entityId: string = GenericHelper.getUrlParams(window.location.href, "RequestId");

        this.$redStatusImg = this._statusId.find("img[data-name=statusRed]");
        this.$yelStatusImg = this._statusId.find("img[data-name=statusYellow]");
        this.$greStatusImg = this._statusId.find("img[data-name=statusGreen]");
        this.$bluStatusImg = this._statusId.find("img[data-name=statusBlue]");
        this.$statusText = this._statusId.find("#statusTextId");

        this.loadOutGoingData(entityId);
    }

    private loadOutGoingData(entityId: string): void {
        let _this = this;

        this._tNoticeService.getRequestByRequestId(entityId, (data) => {
            if (!data) return;

            let summary: POLRequest = data;
            _this.request = data;

            //created at
            this._dateId.text(summary.RegistrationDate);

            //sender
            this._senderId.text(NullSafe.Do<string>(() => {
                let fullName: string = "";

                if (!summary.ExtendedPropertiesDeserialized) {
                    return null;
                }

                if (!summary.ExtendedPropertiesDeserialized.MetaData) {
                    return null;
                }

                let polAccountName: string = summary.ExtendedPropertiesDeserialized.MetaData.PolAccountName;
                let senderName: string = summary.ExtendedPropertiesDeserialized.MetaData.PolRequestContactName;


                if (polAccountName) {
                    fullName = polAccountName;
                }

                if (fullName) {
                    if (fullName && fullName !== "") {
                        fullName = `${fullName} - ${senderName}`;
                    } else {
                        fullName = `${senderName}`;
                    }
                }

                return fullName;

            }, ""));

            //status
            switch (summary.DisplayColor) {
                case StatusColor.Yellow:
                    TNoticeDetails.ShowElement(this.$yelStatusImg);
                    break;
                case StatusColor.Green:
                    TNoticeDetails.ShowElement(this.$greStatusImg);
                    break;
                case StatusColor.Blue:
                    TNoticeDetails.ShowElement(this.$bluStatusImg);
                    break;
                default:
                    TNoticeDetails.ShowElement(this.$redStatusImg);
                    break;

            }

            this.$statusText.text(NullSafe.Do<string>(() => summary.ExtendedPropertiesDeserialized.GetStatus.StatusDescription, summary.StatusDescription));

            //urlaccept
            let _urlAcceptUrl = NullSafe.Do<string>(() => summary.ExtendedPropertiesDeserialized.GetStatus.UrlAccept, "");
            TNoticeDetails.ToggleLinkContent(this._urlAcceptId, _urlAcceptUrl);

            //urlcpf
            let _urlCpfUrl = NullSafe.Do<string>(() => summary.ExtendedPropertiesDeserialized.GetStatus.UrlCPF, "");
            TNoticeDetails.ToggleLinkContent(this._urlCpfId, _urlCpfUrl);

            if (_this.request.IdArchiveChainPoste) {
                this._btnDocuments.set_enabled(true);
            }
        })
    }

    private static ToggleLinkContent(anchorContainer: JQuery, href: string): void {
        let $anchor: JQuery = anchorContainer.find("a:first");
        let $noLinkSpan: JQuery = anchorContainer.find("span:first");

        if (!this.IsNullOrEmpty(href)) {
            this.ChangeAnchorHref($anchor, href);
            this.ShowElement($anchor);
            this.HideElement($noLinkSpan);
        } else {
            this.ShowElement($noLinkSpan);
            this.HideElement($anchor);
        }
    }

    private static IsNullOrEmpty(str: string): boolean {
        return !str;
    }

    private static ShowElement(element: JQuery): void {
        element.css({ 'display': 'inline-block' });
    }

    private static HideElement(element: JQuery): void {
        element.css({ 'display': 'none' });
    }

    private static ChangeAnchorHref(element: JQuery, href: string) {
        element.attr("href", href);
    }


    btnDocuments_OnClicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        if (!!this.request) {
            window.location.href = `../Viewers/PosteWebActivityViewer.aspx?Title=${this.request.DocumentName}&IdArchiveChain=${this.request.IdArchiveChainPoste}`;
        }
    }
}


export = TNoticeDetails;