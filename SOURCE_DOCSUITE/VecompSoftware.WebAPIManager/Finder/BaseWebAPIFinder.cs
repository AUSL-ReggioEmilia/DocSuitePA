using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VecompSoftware.Clients.WebAPI.Http;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.DTO.OData;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
using VecompSoftware.DocSuiteWeb.EntityMapper.WebAPI;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.DocSuiteWeb.Model.WebAPI.Client;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.Services.Logging;
using VecompSoftware.WebAPIManager.Exceptions;

namespace VecompSoftware.WebAPIManager.Finder
{
    public abstract class BaseWebAPIFinder<T, THeader> : IWebAPIFinder<T, THeader>
        where T : class
        where THeader : class, new()
    {
        #region [ Fields ]

        //private IReadOnlyCollection<TenantModel> _tenants;
        private readonly TenantModel _tenant;
        private readonly WebAPIDtoMapper<THeader> _headerMapper;
        private readonly WebAPIDtoMapper<T> _mapper;
        private IDictionary<string, string> _sortExpressions;
        private IDictionary<string, IFilterExpression> _filterExpressions;
        private int _virtualCount;

        private const string _oDataDateConversion = "yyyyMMddHHmmss";
        #endregion

        #region [ Constructor ]

        public BaseWebAPIFinder(TenantModel tenant)
            : this(new List<TenantModel>() { tenant })
        { }

        public BaseWebAPIFinder(IReadOnlyCollection<TenantModel> tenants)
        {
            _headerMapper = new WebAPIDtoMapper<THeader>();
            _mapper = new WebAPIDtoMapper<T>();
            //_tenant = CloneTenantModel(tenants.Single(f => f.CurrentTenant)); dosn't work
            _tenant = CloneTenantModel(tenants.First());
            ResetDecoration();
            EnablePaging = true;
            EnableTopOdata = true;
        }

        #endregion

        #region [ Properties ]

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public IDictionary<string, string> SortExpressions
        {
            get
            {
                if (_sortExpressions == null)
                {
                    _sortExpressions = new Dictionary<string, string>();
                }

                return _sortExpressions;
            }
        }

        public Guid? UniqueId { get; set; }

        public IDictionary<string, IFilterExpression> FilterExpressions
        {
            get
            {
                if (_filterExpressions == null)
                {
                    _filterExpressions = new Dictionary<string, IFilterExpression>();
                }

                return _filterExpressions;
            }
        }

        public bool EnablePaging { get; set; }

        public bool EnableTopOdata { get; set; }

        protected string Logger
        {
            get { return LogName.FileLog; }
        }

        public bool EnableTableJoin { get; set; }

        public int CustomPageIndex
        {
            get
            {
                return (int)Math.Ceiling(PageIndex / (double)PageSize);
            }
            set
            {
                PageIndex = value * PageSize;
            }
        }

        public string ODataDateConversion
        {
            get { return _oDataDateConversion; }
        }

        protected TenantModel CurrentTenant => _tenant;
        #endregion

        #region [ Methods ]

        public abstract void ResetDecoration();

        internal ICollection<WebAPIDto<TModel>> CurrentTenantExecutionWebAPI<TModel>(Func<TenantModel, ICollection<WebAPIDto<TModel>>, ICollection<WebAPIDto<TModel>>> func,
            Func<IEnumerable<WebAPIDto<TModel>>, ICollection<WebAPIDto<TModel>>> finalizeSearchLambda, string methodName)
        {
            string errorMessage = string.Concat("Errore nell'esecuzione del metodo ", methodName, " .");
            BlockingCollection<WebAPIDto<TModel>> results = new BlockingCollection<WebAPIDto<TModel>>();

            try
            {
                func(_tenant, new List<WebAPIDto<TModel>>()).ToList().ForEach(x => results.Add(x));
            }
            catch (Exception ex)
            {
                FileLogger.Error(Logger, errorMessage, ex);
                throw new WebAPIException<ICollection<WebAPIDto<TModel>>>(ex.Message, ex) { Results = finalizeSearchLambda(results) };
            }
            return finalizeSearchLambda(results.OrderByDescending(x => x.TenantModel.CurrentTenant).ToList());
        }

        /// <summary>
        /// Aggancia le espressioni di ordinamento
        /// </summary>
        protected virtual ICollection<WebAPIDto<U>> AttachSortExpressions<U>(IEnumerable<WebAPIDto<U>> source)
            where U : class
        {
            AttachSortExpressions(ref source);
            return source.ToList();
        }

        /// <summary>
        /// Aggancia le espressioni di filtro
        /// </summary>

        protected virtual IODATAQueryManager AttachFilterExpressions(IODATAQueryManager odataQuery)
        {
            AttachFilterExpressions(ref odataQuery);
            return odataQuery;
        }

        /// <summary>
        /// Aggiunge le espressioni di paginazione
        /// </summary>
        protected virtual ICollection<U> ExecutePaging<U>(IEnumerable<U> source)
        {
            if (EnablePaging)
            {
                source = source.Skip(PageIndex).Take(PageSize);
            }
            return source.ToList();
        }

        /// <summary>
        /// Decora il Finder. Metodo che deve essere implementato nelle classi figlie.
        /// </summary>
        public virtual IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (UniqueId.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Concat("UniqueId eq ", UniqueId));
            }
            return odataQuery;
        }

        private void SetEntityODATA(IHttpClientConfiguration config, string controllerName)
        {
            string entityName = typeof(T).Name;
            IWebApiControllerEndpoint controller = config.EndPoints.Single(f => f.EndpointName.Equals(entityName, StringComparison.InvariantCultureIgnoreCase));
            controller.AddressName = WebApiHttpClient.ODATA_ADDRESS_NAME;
            controller.ControllerName = controllerName;
        }

        protected WebApiHttpClient GetWebAPIClient(TenantModel tenant, TenantEntityConfiguration config)
        {
            SetEntityODATA(tenant.WebApiClientConfig, config.ODATAControllerName);
            return new WebApiHttpClient(tenant.WebApiClientConfig, tenant.OriginalConfiguration, (s) => FileLogger.Debug(LogName.WebAPIClientLog, s));
        }

        protected IODATAQueryManager GetODataQuery()
        {
            return new ODATAQueryManager();
        }

        /// <summary>      
        /// ATTENZIONE! Tale metodo non è ancora implementato in quanto $count=true non ritorna
        /// un singolo valore intero, ma aggiunge una proprietà count al json di ritorno che in 
        /// questo momento la libreria Simple.OData.Client non gestisce.
        /// </summary>
        /// <remarks>
        /// Non è possibile aggiungere una eccezione in quanto il metodo Count viene utilizzato dalla
        /// DSW grid.
        /// </remarks>
        /// <returns>
        /// Ritorna il counting degli elementi.
        /// </returns>
        public virtual int Count()
        {
            return _virtualCount;
        }

        /// <summary>
        /// Crea la DoSearch con i metodi standard.
        /// Se non implementati, vengono usati quelli standard
        /// </summary>
        public ICollection<WebAPIDto<T>> DoSearch()
        {
            ICollection<WebAPIDto<T>> elements = CurrentTenantExecutionWebAPI<T>((tenant, results) =>
            {
                WebApiHttpClient httpClient = GetWebAPIClient(tenant, tenant.Entities.GetFromType<T>());
                IODATAQueryManager odataQuery = GetODataQuery();
                odataQuery = DecorateFinder(odataQuery);
                if (EnableTopOdata)
                {
                    odataQuery = odataQuery.Top(DocSuiteContext.Current.DefaultODataTopQuery);
                }
                odataQuery = AttachFilterExpressions(odataQuery);
                ODataModel<ICollection<T>> foundResults = httpClient.GetAsync<T>().WithRowQuery(odataQuery.Compile()).ResponseToModel<ODataModel<ICollection<T>>>();
                if (foundResults == null || foundResults.Value == null)
                {
                    return new List<WebAPIDto<T>>();
                }
                foundResults.Value.ToList().ForEach(x => results.Add(_mapper.TransformDTO(x, tenant)));
                return results;
            }, FinalizeDoSearch, "DoSearch");

            return elements;
        }

        /// <summary>
        /// Crea la DoSearchHeader con i metodi standard.
        /// Se non implementati, vengono usati quelli standard
        /// </summary>
        public ICollection<WebAPIDto<THeader>> DoSearchHeader()
        {
            ICollection<WebAPIDto<THeader>> searchHeaderList = CurrentTenantExecutionWebAPI<THeader>((tenant, results) =>
            {
                WebApiHttpClient httpClient = GetWebAPIClient(tenant, tenant.Entities.GetFromType<T>());
                IODATAQueryManager odataQuery = GetODataQuery();
                odataQuery = DecorateFinder(odataQuery);
                if (EnableTopOdata)
                {
                    odataQuery = odataQuery.Top(DocSuiteContext.Current.DefaultODataTopQuery);
                }
                odataQuery = AttachFilterExpressions(odataQuery);
                SetProjectionsHeaders(httpClient, odataQuery).ToList().ForEach(x => results.Add(_headerMapper.TransformDTO(x, tenant)));
                return results;
            }, FinalizeDoSearchHeader, "DoSearchHeader");

            return searchHeaderList;
        }

        private ICollection<WebAPIDto<T>> FinalizeDoSearch(IEnumerable<WebAPIDto<T>> source)
        {
            source = AttachSortExpressions(source);
            return ExecutePaging(source);
        }

        private ICollection<WebAPIDto<THeader>> FinalizeDoSearchHeader(IEnumerable<WebAPIDto<THeader>> source)
        {
            source = AttachTenantFilter(source);
            source = AttachSortExpressions(source);
            _virtualCount = source.Count();
            return ExecutePaging(source);
        }
        private TenantModel CloneTenantModel(TenantModel source)
        {
            string serializedSource = JsonConvert.SerializeObject(source, DocSuiteContext.DefaultWebAPIJsonSerializerSettings);
            return JsonConvert.DeserializeObject<TenantModel>(serializedSource, DocSuiteContext.DefaultWebAPIJsonSerializerSettings);
        }
        private IReadOnlyCollection<TenantModel> CloneTenantModel(IEnumerable<TenantModel> source)
        {
            string serializedSource = JsonConvert.SerializeObject(source, DocSuiteContext.DefaultWebAPIJsonSerializerSettings);
            return JsonConvert.DeserializeObject<List<TenantModel>>(serializedSource, DocSuiteContext.DefaultWebAPIJsonSerializerSettings);
        }

        /// <summary>
        /// Applica le espressioni di trasformata del risultato
        /// </summary>
        public virtual IEnumerable<THeader> SetProjectionsHeaders(WebApiHttpClient httpClient, IODATAQueryManager odataQuery)
        {
            ODataModel<ICollection<THeader>> foundResults = httpClient.GetAsync<T>().WithRowQuery(odataQuery.Compile()).ResponseToModel<ODataModel<ICollection<THeader>>>();
            if (foundResults == null || foundResults.Value == null)
            {
                return Enumerable.Empty<THeader>();
            }
            return foundResults.Value;
        }

        protected virtual bool AttachSortExpressions<U>(ref IEnumerable<WebAPIDto<U>> source) where U : class
        {
            return AttachSortExpressions(ref source, SortExpressions);
        }

        protected virtual bool AttachSortExpressions<U>(ref IEnumerable<WebAPIDto<U>> source, IDictionary<string, string> conditions) where U : class
        {
            if (conditions == null)
            {
                return false;
            }

            IQueryable<WebAPIDto<U>> query = source.AsQueryable();
            try
            {
                foreach (KeyValuePair<string, string> condition in conditions)
                {
                    string[] compositeCondition = condition.Key.Split(',');
                    ParameterExpression param = Expression.Parameter(typeof(WebAPIDto<U>), "x");

                    if (condition.Value.Eq("ASC"))
                    {
                        IOrderedQueryable<WebAPIDto<U>> tmpQuery = query.OrderBy(GetSortExpression<WebAPIDto<U>>(param, compositeCondition[0]));
                        foreach (string childColumn in compositeCondition.Skip(1))
                        {
                            tmpQuery = tmpQuery.ThenBy(GetSortExpression<WebAPIDto<U>>(param, childColumn));
                        }

                        source = tmpQuery;
                    }
                    else
                    {
                        IOrderedQueryable<WebAPIDto<U>> tmpQuery = query.OrderByDescending(GetSortExpression<WebAPIDto<U>>(param, compositeCondition[0]));
                        foreach (string childColumn in compositeCondition.Skip(1))
                        {
                            tmpQuery = tmpQuery.ThenByDescending(GetSortExpression<WebAPIDto<U>>(param, childColumn));
                        }

                        source = tmpQuery;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DocSuiteException("Errore ordinamento", "Espressione di ordinamento non corretta", ex);
            }
            return true;
        }

        protected virtual bool AttachFilterExpressions(ref IODATAQueryManager odataQuery)
        {
            if (FilterExpressions == null)
            {
                return false;
            }

            try
            {
                foreach (KeyValuePair<string, IFilterExpression> filter in FilterExpressions.Where(x => !x.Key.Contains("TenantModel")))
                {
                    string expr = GetExpression(filter.Value);
                    odataQuery.Filter(expr);
                }
            }
            catch (Exception ex)
            {
                throw new DocSuiteException("Errore filtro", "Espressione di filtro non corretta", ex);
            }
            return true;
        }

        private ICollection<WebAPIDto<THeader>> AttachTenantFilter(IEnumerable<WebAPIDto<THeader>> source)
        {
            if (FilterExpressions == null || !FilterExpressions.Any(x => x.Key.Contains("TenantModel")))
            {
                return source.ToList();
            }

            try
            {
                foreach (KeyValuePair<string, IFilterExpression> filter in FilterExpressions.Where(x => x.Key.Contains("TenantModel")))
                {
                    ParameterExpression param = Expression.Parameter(typeof(WebAPIDto<THeader>), "x");
                    string[] parts = filter.Value.PropertyName.Replace("Entity.", "").Split('.');
                    Expression parent = param;
                    foreach (string part in parts)
                    {
                        parent = Expression.Property(parent, part);
                    }

                    ConstantExpression constant = Expression.Constant(filter.Value.FilterValue);
                    Expression expr = Expression.Equal(parent, constant);
                    source = source.AsQueryable().Where(Expression.Lambda<Func<WebAPIDto<THeader>, bool>>(expr, param));
                }
            }
            catch (Exception ex)
            {
                throw new DocSuiteException("Errore filtro", "Espressione di filtro non corretta", ex);
            }
            return source.ToList();
        }

        private static Expression<Func<U, object>> GetSortExpression<U>(ParameterExpression param, string column)
        {
            string[] parts = column.Split('.');
            Expression parent = param;
            foreach (string part in parts)
            {
                parent = Expression.Property(parent, part);
            }
            Expression conversion = Expression.Convert(parent, typeof(object));
            Expression<Func<U, object>> expr = Expression.Lambda<Func<U, object>>(conversion, param);
            return expr;
        }

        private static string GetExpression(IFilterExpression filter)
        {
            string filterExpression = string.Empty;
            string propertyFormatted = filter.PropertyName.Replace("Entity.", "").Replace(".", "/");
            switch (filter.FilterExpression)
            {
                case FilterExpression.FilterType.GreaterThan:
                    filterExpression = string.Concat(propertyFormatted, " gt ", GetValueFormatted(filter.FilterValue));
                    break;

                case FilterExpression.FilterType.GreaterThanOrEqualTo:
                    filterExpression = string.Concat(propertyFormatted, " ge ", GetValueFormatted(filter.FilterValue));
                    break;

                case FilterExpression.FilterType.LessThan:
                    filterExpression = string.Concat(propertyFormatted, " lt ", GetValueFormatted(filter.FilterValue));
                    break;

                case FilterExpression.FilterType.LessThanOrEqualTo:
                    filterExpression = string.Concat(propertyFormatted, " le ", GetValueFormatted(filter.FilterValue));
                    break;

                case FilterExpression.FilterType.IsNull:
                    filterExpression = string.Concat(propertyFormatted, " eq null");
                    break;

                case FilterExpression.FilterType.IsNotNull:
                    filterExpression = string.Concat(propertyFormatted, " not eq null");
                    break;

                case FilterExpression.FilterType.Contains:
                    filterExpression = string.Concat("contains(", propertyFormatted, ", '", filter.FilterValue, "')");
                    break;

                case FilterExpression.FilterType.IsEnum:
                    filterExpression = string.Concat(propertyFormatted, " eq ", filter.PropertyType.FullName, "'", filter.FilterValue, "'");
                    break;

                default:
                    filterExpression = string.Concat(propertyFormatted, " eq ", GetValueFormatted(filter.FilterValue));
                    break;
            }

            return filterExpression;
        }

        private static string GetValueFormatted(object value)
        {
            if (value is string)
            {
                return string.Format("'{0}'", value);
            }
            return value.ToString();
        }

        public static List<TenantModel> DeepCopy(IList<TenantModel> source)
        {
            string serializedSource = JsonConvert.SerializeObject(source, DocSuiteContext.DefaultWebAPIJsonSerializerSettings);
            return JsonConvert.DeserializeObject<List<TenantModel>>(serializedSource, DocSuiteContext.DefaultWebAPIJsonSerializerSettings);
        }
        #endregion

        #region [ Not implemented Methods ]

        [Obsolete("Utilizzare DoSearch() e popolare esternamente le proprità di ordinamento")]
        public ICollection<T> DoSearch(ICollection<Expression<Func<T, object>>> sortExpr)
        {
            throw new NotImplementedException();
        }

        [Obsolete("Utilizzare DoSearch() e popolare esternamente le proprità di ordinamento")]
        public ICollection<T> DoSearch(ICollection<Expression<Func<T, object>>> sortExpr, int startRow, int pageSize)
        {
            throw new NotImplementedException();
        }

        [Obsolete("Utilizzare DoSearch() e popolare esternamente le proprità di ordinamento")]
        IList<T> IFinder<T>.DoSearch()
        {
            throw new NotImplementedException();
        }


        [Obsolete("Utilizzare DoSearch() e popolare esternamente le proprità di ordinamento")]
        public IList<T> DoSearch(string sortExpr)
        {
            throw new NotImplementedException();
        }

        [Obsolete("Utilizzare DoSearch() e popolare esternamente le proprità di ordinamento")]
        public IList<T> DoSearch(string sortExpr, int startRow, int PageSize)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
