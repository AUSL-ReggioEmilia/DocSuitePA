using AutoMapper;
using Newtonsoft.Json;
using System.Linq;
using VecompSoftware.DocSuite.Public.Core.Models.Workflows;
using VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.DocSuiteWeb.Service.Workflow;
using VecompSoftware.Helpers.Workflow;
using Entities = VecompSoftware.DocSuiteWeb.Entity;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Workflows
{
    public class WorkflowStatusModelProfile : Profile
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly Security.ISecurity _security;

        #endregion

        #region [ Constructor ]
        public WorkflowStatusModelProfile(IDataUnitOfWork unitOfWork, Security.ISecurity security)
        {
            _unitOfWork = unitOfWork;
            _security = security;
            CreateMap<Entities.Workflows.WorkflowInstance, WorkflowStatusModel>()
                .ForCtorParam("instanceId", opt => opt.MapFrom(src => src.InstanceId))
                .ForCtorParam("workflowName", opt => opt.MapFrom((src) => src.WorkflowRepository == null ? string.Empty : src.WorkflowRepository.Name))
                .AfterMap((src, dest) => dest.Date = src.RegistrationDate);
        }

        #endregion

        #region [ Methods ]
       
        #endregion

    }
}
