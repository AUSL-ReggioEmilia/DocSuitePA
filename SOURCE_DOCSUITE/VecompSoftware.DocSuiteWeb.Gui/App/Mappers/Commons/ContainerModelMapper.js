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
define(["require", "exports", "App/Mappers/BaseMapper"], function (require, exports, BaseMapper) {
    var ContainerModelMapper = /** @class */ (function (_super) {
        __extends(ContainerModelMapper, _super);
        function ContainerModelMapper() {
            return _super.call(this) || this;
        }
        ContainerModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.EntityShortId = source.EntityShortId;
            toMap.Name = source.Name;
            toMap.Note = source.Note;
            toMap.isActive = source.isActive;
            toMap.Massive = source.Massive;
            toMap.Conservation = source.Conservation;
            toMap.DocumentSeriesAnnexedLocation = source.DocumentSeriesAnnexedLocation;
            toMap.DocumentSeriesLocation = source.DocumentSeriesLocation;
            toMap.DocumentSeriesUnpublishedAnnexedLocation = source.DocumentSeriesUnpublishedAnnexedLocation;
            toMap.ProtocolRejection = source.ProtocolRejection;
            toMap.ActiveFrom = source.ActiveFrom;
            toMap.ActiveTo = source.ActiveTo;
            toMap.idArchive = source.idArchive;
            toMap.Privacy = source.Privacy;
            toMap.HeadingFrontalino = source.HeadingFrontalino;
            toMap.HeadingLetter = source.HeadingLetter;
            toMap.ProtAttachLocation = source.ProtAttachLocation;
            toMap.ProtLocation = source.ProtLocation;
            toMap.ReslLocation = source.ReslLocation;
            return toMap;
        };
        return ContainerModelMapper;
    }(BaseMapper));
    return ContainerModelMapper;
});
//# sourceMappingURL=ContainerModelMapper.js.map