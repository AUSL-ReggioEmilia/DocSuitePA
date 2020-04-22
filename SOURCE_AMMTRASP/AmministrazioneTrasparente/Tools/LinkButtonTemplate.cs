using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;

namespace AmministrazioneTrasparente.Tools
{
    public class LinkButtonTemplate : ITemplate
    {
        protected HyperLink Link;
        protected Image Image;

        private readonly string _colname;
        private readonly string _imageName;
        private readonly string _alt;
        private readonly bool? _historyEnable;

        public LinkButtonTemplate(string cName, string imagename, string alt, bool? historyEnable)
        {
            _colname = cName;
            _imageName = imagename;
            _alt = alt;
            _historyEnable = historyEnable;
        }

        public void InstantiateIn(Control container)
        {
            Link = new HyperLink {ID = string.Format("LINK_{0}", _colname)};
            if(_historyEnable.HasValue)
                Link.Attributes.Add("historyEnabled", _historyEnable.ToString());
            Link.DataBinding += LinkDataBinding;

            Image = new Image {ImageUrl = "~/img/" + _imageName, AlternateText = _alt};

            Link.Controls.Add(Image);

            container.Controls.Add(Link);
        }

        static void LinkDataBinding(object sender, EventArgs e)
        {
            HyperLink link = sender as HyperLink;
            
            if (link == null) return;

            var container = (GridDataItem)link.NamingContainer;
            DocumentSeriesItemWSO wso = container.DataItem as DocumentSeriesItemWSO;

            bool historyEnable;
            if (wso != null) link.NavigateUrl = string.Format("~/SeriesItem.aspx?IdSeriesItem={0}{1}", wso.Id, 
                bool.TryParse(link.Attributes["historyEnabled"], out historyEnable) ? string.Concat("&history=", historyEnable) : string.Empty);
        }
    }
}