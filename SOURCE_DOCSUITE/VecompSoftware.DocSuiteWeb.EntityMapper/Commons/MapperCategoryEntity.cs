using System;
using DSW = VecompSoftware.DocSuiteWeb.Data;
using APICommon = VecompSoftware.DocSuiteWeb.Entity.Commons;
using NHibernate;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Commons
{
    public class MapperCategoryEntity : BaseEntityMapper<DSW.Category, APICommon.Category>
    {
        #region
        public MapperCategoryEntity()
        {

        }
        #endregion

        #region
        protected override IQueryOver<DSW.Category, DSW.Category> MappingProjection(IQueryOver<DSW.Category, DSW.Category> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APICommon.Category TransformDTO(DSW.Category entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare Category se l'entità non è inizializzata");

            APICommon.Category apiCategory = new APICommon.Category();
            apiCategory.UniqueId = entity.UniqueId;
            apiCategory.EntityShortId = Convert.ToInt16(entity.Id);
            apiCategory.Name = entity.Name;
            apiCategory.IsActive = (APICommon.ActiveType?)entity.IsActive;
            apiCategory.Code = Convert.ToInt16(entity.Code);
            apiCategory.FullCode = entity.FullCode;
            apiCategory.FullIncrementalPath = entity.FullIncrementalPath;
            apiCategory.RegistrationDate = entity.RegistrationDate;
            apiCategory.RegistrationUser = entity.RegistrationUser;

            return apiCategory;
        }
        #endregion
    }
}
