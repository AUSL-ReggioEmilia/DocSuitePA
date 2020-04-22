using System;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Model.Workflow
{
    public class WorkflowReferenceBiblosModel
    {
        #region [ Constructor ]

        #endregion

        #region [ Fields ]

        #endregion

        #region [ Properties ]

        public string DocumentName { get; set; }

        public string ArchiveName { get; set; }

        public ChainType ChainType { get; set; }

        public Guid? ArchiveChainId { get; set; }

        public Guid? ArchiveDocumentId { get; set; }

        public bool? Simulation { get; set; }

        public string ReferenceModel { get; set; }

        public WorkflowReferenceBiblosModel ReferenceDocument { get; set; }

        #endregion
    }
}
