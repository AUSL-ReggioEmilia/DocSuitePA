using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Templates;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Templates
{
    public class TemplateDocumentRepositoryValidatorMapper : BaseMapper<TemplateDocumentRepository, TemplateDocumentRepositoryValidator>, ITemplateDocumentRepositoryValidatorMapper
    {
        public TemplateDocumentRepositoryValidatorMapper() { }

        public override TemplateDocumentRepositoryValidator Map(TemplateDocumentRepository entity, TemplateDocumentRepositoryValidator entityTransformed)
        {
            #region [ Base ]

            entityTransformed.Object = entity.Object;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Status = entity.Status;
            entityTransformed.Name = entity.Name;
            entityTransformed.QualityTag = entity.QualityTag;
            entityTransformed.Version = entity.Version;
            entityTransformed.IdArchiveChain = entity.IdArchiveChain;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.Timestamp = entity.Timestamp;

            #endregion

            #region [ Navigation Properties ]

            #endregion

            return entityTransformed;
        }

    }
}
