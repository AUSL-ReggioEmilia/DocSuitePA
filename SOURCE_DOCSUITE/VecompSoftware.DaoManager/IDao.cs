using System.Collections.Generic;

namespace VecompSoftware.DaoManager
{
    public interface IDao<T, TKey>
    {
        #region [ Methods ]

        T GetById(TKey id);
        IList<T> GetAll();
        int Count();
        void Save(ref T entity);
        void Update(ref T entity);
        void Delete(ref T entity);

        #endregion
    }
}
