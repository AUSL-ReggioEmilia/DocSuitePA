import DSWEnvironment = require('App/Models/Environment');
import WorkflowReferenceType = require('App/Models/Workflows/WorkflowReferenceType');
import WorkflowReferenceBiblosModel = require('./WorkflowReferenceBiblosModel');

interface WorkflowReferenceModel {
    ReferenceId: string;
    ReferenceType: DSWEnvironment;
    Title: string;
    ReferenceModel: string;
    WorkflowReferenceType: WorkflowReferenceType;
    Documents: WorkflowReferenceBiblosModel[];
}
export = WorkflowReferenceModel;