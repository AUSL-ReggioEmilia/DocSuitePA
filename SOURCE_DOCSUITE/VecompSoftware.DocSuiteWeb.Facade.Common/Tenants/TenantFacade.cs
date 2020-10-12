using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using VecompSoftware.Clients.WebAPI.Http;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Tenants;
using VecompSoftware.DocSuiteWeb.DTO.OData;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Facade.Common.WebAPI;
using VecompSoftware.DocSuiteWeb.Model.Entities.Tenants;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.DocSuiteWeb.Model.WebAPI.Client;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.DocSuiteWeb.Facade.Common.Tenants
{
    public class TenantFacade
    {
        public Tenant GetCurrentTenant()
        {
            try
            {
                ICollection<Tenant> userTenants = WebAPIImpersonatorFacade.ImpersonateFinder(new UserTenantFinder(DocSuiteContext.Current.Tenants),
                    (impersonationType, finder) =>
                    {
                        finder.EnablePaging = false;
                        ICollection<WebAPIDto<Tenant>> results = finder.DoSearch();
                        ICollection<Tenant> tenants = null;
                        if (results != null)
                        {
                            tenants = results.Select(r => r.Entity).ToList();                        
                        }
                        return tenants;
                    });

                UserLog currentUserLog = FacadeFactory.Instance.UserLogFacade.GetByUser(DocSuiteContext.Current.User.FullUserName);
                Tenant currentTenant;
                ICollection<Entity.Commons.Container> tmpTenantContainers;
                int odataSkip = 0;
                bool readContainers = true;
                foreach (Tenant item in userTenants)
                {
                    if (item.UniqueId == currentUserLog.CurrentTenantId)
                    {
                        currentTenant = WebAPIImpersonatorFacade.ImpersonateFinder(new TenantFinder(DocSuiteContext.Current.Tenants),
                            (impersonationType, finder) =>
                            {
                                finder.IncludeContainers = false;
                                finder.IncludeTenantAOO = true;
                                finder.UniqueId = item.UniqueId;
                                finder.EnablePaging = false;
                                return finder.DoSearch().Select(s => s.Entity).FirstOrDefault();
                            });                        

                        do
                        {
                            tmpTenantContainers = WebAPIImpersonatorFacade.ImpersonateFinder(new Data.WebAPI.Finder.Commons.ContainerFinder(DocSuiteContext.Current.Tenants),
                                (impersonationType, finder) =>
                                {
                                    finder.IdTenant = item.UniqueId;
                                    finder.EnablePaging = true;
                                    finder.PageIndex = odataSkip;
                                    finder.PageSize = DocSuiteContext.Current.DefaultODataTopQuery;
                                    return finder.DoSearch().Select(s => s.Entity).ToList();
                                });

                            currentTenant.Containers = currentTenant.Containers.Union(tmpTenantContainers).ToList();
                            odataSkip += tmpTenantContainers.Count;
                            readContainers = tmpTenantContainers.Count >= DocSuiteContext.Current.DefaultODataTopQuery;
                        } while (readContainers);
                        return currentTenant;
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

        public ICollection<Tenant> GetAuthorizedTenants()
        {
            return WebAPIImpersonatorFacade.ImpersonateFinder(new UserTenantFinder(DocSuiteContext.Current.Tenants),
                (impersonationType, finder) =>
            {
                try
                {
                    finder.EnablePaging = false;
                    ICollection<WebAPIDto<Tenant>> results = finder.DoSearch();
                    ICollection<Tenant> tenants;
                    if (results != null)
                    {
                        tenants = results.Select(r => r.Entity).ToList();
                        return tenants;
                    }
                }
                catch (Exception ex)
                {
                    FileLogger.Error(LogName.FileLog, string.Concat("GetAuthorizedTenants -> ", ex.Message), ex);
                }
                FileLogger.Warn(LogName.FileLog, string.Concat("GetAuthorizedTenants -> Result is null.", HttpContext.Current.User.Identity));
                return null;
            });
        }
    }
}
