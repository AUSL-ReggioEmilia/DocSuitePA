using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;

namespace AmministrazioneTrasparente.Tools
{
    public class DynamicColumnTemplate : ITemplate
    {
        protected HtmlGenericControl DynamicControl;
        private readonly string _colname;

        public DynamicColumnTemplate(string cName)
        {
            _colname = cName;
        }

        #region Implementation of ITemplate

        public void InstantiateIn(Control container)
        {
            DynamicControl = new HtmlGenericControl { ID = string.Format("DYNAMIC_{0}", _colname) };

            DynamicControl.DataBinding += GenericControlDataBinding;

            container.Controls.Add(DynamicControl);
        }

        void GenericControlDataBinding(object sender, EventArgs e)
        {
            HtmlGenericControl div = sender as HtmlGenericControl;
            if (div == null) return;
            var container = (GridDataItem)div.NamingContainer;

            var wso = container.DataItem as DocumentSeriesItemWSO;

            if (wso != null)
            {
                foreach (var attributeWSO in wso.DynamicData.Where(attributeWSO => attributeWSO.Key == _colname))
                {
                    div.InnerHtml = attributeWSO.Value.UrlToAnchor();
                }
            }
        }

        #endregion
    }
}