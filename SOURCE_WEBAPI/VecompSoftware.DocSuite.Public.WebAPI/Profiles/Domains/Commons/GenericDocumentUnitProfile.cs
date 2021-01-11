using AutoMapper;
using System;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Archives.Commons;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Commons
{
    public class GenericDocumentUnitProfile : Profile
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly Security.ISecurity _security; 

        #endregion

        #region [ Constructor ]
        public GenericDocumentUnitProfile(IDataUnitOfWork unitOfWork, Security.ISecurity security)
        {
            _unitOfWork = unitOfWork;
            _security = security;
            CreateMap<DocumentUnitTableValuedModel, GenericDocumentUnitModel>()
                .ForCtorParam("location", opt => opt.MapFrom(src => ""))
                .ForCtorParam("category", opt => opt.MapFrom((src) => new CategoryModel(Guid.Empty, "")))
                .ForCtorParam("container", opt => opt.MapFrom((src) => new ContainerModel(Guid.Empty, "")))
                .ForCtorParam("id", opt => opt.MapFrom(src => src.UniqueId))
                .ForCtorParam("year", opt => opt.MapFrom(src => src.Year))
                .ForCtorParam("number", opt => opt.MapFrom(src => src.Number))
                .ForCtorParam("subject", opt => opt.MapFrom(src => src.Subject))
                .AfterMap((src, dest) => dest.DocumentUnitName = src.DocumentUnitName)
                .AfterMap((src, dest) => dest.Title = src.Title)
                .AfterMap((src, dest) => dest.Name = string.Concat(src.DocumentUnitName, " ", src.Title));
        }

        #endregion
    }
}