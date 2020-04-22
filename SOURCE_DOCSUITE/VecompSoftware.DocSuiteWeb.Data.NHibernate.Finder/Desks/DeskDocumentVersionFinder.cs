using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.DocSuiteWeb.EntityMapper;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Desks
{
    public class DeskDocumentVersionFinder : BaseDeskFinder<DeskDocumentVersion, DeskDocumentVersion>
    {
        #region [ Fields ]

        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public DeskDocumentVersionFinder(IEntityMapper<DeskDocumentVersion, DeskDocumentVersion> mapper) 
            : base(mapper)
        {
        }

        public DeskDocumentVersionFinder(string dbName, IEntityMapper<DeskDocumentVersion, DeskDocumentVersion> mapper)
            : base(dbName, mapper)
        {
        }

        #endregion [ Constructor ]

        #region [ Methods ]

        #endregion [ Methods ]
    }
}
