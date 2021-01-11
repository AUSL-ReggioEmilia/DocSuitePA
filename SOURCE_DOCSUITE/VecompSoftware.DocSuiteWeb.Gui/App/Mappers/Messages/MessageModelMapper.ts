import BaseMapper = require('App/Mappers/BaseMapper');
import MessageModel = require("App/Models/Messages/MessageModel");

class MessageModelMapper extends BaseMapper<MessageModel> {
    constructor() {
        super();
    }

    public Map(source: any): MessageModel {
        let toMap: MessageModel = <MessageModel>{};

        if (!source) {
            return null;
        }

        toMap.EntityId = source.EntityId;
        toMap.Status = source.Status;
        toMap.UniqueId = source.UniqueId;
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.RegistrationUser = source.RegistrationUser;
        toMap.MessageContacts = source.MessageContacts;
        toMap.MessageEmails = source.MessageEmails;

        return toMap;
    }
}
export = MessageModelMapper;