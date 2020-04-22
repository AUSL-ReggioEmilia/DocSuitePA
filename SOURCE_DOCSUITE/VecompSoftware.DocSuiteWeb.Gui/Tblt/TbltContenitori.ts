/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');

class TbltContenitori {
    private _serviceConfigurations: ServiceConfiguration[];

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    /**
     *------------------------- Events -----------------------------
     */

    /**
     *------------------------- Methods -----------------------------
     */
    initialize(): void {

    }
}

export = TbltContenitori;