using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Fascicles
{
    public class FascicleMapper : BaseEntityMapper<Fascicle, Fascicle>, IFascicleMapper
    {
        public override Fascicle Map(Fascicle entity, Fascicle entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Year = entity.Year;
            entityTransformed.Number = entity.Number;
            entityTransformed.Conservation = entity.Conservation;
            entityTransformed.StartDate = entity.StartDate;
            entityTransformed.EndDate = entity.EndDate;
            entityTransformed.Name = entity.Name;
            entityTransformed.Title = entity.Title;
            entityTransformed.FascicleObject = entity.FascicleObject;
            entityTransformed.Manager = entity.Manager;
            entityTransformed.Rack = entity.Rack;
            entityTransformed.Note = entity.Note;
            entityTransformed.FascicleType = entity.FascicleType;
            entityTransformed.VisibilityType = entity.VisibilityType;
            entityTransformed.MetadataValues = entity.MetadataValues;
            entityTransformed.MetadataDesigner = entity.MetadataDesigner;
            entityTransformed.CustomActions = entity.CustomActions;
            #endregion

            return entityTransformed;
        }
    }
}
