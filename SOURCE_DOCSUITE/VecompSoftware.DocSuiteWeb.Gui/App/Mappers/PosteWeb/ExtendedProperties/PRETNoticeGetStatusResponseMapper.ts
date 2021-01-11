import BaseMapper = require("App/Mappers/BaseMapper");
import PRETNoticeGetStatusResponse = require("App/Models/PosteWeb/ExtendedProperties/PRETNoticeGetStatusResponse");

class PRETNoticeGetStatusResponseMapper extends BaseMapper<PRETNoticeGetStatusResponse>{

    constructor() {
        super();
    }

    public Map(source: any): PRETNoticeGetStatusResponse {
        let toMap: PRETNoticeGetStatusResponse = <PRETNoticeGetStatusResponse>{};

        if (!source) {
            return null;
        }

        toMap.ReturnCode = source.ReturnCode;
        toMap.Message = source.Message;
        toMap.TimeStamp = source.TimeStamp;
        toMap.FatalErrorType = source.FatalErrorType;

        toMap.UrltNotice = source.UrltNotice;
        toMap.UrltNotice_xml = source.UrltNotice_xml;
        toMap.UrlAccept = source.UrlAccept;
        toMap.UrlCPF = source.UrlCPF;
        toMap.UrlCPF_xml = source.UrlCPF_xml;

        toMap.StatusDescription = source.StatusDescription;
        toMap.StatusCodeType = source.StatusCodeType;

        return toMap;
    }
    
}

export = PRETNoticeGetStatusResponseMapper