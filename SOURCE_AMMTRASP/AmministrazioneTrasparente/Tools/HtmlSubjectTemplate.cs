using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;

namespace AmministrazioneTrasparente.Tools
{
    public class HtmlSubjectTemplate : ITemplate
    {
        protected HtmlGenericControl ContainerDIV;

        #region Implementation of ITemplate
        public void InstantiateIn(Control container)
        {
            ContainerDIV = new HtmlGenericControl("DIV");
            ContainerDIV.DataBinding += ContainerDIV_DataBinding;
            container.Controls.Add(ContainerDIV);
        }

        static void ContainerDIV_DataBinding(object sender, EventArgs e)
        {
            var div = sender as HtmlGenericControl;
            if (div == null)
            {
                return;
            }

            var container = (GridDataItem) div.NamingContainer;
            var wso = container.DataItem as DocumentSeriesItemWSO;

            if (wso != null) div.InnerHtml = wso.Subject.UrlToAnchor();
        }

        #endregion
    }
}