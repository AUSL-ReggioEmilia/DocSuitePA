import TemplateReportStatus = require('App/Models/Templates/TemplateReportStatus');
import Environment = require('App/Models/Environment');

class TemplateReportModel {
    UniqueId: string;
    Name: string;
    IdArchiveChain: string;
    Status: TemplateReportStatus;
    Environment: Environment;
    ReportBuilderJsonModel: string;
    RegistrationUser: string;
    RegistrationDate: Date;
}

export = TemplateReportModel;