using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
using VecompSoftware.DocSuiteWeb.Entity;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
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
        private readonly Tenant _currentTenant;

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
        protected string LogName
        {
            get { return Services.Logging.LogName.WebAPIClientLog; }
        }

        protected Tenant CurrentTenant => _currentTenant;

        #endregion

        #region [ Constructor ]

        public FacadeWebAPIBase(ICollection<WebAPITenantConfiguration<T, TDao>> daoConfigurations, Tenant currentTenant)
        {
            _mapper = new WebAPIDtoMapper<T>();
            _daoConfigurations = daoConfigurations.Where(f => f.Tenant.CurrentTenant || (!f.Tenant.CurrentTenant && f.Tenant.Entities.Any(x => x.Key == typeof(T).Name && x.Value.IsActive))).ToList();
            _currentTenant = currentTenant;
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
                    WebAPIImpersonatorFacade.ImpersonateDao<TDao, T>(configuration.Dao, (impersonationType, dao) =>
                    {
                        count += dao.Count();
                    });                    
                }
                catch (Exception ex)
                {
                    FileLogger.Error(LogName, string.Format(ERROR_MESSAGE, "Count", configuration.Tenant.TenantName), ex);
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
                    WebAPIImpersonatorFacade.ImpersonateDao<TDao, T>(configuration.Dao, (impersonationType, dao) =>
                    {
                        results = dao.GetAll();
                        items.AddRange(results.Select(s => _mapper.TransformDTO(s, configuration.Tenant)));
                    });                    
                }
                catch (Exception ex)
                {
                    FileLogger.Error(LogName, string.Format(ERROR_MESSAGE, "GetAll", configuration.Tenant.TenantName), ex);
                    throw new WebAPIException<List<WebAPIDto<T>>>(ex.Message, ex) { Results = items, TenantName = configuration.Tenant.TenantName };
                }                
            }
            return items;
        }

        public WebAPIDto<T> GetById(Guid id)
        {
            IWebAPITenantConfiguration<T, TDao> currentTenant = _daoConfigurations.Single(s => s.IsCurrent);
            return WebAPIImpersonatorFacade.ImpersonateDao<TDao, T, WebAPIDto<T>>(currentTenant.Dao, (impersonationType, dao) =>
            {                
                return _mapper.TransformDTO(dao.GetById(id), currentTenant.Tenant);
            });            
        }

        public void Save(T entity)
        {
            TDao currentTenant = _daoConfigurations.Single(s => s.IsCurrent).Dao;
            WebAPIImpersonatorFacade.ImpersonateDao<TDao, T>(currentTenant, (impersonationType, dao) =>
            {
                dao.Save(ref entity);
            });
        }

        public void Save(T entity, string actionType = "")
        {
            TDao currentTenant = _daoConfigurations.Single(s => s.IsCurrent).Dao;
            WebAPIImpersonatorFacade.ImpersonateDao<TDao, T>(currentTenant, (impersonationType, dao) =>
            {
                dao.Save(ref entity, actionType);
            });
        }

        public void Update(T entity)
        {
            TDao currentTenant = _daoConfigurations.Single(s => s.IsCurrent).Dao;
            WebAPIImpersonatorFacade.ImpersonateDao<TDao, T>(currentTenant, (impersonationType, dao) =>
            {
                dao.Update(ref entity);
            });
        }

        public void Update(T entity, string actionType = "")
        {
            TDao currentTenant = _daoConfigurations.Single(s => s.IsCurrent).Dao;
            WebAPIImpersonatorFacade.ImpersonateDao<TDao, T>(currentTenant, (impersonationType, dao) =>
            {
                dao.Update(ref entity, actionType);
            });
        }
        public void Delete(T entity)
        {
            TDao currentTenant = _daoConfigurations.Single(s => s.IsCurrent).Dao;
            WebAPIImpersonatorFacade.ImpersonateDao<TDao, T>(currentTenant, (impersonationType, dao) =>
            {
                dao.Delete(ref entity);
            });
        }
        #endregion
    }
}
