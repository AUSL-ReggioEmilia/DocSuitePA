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
define(["require", "exports", "App/Mappers/BaseMapper", "App/Mappers/Commons/ContainerModelMapper"], function (require, exports, BaseMapper, ContainerModelMapper) {
    var UDSRepositoryModelMapper = /** @class */ (function (_super) {
        __extends(UDSRepositoryModelMapper, _super);
        function UDSRepositoryModelMapper() {
            return _super.call(this) || this;
        }
        UDSRepositoryModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.UniqueId = source.UniqueId;
            toMap.Name = source.Name;
            toMap.SequenceCurrentYear = source.SequenceCurrentYear;
            toMap.SequenceCurrentNumber = source.SequenceCurrentNumber;
            toMap.ModuleXML = source.ModuleXML;
            toMap.Version = source.Version;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.ActiveDate = source.ActiveDate;
            toMap.ExpiredDate = source.ExpiredDate;
            toMap.DSWEnvironment = source.DSWEnvironment;
            toMap.Alias = source.Alias;
            toMap.Status = source.Status;
            toMap.Container = source.Container ? new ContainerModelMapper().Map(source.Container) : null;
            return toMap;
        };
        return UDSRepositoryModelMapper;
    }(BaseMapper));
    return UDSRepositoryModelMapper;
});
//# sourceMappingURL=UDSRepositoryModelMapper.js.map