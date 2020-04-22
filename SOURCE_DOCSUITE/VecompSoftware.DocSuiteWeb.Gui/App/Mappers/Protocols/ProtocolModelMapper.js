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
define(["require", "exports", "App/Mappers/BaseMapper", "App/Mappers/Commons/CategoryModelMapper", "App/Mappers/Commons/ContainerModelMapper"], function (require, exports, BaseMapper, CategoryModelMapper, ContainerModelMapper) {
    var ProtocolModelMapper = /** @class */ (function (_super) {
        __extends(ProtocolModelMapper, _super);
        function ProtocolModelMapper() {
            return _super.call(this) || this;
        }
        ProtocolModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.Year = source.Year;
            toMap.Number = source.Number;
            toMap.Object = source.Object;
            toMap.ObjectChangeReason = source.ObjectChangeReason;
            toMap.DocumentDate = source.DocumentDate;
            toMap.DocumentProtocol = source.DocumentProtocol;
            toMap.IdDocument = source.IdDocument;
            toMap.IdAttachments = source.IdAttachments;
            toMap.DocumentCode = source.DocumentCode;
            toMap.IdStatus = source.IdStatus;
            toMap.LastChangedReason = source.LastChangedReason;
            toMap.AlternativeRecipient = source.AlternativeRecipient;
            toMap.CheckPublication = source.CheckPublication;
            toMap.JournalDate = source.JournalDate;
            toMap.ConservationStatus = source.ConservationStatus;
            toMap.LastConservationDate = source.LastConservationDate;
            toMap.HasConservatedDocs = source.HasConservatedDocs;
            toMap.IdAnnexed = source.IdAnnexed;
            toMap.HandlerDate = source.HandlerDate;
            toMap.Modified = source.Modified;
            toMap.IdHummingBird = source.IdHummingBird;
            toMap.ProtocolCheckDate = source.ProtocolCheckDate;
            toMap.TdIdDocument = source.TdIdDocument;
            toMap.TDError = source.TDError;
            toMap.DocAreaStatus = source.DocAreaStatus;
            toMap.DocAreaStatusDesc = source.DocAreaStatusDesc;
            toMap.IdAttachLocation = source.IdAttachLocation;
            toMap.IdProtocolKind = source.IdProtocolKind;
            toMap.IdProtocolJournalLog = source.IdProtocolJournalLog;
            toMap.UniqueId = source.UniqueId;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.Category = source.Category ? new CategoryModelMapper().Map(source.Category) : null;
            toMap.Container = source.Container ? new ContainerModelMapper().Map(source.Container) : null;
            toMap.DocumentSeriesItems = source.DocumentSeriesItems;
            toMap.Messages = source.Messages;
            return toMap;
        };
        return ProtocolModelMapper;
    }(BaseMapper));
    return ProtocolModelMapper;
});
//# sourceMappingURL=ProtocolModelMapper.js.map