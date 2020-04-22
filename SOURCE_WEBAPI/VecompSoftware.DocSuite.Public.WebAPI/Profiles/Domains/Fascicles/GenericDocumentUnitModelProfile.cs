using AutoMapper;
using System;
using System.Linq;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Archives.Commons;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Fascicles
{
    public class GenericDocumentUnitModelProfile : Profile
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly Security.ISecurity _security;
        #endregion

        #region [ Constructor ]
        public GenericDocumentUnitModelProfile(IDataUnitOfWork unitOfWork, Security.ISecurity security)
        {
            _unitOfWork = unitOfWork;
            _security = security;
            CreateMap<FascicleDocumentUnit, GenericDocumentUnitModel>()
                .ForCtorParam("id", opt => opt.MapFrom(src => src.DocumentUnit.UniqueId))
                .ForCtorParam("year", opt => opt.MapFrom(src => src.DocumentUnit.Year))
                .ForCtorParam("number", opt => opt.MapFrom(src => src.DocumentUnit.Number))
                .ForCtorParam("subject", opt => opt.MapFrom(src => src.DocumentUnit.Subject))
                .ForCtorParam("category", opt => opt.MapFrom(src => new CategoryModel(Guid.NewGuid(), string.Empty)))
                .ForCtorParam("container", opt => opt.MapFrom(src => new ContainerModel(Guid.NewGuid(), string.Empty)))
                .ForCtorParam("location", opt => opt.MapFrom(src => string.Empty))
                .AfterMap((src, dest) => dest.Title = src.DocumentUnit.Title)
                .AfterMap((src, dest) => dest.Environment = src.DocumentUnit.Environment)
                .AfterMap((src, dest) => dest.DocumentUnitName = src.DocumentUnit.DocumentUnitName)
                .AfterMap((src, dest) => dest.RegistrationUser = _security.GetUser(src.RegistrationUser).DisplayName)
                .AfterMap((src, dest) => dest.LastChangedUser = src.LastChangedUser != null ? _security.GetUser(src.LastChangedUser).DisplayName : null);
        }
        #endregion
    }
}