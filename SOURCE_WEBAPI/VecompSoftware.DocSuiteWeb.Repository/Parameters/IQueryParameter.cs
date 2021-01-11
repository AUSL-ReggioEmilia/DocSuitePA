namespace VecompSoftware.DocSuiteWeb.Repository.Parameters
{
    public interface IQueryParameter
    {
        string ParameterName { get; set; }

        object ParameterValue { get; set; }

        string ParameterTypeName { get; set; }
    }
}
