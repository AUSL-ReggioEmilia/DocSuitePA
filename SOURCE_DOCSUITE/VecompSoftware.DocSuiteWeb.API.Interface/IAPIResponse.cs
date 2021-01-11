namespace VecompSoftware.DocSuiteWeb.API
{
    public interface IAPIResponse<T> : IAPISerializable
    {

        #region [ Properties ]

        string TypeName { get; set; }

        T Argument { get; set; }

        IExceptionDTO Exception { get; set; }

        #endregion

    }
}
