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
    var ContainerViewModelMapper = /** @class */ (function (_super) {
        __extends(ContainerViewModelMapper, _super);
        function ContainerViewModelMapper() {
            return _super.call(this) || this;
        }
        ContainerViewModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.EntityShortId = source.IdContainer;
            toMap.Name = source.Name;
            toMap.Note = source.Note;
            toMap.UniqueId = source.UniqueId;
            return toMap;
        };
        return ContainerViewModelMapper;
    }(BaseMapper));
    return ContainerViewModelMapper;
});
//# sourceMappingURL=ContainerViewModelMapper.js.map