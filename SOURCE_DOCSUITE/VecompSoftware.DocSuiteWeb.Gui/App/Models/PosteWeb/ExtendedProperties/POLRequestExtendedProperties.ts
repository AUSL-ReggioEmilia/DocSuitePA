import PRETNoticePushResponse = require("./PRETNoticePushResponse");
import PRETNoticeGetStatusResponse = require("./PRETNoticeGetStatusResponse");
import ExceptionInfo = require("./ExceptionInfo");
import POLRequestMetaData = require("./POLRequestMetaData");
import NullSafe = require("App/Helpers/NullSafe");

interface POLRequestExtendedProperties {
    PushResponse: PRETNoticePushResponse,
    GetStatus: PRETNoticeGetStatusResponse,
    IsFaulted: boolean,
    ExceptionInfo: ExceptionInfo,
    MetaData : POLRequestMetaData
}

export = POLRequestExtendedProperties