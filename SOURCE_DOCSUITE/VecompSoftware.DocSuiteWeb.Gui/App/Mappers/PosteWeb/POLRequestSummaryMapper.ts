import BaseMapper = require('App/Mappers/BaseMapper');
import POLRequestExtendedPropertiesMapper = require("./ExtendedProperties/POLRequestExtendedProperties");
import NullSafe = require('App/Helpers/NullSafe');
import TNoticeStatusSummaryDTO = require('App/DTOs/TNoticeStatusSummaryDTO');
import POLRequestHelper = require('App/Models/PosteWeb/ExtendedProperties/POLRequestHelper');
import StatusColor = require('App/Models/PosteWeb/StatusColor');

class POLRequestSummaryMapper extends BaseMapper<TNoticeStatusSummaryDTO>{

    constructor() {
        super();
    }

    public Map(source: any): TNoticeStatusSummaryDTO {
        let toMap: TNoticeStatusSummaryDTO = <TNoticeStatusSummaryDTO>{};

        if (!source) {
            return null;
        }
        if (source.ExtendedProperties) {
            //the source is a POLRequest object
            let extendedPropsJson: any = JSON.parse(source.ExtendedProperties);
            let extendedPropertiesDeserialized = (new POLRequestExtendedPropertiesMapper()).Map(extendedPropsJson);

            if (extendedPropertiesDeserialized) {
                toMap.Status = NullSafe.Do<string>(() => extendedPropertiesDeserialized.GetStatus.StatusDescription, "");
                toMap.DisplayColor = POLRequestHelper.DetermineStatusColor(extendedPropertiesDeserialized);
            }
        }

        if (!toMap.Status || toMap.Status === "") {
            toMap.Status = source.StatusDescription;
            toMap.DisplayColor = StatusColor.Blue;
        }

        toMap.RegistrationDate = moment(source.RegistrationDate).format("DD/MM/YYYY");
        toMap.RequestUniqueId = source.UniqueId;

        return toMap;
    }

}

export = POLRequestSummaryMapper