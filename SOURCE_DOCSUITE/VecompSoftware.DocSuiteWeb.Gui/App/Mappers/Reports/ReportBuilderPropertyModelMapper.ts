import BaseMapper = require('App/Mappers/BaseMapper');
import ReportBuilderPropertyModel = require('App/Models/Reports/ReportBuilderPropertyModel');

class ReportBuilderPropertyModelMapper extends BaseMapper<ReportBuilderPropertyModel> {
    constructor() {
        super();
    }

    public Map(source: any): ReportBuilderPropertyModel {
        let toMap: ReportBuilderPropertyModel = new ReportBuilderPropertyModel();
        toMap.DisplayName = source.DisplayName;
        toMap.EntityType = source.EntityType;
        toMap.Name = source.Name;
        toMap.PropertyType = source.PropertyType;
        toMap.Children = this.MapCollection(source.Children);
        return toMap;
    }
}

export = ReportBuilderPropertyModelMapper;