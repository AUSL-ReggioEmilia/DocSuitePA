import FascicleFolderModel = require("App/Models/Fascicles/FascicleFolderModel");
import ContentBase = require("App/Models/ContentBase");
import WorkflowActionModel = require("App/Models/Workflows/WorkflowActionModel");

class WorkflowActionFascicleModel extends WorkflowActionModel {
    constructor() {
        super();
        this.$type = "VecompSoftware.DocSuiteWeb.Model.Workflow.Actions.WorkflowActionFascicleModel, VecompSoftware.DocSuiteWeb.Model";
    }

    Fascicle: ContentBase;
    Folder: FascicleFolderModel;
}

export = WorkflowActionFascicleModel;