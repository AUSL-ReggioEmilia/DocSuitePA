import BaseMapper = require("App/Mappers/BaseMapper");
import POLRequestExtendedProperties = require("App/Models/PosteWeb/ExtendedProperties/POLRequestExtendedProperties");
import PRETNoticePushResponseMapper = require("./PRETNoticePushResponseMapper");
import PRETNoticeGetStatusResponseMapper = require("./PRETNoticeGetStatusResponseMapper");
import ExceptionInfoMapper = require("./ExceptionInfoMapper");
import POLRequestMetaDataMapper = require("./POLRequestMetaDataMapper");

class POLRequestExtendedPropertiesMapper extends BaseMapper<POLRequestExtendedProperties>{

    constructor() {
        super();
    }

    public Map(source: any): POLRequestExtendedProperties {
        let toMap: POLRequestExtendedProperties = <POLRequestExtendedProperties>{};

        if (!source) {
            return null;
        }

        toMap.PushResponse = (new PRETNoticePushResponseMapper()).Map(source.PushResponse);
        toMap.GetStatus = (new PRETNoticeGetStatusResponseMapper()).Map(source.GetStatus);
        toMap.IsFaulted = source.IsFaulted;
        toMap.ExceptionInfo = (new ExceptionInfoMapper()).Map(source.ExceptionInfo);
        toMap.MetaData = (new POLRequestMetaDataMapper()).Map(source.Metadata);

        return toMap;
    }

}

export = POLRequestExtendedPropertiesMapper