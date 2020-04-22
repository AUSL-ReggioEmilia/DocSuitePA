using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
using VecompSoftware.DocSuiteWeb.Entity;
using VecompSoftware.DocSuiteWeb.EntityMapper.WebAPI;
using VecompSoftware.Services.Logging;
using VecompSoftware.WebAPIManager.Dao;
using VecompSoftware.WebAPIManager.Exceptions;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI
{
    public abstract class FacadeWebAPIBase<T, TDao> : IFacadeWebAPIBase<T>
        where T : IDSWEntity
        where TDao : IWebAPIDao<T>
    {
        #region [ Fields ]        

        private readonly WebAPIDtoMapper<T> _mapper;
        public const string ERROR_MESSAGE = "Errore nell'esecuzione del metodo {0} per il Tenant {1}";

        #endregion

        #region [ Properties ]

        protected ICollection<WebAPITenantConfiguration<T, TDao>> _daoConfigurations;

        protected WebAPITenantConfiguration<T, TDao> CurrentTenantConfiguration
        {
            get
            {
                return _daoConfigurations.First(f => f.IsCurrent);
            }
        }
        protected string Logger
        {
            get { return LogName.WebAPIClientLog; }
        }

        #endregion

        #region [ Constructor ]

        public FacadeWebAPIBase(ICollection<WebAPITenantConfiguration<T, TDao>> daoConfigurations)
        {
            _mapper = new WebAPIDtoMapper<T>();
            _daoConfigurations = daoConfigurations.Where(f => f.Tenant.CurrentTenant || (!f.Tenant.CurrentTenant && f.Tenant.Entities.Any(x => x.Key == typeof(T).Name && x.Value.IsActive))).ToList();
        }

        #endregion

        #region [ Methods ]

        public int Count()
        {
            int count = 0;
            foreach (IWebAPITenantConfiguration<T, TDao> configuration in _daoConfigurations)
            {
                try
                {
                    count += configuration.Dao.Count();
                }
                catch (Exception ex)
                {
                    FileLogger.Error(Logger, string.Format(ERROR_MESSAGE, "Count", configuration.Tenant.TenantName), ex);
                    throw new WebAPIException<int>(ex.Message, ex) { Results = count };
                }                
            }
            return count;
        }

        public IList<WebAPIDto<T>> GetAll()
        {
            List<WebAPIDto<T>> items = new List<WebAPIDto<T>>();
            ICollection<T> results = null;
            foreach (IWebAPITenantConfiguration<T, TDao> configuration in _daoConfigurations)
            {
                try
                {
                    results = configuration.Dao.GetAll();
                    items.AddRange(results.Select(s => _mapper.TransformDTO(s, configuration.Tenant)));
                }
                catch (Exception ex)
                {
                    FileLogger.Error(Logger, string.Format(ERROR_MESSAGE, "GetAll", configuration.Tenant.TenantName), ex);
                    throw new WebAPIException<List<WebAPIDto<T>>>(ex.Message, ex) { Results = items, TenantName = configuration.Tenant.TenantName };
                }                
            }
            return items;
        }

        public WebAPIDto<T> GetById(Guid id)
        {
            IWebAPITenantConfiguration<T, TDao> currentTenant = _daoConfigurations.Single(s => s.IsCurrent);
            return _mapper.TransformDTO(currentTenant.Dao.GetById(id), currentTenant.Tenant);
        }

        public void Save(T entity)
        {
            WindowsIdentity wi = HttpContext.Current.User.Identity as WindowsIdentity;
            using (WindowsImpersonationContext wic = wi.Impersonate())
            using (ExecutionContext.SuppressFlow())
            {
                TDao currentTenant = _daoConfigurations.Single(s => s.IsCurrent).Dao;
                currentTenant.Save(ref entity);
            }
        }

        public void Update(T entity)
        {
            WindowsIdentity wi = HttpContext.Current.User.Identity as WindowsIdentity;
            using (WindowsImpersonationContext wic = wi.Impersonate())
            using (ExecutionContext.SuppressFlow())
            {
                TDao currentTenant = _daoConfigurations.Single(s => s.IsCurrent).Dao;
                currentTenant.Update(ref entity);
            }
        }

        public void Update(T entity, string actionType = "")
        {
            WindowsIdentity wi = HttpContext.Current.User.Identity as WindowsIdentity;
            using (WindowsImpersonationContext wic = wi.Impersonate())
            using (ExecutionContext.SuppressFlow())
            {
                TDao currentTenant = _daoConfigurations.Single(s => s.IsCurrent).Dao;
                currentTenant.Update(ref entity, actionType);
            }
        }
        public void Delete(T entity)
        {
            WindowsIdentity wi = HttpContext.Current.User.Identity as WindowsIdentity;
            using (WindowsImpersonationContext wic = wi.Impersonate())
            using (ExecutionContext.SuppressFlow())
            {
                TDao currentTenant = _daoConfigurations.Single(s => s.IsCurrent).Dao;
                currentTenant.Delete(ref entity);
            }
        }
        #endregion
    }
}
