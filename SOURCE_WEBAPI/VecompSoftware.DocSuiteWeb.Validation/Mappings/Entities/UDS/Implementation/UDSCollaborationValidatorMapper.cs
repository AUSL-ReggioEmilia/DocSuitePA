using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.UDS
{
    public class UDSCollaborationValidatorMapper : BaseMapper<UDSCollaboration, UDSCollaborationValidator>, IUDSCollaborationValidatorMapper
    {
        public UDSCollaborationValidatorMapper() { }

        public override UDSCollaborationValidator Map(UDSCollaboration entity, UDSCollaborationValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.IdUDS = entity.IdUDS;
            entityTransformed.Environment = entity.Environment;
            entityTransformed.RelationType = entity.RelationType;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Repository = entity.Repository;
            entityTransformed.Collaboration = entity.Relation;
            #endregion

            return entityTransformed;
        }
    }
}
