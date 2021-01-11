using AutoMapper;
using System;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Archives.Commons;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Fascicles
{
    public class FascicleDocumentModelProfile : Profile
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly Security.ISecurity _security;
        #endregion

        #region [ Constructor ]
        public FascicleDocumentModelProfile(IDataUnitOfWork unitOfWork, Security.ISecurity security)
        {
            _unitOfWork = unitOfWork;
            _security = security;
            CreateMap<FascicleDocument, GenericDocumentUnitModel>()
                .ForCtorParam("id", opt => opt.MapFrom(src => src.UniqueId))
                .ForCtorParam("year", opt => opt.MapFrom(src => src.Fascicle.Year))
                .ForCtorParam("number", opt => opt.MapFrom(src => src.Fascicle.Number))
                .ForCtorParam("subject", opt => opt.MapFrom(src => src.GetTitle()))
                .ForCtorParam("category", opt => opt.MapFrom(src => new CategoryModel(Guid.NewGuid(), string.Empty)))
                .ForCtorParam("container", opt => opt.MapFrom(src => new ContainerModel(Guid.NewGuid(), string.Empty)))
                .ForCtorParam("location", opt => opt.MapFrom(src => string.Empty))
                .AfterMap((src, dest) => dest.Title = src.GetTitle())
                .AfterMap((src, dest) => dest.Subject = src.IdArchiveChain.ToString())
                .AfterMap((src, dest) => dest.Environment = 0)
                .AfterMap((src, dest) => dest.DocumentUnitName = src.GetTitle())
                .AfterMap((src, dest) => dest.RegistrationUser = _security.GetUser(src.RegistrationUser).DisplayName)
                .AfterMap((src, dest) => dest.LastChangedUser = src.LastChangedUser != null ? _security.GetUser(src.LastChangedUser).DisplayName : null);
        }
        #endregion
    }
}