using System;
using System.Web.Security;
using AmministrazioneTrasparente.Services;

namespace AmministrazioneTrasparente.Admin
{
    public partial class Login : System.Web.UI.Page
    {
        private readonly UserService _userService = new UserService();
        private readonly UserLogService _userLogService = new UserLogService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Initialize();
            }
        }

        private void Initialize()
        {
            if (Request.IsAuthenticated)
                Response.Redirect("Default.aspx");
        }

        protected void btnLogin_OnClick(object sender, EventArgs e)
        {
            bool authenticate = this._userService.Authenticate(username.Text, password.Text);
            if (authenticate)
            {
                this._userLogService.AddLog(username.Text, String.Format("Accesso eseguito per l'utente {0}", username.Text));
                FormsAuthentication.RedirectFromLoginPage(username.Text, rememberMe.Checked);
            }                

            ClientScript.RegisterStartupScript(this.GetType(), "LoginErrato", "showAlert('Username o Password errati','alert-danger');", true);
        }
    }
}