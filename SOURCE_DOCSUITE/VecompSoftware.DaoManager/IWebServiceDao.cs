using System.Collections.Generic;

namespace VecompSoftware.DaoManager
{
    public interface IWebServiceDao<T, TKey> : IDao<T, TKey>
    {
        void Update(ref T entity, string actionType);
    }
}
