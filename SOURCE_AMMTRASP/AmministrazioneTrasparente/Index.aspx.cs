using System;

namespace AmministrazioneTrasparente
{
    public partial class Index : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MyMaster.SelectedIdFamily = null;

            lblStatistics.Visible = !String.IsNullOrEmpty(MyMaster.GoogleAnalyticsCode);
        }
    }
}