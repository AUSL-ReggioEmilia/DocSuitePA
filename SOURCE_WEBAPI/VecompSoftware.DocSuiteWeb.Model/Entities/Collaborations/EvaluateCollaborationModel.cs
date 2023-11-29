using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations
{
    public class EvaluateCollaborationModel
    {
        public List<EvaluateCollaborationDocumentModel> DocumentModels { get; set; }
        public bool FromWorkflow { get; set; }
    }
}
