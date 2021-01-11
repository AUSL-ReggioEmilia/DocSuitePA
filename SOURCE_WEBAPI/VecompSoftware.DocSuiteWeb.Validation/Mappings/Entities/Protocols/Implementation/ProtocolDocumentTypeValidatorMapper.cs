using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols
{
    public class ProtocolDocumentTypeValidatorMapper : BaseMapper<ProtocolDocumentType, ProtocolDocumentTypeValidator>, IProtocolDocumentTypeValidatorMapper
    {
        public ProtocolDocumentTypeValidatorMapper() { }

        public override ProtocolDocumentTypeValidator Map(ProtocolDocumentType entity, ProtocolDocumentTypeValidator entityTransformed)
        {
            #region[ Base ]
            entityTransformed.EntityShortId = entity.EntityShortId;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Description = entity.Description;
            entityTransformed.Code = entity.Code;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.HiddenFields = entity.HiddenFields;
            entityTransformed.NeedPackage = entity.NeedPackage;
            entityTransformed.CommonUser = entity.CommonUser;
            #endregion

            #region [ Navigation Properties ]

            entityTransformed.Protocols = entity.Protocols;

            #endregion


            return entityTransformed;
        }
    }
}
