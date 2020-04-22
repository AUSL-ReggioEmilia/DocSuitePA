using AutoMapper;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Protocols;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Protocols
{
    public class ProtocolModelProfile : Profile
    {
        #region [ Constructor ]
        public ProtocolModelProfile(IDataUnitOfWork unitOfWork, Security.ISecurity security)
        {
            CreateMap<Protocol, ProtocolModel>()
                .ForCtorParam("location", opt => opt.MapFrom(src => src.Container != null && src.Container.ProtLocation != null ? src.Container.ProtLocation.Name : "location non definita"))
                .ForCtorParam("id", opt => opt.MapFrom(src => src.UniqueId))
                .ForCtorParam("year", opt => opt.MapFrom(src => src.Year))
                .ForCtorParam("number", opt => opt.MapFrom(src => src.Number))
                .ForCtorParam("subject", opt => opt.MapFrom(src => src.Object))
                .AfterMap((src, dest) => dest.AnnulmentReason = src.LastChangedReason)
                .AfterMap((src, dest) => dest.RegistrationUser = src.RegistrationUser) //TODO:usare il metodo UsersFinder di ActiveDirectory per avere il DisplayName
                .AfterMap((src, dest) => dest.LastChangedUser = src.LastChangedUser)
                .AfterMap((src, dest) => dest.Addressee = src.ProtocolType?.EntityShortId == (short)ProtocolTypology.Outgoing ? src.AdvancedProtocol?.Subject : string.Empty)
                .AfterMap((src, dest) => dest.Assignee = src.ProtocolType?.EntityShortId == (short)ProtocolTypology.Inbound ? src.AdvancedProtocol?.Subject : string.Empty)
                .AfterMap((src, dest) => dest.RegistrationUser = security.GetUser(src.RegistrationUser).DisplayName)
                .AfterMap((src, dest) => dest.LastChangedUser = src.LastChangedUser != null ? security.GetUser(src.LastChangedUser).DisplayName : null);
        }

        #endregion

    }
}