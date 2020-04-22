namespace VecompSoftware.NHibernateManager
{
    public class NHibernateUnitOfWork : IUnitOfWork
    {
        #region [ Fields ]

        private string _sessionFactoryName;

        #endregion

        #region [ Constructors ]

        public NHibernateUnitOfWork() { }

        /// <summary></summary>
        /// <param name="pDataBaseName"></param>
        /// <remarks></remarks>
        public NHibernateUnitOfWork(string pDataBaseName)
        {
            _sessionFactoryName = pDataBaseName;
        }

        #endregion

        #region [ Properties ]

        public bool IsOpen
        {
            get { return NHibernateSessionManager.Instance.GetSessionFrom(_sessionFactoryName).IsOpen; }
        }

        public string DbName
        {
            get { return _sessionFactoryName; }
            set { _sessionFactoryName = value; }
        }

        #endregion

        #region [ Methods ]

        /// <summary></summary>
        /// <remarks></remarks>
        public void BeginTransaction()
        {
            NHibernateSessionManager.Instance.BeginTransactionOn(_sessionFactoryName);
        }

        /// <summary></summary>
        /// <remarks></remarks>
        public void Commit()
        {
            if (NHibernateSessionManager.Instance.HasOpenTransactionOn(_sessionFactoryName))
            {
                NHibernateSessionManager.Instance.CommitTransactionOn(_sessionFactoryName);
            }
            else
            {
                NHibernateSessionManager.Instance.GetSessionFrom(_sessionFactoryName).Flush();
            }
        }

        /// <summary></summary>
        /// <remarks></remarks>
        public void Rollback()
        {
            NHibernateSessionManager.Instance.RollbackTransactionOn(_sessionFactoryName);
        }

        /// <summary></summary>
        /// <remarks></remarks>
        public void RegisterNew(ref object obj)
        {
            NHibernateSessionManager.Instance.GetSessionFrom(_sessionFactoryName).Save(obj);
        }

        public void RegisterDirty(ref object obj)
        {
            NHibernateSessionManager.Instance.GetSessionFrom(_sessionFactoryName).Update(obj);
        }

        public void RegisterClean(ref object obj)
        {
            NHibernateSessionManager.Instance.GetSessionFrom(_sessionFactoryName).Lock(obj, NHibernate.LockMode.None);
        }

        public void RegisterDelete(ref object obj)
        {
            NHibernateSessionManager.Instance.GetSessionFrom(_sessionFactoryName).Delete(obj);
        }

        public void SaveChanges()
        {
            NHibernateSessionManager.Instance.GetSessionFrom(_sessionFactoryName).Flush();
        }

        public void Detach(ref object obj)
        {
            NHibernateSessionManager.Instance.GetSessionFrom(_sessionFactoryName).Evict(obj);
        }

        public void Clear()
        {
            NHibernateSessionManager.Instance.GetSessionFrom(_sessionFactoryName).Clear();
        }

        #endregion
    }
}
