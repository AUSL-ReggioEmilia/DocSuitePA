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
define(["require", "exports", "App/Mappers/BaseMapper", "App/ViewModels/Reports/ReportInformationViewModel", "App/Models/Templates/TemplateReportStatus"], function (require, exports, BaseMapper, ReportInformationViewModel, TemplateReportStatus) {
    var ReportInformationViewModelMapper = /** @class */ (function (_super) {
        __extends(ReportInformationViewModelMapper, _super);
        function ReportInformationViewModelMapper() {
            return _super.call(this) || this;
        }
        ReportInformationViewModelMapper.prototype.Map = function (source) {
            var toMap = new ReportInformationViewModel();
            var builderModel = JSON.parse(source.ReportBuilderJsonModel);
            toMap.SelectedDocumentUnit = builderModel.UDType;
            toMap.SelectedEnvironment = builderModel.Entity;
            toMap.SelectedMetadata = builderModel.MetadataRepository.UniqueId;
            toMap.Name = source.Name;
            toMap.CreatedBy = source.RegistrationUser;
            toMap.CreatedDate = source.RegistrationDate;
            toMap.StatusLabel = TemplateReportStatus.toPublicDescription(TemplateReportStatus[source.Status]);
            return toMap;
        };
        return ReportInformationViewModelMapper;
    }(BaseMapper));
    return ReportInformationViewModelMapper;
});
//# sourceMappingURL=ReportInformationViewModelMapper.js.map