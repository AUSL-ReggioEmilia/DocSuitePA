import SignModel = require("App/Models/SignDocuments/SignModel");

class UserProfileModel {
    DefaultProvider: number;
    ArubaAutomatic: SignModel;
    ArubaRemote: SignModel;
    InfocertAutomatic: SignModel;
    InfocertRemote: SignModel;
    Smartcard: SignModel;
}

export = UserProfileModel;