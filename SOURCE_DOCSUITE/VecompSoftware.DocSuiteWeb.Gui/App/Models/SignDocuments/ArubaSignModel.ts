import SignRequestType = require("./SignRequestType");
import SignType = require("./SignType");

class ArubaSignModel {
    ServiceName: string;
    DelegatedDomain: string;
    DelegatedPassword: string;
    DelegatedUser: string;
    OTPPassword: string;
    OTPAuthType: string;
    User: string;
    UserPassword: string;
    CertificateId: string;
    RequestType: SignRequestType;
    SignType: SignType;
}

export = ArubaSignModel;