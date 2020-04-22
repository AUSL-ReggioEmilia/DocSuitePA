namespace VecompSoftware.DocSuiteWeb.API
{
    public interface IContainerDTO : IAPIArgument
    {

        #region [ Properties ]

        int? Id { get; set; }

        string Name { get; set; }

        #endregion

    }
}
