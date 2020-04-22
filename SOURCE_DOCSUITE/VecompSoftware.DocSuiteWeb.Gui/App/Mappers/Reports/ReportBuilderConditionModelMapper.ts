import BaseMapper = require('App/Mappers/BaseMapper');
import ReportBuilderConditionModel = require('App/Models/Reports/ReportBuilderConditionModel');

class ReportBuilderConditionModelMapper extends BaseMapper<ReportBuilderConditionModel> {
    constructor() {
        super();
    }

    public Map(source: any): ReportBuilderConditionModel {
        let toMap: ReportBuilderConditionModel = new ReportBuilderConditionModel();
        toMap.ConditionName = source.ConditionName;
        return toMap;
    }
}

export = ReportBuilderConditionModelMapper;