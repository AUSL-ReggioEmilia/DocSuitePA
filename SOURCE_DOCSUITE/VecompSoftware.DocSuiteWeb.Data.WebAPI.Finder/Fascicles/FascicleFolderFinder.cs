using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Fascicles
{
    public class FascicleFolderFinder : BaseWebAPIFinder<FascicleFolder, FascicleFolderModel>
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]
        public Guid? IdFascicle { get; set; }

        public bool? ReadChildren { get; set; }

        public bool? ReadDefaultFolder { get; set; }
        public bool ExpandProperties { get; set; }
        #endregion

        #region [ Constructor ]
        public FascicleFolderFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public FascicleFolderFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {

        }
        #endregion

        #region [ Methods ]
        public override void ResetDecoration()
        {
            IdFascicle = null;
            ReadChildren = null;
            ReadDefaultFolder = null;
            ExpandProperties = false;
        }

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (ReadChildren.HasValue && ReadChildren.Value && UniqueId.HasValue)
            {
                odataQuery = odataQuery.Function(string.Format(CommonDefinition.OData.FascicleService.FX_GetChildrenByParent, UniqueId));
                return odataQuery;
            }

            if (IdFascicle.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Concat($"Fascicle/UniqueId eq {IdFascicle}"));
            }

            if (ReadDefaultFolder.HasValue && ReadDefaultFolder.Value)
            {
                odataQuery = odataQuery.Filter("FascicleFolderLevel eq 2");
            }

            if (ExpandProperties)
            {
                odataQuery = odataQuery.Expand("FascicleDocuments");
            }
            return base.DecorateFinder(odataQuery);
        }
        #endregion
    }
}
