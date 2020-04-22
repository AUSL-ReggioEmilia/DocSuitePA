using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Fascicles
{
    public class FascicleFolderMapper : BaseEntityMapper<FascicleFolder, FascicleFolder>, IFascicleFolderMapper
    {
        public override FascicleFolder Map(FascicleFolder entity, FascicleFolder entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Name = entity.Name;
            entityTransformed.Status = entity.Status;
            entityTransformed.Typology = entity.Typology;
            entityTransformed.FascicleFolderLevel = entity.FascicleFolderLevel;
            entityTransformed.FascicleFolderPath = entity.FascicleFolderPath;
            entityTransformed.ParentInsertId = entity.ParentInsertId;
            #endregion

            return entityTransformed;
        }
    }
}
