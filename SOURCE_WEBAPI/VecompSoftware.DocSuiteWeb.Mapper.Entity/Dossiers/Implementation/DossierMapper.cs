using VecompSoftware.DocSuiteWeb.Entity.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Dossiers
{
    public class DossierMapper : BaseEntityMapper<Dossier, Dossier>, IDossierMapper
    {
        public override Dossier Map(Dossier entity, Dossier entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Year = entity.Year;
            entityTransformed.Subject = entity.Subject;
            entityTransformed.Note = entity.Note;
            entityTransformed.Number = entity.Number;
            entityTransformed.StartDate = entity.StartDate;
            entityTransformed.EndDate = entity.EndDate;
            entityTransformed.MetadataDesigner = entity.MetadataDesigner;
            entityTransformed.MetadataValues = entity.MetadataValues;
            entityTransformed.DossierType = entity.DossierType;
            entityTransformed.Status = entity.Status;
            #endregion

            return entityTransformed;
        }
    }
}
