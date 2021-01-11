using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.DocSuiteWeb.DTO.Desks;
using VecompSoftware.DocSuiteWeb.EntityMapper;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Desks
{
    public class DeskFinder : BaseDeskFinder<Desk, DeskResult>
    {
        #region [ Fields ]
        /// <summary>
        /// Join Object
        /// </summary>
        private Container container = null;
        private DeskRoleUser deskRoleUser = null;


        private readonly string _currentUserName;

        public IList<Container> UserContainers{ get; set; }

        /// <summary>
        /// Filtro i tavoli per nome
        /// </summary>
        public string DeskName { get; set; }

        /// <summary>
        /// Filtro i tavoli per Description
        /// </summary>
        public string DeskDescription { get; set; }

        /// <summary>
        /// Filtro che include solo i tavoli in cui il diritto è esplicitato per l'utente corrente
        /// </summary>
        public Boolean ExplicitPermission { get; set; }

        /// <summary>
        /// Filtro i tavoli per gli stati
        /// </summary>
        public ICollection<DeskState> DeskStates { get; set; }
        /// <summary>
        /// Filtro che include solo i tavoli aperti
        /// </summary>
        public Boolean? IsOpen { get; set; }

        /// <summary>
        /// Filtro i tavoli che non sono scaduti
        /// </summary>
        public bool DeskNotExprired { get; set; }
        /// <summary>
        /// Filtro per i contenitori
        /// </summary>
        public int DeskContainerId { get; set; }
        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public DeskFinder(IEntityMapper<Desk, DeskResult> mapper, string currentUserName)
            : this(System.Enum.GetName(typeof(EnvironmentDataCode), EnvironmentDataCode.ProtDB), mapper, currentUserName)
        {
            InitializeProperty();
        }

        public DeskFinder(string dbName, IEntityMapper<Desk, DeskResult> mapper, string currentUserName)
            : base(dbName, mapper)
        {
            ExplicitPermission = false;
            _currentUserName = currentUserName;            
            InitializeProperty();
            DeskStates = new List<DeskState>();
        }

        #endregion [ Constructor ]

        #region [ Methods ]

        /// <summary>
        /// Inizializza le proprietà della classe
        /// </summary>
        private void InitializeProperty()
        {
            UserContainers = new List<Container>();
        }

        protected override IQueryOver<Desk, Desk> DecorateCriteria(IQueryOver<Desk, Desk> queryOver)
        {
            if (IsOpen.HasValue && IsOpen.Value)
            {
                queryOver = queryOver.Where(q => q.Status.Value == DeskState.Open);
            }
            if (DeskStates.Count > 0)
            {
                queryOver = queryOver.Where(q => q.Status.IsIn(DeskStates.ToList()));
            }
            queryOver = FilterByUserPermission(queryOver);
            queryOver = FilterBySearchField(queryOver);
            return queryOver;            
        }

        /// <summary>
        /// Conteggio degli elementi restituiti dalla query attualmente utilizzata
        /// </summary>
        /// <returns></returns>
        public override int Count()
        {
            Container container = null;
            IQueryOver<Desk, Desk> queryOver = CreateQueryOver();
            queryOver = DecorateCriteria(queryOver);
            queryOver = AttachFilterExpressions(queryOver);

            return queryOver.JoinQueryOver<Container>(o => o.Container, () => container)
                            .Select(Projections.CountDistinct<Desk>(desk => desk.Id))
                            .FutureValue<int>()
                            .Value;
        }

        /// <summary>
        /// Vengono filtrati tutti i tavoli ai quali l'utente ha i seguenti diritti:
        /// 1) L'utente ha creato il tavolo oppure l'utente è stato inviato al tavolo
        /// 2) L'utente ha diritti nel contenitore in cui è stato creato il tavolo.
        /// </summary>
        /// <param name="queryOver">queryOver</param>
        /// <returns>Ritorna il queryOver con una sotto query per valutare i permessi</returns>
        private IQueryOver<Desk, Desk> FilterByUserPermission(IQueryOver<Desk, Desk> queryOver)
        {
            Disjunction filterByUserPermision = Restrictions.Disjunction();
            try
            {
                // Tavoli dell'utente
                if(!string.IsNullOrEmpty(_currentUserName))
                {
                    QueryOver<Desk, Desk> allDesksFromUser = QueryOver.Of<Desk>()
                                                                        .JoinAlias(desk => desk.DeskRoleUsers, () => deskRoleUser)
                                                                        .Where(() => deskRoleUser.AccountName == _currentUserName)
                                                                        .Select(desk => desk.Id);
                    filterByUserPermision.Add(Subqueries.WhereProperty<Desk>(desk => desk.Id).In(allDesksFromUser));                
                }
                
                // Tavoli del container
                if(!ExplicitPermission && UserContainers != null && UserContainers.Any())
                {   
                    QueryOver<Desk, Desk> allDesksFromContainer = QueryOver.Of<Desk>()
                                                                           .JoinAlias(desk => desk.Container, () => container)
                                                                           .Where(desk => container.Id
                                                                                              .IsIn(UserContainers
                                                                                                    .Select(c => c.Id)
                                                                                                    .ToArray()
                                                                                              ))
                                                                           .Select(desk => desk.Id);
                    filterByUserPermision.Add(Subqueries.WhereProperty<Desk>(desk => desk.Id).In(allDesksFromContainer));
                }

                // Aggiunta in OR delle 2 condizioni
                queryOver.Where(filterByUserPermision);
            }
            catch (Exception ex)
            {
                throw new DocSuiteException("Errore in FilterByUserPermission", ex);
            }
            return queryOver;
        }

        private IQueryOver<Desk, Desk> FilterBySearchField(IQueryOver<Desk, Desk> queryOver)
        {
            try
            {
                // Filtro sul nome del tavolo
                if (!string.IsNullOrEmpty(DeskName))
                {
                    queryOver.WhereRestrictionOn(x => x.Name).IsLike(DeskName, MatchMode.Anywhere);
                }
                if (!string.IsNullOrEmpty(DeskDescription))
                {
                    queryOver.WhereRestrictionOn(x => x.Description).IsLike(DeskDescription, MatchMode.Anywhere);
                }
                if (DeskNotExprired)
                {
                    queryOver.Where(x => x.ExpirationDate >= new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day,0,0,0) );
                }
                if (DeskContainerId != default(int))
                {
                    queryOver.Where(desk => container.Id == DeskContainerId);
                }
            }
            catch (Exception ex)
            {
                throw new DocSuiteException("Errore in FilterByUserPermission", ex);
            }
            return queryOver;
        }
        #endregion [ Methods ]
    }
}
