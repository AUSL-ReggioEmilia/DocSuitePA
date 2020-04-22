import ReportBuilderPropertyModel = require('App/Models/Reports/ReportBuilderPropertyModel');
import ReportBuilderProjectionType = require('App/Models/Reports/ReportBuilderProjectionType');

class ReportBuilderProjectionModel {
    constructor() {
        this.ReportProperties = [];
    }

    ReportProperties: ReportBuilderPropertyModel[];
    ProjectionType: ReportBuilderProjectionType;
    Alias: string;
    TagName: string;
}

export = ReportBuilderProjectionModel;