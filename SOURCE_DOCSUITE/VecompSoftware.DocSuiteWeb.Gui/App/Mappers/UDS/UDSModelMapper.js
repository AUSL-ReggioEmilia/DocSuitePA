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
define(["require", "exports", "App/Mappers/Commons/CategoryModelMapper", "App/Mappers/Commons/ContainerModelMapper", "App/Mappers/BaseMapper", "App/Mappers/UDS/UDSDocumentModelMapper"], function (require, exports, CategoryModelMapper, ContainerModelMapper, BaseMapper, UDSDocumentModelMapper) {
    var UDSModelMapper = /** @class */ (function (_super) {
        __extends(UDSModelMapper, _super);
        function UDSModelMapper() {
            return _super.call(this) || this;
        }
        UDSModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.UniqueId = source.UDSId;
            toMap.Year = source._year;
            toMap.Number = source._number;
            toMap.Subject = source._subject;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.Category = source.Category ? new CategoryModelMapper().Map(source.Category) : null;
            toMap.Container = source.Container ? new ContainerModelMapper().Map(source.Container) : null;
            toMap.Documents = source.Documents && source.Documents.$values ? new UDSDocumentModelMapper().MapCollection(source.Documents.$values) : null;
            return toMap;
        };
        return UDSModelMapper;
    }(BaseMapper));
    return UDSModelMapper;
});
//# sourceMappingURL=UDSModelMapper.js.map