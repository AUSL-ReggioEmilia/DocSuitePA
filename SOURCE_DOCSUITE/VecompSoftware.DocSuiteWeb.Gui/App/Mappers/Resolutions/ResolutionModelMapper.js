var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "App/Models/Resolutions/ResolutionModel", "App/Mappers/Commons/CategoryModelMapper", "App/Mappers/Commons/ContainerModelMapper", "App/Mappers/BaseMapper"], function (require, exports, ResolutionModel, CategoryModelMapper, ContainerModelMapper, BaseMapper) {
    var ResolutionModelMapper = /** @class */ (function (_super) {
        __extends(ResolutionModelMapper, _super);
        function ResolutionModelMapper() {
            return _super.call(this) || this;
        }
        ResolutionModelMapper.prototype.Map = function (source) {
            var toMap = new ResolutionModel();
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
            toMap.Category = source.Category ? new CategoryModelMapper().Map(source.Category) : null;
            toMap.Container = source.Container ? new ContainerModelMapper().Map(source.Container) : null;
            toMap.Messages = source.Messages;
            return toMap;
        };
        return ResolutionModelMapper;
    }(BaseMapper));
    return ResolutionModelMapper;
});
//# sourceMappingURL=ResolutionModelMapper.js.map