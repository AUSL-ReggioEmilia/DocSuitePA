using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Validations
{
    public class ValidationModel
    {
        public int ValidationCode { get; set; }
        public IReadOnlyCollection<ValidationMessageModel> ValidationMessages { get; set; }
    }
}
