import CategoryModel = require('App/Models/Commons/CategoryModel');
import ContainerModel = require('App/Models/Commons/ContainerModel');
import MessageModel = require('../Messages/MessageModel');

class ResolutionModel {
    EntityId: number;
    AdoptionDate: Date;
    AlternativeAssignee: string;
    AlternativeManager: string;
    AlternativeProposer: string;
    AlternativeRecipient: string;
    ConfirmDate: Date;
    EffectivenessDate: Date;
    LeaveDate: Date;
    Number: number;
    IdType: number;
    ProposeDate: Date;
    PublishingDate: Date;
    ResponseDate: Date;
    ServiceNumber: string;
    InclusiveNumber: string;
    Object: string;
    WaitDate: Date;
    WarningDate: Date;
    WorkflowType: string;
    Year: number;
    ProposeUser: string;
    LeaveUser: string;
    EffectivenessUser: string;
    ResponseUser: string;
    WaitUser: string;
    ConfirmUser: string;
    WarningUser: string;
    PublishingUser: string;
    AdoptionUser: string;
    UniqueId: string;
    Category: CategoryModel;
    Container: ContainerModel;
    Messages: MessageModel[];
    constructor() {     
    }
}

export = ResolutionModel;