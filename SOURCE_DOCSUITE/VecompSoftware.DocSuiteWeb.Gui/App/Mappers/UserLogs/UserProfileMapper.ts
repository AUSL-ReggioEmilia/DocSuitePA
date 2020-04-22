import UserProfileModel = require("App/Models/UserLogs/UserProfileModel");
import BaseMapper = require('App/Mappers/BaseMapper');

class UserProfileMapper extends BaseMapper<UserProfileModel>{
    constructor() {
        super();
    }

    public Map(source: any): UserProfileModel {
        let toMap: UserProfileModel = new UserProfileModel();

        if (!source) {
            return null;
        }

        toMap.DefaultProvider = source.DefaultProvider;
        toMap.ArubaAutomatic = source.Value.ArubaAutomatic;
        toMap.ArubaRemote = source.Value.ArubaRemote;
        toMap.Smartcard = source.Value.Smartcard;
        toMap.InfocertAutomatic = source.Value.InfocertAutomatic;
        toMap.InfocertRemote = source.Value.InfocertRemote;

        return toMap;
    }
}

export = UserProfileMapper;