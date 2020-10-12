import BaseMapper = require("App/Mappers/BaseMapper");
import PRETNoticePushResponse = require("App/Models/PosteWeb/ExtendedProperties/PRETNoticePushResponse");

class PRETNoticePushResponseMapper extends BaseMapper<PRETNoticePushResponse>{

    constructor() {
        super();
    }

    public Map(source: any): PRETNoticePushResponse {
        let toMap: PRETNoticePushResponse = <PRETNoticePushResponse>{};

        if (!source) {
            return null;
        }

        toMap.ReturnCode = source.ReturnCode;
        toMap.Message = source.Message;
        toMap.Timestamp = source.TimeStamp;
        toMap.Reference = source.Reference;
        toMap.FatalError = source.FatalError;

        return toMap;
    }
}

export = PRETNoticePushResponseMapper