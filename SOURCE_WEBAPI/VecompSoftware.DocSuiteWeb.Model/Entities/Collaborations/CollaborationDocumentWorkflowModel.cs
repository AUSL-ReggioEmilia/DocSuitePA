using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations
{
    public class CollaborationDocumentWorkflowModel
    {
        #region [ Constructor ]
        public CollaborationDocumentWorkflowModel()
        {

        }
        #endregion

        #region [ Properties ]
        public string DocumentName { get; set; }

        public Guid IdDocument { get; set; }

        public DateTimeOffset? SignDate { get; set; }
        #endregion
    }
}
