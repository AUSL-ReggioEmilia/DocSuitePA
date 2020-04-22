using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols
{
    public class ProtocolDraftValidatorMapper : BaseMapper<ProtocolDraft, ProtocolDraftValidator>, IProtocolDraftValidatorMapper
    {
        public ProtocolDraftValidatorMapper() { }

        public override ProtocolDraftValidator Map(ProtocolDraft entity, ProtocolDraftValidator entityTransformed)
        {
            #region [ Base ]

            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.Description = entity.Description;
            entityTransformed.Data = entity.Data;
            entityTransformed.DraftType = entity.DraftType;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Timestamp = entity.Timestamp;

            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Collaboration = entity.Collaboration;
            #endregion

            return entityTransformed;
        }

    }
}
