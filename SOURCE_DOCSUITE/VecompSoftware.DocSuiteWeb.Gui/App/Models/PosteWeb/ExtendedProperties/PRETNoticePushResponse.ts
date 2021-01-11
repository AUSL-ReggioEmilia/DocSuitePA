import TNoticeReturnCodeType = require("./TNoticeReturnCodeType");
import TNoticeFatalErrorType = require("./TNoticeFatalErrorType");

interface PRETNoticePushResponse {
    ReturnCode: TNoticeReturnCodeType,
    Message: string,
    Timestamp: Date,
    Reference: string,
    FatalError : TNoticeFatalErrorType
}

export = PRETNoticePushResponse