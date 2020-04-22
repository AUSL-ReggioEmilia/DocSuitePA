using System;
using NHibernate;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Commons
{
    public class MapperRoleModel : BaseEntityMapper<Role, RoleModel>
    {
        protected override IQueryOver<Role, Role> MappingProjection(IQueryOver<Role, Role> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override RoleModel TransformDTO(Role entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare Role se l'entità non è inizializzata");

            RoleModel model = new RoleModel(Convert.ToInt16(entity.Id))
            {
                Name = entity.Name,
                FullIncrementalPath = entity.FullIncrementalPath,
                UniqueId = entity.UniqueId
            };
            

            return model;
        }
    }
}
