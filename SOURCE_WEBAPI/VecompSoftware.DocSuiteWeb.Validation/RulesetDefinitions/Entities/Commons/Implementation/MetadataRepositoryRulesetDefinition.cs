namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Commons
{
    public class MetadataRepositoryRulesetDefinition : IMetadataRepositoryRuleset
    {
        public string READ => "MetadataRepositoryRead";

        public string INSERT => "MetadataRepositoryInsert";

        public string UPDATE => "MetadataRepositoryUpdate";

        public string DELETE => "MetadataRepositoryDelete";
    }
}
