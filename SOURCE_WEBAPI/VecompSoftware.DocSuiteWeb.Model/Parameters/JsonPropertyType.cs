
namespace VecompSoftware.DocSuiteWeb.Model.Parameters
{
    public enum JsonPropertyType : short
    {
        Json = 1,
        PropertyString = 2 * Json,
        PropertyInt = 2 * PropertyString,
        PropertyDate = 2 * PropertyInt,
        PropertyDouble = 2 * PropertyDate,
        PropertyBoolean = 2 * PropertyDouble,
        PropertyGuid = 2 * PropertyBoolean
    }
}
