using VecompSoftware.DocSuiteWeb.Entity.MassimariScarto;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.MassimariScarto
{
    public class MassimarioScartoMapper : BaseEntityMapper<MassimarioScarto, MassimarioScarto>, IMassimarioScartoMapper
    {
        public override MassimarioScarto Map(MassimarioScarto entity, MassimarioScarto entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Code = entity.Code;
            entityTransformed.ConservationPeriod = entity.ConservationPeriod;
            entityTransformed.EndDate = entity.EndDate;
            entityTransformed.Name = entity.Name;
            entityTransformed.Note = entity.Note;
            entityTransformed.StartDate = entity.StartDate;
            entityTransformed.Status = entity.Status;
            entityTransformed.FakeInsertId = entity.FakeInsertId;
            #endregion

            return entityTransformed;
        }
    }
}
