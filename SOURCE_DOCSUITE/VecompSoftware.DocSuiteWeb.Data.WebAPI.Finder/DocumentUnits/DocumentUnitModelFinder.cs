using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentUnits
{
    public class DocumentUnitModelFinder : BaseWebAPIFinder<DocumentUnitModel, DocumentUnitModel>
    {
        #region [ Constructor ]

        public DocumentUnitModelFinder(TenantModel tenant)
         : this(new List<TenantModel>() { tenant })
        { }

        public DocumentUnitModelFinder(IReadOnlyCollection<TenantModel> tenants) 
            : base(tenants)
        {
        }

        #endregion

        #region [ Properties ]

        public string UserName { get; set; }

        public string Domain { get; set; }

        public DateTimeOffset? DateFrom { get; set; }

        public DateTimeOffset? DateTo { get; set; }

        public bool IsSecurityUserEnabled { get; set; }

        public bool IncludeThreshold { get; set; }

        public DateTime? FascicolableThresholdDate { get; set; }

        public DocumentUnitFinderActionType? DocumentUnitFinderAction { get; set; }

        public Guid? CategoryId { get; set; }

        public Guid? IdDocumentUnit { get; set; }

        public bool ExcludeLinked { get; set; }

        #endregion

        #region [ Methods ]

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (DocumentUnitFinderAction.HasValue)
            {
                switch (DocumentUnitFinderAction.Value)
                {
                    case DocumentUnitFinderActionType.FascicolableUD:
                        odataQuery = odataQuery.Function(string.Format(
                           CommonDefinition.OData.DocumentUnitService.FX_GetFascicolableDocuments,
                            UserName, Domain, DateFrom.Value.ToString(ODataDateConversion), DateTo.Value.ToString(ODataDateConversion),
                            IncludeThreshold.ToString().ToLower(), FascicolableThresholdDate.Value.ToString(ODataDateConversion),
                            ExcludeLinked.ToString().ToLower()));
                        break;
                    case DocumentUnitFinderActionType.AuthorizedUD:
                        odataQuery = odataQuery.Function(string.Format(CommonDefinition.OData.DocumentUnitService.FX_GetAutorizedDocuments,
                            UserName, Domain, DateFrom.Value.ToString(ODataDateConversion), DateTo.Value.ToString(ODataDateConversion), IsSecurityUserEnabled.ToString().ToLower()));
                        break;
                    case DocumentUnitFinderActionType.CategorizedUD:
                        odataQuery = odataQuery.Filter(string.Format("Category/UniqueId eq {0}", CategoryId));
                        break;
                }
            }

            if (IdDocumentUnit.HasValue && IdDocumentUnit.Value != Guid.Empty)
            {
                string odataExpression = string.Format("UniqueId eq {0}", IdDocumentUnit.Value);
                odataQuery = odataQuery.Filter(odataExpression);
            }
            return odataQuery;
        }

        public override void ResetDecoration()
        {
            UserName = string.Empty;
            Domain = string.Empty;
            DateFrom = null;
            DateTo = null;
            IsSecurityUserEnabled = false;
            IncludeThreshold = false;
            FascicolableThresholdDate = null;
            ExcludeLinked = false;
        }

        #endregion

    }
}
