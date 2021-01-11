import UscAmmTraspMonitorLog = require('UserControl/uscAmmTraspMonitorLog');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import TransparentAdministrationMonitorLogBase = require('Monitors/TransparentAdministrationMonitorLogBase');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');

class Item extends TransparentAdministrationMonitorLogBase {

    uscAmmTraspMonitorLogId: string;
    btnNuovoMonitoraggioId: string;
    private _btnNuovo: HTMLButtonElement;
    private _windowNuovo: Telerik.Web.UI.RadWindow;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, TransparentAdministrationMonitorLogBase.TransparentAdministrationMonitorLog_TYPE_NAME));
        $(document).ready(() => {

        });
    }

    initialize() {
        this._btnNuovo = <HTMLButtonElement>document.getElementById(this.btnNuovoMonitoraggioId);
    }

    showWindow(): void {
        this._windowNuovo = <Telerik.Web.UI.RadWindow>$find(this.uscAmmTraspMonitorLogId);
        this._windowNuovo.show();
    }
}

export = Item;