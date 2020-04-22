using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.BusinessRule.Rules.Validators
{
    public class ValidatorResult
    {
        #region [Constructor]
        public ValidatorResult()
        {
            this.Errors = new List<string>();
        }
        #endregion

        #region [Properties]
        public ICollection<string> Errors { get; set; }

        public bool HasErrors
        {
            get { return Errors.Count > 0; }
        }
        #endregion
    }
}
