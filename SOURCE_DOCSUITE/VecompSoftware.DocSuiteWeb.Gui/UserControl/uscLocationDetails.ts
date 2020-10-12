import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import EnumHelper = require("App/Helpers/EnumHelper");
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import LocationService = require("App/Services/Commons/LocationService");
import LocationViewModel = require("App/ViewModels/Commons/LocationViewModel");

class uscLocationDetails {
    lblNameId: string;
    lblArchiveProtocolId: string;
    lblArchiveDossierId: string;
    lblArchiveAttiId: string;
    pnlDetailsId: string;

    private _lblName: HTMLLabelElement;
    private _lblArchiveProtocol: HTMLLabelElement;
    private _lblArchiveDossier: HTMLLabelElement;
    private _lblArchiveAtti: HTMLLabelElement;

    private locationModel: LocationViewModel;
    private _serviceConfigurations: ServiceConfiguration[];
    private _enumHelper: EnumHelper;
    private _locationService: LocationService;

    public static selectedLocationUniqueId: string;

    private static LOCATION_SERVICE = "Location";

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._enumHelper = new EnumHelper();
    }

    initialize(): void {
        let locationConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, uscLocationDetails.LOCATION_SERVICE);
        this._locationService = new LocationService(locationConfiguration);
        
        $(`#${this.pnlDetailsId}`).hide();
        this._lblName = <HTMLLabelElement>document.getElementById(this.lblNameId);
        this._lblArchiveProtocol = <HTMLLabelElement>document.getElementById(this.lblArchiveProtocolId);
        this._lblArchiveDossier = <HTMLLabelElement>document.getElementById(this.lblArchiveDossierId);
        this._lblArchiveAtti = <HTMLLabelElement>document.getElementById(this.lblArchiveAttiId);

        this.bindLoaded();
    }

    clearLocationDetails(): void {
        this._lblName.innerText = "";
        this._lblArchiveProtocol.innerText = "";
        this._lblArchiveDossier.innerText = "";
        this._lblArchiveAtti.innerText = "";
    }

    setLocationDetails(): void {
        this._locationService.getLocationDetailsByUniqueId(uscLocationDetails.selectedLocationUniqueId, (data) => {
            this.locationModel = data;
            this._lblName.innerText = `${this.locationModel.Name} (${this.locationModel.EntityShortId} - ${this.locationModel.UniqueId})`;
            this._lblArchiveProtocol.innerText = this.locationModel.ProtocolArchive;
            this._lblArchiveDossier.innerText = this.locationModel.DossierArchive;
            this._lblArchiveAtti.innerText = this.locationModel.ResolutionArchive;
        });
    }

    private bindLoaded(): void {
        $(`#${this.pnlDetailsId}`).data(this);
    }
}

export = uscLocationDetails;