using System;
using DSW = VecompSoftware.DocSuiteWeb.Data;
using APICommons = VecompSoftware.DocSuiteWeb.Entity.Commons;
using NHibernate;

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

            APICommons.Role apiRole = new APICommons.Role();
            apiRole.EntityShortId = Convert.ToInt16(entity.Id);
            apiRole.Name = entity.Name;
            apiRole.IsActive = Convert.ToByte(entity.IsActive);
            apiRole.ActiveFrom = entity.ActiveFrom;
            apiRole.ActiveTo = entity.ActiveTo;
            apiRole.FullIncrementalPath = entity.FullIncrementalPath;
            apiRole.Collapsed = Convert.ToByte(entity.Collapsed);
            apiRole.EMailAddress = entity.EMailAddress;
            apiRole.ServiceCode = entity.ServiceCode;
            apiRole.UniqueId = entity.UniqueId;
            apiRole.IdRoleTenant = entity.IdRoleTenant;
            apiRole.TenantId = entity.TenantId;
            apiRole.RegistrationDate = entity.RegistrationDate;
            apiRole.RegistrationUser = entity.RegistrationUser;

            return apiRole;
        }
    }
}
