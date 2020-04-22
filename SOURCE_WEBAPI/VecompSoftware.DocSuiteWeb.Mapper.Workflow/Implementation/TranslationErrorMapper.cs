using VecompSoftware.DocSuiteWeb.Model.Workflow;

namespace VecompSoftware.DocSuiteWeb.Mapper.Workflow
{
    public class TranslationErrorMapper : BaseWorkflowMapper<TranslationErrorExtended, TranslationError>, ITranslationErrorMapper
    {
        public override TranslationError Map(TranslationErrorExtended entity, TranslationError entityTransformed)
        {
            #region [ Base ]
            entityTransformed.ActivityId = entity.ActivityId;
            entityTransformed.EndColumn = entity.EndColumn;
            entityTransformed.EndLine = entity.EndLine;
            entityTransformed.ExpressionText = entity.ExpressionText;
            entityTransformed.Message = entity.Message;
            entityTransformed.StartColumn = entity.StartColumn;
            entityTransformed.StartLine = entity.StartLine;
            #endregion

            #region [ Navigation Properties ]

            #endregion

            return entityTransformed;
        }

    }
}
