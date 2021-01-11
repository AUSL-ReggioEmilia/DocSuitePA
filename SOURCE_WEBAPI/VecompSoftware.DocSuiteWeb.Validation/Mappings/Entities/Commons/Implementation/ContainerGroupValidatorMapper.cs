using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons
{
    public class ContainerGroupValidatorMapper : BaseMapper<ContainerGroup, ContainerGroupValidator>, IContainerGroupValidatorMapper
    {
        public ContainerGroupValidatorMapper() { }

        public override ContainerGroupValidator Map(ContainerGroup entity, ContainerGroupValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.GroupName = entity.GroupName;
            entityTransformed.ProtocolRights = entity.ProtocolRights;
            entityTransformed.ResolutionRights = entity.ResolutionRights;
            entityTransformed.DocumentRights = entity.DocumentRights;
            entityTransformed.DocumentSeriesRights = entity.DocumentSeriesRights;
            entityTransformed.DeskRights = entity.DeskRights;
            entityTransformed.UDSRights = entity.UDSRights;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.PrivacyLevel = entity.PrivacyLevel;
            entityTransformed.FascicleRights = entity.FascicleRights;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Container = entity.Container;
            entityTransformed.SecurityGroup = entity.SecurityGroup;
            #endregion

            return entityTransformed;
        }

    }
}
