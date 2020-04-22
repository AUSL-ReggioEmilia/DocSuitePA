using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Commons
{
    public class ContainerTableValuedEntityMapper : BaseModelMapper<IContainerTableValuedModel, Container>, IContainerTableValuedEntityMapper
    {
        public override Container Map(IContainerTableValuedModel entity, Container entityTransformed)
        {
            entityTransformed = null;
            if (entity.Container_IdContainer.HasValue)
            {
                entityTransformed = new Container
                {
                    EntityShortId = entity.Container_IdContainer.Value,
                    Name = entity.Container_Name
                };
            }

            return entityTransformed;
        }

    }
}
