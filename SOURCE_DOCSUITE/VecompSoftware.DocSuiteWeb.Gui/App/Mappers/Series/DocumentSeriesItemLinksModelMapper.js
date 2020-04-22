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
    var DocumentSeriesItemLinksModelMapper = /** @class */ (function (_super) {
        __extends(DocumentSeriesItemLinksModelMapper, _super);
        function DocumentSeriesItemLinksModelMapper() {
            return _super.call(this) || this;
        }
        DocumentSeriesItemLinksModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.EntityId = source.EntityId;
            toMap.LinkType = source.LinkType;
            toMap.EntityShortId = source.EntityShortId;
            toMap.UniqueId = source.UniqueId;
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.Resolution = source.Resolution;
            return toMap;
        };
        return DocumentSeriesItemLinksModelMapper;
    }(BaseMapper));
    return DocumentSeriesItemLinksModelMapper;
});
//# sourceMappingURL=DocumentSeriesItemLinksModelMapper.js.map