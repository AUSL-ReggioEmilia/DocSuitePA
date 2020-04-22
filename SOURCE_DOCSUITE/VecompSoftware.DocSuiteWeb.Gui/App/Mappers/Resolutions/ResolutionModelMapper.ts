import ResolutionModel = require('App/Models/Resolutions/ResolutionModel');
import CategoryModelMapper = require('App/Mappers/Commons/CategoryModelMapper');
import ContainerModelMapper = require('App/Mappers/Commons/ContainerModelMapper');
import BaseMapper = require('App/Mappers/BaseMapper');

class ResolutionModelMapper extends BaseMapper<ResolutionModel> {
    
    constructor() {
        super();
    }

    public Map(source: any): ResolutionModel {
        let toMap: ResolutionModel = new ResolutionModel();

        if (!source) {
            return null;
        }

        toMap.EntityId = source.EntityId;
        toMap.AdoptionDate = source.AdoptionDate;
        toMap.AlternativeAssignee = source.AlternativeAssignee;
        toMap.AlternativeManager = source.AlternativeManager;
        toMap.AlternativeProposer = source.AlternativeProposer;
        toMap.AlternativeRecipient = source.AlternativeRecipient;
        toMap.ConfirmDate = source.ConfirmDate;
        toMap.EffectivenessDate = source.EffectivenessDate;
        toMap.LeaveDate = source.LeaveDate;
        toMap.Number = source.Number;
        toMap.IdType = source.IdType;
        toMap.ProposeDate = source.ProposeDate;
        toMap.PublishingDate = source.PublishingDate;
        toMap.ResponseDate = source.ResponseDate;
        toMap.ServiceNumber = source.ServiceNumber;
        toMap.InclusiveNumber = source.InclusiveNumber;
        toMap.Object = source.Object;
        toMap.WaitDate = source.WaitDate;
        toMap.WarningDate = source.WarningDate;
        toMap.WorkflowType = source.WorkflowType;
        toMap.Year = source.Year;
        toMap.ProposeUser = source.ProposeUser;
        toMap.LeaveUser = source.LeaveUser;
        toMap.EffectivenessUser = source.EffectivenessUser;
        toMap.ResponseUser = source.ResponseUser;
        toMap.WaitUser = source.WaitUser;
        toMap.ConfirmUser = source.ConfirmUser;
        toMap.WarningUser = source.WarningUser;
        toMap.PublishingUser = source.PublishingUser;
        toMap.AdoptionUser = source.AdoptionUser;
        toMap.UniqueId = source.UniqueId;
        toMap.Category = source.Category ? new CategoryModelMapper().Map(source.Category): null;
        toMap.Container = source.Container ? new ContainerModelMapper().Map(source.Container) : null;
        toMap.Messages = source.Messages;

        return toMap;
    }
}

export = ResolutionModelMapper;