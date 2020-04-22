/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ResolutionKindModel = require('App/Models/Resolutions/ResolutionKindModel');

class UscResolutionKindDetails {
    lblStatusId: string;
    pnlInformationsId: string;

    private get labelStatusControl(): JQuery {
        return $("#".concat(this.lblStatusId));
    }

    private _serviceConfigurations: ServiceConfiguration[];

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    /**
     *------------------------- Methods -----------------------------
     */

    initialize() {
        this.bindLoaded();
    }

    private bindLoaded() {
        $("#".concat(this.pnlInformationsId)).data(this);
    }

    loadDetails(resolutionKind: ResolutionKindModel): void {
        if (!resolutionKind) {
            return;
        }

        this.labelStatusControl.html((resolutionKind.IsActive) ? "Attivo" : "Disattivo");
    }
}
export = UscResolutionKindDetails;