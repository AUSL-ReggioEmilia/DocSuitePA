using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.DocSuiteWeb.EntityMapper;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Desks
{
    public class DeskDocumentFinder : BaseDeskFinder<DeskDocument, DeskDocument>
    {
        #region [ Fields ]

        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public DeskDocumentFinder(IEntityMapper<DeskDocument, DeskDocument> mapper) 
            : base(mapper)
        {
        }

        public DeskDocumentFinder(string dbName, IEntityMapper<DeskDocument, DeskDocument> mapper) 
            : base(dbName, mapper)
        {
        }

        #endregion [ Constructor ]

        #region [ Methods ]

       

        #endregion [ Methods ]
    }
}
