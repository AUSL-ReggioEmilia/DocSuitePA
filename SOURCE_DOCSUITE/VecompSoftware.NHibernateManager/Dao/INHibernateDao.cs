using NHibernate;
using VecompSoftware.DaoManager;

namespace VecompSoftware.NHibernateManager.Dao
{
    public interface INHibernateDao<T> : IORMDao<T, object>
    {
        ICriteria HCriteria { get; set; }
    }
}
