using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Protocols
{
    public class ProtocolRoleMapper : BaseEntityMapper<ProtocolRole, ProtocolRole>, IProtocolRoleMapper
    {
        public override ProtocolRole Map(ProtocolRole entity, ProtocolRole entityTransformed)
        {
            #region [ Base ]
            //entityTransformed.Year = entity.Year;
            //entityTransformed.Number = entity.Number;
            entityTransformed.Rights = entity.Rights;
            entityTransformed.Note = entity.Note;
            entityTransformed.Type = entity.Type;
            entityTransformed.DistributionType = entity.DistributionType;
            #endregion

            return entityTransformed;
        }

    }
}
