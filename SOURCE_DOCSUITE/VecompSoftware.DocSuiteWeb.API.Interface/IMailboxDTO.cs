namespace VecompSoftware.DocSuiteWeb.API
{
    public interface IMailboxDTO : IAPIArgument
    {

        #region [ Properties ]

        string TypeName { get; set; }

        int? Id { get; set; }

        string Name { get; set; }

        string Address { get; set; }

        #endregion

    }
}
