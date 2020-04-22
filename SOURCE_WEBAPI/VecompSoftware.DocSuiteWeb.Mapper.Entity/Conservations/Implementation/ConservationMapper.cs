using VecompSoftware.DocSuiteWeb.Entity.Conservations;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Conservations
{
    public class ConservationMapper : BaseEntityMapper<Conservation, Conservation>, IConservationMapper
    {
        public ConservationMapper()
        {

        }

        public override Conservation Map(Conservation entity, Conservation entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityType = entity.EntityType;
            entityTransformed.Status = entity.Status;
            entityTransformed.Message = entity.Message;
            entityTransformed.Type = entity.Type;
            entityTransformed.SendDate = entity.SendDate;
            entityTransformed.Uri = entity.Uri;
            #endregion

            return entityTransformed;
        }
    }
}
