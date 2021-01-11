using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Commons
{
    public class ContainerTableValuedModelMapper : BaseModelMapper<IContainerTableValuedModel, ContainerModel>, IContainerTableValuedModelMapper
    {
        public override ContainerModel Map(IContainerTableValuedModel entity, ContainerModel entityTransformed)
        {
            entityTransformed = null;
            if (entity.Container_IdContainer.HasValue)
            {
                entityTransformed = new ContainerModel
                {
                    IdContainer = entity.Container_IdContainer,
                    Name = entity.Container_Name
                };
            }

            return entityTransformed;
        }

    }
}
