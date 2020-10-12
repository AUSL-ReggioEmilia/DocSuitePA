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
    var DocumentUnitChainMapper = /** @class */ (function (_super) {
        __extends(DocumentUnitChainMapper, _super);
        function DocumentUnitChainMapper() {
            return _super.call(this) || this;
        }
        DocumentUnitChainMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.ArchiveName = source.ArchiveName;
            toMap.ChainType = source.ChainType;
            toMap.DocumentLabel = source.DocumentLabel;
            toMap.DocumentName = source.DocumentName;
            toMap.IdArchiveChain = source.IdArchiveChain;
            toMap.UniqueId = source.UniqueId;
            return toMap;
        };
        return DocumentUnitChainMapper;
    }(BaseMapper));
    return DocumentUnitChainMapper;
});
//# sourceMappingURL=DocumentUnitChainMapper.js.map