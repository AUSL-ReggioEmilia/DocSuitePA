import UDSLogViewModel = require('App/ViewModels/UDS/UDSLogViewModel');
import IMapper = require('App/Mappers/IMapper');
import EnumHelper = require('App/Helpers/EnumHelper');

class UDSLogViewModelMapper implements IMapper<UDSLogViewModel>{

    private _enumHelper: EnumHelper;

    constructor() {
        this._enumHelper = new EnumHelper();
    }
    public Map(source: any): UDSLogViewModel {
        let toMap: UDSLogViewModel = <UDSLogViewModel>{};

        if (!source) {
            return null;
        }

        toMap.Computer = source.SystemComputer;
        toMap.Description = source.LogDescription;
        toMap.TypeDescription = this._enumHelper.getUDSLogType(source.LogType);
        toMap.LogDate =moment(source.RegistrationDate).format("L").concat(" ").concat(moment(source.RegistrationDate).format("LTS")); 
        toMap.LogUser = source.RegistrationUser;
        toMap.IdUDS = source.IdUDS;

        return toMap;
    }
}

export = UDSLogViewModelMapper;