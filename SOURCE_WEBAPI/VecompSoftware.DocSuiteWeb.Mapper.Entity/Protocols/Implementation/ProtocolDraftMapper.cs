using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Protocols
{
    public class ProtocolDraftMapper : BaseEntityMapper<ProtocolDraft, ProtocolDraft>, IProtocolDraftMapper
    {
        public override ProtocolDraft Map(ProtocolDraft entity, ProtocolDraft entityTransformed)
        {
            #region [ Base ]
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.Description = entity.Description;
            entityTransformed.Data = entity.Data;
            entityTransformed.DraftType = entity.DraftType;
            #endregion

            return entityTransformed;
        }

    }
}
