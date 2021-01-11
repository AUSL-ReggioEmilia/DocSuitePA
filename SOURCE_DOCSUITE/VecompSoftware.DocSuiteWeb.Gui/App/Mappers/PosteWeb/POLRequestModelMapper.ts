import BaseMapper = require('App/Mappers/BaseMapper');
import POLRequestExtendedPropertiesMapper = require("./ExtendedProperties/POLRequestExtendedProperties");
import POLRequest = require("App/Models/PosteWeb/POLRequest");
import POLRequestHelper = require('App/Models/PosteWeb/ExtendedProperties/POLRequestHelper');
import StatusColor = require('App/Models/PosteWeb/StatusColor');

class POLRequestModelMapper extends BaseMapper<POLRequest>{

    constructor() {
        super();
    }

    public Map(source: any): POLRequest {
        let toMap: POLRequest = <POLRequest>{};

        if (!source) {
            return null;
        }

        toMap.Id = source.UniqueId;
        toMap.DocumentName = source.DocumentName;
        toMap.DocumentPosteFileType = source.DocumentPosteFileType;
        toMap.ErrorMessage = source.ErrorMessage;
        toMap.ExtendedProperties = source.ExtendedProperties;
        toMap.GuidPoste = source.GuidPoste;
        toMap.IdArchiveChain = source.IdArchiveChain;
        toMap.IdArchiveChainPoste = source.IdArchiveChainPoste;
        toMap.IdOrdine = source.IdOrdine;
        toMap.RequestId = source.UniqueId;

        toMap.Status = source.Status;
        toMap.StatusDescription = source.StatusDescription;
        toMap.TotalCost = source.TotalCost;
        toMap.RegistrationDate = moment(source.RegistrationDate).format("DD/MM/YYYY");

        let extendedPropsJson: any = JSON.parse(toMap.ExtendedProperties);

        toMap.ExtendedPropertiesDeserialized = (new POLRequestExtendedPropertiesMapper()).Map(extendedPropsJson);

        if (toMap.ExtendedPropertiesDeserialized) {
            toMap.DisplayColor = POLRequestHelper.DetermineStatusColor(toMap.ExtendedPropertiesDeserialized);
        } else {
            toMap.DisplayColor = StatusColor.Blue;
        }


        return toMap;
    }

}

export = POLRequestModelMapper