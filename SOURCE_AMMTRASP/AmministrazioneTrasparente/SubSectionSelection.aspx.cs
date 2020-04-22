using AmministrazioneTrasparente.Code;
using System;
using System.Linq;

namespace AmministrazioneTrasparente
{
    public partial class SubSectionSelection : BaseSeriesPage
    {        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            if (SeriesWso.DocumentSeriesSubsections.Count == 0)
            {
                Response.Redirect(ResolveUrl(string.Concat("~/Series.aspx?idSeries=" + SeriesWso.Id, MyMaster.HistoryEnable ? "&history=" + MyMaster.StoricoEnabled : "")), true);
            }

            lblSeriesName.Text = SeriesWso.Name;

            DocumentSeriesHeader headerItem = Singleton.Instance.DocumentSeriesHeaders.SingleOrDefault(t => t.IdSeries.HasValue && t.IdSeries.Value.Equals(IdSeries) && !t.IdSubSection.HasValue);
            lblHeader.Text = headerItem != null ? headerItem.Header : string.Empty;

            SubSectionRepeater.DataSource = SeriesWso.DocumentSeriesSubsections;
            SubSectionRepeater.DataBind();
        }
    }
}