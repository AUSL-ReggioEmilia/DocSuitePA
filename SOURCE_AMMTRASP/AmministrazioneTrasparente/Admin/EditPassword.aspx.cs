using System;
using System.Web.UI;
using AmministrazioneTrasparente.Services;

namespace AmministrazioneTrasparente.Admin
{
    public partial class EditPassword : System.Web.UI.Page
    {
        private readonly UserService _userService = new UserService();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Close_OnClick(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(Page.GetType(), "cancel", "CloseEdit();", true);
        }

        protected void Save_OnClick(object sender, EventArgs e)
        {
            this._userService.ChangePassword(newPassword.Text);
            ClientScript.RegisterStartupScript(Page.GetType(), "update", "UpdateConfirm();", true);
        }
    }
}