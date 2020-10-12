using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Commons
{
    public class ContainerFinder : BaseWebAPIFinder<Container, Container>
    {
        #region [ Properties ]
        public Guid? IdTenant { get; set; }
        #endregion

        #region [ Constructor ]  
        public ContainerFinder(TenantModel tenant)
            : this(new List<TenantModel>() { tenant })
        {
        }

        public ContainerFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
        }
        #endregion

        #region [ Methods ]
        public override void ResetDecoration()
        {
            UniqueId = null;
            IdTenant = null;
        }

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (IdTenant.HasValue)
            {
                odataQuery = odataQuery.Filter($"Tenants/any(a:a/UniqueId eq {IdTenant.Value})");
            }

            if (EnablePaging)
            {
                odataQuery.Skip(PageIndex).Top(PageSize);
            }

            return odataQuery;
        }

        protected override ICollection<U> ExecutePaging<U>(IEnumerable<U> source)
        {
            return source.ToList();
        }
        #endregion
    }
}
