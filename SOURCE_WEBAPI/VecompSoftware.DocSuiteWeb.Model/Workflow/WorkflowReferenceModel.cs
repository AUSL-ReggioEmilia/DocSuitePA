using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Workflow
{
    public class WorkflowReferenceModel
    {
        #region [ Constructor ]
        public WorkflowReferenceModel()
        {
            Documents = new List<WorkflowReferenceBiblosModel>();
            DocumentUnits = new List<WorkflowReferenceDocumentUnitModel>();
        }
        #endregion

        #region [ Properties ]

        public Guid ReferenceId { get; set; }

        public DSWEnvironmentType ReferenceType { get; set; }

        public string ReferenceModel { get; set; }

        public string Title { get; set; }

        public ICollection<WorkflowReferenceBiblosModel> Documents { get; set; }

        public ICollection<WorkflowReferenceDocumentUnitModel> DocumentUnits { get; set; }
        #endregion
    }
}
