using VecompSoftware.DocSuiteWeb.Entity.UDS;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.UDS
{
    public class UDSTypologyMapper : BaseEntityMapper<UDSTypology, UDSTypology>, IUDSTypologyMapper
    {
        public override UDSTypology Map(UDSTypology entity, UDSTypology entityTransformed)
        {
            #region [ Base ]

            entityTransformed.Name = entity.Name;
            entityTransformed.Status = entity.Status;

            #endregion

            return entityTransformed;
        }

    }
}
