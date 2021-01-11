using System;
using System.Web.UI;
using AmministrazioneTrasparente.Services;
using AmministrazioneTrasparente.SQLite.Entities;

namespace AmministrazioneTrasparente.Admin
{
    public partial class EditUser : System.Web.UI.Page
    {
        private readonly UserService _userService = new UserService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Initialize();
            }
        }

        private void Initialize()
        {
            User user = this._userService.GetUserLogged();
            Username.Text = user.Username;
            Name.Text = user.Name;
            Surname.Text = user.Surname;
            Email.Text = user.Email;
        }

        protected void Close_OnClick(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(Page.GetType(), "cancel", "CloseEdit();", true);
        }

        protected void Save_OnClick(object sender, EventArgs e)
        {
            this._userService.UpdateUser(Name.Text, Surname.Text, Email.Text);
            ClientScript.RegisterStartupScript(Page.GetType(), "update", "UpdateConfirm();", true);
        }
    }
}