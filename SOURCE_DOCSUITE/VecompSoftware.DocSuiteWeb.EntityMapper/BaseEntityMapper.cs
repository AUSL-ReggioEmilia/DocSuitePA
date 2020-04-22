using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Transform;

namespace VecompSoftware.DocSuiteWeb.EntityMapper
{
    public abstract class BaseEntityMapper<TEntity, TDTO> : IEntityMapper<TEntity, TDTO>
    {
        #region Constructor
        public BaseEntityMapper()
        {

        }
        #endregion

        public TDTO MappingDTO(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Impossibile mappare un'entità non definita");
            }
            return TransformDTO(entity);
        }

        public ICollection<TDTO> MappingDTO(ICollection<TEntity> entities)
        {
            List<TDTO> results = new List<TDTO>();
            if (entities == null)
            {
                throw new ArgumentException("Impossibile mappare una collezione di entità non definite");
            }

            foreach (TEntity entity in entities)
            {
                results.Add(MappingDTO(entity));
            }
            return results;
        }

        public IQueryOver<TEntity, TEntity> ApplyMappingProjections(IQueryOver<TEntity, TEntity> queryOver)
        {
            if (queryOver == null)
            {
                throw new ArgumentException("Impossibile applicare una proiezione se la query non è stata inizializzata");
            }
            return MappingProjection(queryOver)
                .TransformUsing(Transformers.AliasToBean<TDTO>());
        }

        protected abstract TDTO TransformDTO(TEntity entity);

        protected abstract IQueryOver<TEntity, TEntity> MappingProjection(IQueryOver<TEntity, TEntity> queryOver);

    }
}
