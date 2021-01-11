namespace VecompSoftware.DocSuiteWeb.API
{
    public interface IAccountingSectionalDTO : IAPIArgument
    {

        #region [ Properties ]

        int? Id { get; set; }

        string Name { get; set; }

        IContainerDTO Container { get; set; }

        #endregion

    }
}
