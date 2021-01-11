using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Validations;

namespace VecompSoftware.DocSuiteWeb.Validation
{
    public interface IValidator<T>
    {
        ICollection<ValidationMessageModel> Validate();
        ICollection<ValidationMessageModel> Validate(T source);
        ICollection<ValidationMessageModel> Validate(T source, string ruleset);
    }
}
