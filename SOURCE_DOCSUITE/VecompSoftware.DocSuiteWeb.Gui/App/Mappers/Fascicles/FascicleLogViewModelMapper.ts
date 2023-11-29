import FascicleLogViewModel = require('App/ViewModels/Fascicles/FascicleLogViewModel');
import IMapper = require('App/Mappers/IMapper');
import EnumHelper = require('App/Helpers/EnumHelper');

class FascicleLogViewModelMapper implements IMapper<FascicleLogViewModel>{

    private _enumHelper: EnumHelper;

    constructor() {
        this._enumHelper = new EnumHelper();
    }
    public Map(source: any): FascicleLogViewModel {
        let toMap: FascicleLogViewModel = <FascicleLogViewModel>{};

        if (!source) {
            return null;
        }

        toMap.UniqueId = source.UniqueId;
        toMap.Computer = source.SystemComputer;
        toMap.Description = source.LogDescription;
        toMap.TypeDescription = this._enumHelper.getFascicleLogTypeDescription(source.LogType);
        toMap.LogDate = moment(source.RegistrationDate).format("L").concat(" ").concat(moment(source.RegistrationDate).format("LTS"));
        toMap.LogUser = source.RegistrationUser;

        return toMap;
    }
}

export = FascicleLogViewModelMapper;