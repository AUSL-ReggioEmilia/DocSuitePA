using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons
{
    public class ContainerPropertyMapper : BaseEntityMapper<ContainerProperty, ContainerProperty>, IContainerPropertyMapper
    {
        public ContainerPropertyMapper()
        {

        }

        public override ContainerProperty Map(ContainerProperty entity, ContainerProperty entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Name = entity.Name;
            entityTransformed.ContainerType = entity.ContainerType;
            entityTransformed.ValueInt = entity.ValueInt;
            entityTransformed.ValueDate = entity.ValueDate;
            entityTransformed.ValueDouble = entity.ValueDouble;
            entityTransformed.ValueBoolean = entity.ValueBoolean;
            entityTransformed.ValueGuid = entity.ValueGuid;
            entityTransformed.ValueString = entity.ValueString;
            #endregion

            return entityTransformed;
        }

    }
}
