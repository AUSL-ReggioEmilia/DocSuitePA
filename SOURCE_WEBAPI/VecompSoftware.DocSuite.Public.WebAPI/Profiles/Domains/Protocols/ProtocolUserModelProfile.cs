using AutoMapper;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Protocols;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Protocols
{

    public class ProtocolUserModelProfile : Profile
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public ProtocolUserModelProfile(IDataUnitOfWork unitOfWork, Security.ISecurity security)
        {
            CreateMap<ProtocolUser, ProtocolUserModel>()
                .ForCtorParam("id", opt => opt.MapFrom(src => src.UniqueId));
        }

        #endregion

    }

}