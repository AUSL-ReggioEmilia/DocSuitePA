using System.Collections.Generic;
using VecompSoftware.NHibernateManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Resolutions
{
    public class ResolutionKindDao : BaseNHibernateDao<ResolutionKind>
    {
        #region [ Fields ]

        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]

        public ResolutionKindDao()
            : base()
        {
        }

        public ResolutionKindDao(string sessionFactoryName)
            : base(sessionFactoryName)
        {
        }

        #endregion [ Constructor ]

        #region [ Methods ]
        public ICollection<ResolutionKind> GetActiveResolutionKind()
        {
            return NHibernateSession.QueryOver<ResolutionKind>()
                .Where(x => x.IsActive == true)
                .List<ResolutionKind>();
        }

        public IList<ResolutionKind> GetNotActiveResolutionKind()
        {
            return NHibernateSession.QueryOver<ResolutionKind>()
                .Where(x => x.IsActive == false)
                .List<ResolutionKind>();
        }

        public IList<ResolutionKind> GetByName(string name)
        {
            return NHibernateSession.QueryOver<ResolutionKind>()
                .Where(x => x.IsActive == true)
                .Where(x => x.Name == name)
                .List<ResolutionKind>();
        }
        #endregion [ Methods ]
    }
}
