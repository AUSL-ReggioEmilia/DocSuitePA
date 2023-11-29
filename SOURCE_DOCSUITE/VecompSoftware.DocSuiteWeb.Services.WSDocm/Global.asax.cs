using System;
using iText.License;
using Limilabs.Mail.Licensing;

namespace VecompSoftware.DocSuiteWeb.Services.WSDocm
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            LicenseKey.LoadLicenseFile(Server.MapPath("itextkey.xml"));
            string license = LicenseHelper.GetLicensePath();
            LicenseStatus status = LicenseHelper.GetLicenseStatus();
            if (status != LicenseStatus.Valid)
            {
                throw new Exception($"Licenza MailLicense.xml non valida {status} in {license}");
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}