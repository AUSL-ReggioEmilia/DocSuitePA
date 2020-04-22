using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.DocumentUnits
{
    public class DocumentUnitUserValidatorMapper : BaseMapper<DocumentUnitUser, DocumentUnitUserValidator>, IDocumentUnitUserValidatorMapper
    {
        public DocumentUnitUserValidatorMapper() { }

        public override DocumentUnitUserValidator Map(DocumentUnitUser entity, DocumentUnitUserValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Account = entity.Account;
            entityTransformed.AuthorizationType = entity.AuthorizationType;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Timestamp = entity.Timestamp;

            #endregion

            #region [ Navigation Properties ]
            entityTransformed.DocumentUnit = entity.DocumentUnit;
            #endregion

            return entityTransformed;
        }

    }
}
