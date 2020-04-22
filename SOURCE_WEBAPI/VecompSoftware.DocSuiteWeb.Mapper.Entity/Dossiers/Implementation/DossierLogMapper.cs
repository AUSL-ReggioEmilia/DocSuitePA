using VecompSoftware.DocSuiteWeb.Entity.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Dossiers
{
    public class DossierLogMapper : BaseEntityMapper<DossierLog, DossierLog>, IDossierLogMapper
    {
        public override DossierLog Map(DossierLog entity, DossierLog entityTransformed)
        {
            #region [ Base ]
            entityTransformed.LogDescription = entity.LogDescription;
            entityTransformed.SystemComputer = entity.SystemComputer;
            entityTransformed.LogType = entity.LogType;
            entityTransformed.Severity = entity.Severity;
            entityTransformed.Hash = entity.Hash;
            #endregion

            return entityTransformed;
        }
    }
}
