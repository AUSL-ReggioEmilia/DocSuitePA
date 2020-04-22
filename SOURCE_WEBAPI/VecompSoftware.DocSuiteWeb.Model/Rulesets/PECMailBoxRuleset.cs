namespace VecompSoftware.DocSuiteWeb.Model.Rulesets
{
    public class PECMailBoxRuleset
    {
        #region [Constructor]

        public PECMailBoxRuleset()
        {

        }

        #endregion

        #region [Properties

        public string Name { get; set; }
        public RulesetType Rule { get; set; }
        public string Condition { get; set; }
        public string Reference { get; set; }

        #endregion
    }
}
