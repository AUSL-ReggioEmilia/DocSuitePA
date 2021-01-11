using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.DocSuiteWeb.EntityMapper;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Desks
{
    public class DeskLogFinder : BaseDeskFinder<DeskLog, DeskLog>
    {
        #region [ Fields ]

        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public DeskLogFinder(IEntityMapper<DeskLog, DeskLog> mapper)
            : base(mapper)
        {
        }

        public DeskLogFinder(string dbName, IEntityMapper<DeskLog, DeskLog> mapper) 
            : base(dbName, mapper)
        {
        }

        #endregion [ Constructor ]

        #region [ Methods ]
                
        #endregion [ Methods ]
    }
}
