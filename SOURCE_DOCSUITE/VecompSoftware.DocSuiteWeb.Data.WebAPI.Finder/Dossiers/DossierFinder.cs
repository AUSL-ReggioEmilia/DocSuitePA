using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VecompSoftware.Clients.WebAPI.Http;
using VecompSoftware.DocSuiteWeb.DTO.OData;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Dossiers
{
    public class DossierFinder : BaseWebAPIFinder<Dossier, DossierModel>
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]
        public bool ExpandProperties { get; set; }
        public Guid DossierId { get; set; }
        public int? Year { get; set; }
        public int? Number { get; set; }
        public string FascicleTitle { get; set; }
        public string FascicleObject { get; set; }
        public DossierFinderModel DossierFinderModel { get; set; }
        public Guid? IdMetadataRepository { get; set; }
        #endregion

        #region [ Constructor ]

        public DossierFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public DossierFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
            DossierId = new Guid();
        }

        #endregion

        #region [ Methods ]

        public List<WebAPIDto<DossierModel>> GetFromPostMethod()
        {
            WebApiHttpClient httpClient = GetWebAPIClient(CurrentTenant);
            DossierFinderModel.Skip = PageIndex;
            DossierFinderModel.Top = (CustomPageIndex + 1) * PageSize;
            Dictionary<string, DossierFinderModel> finders = new Dictionary<string, DossierFinderModel>();
            finders.Add("finder", DossierFinderModel);
            string bodyQuery = JsonConvert.SerializeObject(finders, new StringEnumConverter());
            IEnumerable<DossierModel> results = httpClient.PostStringAsync<Dossier>($"/{CommonDefinition.OData.DossierService.FX_GetAuthorizedDossiers}", bodyQuery).ResponseToModel<ODataModel<ICollection<DossierModel>>>().Value;
            return results.Select(dossierModel => new WebAPIDto<DossierModel>
            {
                Entity = dossierModel,
                TenantModel = CurrentTenant
            }).ToList();
        }

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (ExpandProperties)
            {
                odataQuery = odataQuery
                    .Expand("DossierDocuments")
                    .Expand("DossierRoles($expand=Role)");
            }

            if (DossierId != null && DossierId != Guid.Empty)
            {
                string expression = string.Format("UniqueId eq {0}", DossierId);
                string odataExpression = expression;
                odataQuery = odataQuery.Filter(odataExpression);
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
                odataQuery = odataQuery.Filter($"DossierFolders /any(df:contains(df/Name, '{FascicleTitle}'))");
            }
            
            if (IdMetadataRepository != null)
            {
                odataQuery = odataQuery.Filter($"MetadataRepository/UniqueId eq {IdMetadataRepository}");
            }
            return base.DecorateFinder(odataQuery);
        }

        public override void ResetDecoration()
        {
            ExpandProperties = false;
            DossierId = new Guid();
        }

        #endregion
    }
}
