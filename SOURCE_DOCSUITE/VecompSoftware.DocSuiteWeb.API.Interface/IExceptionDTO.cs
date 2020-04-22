namespace VecompSoftware.DocSuiteWeb.API
{
    public interface IExceptionDTO : IAPISerializable
    {

        #region [ Properties ]

        string TypeName { get; set; }

        string Message { get; set; }

        string StackTrace { get; set; }

        IExceptionDTO InnerException { get; set; }

        #endregion

    }
}
