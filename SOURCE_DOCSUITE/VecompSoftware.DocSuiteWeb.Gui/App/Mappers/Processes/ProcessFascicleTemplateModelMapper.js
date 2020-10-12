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
define(["require", "exports", "App/Mappers/Processes/ProcessModelMapper", "App/Mappers/Dossiers/DossierFolderModelMapper", "App/Mappers/BaseMapper"], function (require, exports, ProcessModelMapper, DossierFolderModelMapper, BaseMapper) {
    var ProcessFascicleTemplateModelMapper = /** @class */ (function (_super) {
        __extends(ProcessFascicleTemplateModelMapper, _super);
        function ProcessFascicleTemplateModelMapper() {
            return _super.call(this) || this;
        }
        ProcessFascicleTemplateModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.UniqueId = source.UniqueId;
            toMap.Process = source.Process ? new ProcessModelMapper().Map(source.Process) : null;
            toMap.DossierFolder = source.DossierFolder ? new DossierFolderModelMapper().Map(source.DossierFolder) : null;
            toMap.Name = source.Name;
            toMap.JsonModel = source.JsonModel;
            toMap.StartDate = source.StartDate;
            toMap.EndDate = source.EndDate;
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.LastChangedUser = source.LastChangedUser;
            toMap.LastChangedDate = source.LastChangedDate;
            return toMap;
        };
        return ProcessFascicleTemplateModelMapper;
    }(BaseMapper));
    return ProcessFascicleTemplateModelMapper;
});
//# sourceMappingURL=ProcessFascicleTemplateModelMapper.js.map