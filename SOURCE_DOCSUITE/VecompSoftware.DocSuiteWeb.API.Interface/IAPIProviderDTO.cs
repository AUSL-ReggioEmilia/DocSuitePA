
namespace VecompSoftware.DocSuiteWeb.API
{
    public interface IAPIProviderDTO
    {

        #region [ Properties ]

        bool? Enabled { get; set;}
        string Code { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        string Address { get; set; }
        bool? Main { get; set; }

        #endregion

    }
}
