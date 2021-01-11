import ReportBuilderItem = require('App/Models/Reports/ReportBuilderItem');
import Environment = require('App/Models/Environment');
import ReportToolboxItemType = require('App/ViewModels/Reports/ReportToolboxItemType');

class ReportToolboxItemViewModel {
    constructor() {
        this.ReportItems = [];
    }

    Description: string;
    IconUrl: string;
    ReportItems: ReportBuilderItem[];
    Environment: Environment;
    ItemType: ReportToolboxItemType;
}

export = ReportToolboxItemViewModel;