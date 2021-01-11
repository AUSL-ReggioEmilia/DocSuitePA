using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Fascicles
{
    public class FascicleRoleMapper : BaseEntityMapper<FascicleRole, FascicleRole>, IFascicleRoleMapper
    {
        public override FascicleRole Map(FascicleRole entity, FascicleRole entityTransformed)
        {
            #region [ Base ]
            entityTransformed.AuthorizationRoleType = entity.AuthorizationRoleType;
            entityTransformed.IsMaster = entity.IsMaster;
            #endregion

            return entityTransformed;
        }
    }
}
