using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.DocSuiteWeb.EntityMapper;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Desks
{
    public class DeskRoleUserFinder : BaseDeskFinder<DeskRoleUser, DeskRoleUser>
    {
        #region [ Fields ]

        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public DeskRoleUserFinder(IEntityMapper<DeskRoleUser, DeskRoleUser> mapper)
            : base(mapper)
        {
        }

        public DeskRoleUserFinder(string dbName, IEntityMapper<DeskRoleUser, DeskRoleUser> mapper) 
            : base(dbName, mapper)
        {
        }

        #endregion [ Constructor ]

        #region [ Methods ]

        #endregion [ Methods ]
    }
}
