using System;
using VecompSoftware.DocSuiteWeb.Model.Documents;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations
{
    public class EvaluateCollaborationDocumentModel
    {
        public Guid IdCollaboration { get; set; }
        public Guid IdCollaborationVersioning { get; set; }
        public ArchiveDocument ArchiveDocument { get; set; }
    }
}
