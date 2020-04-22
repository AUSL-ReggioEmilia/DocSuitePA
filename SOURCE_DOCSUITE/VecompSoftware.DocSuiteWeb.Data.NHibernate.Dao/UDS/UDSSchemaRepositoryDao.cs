using System;
using VecompSoftware.DocSuiteWeb.Data.Entity.UDS;
using VecompSoftware.NHibernateManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.UDS
{
    public class UDSSchemaRepositoryDao : BaseNHibernateDao<UDSSchemaRepository>
    {
        #region [ Fields ]

        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]

        public UDSSchemaRepositoryDao()
            : base()
        {
        }

        public UDSSchemaRepositoryDao(string sessionFactoryName)
            : base(sessionFactoryName)
        {
        }

        #endregion [ Constructor ]

        #region [ Methods ]

        public UDSSchemaRepository GetCurrentSchema()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            UDSSchemaRepository result = NHibernateSession.QueryOver<UDSSchemaRepository>()
                                           .Where(x => now >= x.ActiveDate && (x.ExpiredDate == null || now < x.ExpiredDate))
                                           .SingleOrDefault<UDSSchemaRepository>();
            return result;
        }

        #endregion [ Methods ]
    }
}
