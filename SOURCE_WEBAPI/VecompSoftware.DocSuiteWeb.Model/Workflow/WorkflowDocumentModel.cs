using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Model.Workflow
{
    public class WorkflowDocumentModel
    {
        #region [ Constructor ]
        public WorkflowDocumentModel()
        {
            Documents = new List<KeyValuePair<ChainType, DocumentModel>>();
        }
        #endregion

        #region [ Fields ]

        #endregion

        #region [ Properties ]

        public string ArchiveName { get; set; }
        public LocationModel Location { get; set; }
        public WorkflowReferenceModel ReferenceModel { get; set; }
        public ICollection<KeyValuePair<ChainType, DocumentModel>> Documents { get; set; }

        #endregion
    }
}
