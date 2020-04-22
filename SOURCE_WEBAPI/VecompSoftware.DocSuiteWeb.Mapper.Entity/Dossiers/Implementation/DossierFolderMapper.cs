using VecompSoftware.DocSuiteWeb.Entity.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Dossiers
{
    public class DossierFolderMapper : BaseEntityMapper<DossierFolder, DossierFolder>, IDossierFolderMapper
    {
        public override DossierFolder Map(DossierFolder entity, DossierFolder entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Name = entity.Name;
            entityTransformed.Status = entity.Status;
            entityTransformed.JsonMetadata = entity.JsonMetadata;
            entityTransformed.DossierFolderLevel = entity.DossierFolderLevel;
            entityTransformed.DossierFolderPath = entity.DossierFolderPath;
            entityTransformed.ParentInsertId = entity.ParentInsertId;
            #endregion

            return entityTransformed;
        }
    }
}
