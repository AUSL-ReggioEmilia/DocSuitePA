using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations
{
    public class CollaborationSignerWorkflowModel
    {
        #region [ Constructor ]
        public CollaborationSignerWorkflowModel()
        {
            SignedDocuments = new List<CollaborationDocumentWorkflowModel>();
        }
        #endregion

        #region [ Properties ]
        public ICollection<CollaborationDocumentWorkflowModel> SignedDocuments { get; set; }

        public string UserName { get; set; }

        public bool HasApproved { get; set; }

        public DateTimeOffset? ExecuteDate { get; set; }
        #endregion
    }
}
