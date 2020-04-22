using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.DocSuiteWeb.EntityMapper;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Desks
{
    public class DeskMessageFinder : BaseDeskFinder<DeskMessage, DeskMessage>
    {
        #region [ Fields ]

        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public DeskMessageFinder(IEntityMapper<DeskMessage, DeskMessage> mapper) 
            : base(mapper)
        {
        }

        public DeskMessageFinder(string dbName, IEntityMapper<DeskMessage, DeskMessage> mapper) 
            : base(dbName, mapper)
        {
        }

        #endregion [ Constructor ]

        #region [ Methods ]

        #endregion [ Methods ]
    }
}
