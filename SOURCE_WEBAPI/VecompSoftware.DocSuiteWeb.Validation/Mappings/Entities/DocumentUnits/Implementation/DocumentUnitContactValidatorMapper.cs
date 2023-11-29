using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.DocumentUnits
{
    public class DocumentUnitContactValidatorMapper : BaseMapper<DocumentUnitContact, DocumentUnitContactValidator>, IDocumentUnitContactValidatorMapper
    {
        public DocumentUnitContactValidatorMapper() { }

        public override DocumentUnitContactValidator Map(DocumentUnitContact entity, DocumentUnitContactValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.ContactManual = entity.ContactManual;
            entityTransformed.ContactType = entity.ContactType;
            entityTransformed.ContactLabel = entity.ContactLabel;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Contact = entity.Contact;
            entityTransformed.DocumentUnit = entity.DocumentUnit;
            #endregion

            return entityTransformed;
        }
    }
}
