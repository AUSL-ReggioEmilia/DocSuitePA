namespace VecompSoftware.DocSuiteWeb.Validation
{
    public interface IValidatorService
    {
        bool Validate<T>(T obj) where T : class;

        bool Validate<T>(T obj, string ruleset) where T : class;
    }
}
