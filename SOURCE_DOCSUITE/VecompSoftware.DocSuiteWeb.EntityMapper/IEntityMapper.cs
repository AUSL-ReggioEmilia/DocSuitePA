using System.Collections.Generic;
using NHibernate;

namespace VecompSoftware.DocSuiteWeb.EntityMapper
{
    public interface IEntityMapper<TEntity, TDTO>
    {
        TDTO MappingDTO(TEntity entity);

        ICollection<TDTO> MappingDTO(ICollection<TEntity> entities);

        IQueryOver<TEntity, TEntity> ApplyMappingProjections(IQueryOver<TEntity, TEntity> queryOver);
    }
}
