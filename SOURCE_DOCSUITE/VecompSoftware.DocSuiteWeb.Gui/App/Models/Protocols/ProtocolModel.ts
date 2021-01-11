
import CategoryModel = require('App/Models/Commons/CategoryModel');
import ContainerModel = require('App/Models/Commons/ContainerModel');
import DocumentSeriesItemModel = require('App/Models/DocumentArchives/DocumentSeriesItemModel');
import MessageModel = require('App/Models/Messages/MessageModel');

interface ProtocolModel {
    Year?: number;
    Number?: number;
    Object?: string;
    ObjectChangeReason?: string;
    DocumentDate?: Date;
    DocumentProtocol?: string;
    IdDocument?: number;
    IdAttachments?: number;
    DocumentCode?: string;
    IdStatus?: number;
    LastChangedReason?: string;
    AlternativeRecipient?: string;
    CheckPublication?: string;
    JournalDate?: Date;
    ConservationStatus?: string;
    LastConservationDate?: Date;
    HasConservatedDocs?: boolean;
    IdAnnexed?: string;
    HandlerDate?: Date;
    Modified?: boolean;
    IdHummingBird?: number;
    ProtocolCheckDate?: Date;
    TdIdDocument?: number;
    TDError?: string;
    DocAreaStatus?: number;
    DocAreaStatusDesc?: string;
    IdAttachLocation?: number;
    IdProtocolKind?: number;
    IdProtocolJournalLog?: number;
    UniqueId: string;
    RegistrationDate: Date;
    RegistrationUser: Date;
    Category: CategoryModel;
    Container: ContainerModel;
    DocumentSeriesItems: DocumentSeriesItemModel[];
    Messages: MessageModel[];
}

export = ProtocolModel;