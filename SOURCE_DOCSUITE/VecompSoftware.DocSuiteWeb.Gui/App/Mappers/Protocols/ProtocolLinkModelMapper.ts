import BaseMapper = require('App/Mappers/BaseMapper');
import CategoryModelMapper = require('App/Mappers/Commons/CategoryModelMapper');
import ContainerModelMapper = require('App/Mappers/Commons/ContainerModelMapper');
import ProtocolLinkModel = require('App/Models/Protocols/ProtocolLinkModel');

class ProtocolLinkModelMapper extends BaseMapper<ProtocolLinkModel> {
    constructor() {
        super();
    }

    public Map(source: any): ProtocolLinkModel {
        let toMap: ProtocolLinkModel = <ProtocolLinkModel>{};

        if (!source) {
            return null;
        }

        toMap.Number = source.Number;
        toMap.Year = source.Year;
        toMap.UniqueId = source.UniqueId;
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.RegistrationUser = source.RegistrationUser;
        toMap.ProtocolLinked = source.ProtocolLinked;

        return toMap;
    }
}
export = ProtocolLinkModelMapper;