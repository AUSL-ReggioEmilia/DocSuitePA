using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.DTO.Workflows;
using VecompSoftware.DocSuiteWeb.EntityMapper;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Workflows
{
    public class WorkflowActivityFinder : BaseWorkflowFinder<WorkflowActivity, WorkflowActivityResult>
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]
        private readonly string _currentUserName;
        private IList<WorkflowActivityResult> _userWorkflowActivity;

        public string WorkflowActivityName { get; set; }

        public string WorkflowInstanceName { get; set; }

        public String WorkflowSubject { get; set; }
        
        public String RequestorUser { get; set; }

        public bool? ExcludeDocumentUnitReferencedId { get; set; }

        public Guid? ExcludeWorkflowActivityId { get; set; }

        public Guid? WorkflowInstanceId { get; set; }
        public DateTime? WorkflowDateFrom { get; set; }
        public DateTime? WorkflowDateTo { get; set; }

        public ICollection<WorkflowStatus> WorkflowActivityStatus { get; set; }

        //public IList<Role> UserRoles { get; set; }
        public ICollection<ActivityType> WorkflowActivityType { get; set; }

        public ICollection<ActivityType> ExcludeDefaultWorkflowActivityType { get; set; }

        public Guid? IdTenant { get; set; }
        #endregion

        #region [ Constructor ]  
        public WorkflowActivityFinder(IEntityMapper<WorkflowActivity, WorkflowActivityResult> mapper, string currentUserName)
            : this(System.Enum.GetName(typeof(EnvironmentDataCode), EnvironmentDataCode.ProtDB), mapper, currentUserName)
        {
            
        }
        public WorkflowActivityFinder(string dbName, IEntityMapper<WorkflowActivity, WorkflowActivityResult> mapper, string currentUserName)
            : base(dbName, mapper)
        {
            _currentUserName = currentUserName;
            InitializeProperty();
        }

        #endregion

        #region [ Methods ]
        /// <summary>
        /// Inizializza le proprietà della classe
        /// </summary>
        private void InitializeProperty()
        {
            _userWorkflowActivity = new List<WorkflowActivityResult>();
            WorkflowActivityType = new List<ActivityType>();
            WorkflowActivityStatus = new List<WorkflowStatus>();
            ExcludeDefaultWorkflowActivityType = new List<ActivityType>();
            ExcludeDefaultWorkflowActivityType.Add(ActivityType.BuildAchive);
            ExcludeDefaultWorkflowActivityType.Add(ActivityType.BuildMessages);
            ExcludeDefaultWorkflowActivityType.Add(ActivityType.BuildPECMail);
            ExcludeDefaultWorkflowActivityType.Add(ActivityType.BuildProtocol);
            ExcludeDefaultWorkflowActivityType.Add(ActivityType.AutomaticActivity);
        }
        WorkflowActivity workflowActivityAlias = null;

        protected override IQueryOver<WorkflowActivity, WorkflowActivity> CreateQueryOver()
        {
            return NHibernateSession.QueryOver(() => workflowActivityAlias);
        }
        protected override IQueryOver<WorkflowActivity, WorkflowActivity> DecorateCriteria(IQueryOver<WorkflowActivity, WorkflowActivity> queryOver)
        {
            WorkflowRepository workflowRepository = null;
            WorkflowInstance workflowInstance = null;
            queryOver
                .JoinQueryOver(o => o.WorkflowInstance, () => workflowInstance)
                .JoinQueryOver(o => o.WorkflowRepository, () => workflowRepository);


            
            queryOver.Where(q => !q.ActivityType.IsIn(ExcludeDefaultWorkflowActivityType.ToList()));
            if (WorkflowActivityType.Count > 0)
            {
                queryOver.Where(q => q.ActivityType.IsIn(WorkflowActivityType.ToList()));
            }

            if (WorkflowActivityStatus.Count > 0)
            {
                queryOver.Where(q => q.Status.IsIn(WorkflowActivityStatus.ToList()));
            }

            if (ExcludeDocumentUnitReferencedId.HasValue && ExcludeDocumentUnitReferencedId.Value)
            {
                queryOver.Where(q => q.DocumentUnitReferencedId == null);
            }

            if (ExcludeWorkflowActivityId.HasValue)
            {
                queryOver.Where(q => q.Id != ExcludeWorkflowActivityId.Value);
            }

            if (WorkflowInstanceId.HasValue)
            {
                queryOver.Where(q => q.WorkflowInstance.Id == WorkflowInstanceId.Value);
            }

            if (WorkflowDateFrom.HasValue && WorkflowDateTo.HasValue)
            {
                queryOver.Where(q => q.RegistrationDate >= WorkflowDateFrom.Value && q.RegistrationDate <= WorkflowDateTo);
            }

            if (IdTenant.HasValue)
            {
                queryOver.Where(q => q.IdTenant == null || q.IdTenant == IdTenant.Value);
            }

            queryOver = FilterBySearchField(queryOver);
            queryOver = FilterByUserPermission(queryOver);
            

            return queryOver;
        }


        private IQueryOver<WorkflowActivity, WorkflowActivity> FilterBySearchField(IQueryOver<WorkflowActivity, WorkflowActivity> queryOver)
        {
            WorkflowRepository workflowRepository = null;
            try
            {
                // Filtro sul nome del wf
                if (!string.IsNullOrEmpty(WorkflowActivityName))
                {
                    queryOver.WhereRestrictionOn(x => x.Name).IsLike(WorkflowActivityName, MatchMode.Anywhere);
                }
                if (!string.IsNullOrEmpty(WorkflowInstanceName))
                {           
                    queryOver.WhereRestrictionOn(x => workflowRepository.Name).IsLike(WorkflowInstanceName, MatchMode.Anywhere);
                }
                if (!string.IsNullOrEmpty(WorkflowSubject))
                {
                    queryOver.WhereRestrictionOn(x => x.Subject).IsLike(WorkflowSubject, MatchMode.Anywhere);
                }
            }
            catch (Exception ex)
            {
                throw new DocSuiteException("Errore in FilterBySearchFields", ex);
            }
            return queryOver;
        }

        /// <summary>
        /// Vengono filtrate le workflow activity ai quali l'utente ha diritto:
        /// </summary>
        private IQueryOver<WorkflowActivity, WorkflowActivity> FilterByUserPermission(IQueryOver<WorkflowActivity, WorkflowActivity> queryOver)
        {
            try
            {
                if (!string.IsNullOrEmpty(_currentUserName))
                {
                    DetachedCriteria dCriteria = DetachedCriteria.For<WorkflowAuthorization>().Add(
                        Restrictions.Where<WorkflowAuthorization>(wa => wa.Account == _currentUserName && wa.WorkflowActivity.Id == workflowActivityAlias.Id))
                        .SetProjection(Projections.Property<WorkflowAuthorization>(wa => wa.Id));

                    queryOver.And(Restrictions.Or(
                        Restrictions.Where<WorkflowActivity>(wa => wa.RegistrationUser == RequestorUser),
                        Subqueries.Exists(dCriteria)));
                }                
            }
            catch (Exception ex)
            {
                throw new DocSuiteException("Errore in FilterByUserPermission", ex);
            }
            return queryOver;
        }

        /// <summary>
        /// Metodo che consente di contare i workflow assegnati all'utente.
        /// Questo metodo viene utilizzato nel frameset per riportare il numero dei workflow assegnati all'utente corrente.
        /// </summary>
        /// <returns></returns>
        public override int Count()
        {
            int counter = default(int);
            IQueryOver<WorkflowActivity, WorkflowActivity> queryOver = CreateQueryOver();
            queryOver = DecorateCriteria(queryOver);
            using (ITransaction transaction = NHibernateSession.BeginTransaction())
            {
                try
                {
                    counter = queryOver.RowCount();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                return counter;
            }
        }


        #endregion
    }
}
