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
define(["require", "exports", "App/Mappers/BaseMapper", "./ExtendedProperties/POLRequestExtendedProperties", "App/Models/PosteWeb/ExtendedProperties/POLRequestHelper", "App/Models/PosteWeb/StatusColor"], function (require, exports, BaseMapper, POLRequestExtendedPropertiesMapper, POLRequestHelper, StatusColor) {
    var POLRequestModelMapper = /** @class */ (function (_super) {
        __extends(POLRequestModelMapper, _super);
        function POLRequestModelMapper() {
            return _super.call(this) || this;
        }
        POLRequestModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.Id = source.UniqueId;
            toMap.DocumentName = source.DocumentName;
            toMap.DocumentPosteFileType = source.DocumentPosteFileType;
            toMap.ErrorMessage = source.ErrorMessage;
            toMap.ExtendedProperties = source.ExtendedProperties;
            toMap.GuidPoste = source.GuidPoste;
            toMap.IdArchiveChain = source.IdArchiveChain;
            toMap.IdArchiveChainPoste = source.IdArchiveChainPoste;
            toMap.IdOrdine = source.IdOrdine;
            toMap.RequestId = source.UniqueId;
            toMap.Status = source.Status;
            toMap.StatusDescription = source.StatusDescription;
            toMap.TotalCost = source.TotalCost;
            toMap.RegistrationDate = moment(source.RegistrationDate).format("DD/MM/YYYY");
            var extendedPropsJson = JSON.parse(toMap.ExtendedProperties);
            toMap.ExtendedPropertiesDeserialized = (new POLRequestExtendedPropertiesMapper()).Map(extendedPropsJson);
            if (toMap.ExtendedPropertiesDeserialized) {
                toMap.DisplayColor = POLRequestHelper.DetermineStatusColor(toMap.ExtendedPropertiesDeserialized);
            }
            else {
                toMap.DisplayColor = StatusColor.Blue;
            }
            return toMap;
        };
        return POLRequestModelMapper;
    }(BaseMapper));
    return POLRequestModelMapper;
});
//# sourceMappingURL=POLRequestModelMapper.js.map