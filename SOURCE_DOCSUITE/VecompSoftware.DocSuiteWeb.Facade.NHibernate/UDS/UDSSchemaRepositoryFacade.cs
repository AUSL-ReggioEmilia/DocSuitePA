using System;
using VecompSoftware.DocSuiteWeb.Data.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.UDS;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS
{
    public class UDSSchemaRepositoryFacade : BaseProtocolFacade<UDSSchemaRepository, Guid, UDSSchemaRepositoryDao>
    {
        #region [ Fields ]

        private readonly string _userName;

        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]

        public UDSSchemaRepositoryFacade(string userName) : base()
        {
            _userName = userName;
        }

        #endregion [ Constructor ]

        #region [ Methods ]

        public UDSSchemaRepository GetCurrentSchema()
        {
            return _dao.GetCurrentSchema();
        }

        #endregion
    }
}
