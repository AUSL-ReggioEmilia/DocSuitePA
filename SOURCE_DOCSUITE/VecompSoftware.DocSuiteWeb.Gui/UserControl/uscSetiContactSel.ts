import ServiceConfiguration = require('App/Services/ServiceConfiguration');

class uscSetiContactSel {

    public static SHOW_SETI_CONTACT_BUTTON: string = "onShowSetiContactButton";
    public static SELECTED_SETI_CONTACT_EVENT: string = "onSelectedSetiContactEvent";

    btnOpenSetiContactId: string;
    wndSetiContactSelId: string;
    setiContactEnabledId: boolean;
    metadataEditId: string;
    metadataAddId: string;
    fascicleInsertCommonIdEvent: string;
    fascicleEditCommonIdEvent: string;

    private _btnOpenSetiContact: Telerik.Web.UI.RadButton;
    private _wndSetiContactSel: Telerik.Web.UI.RadWindow;

    private _serviceConfigurations: ServiceConfiguration[];
    private _serviceConfiguration: ServiceConfiguration;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => { });
    }

    initialize() {
        this._btnOpenSetiContact = <Telerik.Web.UI.RadButton>$find(this.btnOpenSetiContactId);

        this._wndSetiContactSel = <Telerik.Web.UI.RadWindow>$find(this.wndSetiContactSelId);
        this._wndSetiContactSel.add_close(this.onSetiContactWindowClosed);
        if (this._btnOpenSetiContact) {
            this._btnOpenSetiContact.add_clicked(this.showSetiContactWindow);
        }


        $("#".concat(this.btnOpenSetiContactId)).data(this.btnOpenSetiContactId, this);

        $("#".concat(this.btnOpenSetiContactId)).on(uscSetiContactSel.SHOW_SETI_CONTACT_BUTTON, (sender, args) => {
            this._btnOpenSetiContact.set_visible(args);
        });

        $("#".concat(this.btnOpenSetiContactId)).data(this);
    }


    onSetiContactWindowClosed = (sender: any, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument()) {
            $("#".concat(this.metadataAddId)).triggerHandler(uscSetiContactSel.SELECTED_SETI_CONTACT_EVENT, args.get_argument());
            $("#".concat(this.metadataEditId)).triggerHandler(uscSetiContactSel.SELECTED_SETI_CONTACT_EVENT, args.get_argument());
            $("#".concat(this.fascicleInsertCommonIdEvent)).triggerHandler(uscSetiContactSel.SELECTED_SETI_CONTACT_EVENT, args.get_argument());
            $("#".concat(this.fascicleEditCommonIdEvent)).triggerHandler(uscSetiContactSel.SELECTED_SETI_CONTACT_EVENT, args.get_argument());
        }
    }

    public triggerButtonsVisibility(args) {
        this._btnOpenSetiContact.set_visible(args);
    }

    showSetiContactWindow = (sender: any, args: any) => {
        this._wndSetiContactSel.set_navigateUrl(`../UserControl/CommonSetiContactSel.aspx`);
        this._wndSetiContactSel.set_width(760);
        this._wndSetiContactSel.set_height(400);

        this._wndSetiContactSel.show();
    }

}

export = uscSetiContactSel;