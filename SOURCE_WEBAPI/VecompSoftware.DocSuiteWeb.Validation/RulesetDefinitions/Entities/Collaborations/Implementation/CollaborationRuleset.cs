
namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Collaborations
{
    public sealed class CollaborationRuleset : ICollaborationRuleset
    {
        public string READ => "CollaborationRead";

        public string INSERT => "CollaborationInsert";

        public string UPDATE => "CollaborationUpdate";

        public string DELETE => "CollaborationDelete";
    }
}
