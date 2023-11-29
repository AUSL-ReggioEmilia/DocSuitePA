
namespace VecompSoftware.DocSuiteWeb.API
{
    public interface IOChartItemDTO
    {
        #region [ Properties ]

        string Code { get; set; }
        string ParentCode { get; set; }
        string FullCode { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        string Acronym { get; set; }

        IOChartItemDTO Parent { get; set; }
        IOChartItemDTO[] Items { get; set; }

        #endregion
    }
}
