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
    var DocumentSeriesItemModelMapper = /** @class */ (function (_super) {
        __extends(DocumentSeriesItemModelMapper, _super);
        function DocumentSeriesItemModelMapper() {
            return _super.call(this) || this;
        }
        DocumentSeriesItemModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.UniqueId = source.UniqueId;
            toMap.Year = source.Year;
            toMap.Number = source.Number;
            toMap.Status = source.Status;
            toMap.Subject = source.Subject;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.RetireDate = source.RetireDate;
            toMap.PublishingDate = source.PublishingDate;
            toMap.EntityId = source.EntityId;
            toMap.Messages = source.Messages;
            toMap.DocumentSeriesItemLinks = source.DocumentSeriesItemLinks;
            toMap.Protocols = source.Protocols;
            return toMap;
        };
        return DocumentSeriesItemModelMapper;
    }(BaseMapper));
    return DocumentSeriesItemModelMapper;
});
//# sourceMappingURL=DocumentSeriesItemModelMapper.js.map