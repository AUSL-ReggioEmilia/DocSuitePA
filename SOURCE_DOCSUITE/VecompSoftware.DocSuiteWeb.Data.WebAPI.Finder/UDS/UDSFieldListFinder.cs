using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.UDS
{
    public class UDSFieldListFinder : BaseWebAPIFinder<UDSFieldList, UDSFieldList>
    {
        #region [ Fields ]

        #endregion [ Fields ]

        #region [ Properties ]
        public Guid? IdUDSRepository
        {
            get; set;
        }
        public string RootName
        {
            get; set;
        }
        public string FieldUDSRootName
        {
            get; set;
        }

        #endregion [ Properties ]

        #region [ Constructor ]
        public UDSFieldListFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public UDSFieldListFinder(IReadOnlyCollection<TenantModel> tenants)
           : base(tenants)
        {
        }
        #endregion [ Constructor ]

        #region [ Methods ]

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {

            if (IdUDSRepository.HasValue)
            {
                odataQuery = odataQuery.Filter($"Repository/UniqueId eq {IdUDSRepository.Value}");
            }
            if (!string.IsNullOrEmpty(FieldUDSRootName))
            {
                odataQuery = odataQuery.Filter($"FieldName eq '{FieldUDSRootName}' and UDSFieldListLevel eq 1");
            }
            if (!string.IsNullOrEmpty(RootName))
            {
                odataQuery = odataQuery.Filter($"Name eq '{RootName}' and UDSFieldListLevel eq 2");
            }
            if (UniqueId.HasValue)
            {
                odataQuery = odataQuery.Filter($"UniqueId eq {UniqueId}");
            }
            odataQuery = odataQuery.Expand("Repository");

            return odataQuery;
        }

        public override void ResetDecoration()
        {
            IdUDSRepository = null;
            RootName = string.Empty;
            FieldUDSRootName = string.Empty;
            UniqueId = null;
        }
        #endregion
    }
}
