using AutoMapper;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Workflows
{
    public class WorkflowNotifyModelProfile : Profile
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly Security.ISecurity _security;

        #endregion

        #region [ Constructor ]

        public WorkflowNotifyModelProfile(IDataUnitOfWork unitOfWork, Security.ISecurity security)
        {
            _unitOfWork = unitOfWork;
            _security = security;
            CreateMap<WorkflowActivity, WorkflowNotify>()
                .AfterMap((src, dest) =>
                {
                    foreach (WorkflowProperty workflowProperty in src.WorkflowProperties)
                    {
                        WorkflowArgument workflowArgument = new WorkflowArgument
                        {
                            Name = workflowProperty.Name,
                            PropertyType = (ArgumentType)workflowProperty.PropertyType,
                            ValueBoolean = workflowProperty.ValueBoolean,
                            ValueDate = workflowProperty.ValueDate,
                            ValueDouble = workflowProperty.ValueDouble,
                            ValueGuid = workflowProperty.ValueGuid,
                            ValueInt = workflowProperty.ValueInt,
                            ValueString = workflowProperty.ValueString
                        };
                        dest.OutputArguments.Add(workflowProperty.Name, workflowArgument);
                    }
                });
        }

        #endregion
    }
}