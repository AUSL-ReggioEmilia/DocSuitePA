using System.Collections.Generic;
using System.Linq;

namespace VecompSoftware.DocSuiteWeb.Mapper
{
    public abstract class BaseDomainMapper<T, TTransformed> : BaseMapper<T, TTransformed>, IDomainMapper<T, TTransformed>
        where TTransformed : new()
    {
        public BaseDomainMapper() : base() { }

        #region [ Methods ]

        public virtual ICollection<TTransformed> MapCollection(IEnumerable<T> entities)
        {
            return MapCollection(entities.ToList());
        }

        public virtual ICollection<TTransformed> MapCollection(IQueryable<T> entities)
        {
            return MapCollection(entities.ToList());
        }

        public virtual ICollection<TTransformed> MapCollection(ICollection<T> entities)
        {
            if (entities == null)
            {
                return new List<TTransformed>();
            }
            List<TTransformed> entitiesTransformed = new List<TTransformed>();
            entities.ToList().ForEach(f => entitiesTransformed.Add(Map(f, new TTransformed())));
            return entitiesTransformed;
        }

        public IQueryable<TTransformed> MapQueryable(IQueryable<T> query)
        {
            return query.Select(s => Map(s, new TTransformed()));
        }

        #endregion

    }
}
