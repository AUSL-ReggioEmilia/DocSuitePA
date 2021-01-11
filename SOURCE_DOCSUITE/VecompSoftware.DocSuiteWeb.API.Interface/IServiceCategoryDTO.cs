namespace VecompSoftware.DocSuiteWeb.API
{
    public interface IServiceCategoryDTO : IAPIArgument
    {
        #region [ Properties ]
        int? Id { get; set; }
        string Code { get; set; }
        string Description { get; set; }
        #endregion
    }
}
