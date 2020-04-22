using AutoMapper;
using VecompSoftware.DocSuite.Public.Core.Models.Workflows;
using Entities = VecompSoftware.DocSuiteWeb.Entity;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Workflows
{
    public class WorkflowStatusTypeConverter : ITypeConverter<Entities.Workflows.WorkflowStatus, WorkflowStatus>
    {
        public WorkflowStatus Convert(Entities.Workflows.WorkflowStatus status, WorkflowStatus destination, ResolutionContext context)
        {
            switch (status)
            {
                case Entities.Workflows.WorkflowStatus.Todo:
                    return WorkflowStatus.Started;
                case Entities.Workflows.WorkflowStatus.Progress:
                    return WorkflowStatus.Progress;
                case Entities.Workflows.WorkflowStatus.Suspended:
                    return WorkflowStatus.Invalid;
                case Entities.Workflows.WorkflowStatus.Done:
                    return WorkflowStatus.Done;
                case Entities.Workflows.WorkflowStatus.Error:
                    return WorkflowStatus.Invalid;
            }
            return WorkflowStatus.Invalid;
        }
    }
}