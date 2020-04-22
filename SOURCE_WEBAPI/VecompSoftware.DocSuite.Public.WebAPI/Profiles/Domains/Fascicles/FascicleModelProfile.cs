using AutoMapper;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Fascicles;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Fascicles
{
    public class FascicleModelProfile : Profile
    {

        #region [ Constructor ]
        public FascicleModelProfile(IDataUnitOfWork unitOfWork, Security.ISecurity security)
        {
            CreateMap<Fascicle, FascicleModel>()
                .ForCtorParam("id", opt => opt.MapFrom(src => src.UniqueId))
                .AfterMap((src, dest) => dest.FascicleName = src.Name)
                .AfterMap((src, dest) => dest.Subject = src.FascicleObject)
                .AfterMap((src, dest) => dest.Name = "Fascicle")
                .AfterMap((src, dest) => dest.RegistrationUser = security.GetUser(src.RegistrationUser).DisplayName)
                .AfterMap((src, dest) => dest.LastChangedUser = src.LastChangedUser != null ? security.GetUser(src.LastChangedUser).DisplayName : null);
        }

        #endregion

    }
}