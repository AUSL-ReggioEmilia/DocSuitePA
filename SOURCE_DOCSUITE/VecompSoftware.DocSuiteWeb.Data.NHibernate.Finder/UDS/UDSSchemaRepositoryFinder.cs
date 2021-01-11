using System;
using VecompSoftware.DocSuiteWeb.Data.Entity.UDS;
using VecompSoftware.DocSuiteWeb.EntityMapper;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.UDS
{
    public class UDSSchemaRepositoryFinder : BaseFinder<UDSSchemaRepository, UDSSchemaRepository>
    {
        #region [ Fields ]

        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public UDSSchemaRepositoryFinder(IEntityMapper<UDSSchemaRepository, UDSSchemaRepository> mapper, String currentUserName) 
            : this(System.Enum.GetName(typeof(EnvironmentDataCode), EnvironmentDataCode.ProtDB), mapper, currentUserName)
        {
        }

        public UDSSchemaRepositoryFinder(string dbName, IEntityMapper<UDSSchemaRepository, UDSSchemaRepository> mapper, String currentUserName)
            : base(dbName, mapper)
        {
        }
        #endregion [ Constructor ]

        #region [ Methods ]

        #endregion
    }
}
