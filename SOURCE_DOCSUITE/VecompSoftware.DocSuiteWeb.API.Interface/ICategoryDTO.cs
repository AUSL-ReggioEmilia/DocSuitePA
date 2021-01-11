namespace VecompSoftware.DocSuiteWeb.API
{
    public interface ICategoryDTO : IAPIArgument
    {

        #region [ Properties ]

        int? Id { get; set; }

        string Name { get; set; }

        string FullCode { get; set; }

        #endregion

    }
}
