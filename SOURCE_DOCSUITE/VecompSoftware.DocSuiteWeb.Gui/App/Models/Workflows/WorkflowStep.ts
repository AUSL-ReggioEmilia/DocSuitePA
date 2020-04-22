import ActivityOperation = require('./ActivityOperationModel');
import WorkflowArgumentModel = require('./WorkflowArgumentModel');
import WorkflowAuthorizationType = require('./WorkflowAuthorizationType');
import ActivityType = require('./ActivityType');

interface WorkflowStep {
    Position: number;
    Name: string;
    AuthorizationType: WorkflowAuthorizationType;
    ActivityOperation: ActivityOperation;
    ActivityType: ActivityType;
    InputArguments?: WorkflowArgumentModel[];
    EvaluationArguments?: WorkflowArgumentModel[];
    OutputArguments?: WorkflowArgumentModel[];
}

export = WorkflowStep;