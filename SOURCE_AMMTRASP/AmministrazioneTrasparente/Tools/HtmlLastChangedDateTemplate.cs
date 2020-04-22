using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;

namespace AmministrazioneTrasparente.Tools
{
    public class HtmlLastChangedDateTemplate : ITemplate
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

            var container = (GridDataItem)div.NamingContainer;
            var wso = container.DataItem as DocumentSeriesItemWSO;

            if (wso != null)
            {
                div.InnerHtml = wso.LastChangedDate.HasValue ? wso.LastChangedDate?.ToString("dd/MM/yyyy") : wso.PublishingDate?.ToString("dd/MM/yyyy");
            }
        }

        #endregion
    }
}