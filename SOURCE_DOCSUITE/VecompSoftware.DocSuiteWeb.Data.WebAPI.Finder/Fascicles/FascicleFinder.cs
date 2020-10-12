using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.Clients.WebAPI.Http;
using VecompSoftware.DocSuiteWeb.DTO.OData;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
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
        public bool FromPostMethod { get; set; }
        public int? Year { get; set; }
        public int? Number { get; set; }
        public string FascicleTitle { get; set; }
        public string FascicleSubject { get; set; }
        public Guid? Dossier { get; set; }
        public Guid? IdMetadataRepository { get; set; }

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
                WebApiHttpClient httpClient = GetWebAPIClient(CurrentTenant);
                Dictionary<string, FascicleFinderModel> finders = new Dictionary<string, FascicleFinderModel>();
                finders.Add("finder", FascicleFinderModel);
                string bodyQuery = JsonConvert.SerializeObject(finders, new StringEnumConverter());
                int result = httpClient.PostStringAsync<Fascicle>($"/{CommonDefinition.OData.FascicleService.FX_GetCountAuthorizedFascicles}", bodyQuery).ResponseToModel<ODataModel<int>>().Value;

                return result;
            }, nameof(Count));
        }

        public List<WebAPIDto<FascicleModel>> GetFromPostMethod()
        {
            WebApiHttpClient httpClient = GetWebAPIClient(CurrentTenant);
            FascicleFinderModel.Skip = PageIndex;
            FascicleFinderModel.Top = (CustomPageIndex + 1) * PageSize;
            Dictionary<string, FascicleFinderModel> finders = new Dictionary<string, FascicleFinderModel>();
            finders.Add("finder", FascicleFinderModel);
            string bodyQuery = JsonConvert.SerializeObject(finders, new StringEnumConverter());
            IEnumerable<FascicleModel> results = httpClient.PostStringAsync<Fascicle>($"/{CommonDefinition.OData.FascicleService.FX_GetAuthorizedFascicles}", bodyQuery).ResponseToModel<ODataModel<ICollection<FascicleModel>>>().Value;
            return results.Select(fascicleModel => new WebAPIDto<FascicleModel>
            {
                Entity = fascicleModel,
                TenantModel = CurrentTenant
            }).ToList();
        }

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (ExpandProperties)
            {
                odataQuery = odataQuery
                    .Expand("Category")
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

            if (Year.HasValue && Year != 0)
            {
                odataQuery = odataQuery.Filter($"Year eq {Year}");
            }

            if (Number.HasValue && Number != 0)
            {
                odataQuery = odataQuery.Filter($"Number eq {Number}");
            }

            if (!string.IsNullOrEmpty(FascicleTitle))
            {
                odataQuery = odataQuery.Filter($"contains(Title, '{FascicleTitle}')");
            }

            if (!string.IsNullOrEmpty(FascicleSubject))
            {
                odataQuery = odataQuery.Filter($"contains(FascicleObject, '{FascicleSubject}')");
            }

            if (Dossier.HasValue)
            {
                odataQuery = odataQuery.Filter($"DossierFolders/any(df:df/Dossier/UniqueId eq {Dossier})");
            }

            if (IdMetadataRepository != null)
            {
                odataQuery = odataQuery.Filter($"MetadataRepository/UniqueId eq {IdMetadataRepository}");
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
