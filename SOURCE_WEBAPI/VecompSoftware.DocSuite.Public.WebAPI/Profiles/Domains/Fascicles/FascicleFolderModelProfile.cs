using AutoMapper;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Fascicles;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Fascicles
{
    public class FascicleFolderModelProfile : Profile
    {
        #region [ Constructor ]
        public FascicleFolderModelProfile(IDataUnitOfWork unitOfWork, Security.ISecurity security)
        {
            CreateMap<FascicleFolder, FascicleFolderModel>()
                .ForCtorParam("id", opt => opt.MapFrom(src => src.UniqueId));
        }

        #endregion
    }
}