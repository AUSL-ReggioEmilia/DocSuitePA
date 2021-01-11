using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Protocols
{
    public class ProtocolDocumentTypeMapper : BaseEntityMapper<ProtocolDocumentType, ProtocolDocumentType>, IProtocolDocumentTypeMapper
    {
        public override ProtocolDocumentType Map(ProtocolDocumentType entity, ProtocolDocumentType entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Description = entity.Description;
            entityTransformed.Code = entity.Code;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.HiddenFields = entity.HiddenFields;
            entityTransformed.NeedPackage = entity.NeedPackage;
            entityTransformed.CommonUser = entity.CommonUser;
            #endregion

            return entityTransformed;
        }

    }
}
