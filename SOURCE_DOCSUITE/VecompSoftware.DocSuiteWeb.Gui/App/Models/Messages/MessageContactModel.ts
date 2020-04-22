import MessageContactPosition = require("./MessageContactPosition");

interface MessageContactModel {
    EntityId: number;
    ContactType: string;
    Description: string;
    UniqueId: string;
    RegistrationUser: string;
    RegistrationDate: Date;
    EntityShortId: number;
    ContactPosition: MessageContactPosition
}
export = MessageContactModel;