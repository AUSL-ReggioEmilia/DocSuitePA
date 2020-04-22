import DossierLogViewModel = require('App/ViewModels/Dossiers/DossierLogViewModel');
import IMapper = require('App/Mappers/IMapper');
import EnumHelper = require('App/Helpers/EnumHelper');

class DossierLogViewModelMapper implements IMapper<DossierLogViewModel>{

    private _enumHelper: EnumHelper;

    constructor() {
        this._enumHelper = new EnumHelper();
    }
    public Map(source: any): DossierLogViewModel {
        let toMap: DossierLogViewModel = <DossierLogViewModel>{};

        if (!source) {
            return null;
        }

        toMap.Computer = source.SystemComputer;
        toMap.Description = source.LogDescription;
        toMap.TypeDescription = this._enumHelper.getLogTypeDescription(source.LogType);
        toMap.LogDate = moment(source.RegistrationDate).format("L").concat(" ").concat(moment(source.RegistrationDate).format("LTS")); 
        toMap.LogUser = source.RegistrationUser;

        return toMap;
    }
}

export = DossierLogViewModelMapper;