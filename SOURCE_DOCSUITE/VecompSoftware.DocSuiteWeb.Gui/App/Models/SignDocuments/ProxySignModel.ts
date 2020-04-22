import SignRequestType = require("./SignRequestType");
import SignType = require("./SignType");

class ProxySignModel {
    ServiceName: string;
    OTPPassword: string;
    RequestType: SignRequestType;
    SignType: SignType;
    Alias: string;
    PINPassword: string;
}

export = ProxySignModel;