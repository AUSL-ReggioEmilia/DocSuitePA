import Environment = require('App/Models/Environment');
import ReportBuilderItem = require('App/Models/Reports/ReportBuilderItem');
import ReportBuilderPropertyType = require('App/Models/Reports/ReportBuilderPropertyType');

class ReportBuilderPropertyModel implements ReportBuilderItem {
    constructor() {
        this.Children = [];
    }

    Name: string;
    DisplayName: string;
    PropertyType: ReportBuilderPropertyType;
    EntityType: Environment;
    Children: ReportBuilderPropertyModel[];
    get HasChildren() { return this.Children.length > 0; }
}

export = ReportBuilderPropertyModel;