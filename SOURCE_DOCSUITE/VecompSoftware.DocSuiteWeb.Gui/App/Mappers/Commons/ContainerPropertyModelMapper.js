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
    var ContainerPropertyModelMapper = /** @class */ (function (_super) {
        __extends(ContainerPropertyModelMapper, _super);
        function ContainerPropertyModelMapper() {
            return _super.call(this) || this;
        }
        ContainerPropertyModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.Name = source.Name;
            toMap.ContainerType = source.ContainerType;
            toMap.ValueInt = source.ValueInt;
            toMap.ValueDate = source.ValueDate;
            toMap.ValueDouble = source.ValueDouble;
            toMap.ValueString = source.ValueString;
            toMap.ValueGuid = source.ValueGuid;
            toMap.ValueBoolean = source.ValueBoolean;
            toMap.Container = source.Container ? new ContainerModelMapper().Map(source.Container) : null;
            return toMap;
        };
        return ContainerPropertyModelMapper;
    }(BaseMapper));
    return ContainerPropertyModelMapper;
});
//# sourceMappingURL=ContainerPropertyModelMapper.js.map