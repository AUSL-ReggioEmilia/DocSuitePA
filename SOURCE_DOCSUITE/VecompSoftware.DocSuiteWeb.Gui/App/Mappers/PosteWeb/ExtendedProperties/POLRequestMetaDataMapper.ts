import BaseMapper = require("App/Mappers/BaseMapper");
import POLRequestMetaData = require("App/Models/PosteWeb/ExtendedProperties/POLRequestMetaData");

class POLRequestMetaDataMapper extends BaseMapper<POLRequestMetaData>
{
    constructor() {
        super();
    }

    public Map(source: any): POLRequestMetaData {
        let toMap: POLRequestMetaData = <POLRequestMetaData>{};

        if (!source) {
            return null;
        }

        toMap.PushCalledAt = source.PushCalledAt;
        toMap.LastGetStatusAt = source.LastGetStatusAt;
        toMap.DoneWithGetStatus = source.DoneWithGetStatus;
        toMap.DocumentFromUrlCpfSaved = source.DocumentFromUrlCpfSaved;
        toMap.DocumentFromUrlCpfXmlSaved = source.DocumentFromUrlCpfXmlSaved;
        toMap.PolRequestContactName = source.PolRequestContactName;
        toMap.PolAccountName = source.PolAccountName;

        return toMap;
    }
}

export = POLRequestMetaDataMapper