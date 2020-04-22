namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Dossiers
{
    public sealed class DossierRuleset : IDossierRuleset
    {
        public string READ => "DossierRead";

        public string INSERT => "DossierInsert";

        public string UPDATE => "DossierUpdate";

        public string DELETE => "DossierDelete";
    }
}
