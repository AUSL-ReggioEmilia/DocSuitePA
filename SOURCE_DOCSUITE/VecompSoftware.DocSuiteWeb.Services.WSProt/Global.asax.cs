using NHibernate;
using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.DocSuiteWeb.Services.WSProt.Helpers;
using VecompSoftware.NHibernateManager;

namespace VecompSoftware.DocSuiteWeb.Services.WSProt
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
            NHibernateSessionUtil.ApplyFilterActions.Add((ISession session) => session.EnableFilter("TenantFilter").SetParameter("tenantAOOId", ConfigurationHelper.CurrentTenantAOOId));
            FacadeUtil.NeedTenantAction = (t) => t(CurrentTenant);
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            NHibernateSessionManager.Instance.CloseTransactionAndSessions();
        }

    }
}