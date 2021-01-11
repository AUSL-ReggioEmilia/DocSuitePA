using System;
using System.Collections.Generic;
using System.Web;
using AmministrazioneTrasparente.Services;
using AmministrazioneTrasparente.SQLite.Entities;

namespace AmministrazioneTrasparente.Admin
{
    public partial class Default : System.Web.UI.Page
    {
        private readonly UserLogService _userLogService = new UserLogService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
                Response.Cache.SetValidUntilExpires(false);
                Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();
            }
        }
    }
}