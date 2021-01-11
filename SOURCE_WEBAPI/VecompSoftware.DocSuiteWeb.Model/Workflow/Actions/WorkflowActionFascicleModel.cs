using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Model.Workflow.Actions
{
    public class WorkflowActionFascicleModel : WorkflowActionModel, IWorkflowActionFascicle
    {
        #region [ Constructors ]

        public WorkflowActionFascicleModel(FascicleModel fascicleModel, DocumentUnitModel referenced, FascicleFolderModel folder)
        {
            Fascicle = fascicleModel;
            Referenced = referenced;
            Folder = folder;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Fascicle
        /// </summary>
        public IContentBase Fascicle { get; set; }

        public FascicleFolderModel Folder { get; set; }

        #endregion

        #region [ Methods ]
        public DocumentUnitModel GetReferenced()
        {
            return Referenced as DocumentUnitModel;
        }

        public FascicleModel GetFascicle()
        {
            return Fascicle as FascicleModel;
        }

        public FascicleFolderModel GetFascicleFolder()
        {
            return Folder;
        }

        #endregion

    }
}
