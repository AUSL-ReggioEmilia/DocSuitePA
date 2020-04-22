
using AutoMapper;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Commons
{
    public class CategoryModelProfile : Profile
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly Security.ISecurity _security;

        #endregion

        #region [ Constructor ]
        public CategoryModelProfile(IDataUnitOfWork unitOfWork, Security.ISecurity security)
        {
            _unitOfWork = unitOfWork;
            _security = security;
            CreateMap<Category, CategoryModel>()
                .ForCtorParam("id", opt => opt.MapFrom(src => src.UniqueId))
                .BeforeMap((src, dest) => dest.HierarchyCode = _unitOfWork.Repository<Category>().GetHierarcyCode(src.EntityShortId))
                .BeforeMap((src, dest) => dest.HierarchyDescription = _unitOfWork.Repository<Category>().GetHierarcyDescription(src.EntityShortId));
        }

        #endregion

    }
}