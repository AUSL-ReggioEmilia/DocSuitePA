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
    var UDSDocumentModelMapper = /** @class */ (function (_super) {
        __extends(UDSDocumentModelMapper, _super);
        function UDSDocumentModelMapper() {
            return _super.call(this) || this;
        }
        UDSDocumentModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.UDSDocumentId = source.UDSDocumentId;
            toMap.IdDocument = source.IdDocument;
            toMap.DocumentType = source.DocumentType;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.DocumentName = source.DocumentName;
            toMap.UDSId = source.UDSId;
            toMap.DocumentLabel = source.DocumentLabel;
            return toMap;
        };
        return UDSDocumentModelMapper;
    }(BaseMapper));
    return UDSDocumentModelMapper;
});
//# sourceMappingURL=UDSDocumentModelMapper.js.map