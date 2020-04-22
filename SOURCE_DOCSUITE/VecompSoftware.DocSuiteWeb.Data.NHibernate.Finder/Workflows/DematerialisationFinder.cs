using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using VecompSoftware.DocSuiteWeb.Data.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.DTO.Workflows;
using VecompSoftware.DocSuiteWeb.EntityMapper;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Workflows
{
    public class DematerialisationFinder : BaseWorkflowFinder<Protocol, DematerialisationStatementResult>
    {
        #region [ Fields ]


        #endregion [ Fields ]

        #region [ Properties ]

        public string LogType { get; set; }

        public string UserName { get; set; }
        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public ProtocolLog logs = null;

        #endregion [ Properties ]

        #region [ Constructor ]
        public DematerialisationFinder(IEntityMapper<Protocol, DematerialisationStatementResult> mapper)
            : this(System.Enum.GetName(typeof(EnvironmentDataCode), EnvironmentDataCode.ProtDB), mapper)
        {
        }

        public DematerialisationFinder(string dbName, IEntityMapper<Protocol, DematerialisationStatementResult> mapper)
            : base(dbName, mapper)
        {
        }

        #endregion [ Constructor ]

        #region [ Methods ]

        /// <summary>
        /// Inizializza le proprietà della classe
        /// </summary>


        protected override IQueryOver<Protocol, Protocol> DecorateCriteria(IQueryOver<Protocol, Protocol> queryOver)
        {
            if (!string.IsNullOrEmpty(UserName) && (!string.IsNullOrEmpty(LogType)))
            {
                queryOver = FilterByProtocolLog(queryOver);
            }

            return queryOver;
        }

        /// <summary>
        /// Conteggio degli elementi restituiti dalla query attualmente utilizzata
        /// </summary>
        /// <returns></returns>
        public override int Count()
        {
            IQueryOver<Protocol, Protocol> queryOver = CreateQueryOver();
            queryOver = DecorateCriteria(queryOver);
            queryOver = AttachFilterExpressions(queryOver);
            return queryOver
                .Select(Projections.CountDistinct<Protocol>(p => p.UniqueId))
                .FutureValue<int>().Value;
        }

        private IQueryOver<Protocol, Protocol> FilterByProtocolLog(IQueryOver<Protocol, Protocol> queryOver)
        {

            Disjunction logFromProtocol = Restrictions.Disjunction();

            QueryOver<Protocol, Protocol> successLogFromProtocol = QueryOver.Of<Protocol>().JoinAlias(p => p.ProtocolLogs, () => logs)
                           .Where(() => logs.LogType == LogType && logs.SystemUser == UserName && logs.LogDate > DateFrom && logs.LogDate < DateTo)
                           .Select(p => p.UniqueId);

            logFromProtocol.Add(Subqueries.WhereProperty<Protocol>(p => p.UniqueId).In(successLogFromProtocol));

            queryOver.Where(logFromProtocol);

            return queryOver;
        }
        #endregion [ Methods ]
    }
}
