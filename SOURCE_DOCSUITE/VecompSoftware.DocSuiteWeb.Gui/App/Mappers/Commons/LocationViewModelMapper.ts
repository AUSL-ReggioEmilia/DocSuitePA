import LocationViewModel = require('App/ViewModels/Commons/LocationViewModel');
import IMapper = require('App/Mappers/IMapper');

class LocationViewModelMapper implements IMapper<LocationViewModel> {
    constructor() {
    }
    public Map(source: any): LocationViewModel {
        let toMap: LocationViewModel = <LocationViewModel>{};

        if (!source) {
            return null;
        }

        toMap.EntityShortId = source.EntityShortId;
        toMap.Name = source.Name;
        toMap.ProtocolArchive = source.ProtocolArchive;
        toMap.DossierArchive = source.DossierArchive;
        toMap.ResolutionArchive = source.ResolutionArchive;
        toMap.ConservationArchive = source.ConservationArchive;
        toMap.UniqueId = source.UniqueId;
        toMap.RegistrationUser = source.RegistrationUser;
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.LastChangetUser = source.LastChangetUser;
        toMap.LastChangedDate = source.LastChangedDate;

        return toMap;
    }
}

export = LocationViewModelMapper;