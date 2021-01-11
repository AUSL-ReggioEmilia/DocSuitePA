import SignRequestType = require("./SignRequestType");
import ProviderSignType = require("./ProviderSignType");

class CustomProperties {
    CertificateId: string;
    DelegatedDomain: string;
    DelegatedPassword: string;
    DelegatedUser: string;
    OTPAuthType: string;
    User: string;
    ProviderType: ProviderSignType;
    RequestType: SignRequestType;
}

export = CustomProperties;