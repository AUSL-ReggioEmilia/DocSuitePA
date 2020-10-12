using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Model.Workflow.Actions
{
    public class WorkflowActionFascicleCloseModel : WorkflowActionModel, IWorkflowActionFascicleClose
    {
        #region [ Constructors ]

        public WorkflowActionFascicleCloseModel(FascicleModel referenced)
        {
            Referenced = referenced;
        }

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        public FascicleModel GetReferenced()
        {
            return Referenced as FascicleModel;
        }

        #endregion

    }

}
