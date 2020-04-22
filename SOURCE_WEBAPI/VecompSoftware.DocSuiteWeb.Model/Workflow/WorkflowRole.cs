
using System;

namespace VecompSoftware.DocSuiteWeb.Model.Workflow
{
    public class WorkflowRole
    {
        #region [ Properties ]

        public string Name { get; set; }

        public short IdRole { get; set; }

        public Guid UniqueId { get; set; }

        public string EmailAddress { get; set; }

        public Guid TenantId { get; set; }

        public bool Required { get; set; }
        #endregion
    }
}


