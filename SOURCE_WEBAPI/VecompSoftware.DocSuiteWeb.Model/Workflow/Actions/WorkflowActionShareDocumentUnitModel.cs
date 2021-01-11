using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Model.Workflow.Actions
{
    public class WorkflowActionShareDocumentUnitModel : WorkflowActionModel, IWorkflowActionShareDocumentUnit
    {
        #region [ Constructors ]
        public WorkflowActionShareDocumentUnitModel(DocumentUnitModel referenced)
        {
            Referenced = referenced;
        }
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]
        public DocumentUnitModel GetReferenced()
        {
            return Referenced as DocumentUnitModel;
        }
        #endregion

    }
}
