import MessageModel = require("../Messages/MessageModel");
import DocumentSeriesItemLinksModel = require("./DocumentSeriesItemLinksModel");
import ProtocolModel = require("../Protocols/ProtocolModel");

interface DocumentSeriesItem {
    UniqueId: string;
    Year: number;
    Number: number;
    Status: string;
    Subject: string;
    RegistrationDate: Date;
    RegistrationUser: string;
    RetireDate: Date;
    PublishingDate: Date;
    EntityId: string;
    Messages: MessageModel[];
    DocumentSeriesItemLinks: DocumentSeriesItemLinksModel[];
    Protocols: ProtocolModel[];
}

export = DocumentSeriesItem;