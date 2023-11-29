using System;
using DSW = VecompSoftware.DocSuiteWeb.Data;
using APICommons = VecompSoftware.DocSuiteWeb.Entity.Commons;
using NHibernate;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Commons
{
    public class MapperRoleEntity : BaseEntityMapper<DSW.Role, APICommons.Role>
    {
        protected override IQueryOver<DSW.Role, DSW.Role> MappingProjection(IQueryOver<DSW.Role, DSW.Role> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APICommons.Role TransformDTO(DSW.Role entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare Role se l'entità non è inizializzata");

            APICommons.Role apiRole = new APICommons.Role
            {
                EntityShortId = Convert.ToInt16(entity.Id),
                Name = entity.Name,
                IsActive = entity.IsActive,
                FullIncrementalPath = entity.FullIncrementalPath,
                Collapsed = entity.Collapsed,
                EMailAddress = entity.EMailAddress,
                ServiceCode = entity.ServiceCode,
                UniqueId = entity.UniqueId,
                TenantAOO = new TenantAOO { UniqueId = entity.IdTenantAOO },
                RegistrationDate = entity.RegistrationDate,
                RegistrationUser = entity.RegistrationUser
            };

            return apiRole;
        }
    }
}
