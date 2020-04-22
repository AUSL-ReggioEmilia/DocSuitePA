using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AmministrazioneTrasparente
{
    public partial class Error : System.Web.UI.Page
    {

        public string ContactMail
        {
            get
            {
                return ConfigurationManager.AppSettings["ContactMail"];
            }
        }

        public string LogoImg
        {
            get
            {
                var temp = ConfigurationManager.AppSettings["LogoImg"];
                return string.IsNullOrEmpty(temp) ? "LogoAzienda.gif" : temp;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}