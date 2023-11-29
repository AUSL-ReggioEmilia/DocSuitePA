import DocSuiteModel = require("./DocSuiteModel");

interface DocSuiteEvent {
    UniqueId: string;
    WorkflowReferenceId: string;
    EventModel: DocSuiteModel;
    ReferenceModel: DocSuiteModel;
    EventDate: Date;
    WorkflowAutoComplete: boolean;
    WorkflowName: string;
    IdWorkflowActivity: string;
    RegistrationUser: string;
}

export = DocSuiteEvent;