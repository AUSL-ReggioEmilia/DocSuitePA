using System;
using System.Collections.Generic;

namespace VecompSoftware.DaoManager
{
    public interface IORMDao<T, TKey> : IDao<T, TKey>
    {

        #region [ Properties ]

        string ConnectionName { get; set; }

        #endregion

        #region [ Methods ]

        T GetById(TKey id, bool shouldLock);
        IList<T> GetListByIds(Array ids);
        IList<T> GetAll(string orderedExpression);
        DateTime GetServerDate();
        void SaveWithoutTransaction(ref T entity);
        void UpdateWithoutTransaction(ref T entity);
        void UpdateOnlyWithoutTransaction(ref T entity);
        void UpdateOnly(ref T entity);
        void UpdateNoLastChange(ref T entity);
        void DeleteWithoutTransaction(ref T entity);
        void Evict(T entity);
        void FlushSession();

        #endregion

    }
}
