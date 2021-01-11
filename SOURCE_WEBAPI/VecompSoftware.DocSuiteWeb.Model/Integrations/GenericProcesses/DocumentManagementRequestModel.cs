using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Documents.Signs;
using VecompSoftware.DocSuiteWeb.Model.Workflow;

namespace VecompSoftware.DocSuiteWeb.Model.Integrations.GenericProcesses
{
    public class DocumentManagementRequestModel : IDocumentManagementRequestModel
    {
        #region [ Constructor ]
        public DocumentManagementRequestModel()
        {
            UniqueId = Guid.NewGuid();
            Documents = new List<WorkflowReferenceBiblosModel>();
            Signers = new List<WorkflowMapping>();
            Roles = new List<WorkflowRole>();
        }
        #endregion

        #region [ Fields ]

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }

        public WorkflowReferenceModel DocumentUnit { get; set; }

        public ICollection<WorkflowMapping> Signers { get; set; }

        public ICollection<WorkflowRole> Roles { get; set; }

        public string RegistrationUser { get; set; }

        public RemoteSignProperty UserProfileRemoteSignProperty { get; set; }

        public ICollection<WorkflowReferenceBiblosModel> Documents { get; set; }
        #endregion

    }
}