using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using VecompSoftware.DocSuiteWeb.Entity;
using VecompSoftware.DocSuiteWeb.Repository.Infrastructure;

namespace VecompSoftware.DocSuiteWeb.Repository.EF.Test
{
    public abstract class FakeDbSet<TEntity> : DbSet<TEntity>, IDbSet<TEntity> where TEntity : DSWBaseEntity, new()
    {
        #region [ Fields ]

        private readonly ObservableCollection<TEntity> _items;
        private readonly IQueryable _query;
        #endregion

        #region [ Constructor ]

        protected FakeDbSet()
        {
            _items = new ObservableCollection<TEntity>();
            _query = _items.AsQueryable();
        }
        #endregion

        IEnumerator IEnumerable.GetEnumerator() { return _items.GetEnumerator(); }

        public IEnumerator<TEntity> GetEnumerator() { return _items.GetEnumerator(); }

        public Expression Expression => _query.Expression;

        public Type ElementType => _query.ElementType;

        public IQueryProvider Provider => _query.Provider;

        public override TEntity Add(TEntity entity)
        {
            _items.Add(entity);
            return entity;
        }

        public override TEntity Remove(TEntity entity)
        {
            _items.Remove(entity);
            return entity;
        }

        public override TEntity Attach(TEntity entity)
        {
            switch (entity.ObjectState)
            {
                case ObjectState.Modified:
                    _items.Remove(entity);
                    _items.Add(entity);
                    break;

                case ObjectState.Deleted:
                    _items.Remove(entity);
                    break;

                case ObjectState.Unchanged:
                case ObjectState.Added:
                    _items.Add(entity);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            return entity;
        }

        public override TEntity Create() { return new TEntity(); }

        public override TDerivedEntity Create<TDerivedEntity>() { return Activator.CreateInstance<TDerivedEntity>(); }

        public override ObservableCollection<TEntity> Local => _items;
    }
}