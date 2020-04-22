using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.Clients.WebAPI.Http;
using VecompSoftware.DocSuiteWeb.DTO.OData;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.Services.Logging;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Exceptions;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Fascicles
{
    public class FascicleFinder : BaseWebAPIFinder<Fascicle, FascicleModel>
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]
        public bool ExpandProperties { get; set; }
        public bool? CheckNotExistEndDate { get; set; }
        public Entity.Fascicles.FascicleType? FascicleType { get; set; }
        public int? IdCategory { get; set; }
        public ICollection<Guid> FascicleIds { get; set; }
        public bool ExpandRoles { get; set; }
        public FascicleFinderModel FascicleFinderModel { get; set; }

        private bool FromFinderModel => FascicleFinderModel != null;

        #endregion

        #region [ Constructor ]

        public FascicleFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public FascicleFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
            FascicleIds = new List<Guid>();
        }

        #endregion

        #region [ Methods ]
        private TResult CurrentTenantExecutionWebAPI<TModel, TResult>(Func<TenantModel, TResult> func, string methodName)
        {
            string errorMessage = string.Concat("Errore nell'esecuzione del metodo ", methodName, " .");

            try
            {
                return func(CurrentTenant);
            }
            catch (Exception ex)
            {
                FileLogger.Error(Logger, errorMessage, ex);
                throw new WebAPIException<TResult>(ex.Message, ex);
            }
        }

        public override int Count()
        {
            if (!FromFinderModel)
            {
                return base.Count();
            }

            return CurrentTenantExecutionWebAPI<Fascicle, int>((tenant) =>
            {
                IODATAQueryManager odataQuery = GetODataQuery();
                odataQuery = odataQuery.Function(string.Format(CommonDefinition.OData.FascicleService.FX_GetCountAuthorizedFascicles, JsonConvert.SerializeObject(FascicleFinderModel,
                    new JsonSerializerSettings() { DateFormatString = "yyyy-MM-ddTHH:mm:ssZ" })));
                TenantEntityConfiguration tenantConfiguration = CurrentTenant.Entities.Where(x => x.Key.Equals(nameof(Fascicle))).Select(s => s.Value).SingleOrDefault();
                WebApiHttpClient httpClient = GetWebAPIClient(CurrentTenant, tenantConfiguration);
                int result = httpClient.GetAsync<Fascicle>().WithRowQuery(odataQuery.Compile()).ResponseToModel<ODataModel<int>>().Value;
                return result;
            }, nameof(Count));            
        }

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (FromFinderModel)
            {
                FascicleFinderModel.Skip = PageIndex;
                FascicleFinderModel.Top = (CustomPageIndex + 1) * PageSize;
                odataQuery = odataQuery.Function(string.Format(CommonDefinition.OData.FascicleService.FX_GetAuthorizedFascicles, JsonConvert.SerializeObject(FascicleFinderModel,
                    new JsonSerializerSettings() { DateFormatString = "yyyy-MM-ddTHH:mm:ssZ" })));
                EnableTopOdata = false;
                return odataQuery;
            }

            if (ExpandProperties)
            {
                odataQuery = odataQuery.Expand("Category")
                    .Expand("FascicleDocumentUnits")
                    .Expand("FascicleDocuments")
                    .Expand("FascicleDocuments($expand=FascicleFolder)")
                    .Expand("WorkflowInstances")
                    .Expand("WorkflowInstances($expand=WorkflowActivities)");
            }

            if (FascicleIds.Count > 0)
            {
                ICollection<string> expressions = FascicleIds.Select(s => string.Format("UniqueId eq {0}", s)).ToList();
                string odataExpression = string.Empty;
                foreach (string item in expressions)
                {
                    if (string.IsNullOrEmpty(odataExpression))
                    {
                        odataExpression = item;
                    }
                    else
                    {
                        odataExpression = string.Format("{0} or {1}", odataExpression, item);
                    }
                }

                odataQuery = odataQuery.Filter(odataExpression);
            }

            if (CheckNotExistEndDate.HasValue && CheckNotExistEndDate.Value)
            {
                odataQuery = odataQuery.Filter("EndDate eq null");
            }

            if (FascicleType.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Concat("FascicleType eq VecompSoftware.DocSuiteWeb.Entity.Fascicles.FascicleType'", (int)FascicleType.Value, "'"));
            }

            if (IdCategory.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Concat("Category/EntityShortId eq ", IdCategory.Value));
            }

            if (ExpandRoles)
            {
                odataQuery = odataQuery.Expand("FascicleRoles($expand=Role)");
            }

            return base.DecorateFinder(odataQuery);
        }

        public override void ResetDecoration()
        {
            CheckNotExistEndDate = null;
            FascicleType = null;
            IdCategory = null;
            ExpandProperties = false;
            ExpandRoles = false;
            FascicleIds = new List<Guid>();
        }

        #endregion
    }
}
