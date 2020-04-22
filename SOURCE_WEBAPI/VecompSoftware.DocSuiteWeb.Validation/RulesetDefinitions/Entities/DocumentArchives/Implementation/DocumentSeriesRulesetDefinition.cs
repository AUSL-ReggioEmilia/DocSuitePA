
namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.DocumentArchives
{
    public sealed class DocumentSeriesRulesetDefinition : IDocumentSeriesRuleset
    {
        public string READ => "DocumentSeriesRead";

        public string INSERT => "DocumentSeriesInsert";

        public string UPDATE => "DocumentSeriesUpdate";

        public string DELETE => "DocumentSeriesDelete";
    }
}
