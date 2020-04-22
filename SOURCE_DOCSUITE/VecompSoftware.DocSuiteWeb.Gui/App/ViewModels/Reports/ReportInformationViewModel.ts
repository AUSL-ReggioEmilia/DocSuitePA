import Environment = require('App/Models/Environment');
import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel');
import ReportBuilderPropertyModel = require('App/Models/Reports/ReportBuilderPropertyModel');

class ReportInformationViewModel {
    constructor() {
        this.Environments = [];
        this.DocumentUnits = [];
        this.MetadataProperties = [];
    }

    Name: string;
    Environments: Environment[];
    SelectedEnvironment: Environment;
    DocumentUnits: Environment[];
    SelectedDocumentUnit: Environment;
    SelectedMetadata: string;
    MetadataProperties: ReportBuilderPropertyModel[];
    CreatedBy: string;
    CreatedDate: Date;
    StatusLabel: string;
}

export = ReportInformationViewModel;