import CustomPropertiesModel = require("App/Models/SignDocuments/CustomPropertiesModel");
import StorageInformationType = require("App/Models/SignDocuments/StorageInformationType");
import ProviderSignType = require("App/Models/SignDocuments/ProviderSignType");

class SignModel {
    Alias: string;
    CustomProperties: CustomPropertiesModel;
    IsDefault: boolean;
    OTP: string;
    PIN: string;
    Password: string;
    RemoteSignType: ProviderSignType;
    StorageInformationType: StorageInformationType;
}

export = SignModel;