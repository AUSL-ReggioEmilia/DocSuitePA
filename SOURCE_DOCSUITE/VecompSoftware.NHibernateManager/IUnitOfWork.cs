namespace VecompSoftware.NHibernateManager
{
    public interface IUnitOfWork
    {
        #region [ Parameters ]

        bool IsOpen { get; }
        string DbName { get; set; }

        #endregion

        #region [ Methods ]

        void RegisterNew(ref object obj);
        void RegisterClean(ref object obj);
        void RegisterDirty(ref object obj);
        void RegisterDelete(ref object obj);
        void Detach(ref object obj);
        void Clear();
        void SaveChanges();

        // Transazioni
        void BeginTransaction();
        void Commit();
        void Rollback();

        #endregion
    }
}

