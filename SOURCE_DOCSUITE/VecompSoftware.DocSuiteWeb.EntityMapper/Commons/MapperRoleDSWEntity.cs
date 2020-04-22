using DSW = VecompSoftware.DocSuiteWeb.Data;
using APICommons = VecompSoftware.DocSuiteWeb.Entity.Commons;
using NHibernate;
using System;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Commons
{
    public class MapperRoleDSWEntity : BaseEntityMapper<APICommons.Role, DSW.Role>
    {
        protected override IQueryOver<APICommons.Role, APICommons.Role> MappingProjection(IQueryOver<APICommons.Role, APICommons.Role> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override DSW.Role TransformDTO(APICommons.Role entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Impossibile trasformare Role se l'entità non è inizializzata");
            }

            DSW.Role dswRole = new DSW.Role();

            dswRole.Id = entity.EntityShortId;
            dswRole.Name = entity.Name;
            dswRole.TenantId = entity.TenantId;
            dswRole.IdRoleTenant = entity.IdRoleTenant;

            return dswRole;
        }

    }
}
