using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Fascicles
{
    public class FascicleDocumentUnitFinder : BaseWebAPIFinder<FascicleDocumentUnit, FascicleDocumentUnit>
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]
        public Guid? IdDocumentUnit { get; set; }
        public Guid? IdFascicleFolder { get; set; }
        public Guid? IdFascicle { get; set; }
        public bool ExpandFascicle { get; set; }
        public ReferenceType? ReferenceType { get; set; }
        public bool? ExpandProperties { get; set; }
        #endregion

        #region [ Constructor ]

        public FascicleDocumentUnitFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public FascicleDocumentUnitFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
        }

        #endregion

        #region [ Methods ]

        public override void ResetDecoration()
        {
            IdDocumentUnit = null;
            IdFascicle = null;
            ExpandFascicle = false;
            ReferenceType = null;
            IdFascicleFolder = null;
            ExpandProperties = null;
        }


        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (ExpandProperties.HasValue && ExpandProperties.Value)
            {
                odataQuery.Expand("DocumentUnit")
                    .Expand("FascicleFolder");
            }

            if (IdDocumentUnit.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Format("DocumentUnit/UniqueId eq {0}", IdDocumentUnit.Value));
            }

            if (IdFascicle.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Format("Fascicle/UniqueId eq {0}", IdFascicle.Value));
            }

            if (ReferenceType.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Format("ReferenceType eq VecompSoftware.DocSuiteWeb.Entity.Fascicles.ReferenceType'{0}'", (int)ReferenceType.Value));
            }

            if (ExpandFascicle)
            {
                odataQuery = odataQuery.Expand("Fascicle");                    
            }

            if (IdFascicleFolder.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Concat("FascicleFolder/UniqueId eq ", IdFascicleFolder.Value));
            }
            return odataQuery;
        }

        #endregion
    }
}
