using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons
{
    public class ContainerPropertyValidatorMapper : BaseMapper<ContainerProperty, ContainerPropertyValidator>, IContainerPropertyValidatorMapper
    {
        public ContainerPropertyValidatorMapper() { }

        public override ContainerPropertyValidator Map(ContainerProperty entity, ContainerPropertyValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Name = entity.Name;
            entityTransformed.ContainerType = entity.ContainerType;
            entityTransformed.ValueInt = entity.ValueInt;
            entityTransformed.ValueDate = entity.ValueDate;
            entityTransformed.ValueDouble = entity.ValueDouble;
            entityTransformed.ValueBoolean = entity.ValueBoolean;
            entityTransformed.ValueGuid = entity.ValueGuid;
            entityTransformed.ValueString = entity.ValueString;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Container = entity.Container;
            #endregion

            return entityTransformed;
        }

    }
}
