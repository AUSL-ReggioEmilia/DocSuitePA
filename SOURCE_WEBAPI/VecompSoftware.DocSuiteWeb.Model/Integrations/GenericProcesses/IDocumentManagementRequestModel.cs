using System.Collections.Generic;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.DocSuiteWeb.Model.Workflow;

namespace VecompSoftware.DocSuiteWeb.Model.Integrations.GenericProcesses
{
    public interface IDocumentManagementRequestModel : IContentBase
    {
        ICollection<WorkflowReferenceBiblosModel> Documents { get; set; }
        WorkflowReferenceModel DocumentUnit { get; set; }
        ICollection<WorkflowRole> Roles { get; set; }
        ICollection<WorkflowMapping> Signers { get; set; }
    }
}