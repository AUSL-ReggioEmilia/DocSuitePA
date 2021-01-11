using System;
using DSW = VecompSoftware.DocSuiteWeb.Data.Entity.Commons;
using APICommon = VecompSoftware.DocSuiteWeb.Entity.Commons;
using NHibernate;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Commons
{
    public class MapperCategoryFascicle : BaseEntityMapper<DSW.CategoryFascicle, APICommon.CategoryFascicle>
    {

        #region [ Fields ]
        private readonly MapperCategoryEntity _mapperCategory;
        private readonly MapperFasciclePeriod _mapperFasciclePeriod;
        #endregion

        #region [ Constructor ]
        public MapperCategoryFascicle()
        {
            _mapperCategory = new MapperCategoryEntity();
            _mapperFasciclePeriod = new MapperFasciclePeriod();
        }
        #endregion

        #region [ Methods ]

        protected override IQueryOver<DSW.CategoryFascicle, DSW.CategoryFascicle> MappingProjection(IQueryOver<DSW.CategoryFascicle, DSW.CategoryFascicle> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APICommon.CategoryFascicle TransformDTO(DSW.CategoryFascicle entity)
        {

            if (entity == null)
            {
                throw new ArgumentException("Impossibile trasformare CategoryFascicle se l'entità non è inizializzata");
            }

            APICommon.CategoryFascicle apiCategoryFascicle = new APICommon.CategoryFascicle();

            #region [ Base ]
            apiCategoryFascicle.DSWEnvironment = entity.DSWEnvironment;
            apiCategoryFascicle.UniqueId = entity.Id;
            apiCategoryFascicle.FascicleType = (FascicleType)entity.FascicleType;
            apiCategoryFascicle.RegistrationDate = entity.RegistrationDate;
            apiCategoryFascicle.RegistrationUser = entity.RegistrationUser;
            apiCategoryFascicle.LastChangedDate = entity.LastChangedDate;
            apiCategoryFascicle.LastChangedDate = entity.LastChangedDate;
            apiCategoryFascicle.FasciclePeriod = entity.FasciclePeriod == null ? null : _mapperFasciclePeriod.MappingDTO(entity.FasciclePeriod);
            apiCategoryFascicle.Category = _mapperCategory.MappingDTO(entity.Category);
            #endregion

            return apiCategoryFascicle;
        }
        #endregion
    }
}
