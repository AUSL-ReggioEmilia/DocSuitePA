using System;
using System.Collections.Generic;
using NHibernate.Criterion;
using VecompSoftware.DocSuiteWeb.Data.Entity.UDS;
using VecompSoftware.NHibernateManager.Dao;
using NHibernate;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.UDS
{
    public class UDSRepositoryDao : BaseNHibernateDao<UDSRepository>
    {
        #region [ Fields ]

        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]

        public UDSRepositoryDao()
            : base()
        {
        }

        public UDSRepositoryDao(string sessionFactoryName)
            : base(sessionFactoryName)
        {
        }

        #endregion [ Constructor ]

        #region [ Methods ]

        public IList<UDSRepository> GetActiveRepositories()
        {
            return GetActiveRepositories(string.Empty);
        }

        public IList<UDSRepository> GetActiveRepositories(string name)
        {
            IList<UDSRepository> results = ActiveRepositories(name).List<UDSRepository>();
            return results;
        }

        private IQueryOver<UDSRepository, UDSRepository> ActiveRepositories(string name)
        {
            UDSSchemaRepository udsSchemaRepository = null;
            DateTimeOffset now = DateTimeOffset.UtcNow;

            IQueryOver<UDSRepository, UDSRepository> query = NHibernateSession.QueryOver<UDSRepository>()
                                           .JoinAlias(x => x.UDSSchemaRepository, () => udsSchemaRepository)
                                           .Where(x => now >= x.ActiveDate && (x.ExpiredDate == null || now < x.ExpiredDate) && x.Status == UDSRepositoryState.Confirmed);

            if (!string.IsNullOrEmpty(name))
            {
                query.Where(x => x.Name == name);
            }

            return query.OrderBy(o => o.Name).Asc;
        }

        public IList<UDSRepository> GetFascicolableRepositories()
        {
            NHibernateSession.EnableFilter("OnlyFascicolable");
            return GetActiveRepositories();
        }

        public IList<UDSRepository> GetProtocollableRepositories()
        {
            NHibernateSession.EnableFilter("OnlyProtocollable");
            return GetActiveRepositories();
        }

        public bool HasProtocollableRepositories()
        {
            NHibernateSession.EnableFilter("OnlyProtocollable");
            return ActiveRepositories(string.Empty).RowCount() > 0;
        }

        public IList<UDSRepository> GetByPecEnabled(string name)
        {
            NHibernateSession.EnableFilter("OnlyPECEnable");
            return GetActiveRepositories(name);
        }

        public IList<UDSRepository> GetByPecAnnexedEnabled(string name)
        {
            NHibernateSession.EnableFilter("OnlyPECEnable");
            NHibernateSession.EnableFilter("AnnexedEnabled");
            return GetActiveRepositories();
        }


        public UDSRepository GetMaxVersionByName(string name)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            UDSRepository result = NHibernateSession.QueryOver<UDSRepository>()
                                           .Where(x => x.Name == name && x.Status == UDSRepositoryState.Confirmed && now >= x.ActiveDate && (x.ExpiredDate == null || now < x.ExpiredDate))
                                           .SingleOrDefault<UDSRepository>();
            return result;
        }

        public UDSRepository GetByDSWEnvironment(int DSWEnvironment)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            UDSRepository result = NHibernateSession.QueryOver<UDSRepository>()
                                           .Where(x => x.DSWEnvironment == DSWEnvironment && x.Status == UDSRepositoryState.Confirmed && now >= x.ActiveDate && (x.ExpiredDate == null || now < x.ExpiredDate))
                                           .SingleOrDefault<UDSRepository>();
            return result;
        }





        #endregion [ Methods ]
    }
}
