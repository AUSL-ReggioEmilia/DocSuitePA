using VecompSoftware.DocSuiteWeb.Entity.Desks;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Desks
{
    public class DeskMapper : BaseEntityMapper<Desk, Desk>, IDeskMapper
    {
        public DeskMapper()
        { }

        public override Desk Map(Desk entity, Desk entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Description = entity.Description;
            entityTransformed.ExpirationDate = entity.ExpirationDate;
            entityTransformed.Name = entity.Name;
            entityTransformed.Status = entity.Status;
            #endregion

            return entityTransformed;
        }

    }
}
