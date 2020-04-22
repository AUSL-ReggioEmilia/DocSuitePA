using System;
using VecompSoftware.DocSuiteWeb.Data.Entity.Commons;
using VecompSoftware.NHibernateManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Commons
{
    public class JeepServiceHostDao : BaseNHibernateDao<JeepServiceHost>
    {
        #region [ Fields ]
        
        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public JeepServiceHostDao()
            : base()
        {
        }

        public JeepServiceHostDao(string sessionFactoryName) : base(sessionFactoryName)
        {
        }
        #endregion [ Constructor ]

        #region [ Methods ]

        public JeepServiceHost GetByHostName(string HostName)
        {
            JeepServiceHost result = NHibernateSession.QueryOver<JeepServiceHost>()
                                           .Where(x => x.Hostname == HostName)
                                           .SingleOrDefault();
            return result;
        }

        public JeepServiceHost GetDefault()
        {
            JeepServiceHost result = NHibernateSession.QueryOver<JeepServiceHost>()
                                           .Where(x => x.IsDefault)
                                           .SingleOrDefault();
            return result;
        }

        public bool ExistHost(Guid idHost)
        {
            return NHibernateSession.QueryOver<JeepServiceHost>()
                                        .Where(x => x.Id == idHost)
                                        .RowCount() > 0;
        }
        #endregion [ Methods ]
    }
}
