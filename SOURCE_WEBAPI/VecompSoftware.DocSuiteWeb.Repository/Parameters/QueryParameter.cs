namespace VecompSoftware.DocSuiteWeb.Repository.Parameters
{
    public class QueryParameter : IQueryParameter
    {
        #region [ Constructor ]

        public QueryParameter() { }

        public QueryParameter(string parameterName, object parameterValue)
        {
            ParameterName = parameterName;
            ParameterValue = parameterValue;
        }

        #endregion

        #region [ Properties ]

        public string ParameterName { get; set; }

        public object ParameterValue { get; set; }

        public string ParameterTypeName { get; set; }

        #endregion

        #region [ Methods ]

        #endregion
    }
}
