using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Model.Workflow.Actions
{
    public class WorkflowActionDocumentUnitLinkModel : WorkflowActionModel, IWorkflowActionDocumentUnitLink
    {
        #region [ Constructors ]

        public WorkflowActionDocumentUnitLinkModel(DocumentUnitModel referenced, DocumentUnitModel destinationLink)
        {
            DestinationLink = destinationLink;
            Referenced = referenced;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Link to DocumentUnit
        /// </summary>
        public IContentBase DestinationLink { get; set; }

        #endregion

        #region [ Methods ]

        public DocumentUnitModel GetReferenced()
        {
            return Referenced as DocumentUnitModel;
        }

        public DocumentUnitModel GetDestinationLink()
        {
            return DestinationLink as DocumentUnitModel;
        }

        #endregion

    }

}
