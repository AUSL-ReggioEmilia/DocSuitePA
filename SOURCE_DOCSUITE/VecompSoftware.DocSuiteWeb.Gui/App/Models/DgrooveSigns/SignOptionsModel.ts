import SignType = require("App/Models/DgrooveSigns/SignType");

class SignOptionsModel {
    SignReason?: string;
    SignTime?: string;
    SignType?: SignType;
    IsSignatureVisible: boolean;
    SignPositionX?: number;
    SignPositionY?: number;
    SignWidth?: number;
    SignHeight?: number; 
}

export = SignOptionsModel;