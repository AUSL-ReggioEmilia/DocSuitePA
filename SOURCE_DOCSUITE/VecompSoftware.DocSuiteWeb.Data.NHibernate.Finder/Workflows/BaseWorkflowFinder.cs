using VecompSoftware.DocSuiteWeb.EntityMapper;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Workflows
{
    public abstract class BaseWorkflowFinder<T, THeader> : BaseFinder<T, THeader>
        where T : class
        where THeader : class
    {
        #region [ Fields ]

        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        protected BaseWorkflowFinder(IEntityMapper<T, THeader> mapper)
            : this(System.Enum.GetName(typeof(EnvironmentDataCode), EnvironmentDataCode.ProtDB), mapper)
        {
        }

        protected BaseWorkflowFinder(string dbName, IEntityMapper<T, THeader> mapper)
            : base(dbName, mapper)
        {
        }
        #endregion [ Constructor ]

        #region [ Methods ]

        #endregion [ Methods ]
    }
}
