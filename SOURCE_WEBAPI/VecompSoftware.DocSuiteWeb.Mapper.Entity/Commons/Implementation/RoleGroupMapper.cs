using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons
{
    public class RoleGroupMapper : BaseEntityMapper<RoleGroup, RoleGroup>, IRoleGroupMapper
    {
        public RoleGroupMapper()
        {

        }

        public override RoleGroup Map(RoleGroup entity, RoleGroup entityTransformed)
        {
            #region [ Base ]
            entityTransformed.GroupName = entity.GroupName;
            entityTransformed.ProtocolRights = entity.ProtocolRights;
            entityTransformed.ResolutionRights = entity.ResolutionRights;
            entityTransformed.DocumentRights = entity.DocumentRights;
            entityTransformed.DocumentSeriesRights = entity.DocumentSeriesRights;
            entityTransformed.FascicleRights = entity.FascicleRights;
            #endregion

            return entityTransformed;
        }

    }
}
