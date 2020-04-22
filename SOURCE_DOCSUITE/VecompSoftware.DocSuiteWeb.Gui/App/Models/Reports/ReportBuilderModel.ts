import Environment = require('App/Models/Environment');
import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel');
import ReportBuilderProjectionModel = require('App/Models/Reports/ReportBuilderProjectionModel');
import ReportBuilderConditionModel = require('App/Models/Reports/ReportBuilderConditionModel');
import ReportBuilderSortModel = require('App/Models/Reports/ReportBuilderSortModel');

class ReportBuilderModel {
    constructor() {
        this.Projections = [];
        this.Conditions = [];
        this.Sorts = [];
    }

    Entity: Environment;
    MetadataRepository: MetadataRepositoryModel;
    UDType: any;
    Projections: ReportBuilderProjectionModel[];
    Conditions: ReportBuilderConditionModel[];
    Sorts: ReportBuilderSortModel[];
}

export = ReportBuilderModel;