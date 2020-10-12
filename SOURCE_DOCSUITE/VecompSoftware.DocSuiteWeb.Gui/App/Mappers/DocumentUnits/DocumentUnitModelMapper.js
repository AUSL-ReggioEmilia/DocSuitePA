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
define(["require", "exports", "App/Mappers/BaseMapper", "App/Models/UDS/UDSRepositoryModel", "App/Mappers/Commons/CategoryModelMapper", "App/Mappers/Commons/ContainerModelMapper", "App/Mappers/UDS/UDSRepositoryModelMapper", "./DocumentUnitRoleMapper", "./DocumentUnitChainMapper"], function (require, exports, BaseMapper, UDSRepositoryModel, CategoryModelMapper, ContainerModelMapper, UDSRepositoryModelMapper, DocumentUnitRoleMapper, DocumentUnitChainMapper) {
    var DocumentUnitModelMapper = /** @class */ (function (_super) {
        __extends(DocumentUnitModelMapper, _super);
        function DocumentUnitModelMapper() {
            return _super.call(this) || this;
        }
        DocumentUnitModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.Environment = source.Environment;
            toMap.UniqueId = source.UniqueId;
            toMap.EntityId = source.EntityId;
            toMap.DocumentUnitName = source.DocumentUnitName;
            toMap.Year = source.Year;
            toMap.Number = source.Number;
            toMap.Title = source.Title;
            toMap.ReferenceType = source.ReferenceType;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.Subject = source.Subject;
            toMap.Category = source.Category ? new CategoryModelMapper().Map(source.Category) : null;
            toMap.Container = source.Container ? new ContainerModelMapper().Map(source.Container) : null;
            toMap.UDSRepository = source.UDSRepository ? new UDSRepositoryModelMapper().Map(source.UDSRepository) : null;
            toMap.DocumentUnitRoles = source.DocumentUnitRoles ? new DocumentUnitRoleMapper().MapCollection(source.DocumentUnitRoles) : null;
            toMap.DocumentUnitChains = source.DocumentUnitChains ? new DocumentUnitChainMapper().MapCollection(source.DocumentUnitChains) : null;
            if (source.IdUDSRepository && !toMap.UDSRepository) {
                toMap.UDSRepository = new UDSRepositoryModel();
                toMap.UDSRepository.UniqueId = source.IdUDSRepository;
            }
            toMap.IdFascicle = source.IdFascicle;
            toMap.IsFascicolable = source.IsFascicolable;
            return toMap;
        };
        return DocumentUnitModelMapper;
    }(BaseMapper));
    return DocumentUnitModelMapper;
});
//# sourceMappingURL=DocumentUnitModelMapper.js.map