using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using AmministrazioneTrasparente.Services;
using Entities = AmministrazioneTrasparente.SQLite.Entities;
using System.Web.UI.HtmlControls;

namespace AmministrazioneTrasparente.Admin
{
    public partial class Parameters : System.Web.UI.Page
    {
        private readonly ParameterService _parameterService = new ParameterService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
                Response.Cache.SetValidUntilExpires(false);
                Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();
                LoadParameters();
            }
        }

        private void LoadParameters()
        {
            parameters.DataSource = GetDataTable();
        }

        private IList<Entities.Parameter> GetDataTable()
        {
            IList<Entities.Parameter> parameters = this._parameterService.GetParameters();
            return parameters;
        }

        protected void parameters_OnItemDataBound(object sender, GridItemEventArgs e)
        {
            var dataItem = e.Item as GridDataItem;
            if (dataItem != null)
            {
                var editBtn = (HtmlButton)dataItem.FindControl("edit");
                editBtn.Attributes["onclick"] = String.Format("return ShowEditForm('{0}', '{1}');", dataItem["Id"].Text, dataItem.ItemIndex);
            }
        }

        protected void parameters_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            LoadParameters();
        }
    }
}