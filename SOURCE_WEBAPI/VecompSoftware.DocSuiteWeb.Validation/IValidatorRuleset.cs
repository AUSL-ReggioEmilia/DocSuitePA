
namespace VecompSoftware.DocSuiteWeb.Validation
{
    public interface IValidatorRuleset
    {
        string READ { get; }
        string INSERT { get; }
        string UPDATE { get; }
        string DELETE { get; }
    }
}
