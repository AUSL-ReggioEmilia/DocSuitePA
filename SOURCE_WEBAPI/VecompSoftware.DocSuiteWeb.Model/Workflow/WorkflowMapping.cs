
namespace VecompSoftware.DocSuiteWeb.Model.Workflow
{
    public class WorkflowMapping
    {
        #region [ Properties ]

        public string MappingTag { get; set; }

        public WorkflowAuthorizationType AuthorizationType { get; set; }

        public WorkflowRole Role { get; set; }

        public WorkflowAccount Account { get; set; }
        #endregion
    }
}
