using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Criterion;
using VecompSoftware.DocSuiteWeb.EntityMapper;
using VecompSoftware.NHibernateManager;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder
{
    [Serializable(), DataObject()]
    public abstract class BaseFinder<T, THeader> : IFinder<T, THeader> 
        where T: class 
        where THeader: class
    {
        #region Constructor
        public BaseFinder(string dbName, IEntityMapper<T, THeader> mapper)
        {
            this.Mapper = mapper;
            this.SessionFactoryName = dbName;
        }
        #endregion        

        protected ISession NHibernateSession
        {
            get
            {
                return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName);
            }
        }

        #region Fields
        private ICollection<Expression<Func<T, bool>>> _filterExpressions;
        private ICollection<ICriterion> _criteriaFilterExpressions;
        private ICollection<SortExpression<T>> _sortExpressions;
        private Expression<Func<T>> _projections;
        private int _startIndex = 0;
        private int _pageSize = 50;
        private bool _enableTableJoin = true;
        private bool _enableFetchMode = true;
        private bool _enablePaging = true;
        protected const string RIGHTS_LENGTH = "____________________";
        protected IEntityMapper<T, THeader> Mapper { get; set; }       
        #endregion

        #region Properties
        protected string SessionFactoryName { get; set; }

        /// <summary>
        /// Espressioni di filtro.
        /// Utilizzate per i filtri della griglia.
        /// </summary>
        public ICollection<Expression<Func<T, bool>>> FilterExpressions
        {
            get
            {
                if (_filterExpressions == null)
                    _filterExpressions = new List<Expression<Func<T, bool>>>();
                return _filterExpressions;
            }
            set
            {
                _filterExpressions = value;
            }
        }

        /// <summary>
        /// Espressioni di filtro.
        /// Utilizzate per i filtri della griglia.
        /// </summary>
        public ICollection<ICriterion> CriteriaFilterExpressions
        {
            get
            {
                if (_criteriaFilterExpressions == null)
                    _criteriaFilterExpressions = new List<ICriterion>();
                return _criteriaFilterExpressions;
            }
            set
            {
                _criteriaFilterExpressions = value;
            }
        }

        /// <summary>
        /// Dizionario di tutti gli ordinamenti.
        /// </summary>
        public ICollection<SortExpression<T>> SortExpressions
        {
            get
            {
                if (_sortExpressions == null)
                    _sortExpressions = new List<SortExpression<T>>();
                return _sortExpressions;
            }
            set
            {
                _sortExpressions = value;
            }
        }

        /// <summary>
        /// Espressione per la selezione delle colonne.
        /// </summary>
        public Expression<Func<T>> SelectProjections
        {
            get
            {
                return _projections;
            }
            set
            {
                _projections = value;
            }
        }
        public int PageIndex
        {
            get
            {
                return _startIndex;
            }
            set
            {
                _startIndex = value;
            }
        }
        
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value;
            }
        }

        public int CustomPageIndex
        {
            get
            {
                return (int)Math.Ceiling((double)PageIndex / (double)PageSize);
            }
            set
            {
                PageIndex = value * PageSize;
            }
        }

        public bool EnableTableJoin
        {
            get
            {
                return _enableTableJoin;
            }
            set
            {
                _enableTableJoin = value;
            }
        }

        public bool EnableFetchMode
        {
            get
            {
                return _enableFetchMode;
            }
            set
            {
                _enableFetchMode = value;
            }
        }

        public bool EnablePaging
        {
            get
            {
                return _enablePaging;
            }
            set
            {
                _enablePaging = value;
            }
        }

        #endregion

        #region Abstract
        protected virtual IQueryOver<T, T> CreateQueryOver() 
        {
            return NHibernateSession.QueryOver<T>();
        }
        #endregion
        
        #region MetodiDaEstendere
        /// <summary>
        /// Da implementare nella classe che eredita dalla base finder
        /// </summary>
        protected virtual IQueryOver<T, T> DecorateCriteria(IQueryOver<T, T> queryOver)
        {
            return queryOver;
        }
        /// <summary>
        /// Da implementare nella classe che eredita dalla base finder
        /// </summary>
        protected virtual IQueryOver<T, T> AttachSortExpressions(IQueryOver<T, T> queryOver)
        {
            AttachSortExpressions(ref queryOver);
            return queryOver;
        }
        /// <summary>
        /// Da implementare nella classe che eredita dalla base finder
        /// </summary>
        protected virtual IQueryOver<T, T> AttachFilterExpressions(IQueryOver<T, T> queryOver)
        {
            AttachFilterExpressions(ref queryOver);
            return queryOver;
        }
                
        /// <summary>
        /// Esegue la paginazione dei dati.
        /// </summary>
        /// <param name="queryOver"></param>
        /// <returns></returns>
        protected virtual IQueryOver<T, T> ExecutePaging(IQueryOver<T, T> queryOver)
        {
            if (EnablePaging)
            {
                queryOver.Skip(PageIndex);
                queryOver.Take(PageSize);
            }
            return queryOver;
        }
        #endregion

        #region BaseFunction
        /// <summary>
        /// Crea la DoSearch con i metodi standard.
        /// Se non implementati, vengono usati quelli standard
        /// </summary>
        /// <returns></returns>
        public ICollection<T> DoSearch()
        {
            ICollection<T> searchList;
            IQueryOver<T,T> queryOver = CreateQueryOver();
            queryOver = DecorateCriteria(queryOver);
            queryOver = ExecutePaging(queryOver);
            queryOver = AttachSortExpressions(queryOver);
            queryOver = AttachFilterExpressions(queryOver);
            using (ITransaction transaction = NHibernateSession.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    searchList = queryOver.List<T>();
                    transaction.Commit();
                }
                catch(Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                return searchList;
            }
        }

        /// <summary>
        /// Crea la DoSearchHeader con i metodi standard.
        /// Se non implementati, vengono usati quelli standard
        /// </summary>
        /// <returns></returns>
        public ICollection<THeader> DoSearchHeader()
        {
            ICollection<THeader> searchHeaderList;
            IQueryOver<T, T> queryOver = CreateQueryOver();
            queryOver = DecorateCriteria(queryOver);
            queryOver = ExecutePaging(queryOver);
            queryOver = AttachSortExpressions(queryOver);
            queryOver = AttachFilterExpressions(queryOver);
            queryOver = SetProjectionsHeaders(queryOver);
            using (ITransaction transaction = NHibernateSession.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    searchHeaderList = queryOver.List<THeader>();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                return searchHeaderList;
            }
        }
        #endregion

        #region Util Functions
        /// <summary>
        /// Seleziona le colonne
        /// </summary>
        /// <param name="queryOver"></param>
        /// <returns></returns>
        protected virtual IQueryOver<T, T> SetProjectionsHeaders(IQueryOver<T, T> queryOver)
        {
            return Mapper.ApplyMappingProjections(queryOver);
        }

        #endregion

        #region IFinder Implementation
        protected virtual bool AttachSortExpressions(ref IQueryOver<T, T> queryOver)
        {
            return this.AttachSortExpressions(ref queryOver, this.SortExpressions);
        }

        /// <summary>
        /// Aggiungi l'espressione dalla griglia al finder
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        
        protected virtual bool AttachSortExpressions(ref IQueryOver<T, T> queryOver, ICollection<SortExpression<T>> conditions)
        {
            if (conditions == null)
                return false;
            try
            {
                // Attacca una collezione di espressioni SORT alla queryOver
                foreach (SortExpression<T> c in conditions)
                {
                    // Ascendente
                    if(c.Direction == SortDirection.Ascending)
                        queryOver.OrderBy(c.Expression).Asc();
                    // Discendente
                    else
                        queryOver.OrderBy(c.Expression).Desc();
                }
            }
            catch (Exception ex)
            {
                throw new DocSuiteException("Errore ordinamento", "Espressione di ordinamento non corretta", ex);
            }
            return true;            
        }
        
        /// <summary>
        /// Aggiunge ulteriori filtri alla lista già presente per l'attuale queryOver.
        /// </summary>
        /// <param name="queryOver">QueryOver a cui agganciare il filtro</param>
        /// <returns>True se esistono criteri da agganciare, False altrimenti</returns>
        protected virtual bool AttachFilterExpressions(ref IQueryOver<T, T> queryOver)
        {
            if (this.FilterExpressions == null && this.CriteriaFilterExpressions == null)
                return false;

            try
            {
                foreach (Expression<Func<T, bool>> c in this.FilterExpressions)
                {
                    queryOver.Where(c);
                }

                foreach (ICriterion c in this.CriteriaFilterExpressions)
                {
                    queryOver.Where(c);
                }
            }
            catch(Exception ex)
            {
                throw new DocSuiteException("Errore filtro", "Espressione di filtro non corretta", ex);
            }
            return true;
        }

        /// <summary>
        /// Crea le proiezioni (Proprietà Sorgente - Proprietà Destinazione) da attuare sul criterio.
        /// </summary>
        /// <returns>True se esistono proiezioni, False altrimenti</returns>
        
        // TODO: Verificare se è ancora una funzione necessaria
        protected virtual bool CreateProjections(IQueryOver<T, T> queryOver)
        {
            if (SelectProjections == null)
                return false;
        
            ProjectionList projList = Projections.ProjectionList();
            try
            {
                foreach(ParameterExpression property in SelectProjections.Parameters)
                {
                    projList.Add<T>(Projections.Property(property.Name), SelectProjections);
                }
                queryOver.Select(projList);
            }   
            catch(Exception ex)
            {
                throw new DocSuiteException("Errore proiezioni", "Errore nell'impostazione proiezioni", ex);
            }   
            return true;
        }

        // TODO: Verificare se è ancora una funzione necessaria
        protected virtual bool CreateProjections(IQueryOver<T, T> queryOver, Expression<Func<T, object>> projection)
        {
            if (SelectProjections == null)
                return false;

            try
            {
                queryOver.Select(projection);
            }
            catch (Exception ex)
            {
                throw new DocSuiteException("Errore proiezioni", "Errore nell'impostazione proiezioni", ex);
            }
            return true;
        }
                
        /// <summary>
        /// Ricerca con filtro
        /// </summary>        
        // TODO: Verificare se è ancora una funzione necessaria
        public virtual void DoSearchFilter(ICollection<Expression<Func<T, bool>>> filter)
        {
            if (filter == null)
                return;
            FilterExpressions = filter;
        }

        /// <summary>
        /// Ricerca con ordinamento
        /// </summary>
        /// <param name="sortExpr">una stringa contenente la sortExpression (NomeCampo DirezioneOrdinamento).</param>
        /// <returns>Una lista ordinata di risultati</returns>
        // TODO: Verificare se è ancora una funzione necessaria
        public virtual ICollection<T> DoSearch(ICollection<SortExpression<T>> sortExpr)
        {
            if (sortExpr == null)
                return null;
            SortExpressions = sortExpr;
            return null;
        }
        
        /// <summary>
        /// Ricerca con ordinamento, riga di partenza e dimensione della paginazione.
        /// </summary>
        /// <param name="sortExpr">Una stringa contenente la sortExpression (NomeCampo DirezioneOrdinamento)</param>
        /// <param name="startRow">La riga a partire dalla quale tornare il risultato</param>
        /// <param name="pageSize">La dimensione della pagina dei risultati</param>
        /// <returns>Una lista ordinata di risultati limitati a PageSize</returns>
        // TODO: Verificare se è ancora una funzione necessaria
        public virtual ICollection<T> DoSearch(ICollection<SortExpression<T>> sortExpr, int startRow, int pageSize)
        {
            DoSearch(sortExpr);
            PageIndex = startRow;
            PageSize = pageSize;
            return null;
        }

        /// <summary>
        /// Numero dei risultati ottenuti dalla ricerca.
        /// </summary>
        /// <returns>Il numero dei record ottenuti dalla ricerca</returns>
        public virtual int Count()
        {
            IQueryOver<T, T> queryOver = CreateQueryOver();
            return queryOver.RowCount();          
        }

        /// <summary>
        /// Pulizia delle espressioni di ordinamento
        /// </summary>
        public virtual void SortExpressionsClear()
        {
            SortExpressions.Clear();
        }

        /// <summary>
        /// Pulizia dei filtri
        /// </summary>
        public virtual void FilterExpressionsClear()
        {
            FilterExpressions.Clear();
            CriteriaFilterExpressions.Clear();
        }

        public virtual void AddFilterExpression(ICriterion filter)
        {
            CriteriaFilterExpressions.Add(filter);
        }

        public virtual void AddFilterExpression(Expression<Func<T, bool>> filter)
        {
            FilterExpressions.Add(filter);
        }
        #endregion 
    }
}
