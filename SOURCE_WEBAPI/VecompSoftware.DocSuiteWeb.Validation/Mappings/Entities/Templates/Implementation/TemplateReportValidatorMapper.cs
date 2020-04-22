using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Templates;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Templates
{
    public class TemplateReportValidatorMapper : BaseMapper<TemplateReport, TemplateReportValidator>, ITemplateReportValidatorMapper
    {
        public TemplateReportValidatorMapper() { }

        public override TemplateReportValidator Map(TemplateReport entity, TemplateReportValidator entityTransformed)
        {
            #region [ Base ]

            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Name = entity.Name;
            entityTransformed.IdArchiveChain = entity.IdArchiveChain;
            entityTransformed.Status = entity.Status;
            entityTransformed.Environment = entity.Environment;
            entityTransformed.ReportBuilderJsonModel = entity.ReportBuilderJsonModel;
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
