import WorkflowSeverityLogType = require('Workflows/WorkflowSeverityLogType');
import WorkflowActivityModel = require('WorkflowActivityModel');

interface WorkflowActivityLogModel {
    UniqueId: string;
    LogDate: Date;
    SystemComputer: string;
    LogType: string;
    LogDescription: string;
    Severity: WorkflowSeverityLogType;
    RegistrationUser: string;
    RegistrationDate: Date;
    LastChangedUser?: string;
    LastChangedDate?: Date;
    Entity: WorkflowActivityModel;
}

export = WorkflowActivityLogModel;