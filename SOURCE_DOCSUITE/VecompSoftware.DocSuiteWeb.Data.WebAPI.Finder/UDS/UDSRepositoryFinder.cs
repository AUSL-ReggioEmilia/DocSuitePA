using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.UDS
{
    public class UDSRepositoryFinder : BaseWebAPIFinder<UDSRepository, UDSRepositoryModel>
    {
        #region [ Fields ]

        #endregion [ Fields ]

        #region [ Properties ]
        public string UserName { get; set; }
        public string Domain { get; set; }
        public Guid? IdUDSTypology { get; set; }
        public bool PECAnnexedEnabled { get; set; }
        public bool ExpandProperties { get; set; }
        public UDSRepositoryFinderActionType ActionType { get; set; }

        #endregion [ Properties ]

        #region [ Constructor ]
        public UDSRepositoryFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public UDSRepositoryFinder(IReadOnlyCollection<TenantModel> tenants)
           : base(tenants)
        {
        }
        #endregion [ Constructor ]

        #region [ Methods ]

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            string IdUDSTypologyParameter = IdUDSTypology.HasValue ? IdUDSTypology.Value.ToString() : "null";
            switch (ActionType)
            {
                case UDSRepositoryFinderActionType.Insert:
                    {
                        odataQuery = odataQuery.Function(string.Format(CommonDefinition.OData.UDSRepositoryService.FX_GetInsertableRepositoriesByTypology, UserName, Domain, IdUDSTypologyParameter, PECAnnexedEnabled.ToString().ToLower()));
                        break;
                    }
                case UDSRepositoryFinderActionType.Search:
                    {
                        odataQuery = odataQuery.Function(string.Format(CommonDefinition.OData.UDSRepositoryService.FX_GetViewableRepositoriesByTypology, IdUDSTypologyParameter, PECAnnexedEnabled.ToString().ToLower()));
                        break;
                    }
                case UDSRepositoryFinderActionType.FindElement:
                    {
                        odataQuery = UniqueId.HasValue ? odataQuery.Filter(string.Concat("UniqueId eq ", UniqueId.Value.ToString())) : odataQuery;
                        break;
                    }
            }

            if (ExpandProperties)
            {
                odataQuery = odataQuery.Expand("Container");
            }
            return odataQuery;
        }

        public override void ResetDecoration()
        {
            UserName = string.Empty;
            Domain = string.Empty;
            UniqueId = null;
            PECAnnexedEnabled = false;
            IdUDSTypology = null;
            ExpandProperties = false;
        }
        #endregion
    }
}
