using System;
using VecompSoftware.DocSuiteWeb.Model.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders
{
    public class WorkflowActivityFinderModel
    {
        public DateTimeOffset? AuthorizeDateFrom { get; set; }
        public DateTimeOffset? AuthorizeDateTo { get; set; }
        public string RequestorUsername { get; set; }
        public string RequestorRoleName { get; set; }
        public string CategoryCode { get; set; }
        public WorkflowStatus? Status { get; set; }
        public string Note { get; set; }
        public string Subject { get; set; }
        public int? Skip { get; set; }
        public int? Top { get; set; }
        public Guid? IdWorkflowActivity { get; set; }
        public string RegistrationUser { get; set; }
    }
}
