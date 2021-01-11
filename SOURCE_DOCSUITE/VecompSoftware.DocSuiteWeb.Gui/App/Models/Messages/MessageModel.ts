import MessageContactModel = require("./MessageContactModel");
import MessageEmailModel = require("./MessageEmailModel");

interface MessageModel {
    EntityId: number;
    Status: string;
    UniqueId: string;
    RegistrationDate: Date;
    RegistrationUser: string;
    MessageContacts: MessageContactModel[];
    MessageEmails: MessageEmailModel[];
}

export = MessageModel;