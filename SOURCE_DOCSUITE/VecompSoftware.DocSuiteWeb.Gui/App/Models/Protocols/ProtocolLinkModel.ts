import ProtocolModel = require('App/Models/Protocols/ProtocolModel');

interface ProtocolLinkModel {
    Year?: number;
    Number?: number;
    UniqueId: string;
    RegistrationDate: Date;
    RegistrationUser: Date;
    ProtocolLinked: ProtocolModel;
}
export = ProtocolLinkModel;