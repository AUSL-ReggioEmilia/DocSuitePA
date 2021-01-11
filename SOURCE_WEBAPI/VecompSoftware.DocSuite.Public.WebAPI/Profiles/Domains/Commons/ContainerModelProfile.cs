
using AutoMapper;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Commons
{
    public class ContainerModelProfile : Profile
    {
        #region [ Constructor ]
        public ContainerModelProfile(IDataUnitOfWork unitOfWork, Security.ISecurity security)
        {
            CreateMap<Container, ContainerModel>()
                .ForCtorParam("id", opt => opt.MapFrom(src => src.UniqueId));
        }

        #endregion

        #region [ Methods ]

        #endregion
    }
}