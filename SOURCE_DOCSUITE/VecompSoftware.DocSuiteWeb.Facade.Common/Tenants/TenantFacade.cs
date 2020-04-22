using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Tenants;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.DocSuiteWeb.Facade.Common.Tenants
{
    public class TenantFacade
    {
        public Tenant GetCurrentTenant()
        {
            WindowsIdentity wi = (WindowsIdentity)HttpContext.Current.User.Identity;
            using (WindowsImpersonationContext wic = wi.Impersonate())
            using (ExecutionContext.SuppressFlow())
            {
                try
                {
                    UserTenantFinder finder = new UserTenantFinder(DocSuiteContext.Current.Tenants);
                    finder.EnablePaging = false;
                    ICollection<WebAPIDto<Tenant>> results = finder.DoSearch();
                    ICollection<Tenant> tenants;
                    if (results != null)
                    {
                        tenants = results.Select(r => r.Entity).ToList();
                        UserLog currentUserLog = FacadeFactory.Instance.UserLogFacade.GetByUser(DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain);
                        Tenant currentTenant;
                        TenantFinder tenantFinder;
                        foreach (Tenant item in tenants)
                        {
                            if (item.UniqueId == currentUserLog.CurrentTenantId)
                            {
                                tenantFinder = new TenantFinder(DocSuiteContext.Current.Tenants)
                                {
                                    IncludeContainers = true,
                                    UniqueId = item.UniqueId,
                                    EnablePaging = false
                                };
                                currentTenant = tenantFinder.DoSearch().Select(s => s.Entity).FirstOrDefault();
                                return currentTenant;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    FileLogger.Error(LogName.FileLog, string.Concat("GetCurrentTenant -> ", ex.Message), ex);
                }
                FileLogger.Warn(LogName.FileLog, string.Concat("GetCurrentTenant -> Result is null.", HttpContext.Current.User.Identity));
                return null;
            }
        }
    }
}
