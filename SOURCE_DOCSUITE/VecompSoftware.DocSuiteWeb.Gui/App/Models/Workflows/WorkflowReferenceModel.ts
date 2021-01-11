import DSWEnvironment = require('App/Models/Environment');
import WorkflowReferenceType = require('App/Models/Workflows/WorkflowReferenceType');
import WorkflowReferenceBiblosModel = require('./WorkflowReferenceBiblosModel');
import WorkflowReferenceDocumentUnitModel = require('App/Models/Workflows/WorkflowReferenceDocumentUnitModel');

interface WorkflowReferenceModel {
    ReferenceId: string;
    ReferenceType: DSWEnvironment;
    Title: string;
    ReferenceModel: string;
    WorkflowReferenceType: WorkflowReferenceType;
    Documents: WorkflowReferenceBiblosModel[];
    DocumentUnits: WorkflowReferenceDocumentUnitModel[];
}
export = WorkflowReferenceModel;