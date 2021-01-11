using VecompSoftware.DocSuiteWeb.Entity.Templates;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Templates
{
    public class TemplateReportMapper : BaseEntityMapper<TemplateReport, TemplateReport>, ITemplateReportMapper
    {
        public override TemplateReport Map(TemplateReport entity, TemplateReport entityTransformed)
        {
            #region [ Base ]

            entityTransformed.Name = entity.Name;
            entityTransformed.IdArchiveChain = entity.IdArchiveChain;
            entityTransformed.Status = entity.Status;
            entityTransformed.Environment = entity.Environment;
            entityTransformed.ReportBuilderJsonModel = entity.ReportBuilderJsonModel;

            #endregion

            return entityTransformed;
        }
    }
}
