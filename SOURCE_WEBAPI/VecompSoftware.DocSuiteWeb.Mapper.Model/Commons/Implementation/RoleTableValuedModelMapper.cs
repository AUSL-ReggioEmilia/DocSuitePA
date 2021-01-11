using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Commons
{
    public class RoleTableValuedModelMapper : BaseModelMapper<IRoleTableValuedModel, RoleModel>, IRoleTableValuedModelMapper
    {
        public override RoleModel Map(IRoleTableValuedModel model, RoleModel modelTransformed)
        {
            modelTransformed = null;
            if (model.Role_IdRole.HasValue)
            {
                modelTransformed = new RoleModel
                {
                    IdRole = model.Role_IdRole,
                    Name = model.Role_Name
                };
            }

            return modelTransformed;
        }

    }
}
