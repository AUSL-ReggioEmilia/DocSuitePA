using System.Collections.Generic;
using System.Linq;

namespace VecompSoftware.DocSuiteWeb.Mapper
{
    public interface IDomainMapper<T, TTransformed> : IMapper<T, TTransformed>
        where TTransformed : new()
    {
        #region [ Methods ]

        IQueryable<TTransformed> MapQueryable(IQueryable<T> query);

        ICollection<TTransformed> MapCollection(ICollection<T> entity);

        ICollection<TTransformed> MapCollection(IEnumerable<T> entity);

        ICollection<TTransformed> MapCollection(IQueryable<T> entity);

        #endregion
    }
}
