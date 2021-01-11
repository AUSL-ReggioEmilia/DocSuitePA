using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons
{
    public class ContainerGroupMapper : BaseEntityMapper<ContainerGroup, ContainerGroup>, IContainerGroupMapper
    {
        public ContainerGroupMapper()
        {

        }

        public override ContainerGroup Map(ContainerGroup entity, ContainerGroup entityTransformed)
        {
            #region [ Base ]
            entityTransformed.GroupName = entity.GroupName;
            entityTransformed.ProtocolRights = entity.ProtocolRights;
            entityTransformed.ResolutionRights = entity.ResolutionRights;
            entityTransformed.DocumentRights = entity.DocumentRights;
            entityTransformed.DocumentSeriesRights = entity.DocumentSeriesRights;
            entityTransformed.DeskRights = entity.DeskRights;
            entityTransformed.UDSRights = entity.UDSRights;
            entityTransformed.PrivacyLevel = entity.PrivacyLevel;
            entityTransformed.FascicleRights = entity.FascicleRights;
            #endregion

            return entityTransformed;
        }

    }
}
