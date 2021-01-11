using System;
using DSW = VecompSoftware.DocSuiteWeb.Data;
using APICommon = VecompSoftware.DocSuiteWeb.Entity.Commons;
using NHibernate;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Commons
{
    public class MapperLocationEntity : BaseEntityMapper<DSW.Location, APICommon.Location>
    {
        #region
        public MapperLocationEntity()
        {

        }
        #endregion

        #region
        protected override IQueryOver<DSW.Location, DSW.Location> MappingProjection(IQueryOver<DSW.Location, DSW.Location> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APICommon.Location TransformDTO(DSW.Location entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare Location se l'entità non è inizializzata");

            APICommon.Location apiCategory = new APICommon.Location();
            apiCategory.UniqueId = entity.UniqueId;
            apiCategory.EntityShortId = Convert.ToInt16(entity.Id);
            apiCategory.Name = entity.Name;
            apiCategory.ProtocolArchive = entity.ProtBiblosDSDB;
            apiCategory.DossierArchive = entity.DocmBiblosDSDB;
            apiCategory.ResolutionArchive = entity.ReslBiblosDSDB;

            return apiCategory;
        }
        #endregion
    }
}
