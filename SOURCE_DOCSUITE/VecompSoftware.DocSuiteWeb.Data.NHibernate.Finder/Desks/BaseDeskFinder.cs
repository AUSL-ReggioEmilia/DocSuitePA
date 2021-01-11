using VecompSoftware.DocSuiteWeb.EntityMapper;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Desks
{
    public abstract class BaseDeskFinder<T, THeader> : BaseFinder<T, THeader>  
        where T : class
        where THeader : class
    {
        #region [ Fields ]

        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public BaseDeskFinder(IEntityMapper<T, THeader> mapper)
            : this(System.Enum.GetName(typeof(EnvironmentDataCode), EnvironmentDataCode.ProtDB), mapper)
        {
        }

        public BaseDeskFinder(string dbName, IEntityMapper<T, THeader> mapper) 
            : base(dbName, mapper)
        {
        }
        #endregion [ Constructor ]

        #region [ Methods ]
       
        #endregion [ Methods ]
    }
}
