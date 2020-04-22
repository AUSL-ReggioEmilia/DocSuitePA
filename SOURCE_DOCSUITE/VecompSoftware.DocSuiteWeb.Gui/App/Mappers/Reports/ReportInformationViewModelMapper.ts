import BaseMapper = require('App/Mappers/BaseMapper');
import ReportInformationViewModel = require('App/ViewModels/Reports/ReportInformationViewModel');
import ReportBuilderModel = require('App/Models/Reports/ReportBuilderModel');
import TemplateReportStatus = require('App/Models/Templates/TemplateReportStatus');

class ReportInformationViewModelMapper extends BaseMapper<ReportInformationViewModel> {
    constructor() {
        super();
    }

    public Map(source: any): ReportInformationViewModel {
        let toMap: ReportInformationViewModel = new ReportInformationViewModel();
        let builderModel: ReportBuilderModel = JSON.parse(source.ReportBuilderJsonModel);
        toMap.SelectedDocumentUnit = builderModel.UDType;
        toMap.SelectedEnvironment = builderModel.Entity;
        toMap.SelectedMetadata = builderModel.MetadataRepository.UniqueId;
        toMap.Name = source.Name;
        toMap.CreatedBy = source.RegistrationUser;
        toMap.CreatedDate = source.RegistrationDate;
        toMap.StatusLabel = TemplateReportStatus.toPublicDescription(TemplateReportStatus[source.Status as string]);
        return toMap;
    }
}

export = ReportInformationViewModelMapper;