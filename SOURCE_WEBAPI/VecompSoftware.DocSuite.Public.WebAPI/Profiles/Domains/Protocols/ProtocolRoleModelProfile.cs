using AutoMapper;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Protocols;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Protocols
{

    public class ProtocolRoleModelProfile : Profile
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public ProtocolRoleModelProfile(IDataUnitOfWork unitOfWork, Security.ISecurity security)
        {
            CreateMap<RoleTableValuedModel, ProtocolSectorModel>()
                .ForCtorParam("id", opt => opt.MapFrom(src => src.UniqueId))
                .ForCtorParam("name", opt => opt.MapFrom(src => src.Name));
        }

        #endregion

    }

}