using AutoMapper;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Fascicles;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Fascicles
{

    public class FascicleContactModelProfile : Profile
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public FascicleContactModelProfile(IDataUnitOfWork unitOfWork, Security.ISecurity security)
        {
            CreateMap<ContactTableValuedModel, FascicleContactModel>()
                .ForCtorParam("id", opt => opt.MapFrom(src => src.UniqueId))
                .ForCtorParam("name", opt => opt.MapFrom(src => src.Description))
                .AfterMap((src, dest) => dest.Name = src.Description.Replace("|", " "));
        }

        #endregion

    }

}