using System;
using System.Collections.Generic;
using System.Data;
using NHibernate;
using NHibernate.Criterion;
using VecompSoftware.Helpers.ExtensionMethods;

namespace VecompSoftware.NHibernateManager.Dao
{
    public class BaseNHibernateDao<T> : INHibernateDao<T>
    {
        #region [ Fields ]

        protected Type persitentType = typeof(T);
        protected string SessionFactoryName;

        protected ICriteria criteria;
        protected const string RIGHTS_LENGTH = "____________________";

        #endregion

        #region [ Properties ]

        public ICriteria HCriteria
        {
            get { return criteria; }
            set { criteria = value; }
        }

        public string ConnectionName
        {
            get { return SessionFactoryName; }
            set { SessionFactoryName = value; }
        }

        protected virtual ISession NHibernateSession
        {
            get { return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName); }
        }

        #endregion

        #region [ Constructor ]

        public BaseNHibernateDao() { }

        public BaseNHibernateDao(string sessionFactoryName)
        {
            if (string.IsNullOrEmpty(sessionFactoryName))
            {
                throw new Exception("sessionFactoryConfigPath may not be null nor empty");
            }

            SessionFactoryName = sessionFactoryName;
        }

        #endregion

        #region [ Methods ]

        public virtual T GetById(object id)
        {
            return GetById(id, false);
        }

        public virtual T GetById(object id, bool shouldLock)
        {
            T element;
            if (shouldLock)
                element = (T)NHibernateSession.Get(persitentType, id, LockMode.Upgrade);
            else
                element = (T)NHibernateSession.Get(persitentType, id);
            return element;
        }

        public virtual IList<T> GetListByIds(Array ids)
        {
            criteria = NHibernateSession.CreateCriteria(typeof(T));
            string idPropertyName = NHibernateSession.SessionFactory.GetClassMetadata(typeof(T)).IdentifierPropertyName;
            criteria.Add(Restrictions.In(idPropertyName, ids));
            return criteria.List<T>();
        }

        public virtual IList<T> GetAll()
        {
            criteria = NHibernateSession.CreateCriteria(persitentType);
            return criteria.List<T>();
        }

        public virtual IList<T> GetAll(string expression)
        {
            criteria = NHibernateSession.CreateCriteria(persitentType);

            IList<string> orders = new List<string>(expression.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            foreach (var item in orders)
            {
                // Divido il criterio in PropertyName ed eventuale direzione
                var temp = item.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                // Nome della property
                var propertyName = temp[0].Trim(); // item.Split(' ')[0].Trim();
                // Eventuale direzione (ASC di default)
                var orderDirection = temp.Length > 1 ? temp[1].Trim() : "ASC"; // item.Split(' ')[1].Trim();

                criteria.AddOrder(orderDirection.Eq("DESC") ? Order.Desc(propertyName) : Order.Asc(propertyName));
            }
            
            return criteria.List<T>();
        }

        public virtual DateTime GetServerDate()
        {
            /*ISQLQuery query = NHibernateSession.CreateSQLQuery("select getdate()");
            return query.UniqueResult<DateTime?>().Value;*/
            return DateTime.Now;
        }

        public virtual int Count()
        {
            criteria = NHibernateSession.CreateCriteria(persitentType);
            criteria.SetProjection(Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public virtual void SaveWithoutTransaction(ref T entity)
        {
            NHibernateSession.Save(entity);
            NHibernateSession.Flush();
        }

        public virtual void UpdateWithoutTransaction(ref T entity)
        {
            NHibernateSession.SaveOrUpdate(entity);
            NHibernateSession.Flush();
        }

        /// <summary>
        /// Salva l'entità sotto transazione. 
        /// Se avvengono degli errori viene eseguito il rollback e lanciata l'eccezione al chiamante.
        /// <example> Se in un oggetto A sono presenti 2 proprietà (propB e propC).
        /// NHibernate si preoccuperà di eseguire più comandi di INSERT e tutti sotto transazione.</example>
        /// </summary>
        /// <see cref="http://www.hibernatingrhinos.com/products/nhprof/learn/alert/DoNotUseImplicitTransactions"/>
        /// <param name="entity">Entità da salvare nel database</param>
        public virtual void Save(ref T entity)
        {
            using (ITransaction transaction = NHibernateSession.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    NHibernateSession.Save(entity);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }           
        }

        public virtual void Update(ref T entity)
        {
            using (ITransaction transaction = NHibernateSession.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    NHibernateSession.SaveOrUpdate(entity);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }            
        }

        public virtual void UpdateNoLastChange(ref T entity)
        {
            using (ITransaction transaction = NHibernateSession.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    NHibernateSession.Update(entity);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        
        public virtual void UpdateOnlyWithoutTransaction(ref T entity)
        {
            NHibernateSession.Update(entity);
        }

        public virtual void UpdateOnly(ref T entity)
        {
            using (ITransaction transaction = NHibernateSession.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    NHibernateSession.Update(entity);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }            
        }

        public virtual void Delete(ref T entity)
        {
            using (ITransaction transaction = NHibernateSession.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    NHibernateSession.Delete(entity);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }           
        }

        public virtual void DeleteWithoutTransaction(ref T entity)
        {
            NHibernateSession.Delete(entity);
        }
        /// <summary>
        /// Rimuove l'oggetto dalla cache di sessione di NHibernate
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Evict(T entity)
        {
            try
            {
                NHibernateSession.Evict(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Setta la modalità di cache di NHibernate
        /// </summary>
        /// <param name="mode"></param>
        public virtual void SetCacheMode(CacheMode mode)
        {
            NHibernateSession.CacheMode = mode;
        }

        public void FlushSession()
        {
            using (ITransaction transaction = NHibernateSession.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    NHibernateSession.Flush();
                    NHibernateSession.Close();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        #endregion

    }
}
