using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.Clients.WebAPI.Http;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
using VecompSoftware.DocSuiteWeb.Model.WebAPI.Client;
using VecompSoftware.Helpers.WebAPI;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.WebAPIManager.Dao
{
    public abstract class BaseWebAPIDao<T, THeader, TWebAPIFinder> : IWebAPIDao<T>
        where TWebAPIFinder : IWebAPIFinder<T, THeader>
    {
        #region [ Fields ]

        private const string GET_BY_ID_ODATA = "$filter=UniqueId eq {0}";
        private readonly IHttpClientConfiguration _clientConfiguration;
        private readonly IHttpClientConfiguration _originalConfiguration;
        private readonly TWebAPIFinder _finder;
        private WebAPIHelper _context;

        #endregion

        #region [ Properties ]

        public WebAPIHelper Context
        {
            get
            {
                if (_context == null)
                    _context = new WebAPIHelper();

                return _context;
            }
        }

        public TWebAPIFinder Finder
        {
            get
            {               
                return _finder;
            }
        }

        #endregion

        #region [ Constructor ]

        public BaseWebAPIDao(IHttpClientConfiguration configuration, IHttpClientConfiguration originalConfiguration, TWebAPIFinder finder)
        {
            _clientConfiguration = configuration;
            _originalConfiguration = originalConfiguration;
            _finder = finder;
            _finder.EnablePaging = false;
        }

        #endregion

        #region [ Methods ]

        public int Count()
        {
            SetEntityREST();
            //return _finder.Count();
            return Context.GetRequest<T, int>(_clientConfiguration, _originalConfiguration, "$count");
        }

        public void Delete(ref T entity)
        {
            throw new NotImplementedException();
        }

        public IList<T> GetAll()
        {
            _finder.ResetDecoration();
            return _finder.DoSearch().Select(s => s.Entity).ToList();
        }

        public T GetById(Guid id)
        {
            _finder.ResetDecoration();
            _finder.UniqueId = id;
            WebAPIDto<T> result = _finder.DoSearch().SingleOrDefault();
            return result.Entity;
        }

        public void Save(ref T entity)
        {
            SetEntityREST();
            Context.SendRequest(_clientConfiguration, _originalConfiguration, entity);
        }

        public void Update(ref T entity)
        {
            SetEntityREST();
            Context.SendUpdateRequest(_clientConfiguration, _originalConfiguration, entity);
        }

        public void Update(ref T entity, string actionType)
        {
            SetEntityREST();
            Context.SendUpdateRequest(_clientConfiguration, _originalConfiguration, entity, actionType);
        }

        private void SetEntityREST()
        {
            string entityName = typeof(T).Name;
            IWebApiControllerEndpoint controller = _clientConfiguration.EndPoints.Single(f => f.EndpointName.Equals(entityName, StringComparison.InvariantCultureIgnoreCase));
            controller.AddressName = WebApiHttpClient.API_ADDRESS_NAME;
            controller.ControllerName = entityName;
        }
        #endregion
    }
}
