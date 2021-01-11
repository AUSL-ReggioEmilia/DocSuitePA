import TNoticeReturnCodeType = require("./TNoticeReturnCodeType");
import TNoticeFatalErrorType = require("./TNoticeFatalErrorType");
import TNoticeStatusCodeType = require("./TNoticeStatusCodeType");

interface PRETNoticeGetStatusResponse {
    ReturnCode: TNoticeReturnCodeType,
    Message: string,
    TimeStamp: Date,
    FatalErrorType: TNoticeFatalErrorType,
    UrltNotice: string,
    UrltNotice_xml: string,
    UrlAccept: string,
    UrlCPF: string,
    UrlCPF_xml: string,
    StatusDescription: string,
    StatusCodeType: TNoticeStatusCodeType   
}

export = PRETNoticeGetStatusResponse