using System;
using System.Configuration;
using System.Linq;
using HibernatingRhinos.Profiler.Appender.NHibernate;
using NHibernate;
using VecompSoftware.DocSuiteWeb.API.Helpers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.NHibernateManager;

namespace VecompSoftware.DocSuiteWeb.API
{
    public class Global : System.Web.HttpApplication
    {
        #region [ Properties ]
        private static Tenant _currentTenant;
        private static Tenant CurrentTenant
        {
            get
            {
                if (_currentTenant == null)
                {
                    _currentTenant = WebAPIImpersonatorFacade.ImpersonateFinder(new TenantFinder(DocSuiteContext.Current.Tenants),
                           (impersonationType, finder) =>
                           {
                               finder.IncludeContainers = false;
                               finder.IncludeTenantAOO = true;
                               finder.IdTenantAOO = ConfigurationHelper.CurrentTenantAOOId;
                               finder.EnablePaging = false;
                               return finder.DoSearch().Select(s => s.Entity).FirstOrDefault();
                           });
                }
                return _currentTenant;
            }
        }
        #endregion

        protected void Application_Start(object sender, EventArgs e)
        {
            Services.StampaConforme.Service.InitializeSignatureTemplateXml(DocSuiteContext.Current.ProtocolEnv.SignatureTemplate);

            NHibernateSessionUtil.ApplyFilterActions.Add((ISession session) => session.EnableFilter("TenantFilter").SetParameter("tenantAOOId", ConfigurationHelper.CurrentTenantAOOId));
            FacadeUtil.NeedTenantAction = (t) => t(CurrentTenant);
#if DEBUG
            NHibernateProfiler.Initialize();
#endif
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