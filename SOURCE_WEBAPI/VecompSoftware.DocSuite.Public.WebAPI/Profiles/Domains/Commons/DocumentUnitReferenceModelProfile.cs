using AutoMapper;
using VecompSoftware.DocSuite.Public.Core.Models.Domains;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Commons
{
    public class DocumentUnitReferenceModelProfile : Profile
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly Security.ISecurity _security;

        #endregion

        #region [ Constructor ]
        public DocumentUnitReferenceModelProfile(IDataUnitOfWork unitOfWork, Security.ISecurity security)
        {
            _unitOfWork = unitOfWork;
            _security = security;
            CreateMap<Protocol, DocumentUnitReferenceModel>()
                .ForCtorParam("referenceId", opt => opt.MapFrom(src => src.UniqueId))
                .ForCtorParam("name", opt => opt.MapFrom(src => "Protocollo"))
                .ForCtorParam("year", opt => opt.MapFrom(src => src.Year))
                .ForCtorParam("number", opt => opt.MapFrom(src => src.Number.ToString()))
                .ForCtorParam("subject", opt => opt.MapFrom(src => src.Object));
        }

        #endregion
    }
}