using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.DocumentUnits
{
    public class DocumentUnitChainValidatorMapper : BaseMapper<DocumentUnitChain, DocumentUnitChainValidator>, IDocumentUnitChainValidatorMapper
    {
        public DocumentUnitChainValidatorMapper() { }

        public override DocumentUnitChainValidator Map(DocumentUnitChain entity, DocumentUnitChainValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.DocumentName = entity.DocumentName;
            entityTransformed.DocumentLabel = entity.DocumentLabel;
            entityTransformed.ArchiveName = entity.ArchiveName;
            entityTransformed.IdArchiveChain = entity.IdArchiveChain;
            entityTransformed.ChainType = entity.ChainType;
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
