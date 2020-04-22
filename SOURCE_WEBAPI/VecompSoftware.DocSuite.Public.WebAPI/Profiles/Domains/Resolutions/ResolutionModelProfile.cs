using AutoMapper;
using System;
using System.Linq;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Resolutions;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Finder.Resolutions;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Resolutions
{
    public class ResolutionModelProfile : Profile
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly Security.ISecurity _security;

        #endregion

        #region [ Constructor ]
        public ResolutionModelProfile(IDataUnitOfWork unitOfWork, Security.ISecurity security)
        {
            _unitOfWork = unitOfWork;
            _security = security;
            CreateMap<Resolution, ResolutionModel>()
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

        private string MapProposer(Resolution source)
        {
            ResolutionContact resolutionContact = _unitOfWork.Repository<ResolutionContact>().GetByResolution(source.EntityId, "P").FirstOrDefault();

            if (resolutionContact == null)
            {
                return string.Empty;
            }

            return string.Format("{0} {1}", resolutionContact.Contact.Code, resolutionContact.Contact.Description.Replace("|", " "));
        }

        #endregion

    }
}