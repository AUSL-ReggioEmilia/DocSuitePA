using AutoMapper;
using System;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Resolutions;
using VecompSoftware.DocSuiteWeb.Data;
using PrivateResolutionModels = VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Resolutions
{
    public class ResolutionTableValuedModelProfile : Profile
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly Security.ISecurity _security;

        #endregion


        #region [ Constructor ]
        public ResolutionTableValuedModelProfile(IDataUnitOfWork unitOfWork, Security.ISecurity security)
        {
            _unitOfWork = unitOfWork;
            _security = security;
            CreateMap<PrivateResolutionModels.ResolutionTableValuedModel, ResolutionModel>()
                .ForCtorParam("location", opt => opt.MapFrom(src => ""))
                .ForCtorParam("category", opt => opt.MapFrom((src) => new CategoryModel(Guid.Empty, "")))
                .ForCtorParam("container", opt => opt.MapFrom((src) => new ContainerModel(Guid.Empty, "")))
                .ForCtorParam("id", opt => opt.MapFrom(src => src.UniqueId))
                .ForCtorParam("year", opt => opt.MapFrom(src => src.Year))
                .ForCtorParam("number", opt => opt.MapFrom(src => src.Number))
                .ForCtorParam("subject", opt => opt.MapFrom(src => src.Object))
                .AfterMap((src, dest) => dest.Proposer = MapProposer(src));
        }

        #endregion

        #region [ Methods ]

        private string MapProposer(PrivateResolutionModels.ResolutionTableValuedModel source)
        {
            if (string.IsNullOrEmpty(source.ProposerDescription))
            {
                return string.Empty;
            }

            return string.Format("{0} {1}", source.ProposerCode, source.ProposerDescription.Replace("|", " "));
        }

        #endregion

    }
}