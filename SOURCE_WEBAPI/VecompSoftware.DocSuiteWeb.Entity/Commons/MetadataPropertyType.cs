
namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{
    public enum MetadataPropertyType : short
    {
        PropertyString = 1,
        PropertyInt = 2,
        PropertyDate = 2 * PropertyInt,
        PropertyDouble = 2 * PropertyDate,
        PropertyBoolean = 2 * PropertyDouble,
        PropertyGuid = 2 * PropertyBoolean
    }
}
