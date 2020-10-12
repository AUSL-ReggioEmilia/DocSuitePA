namespace VecompSoftware.DocSuiteWeb.Model.Workflow
{
    public class WorkflowRule
    {
        #region [ Constructor ]
        public WorkflowRule()
        {
           
        }
        #endregion

        #region [ Properties ]
        public string Name { get; set; }
        public bool IsExist { get; set; }
        public bool HasFile { get; set; }
        public bool HasSignedFile { get; set; }
        public bool HasDocumentUnit { get; set; }
        public string ValidationMessage { get; set; }
        #endregion
    }
}