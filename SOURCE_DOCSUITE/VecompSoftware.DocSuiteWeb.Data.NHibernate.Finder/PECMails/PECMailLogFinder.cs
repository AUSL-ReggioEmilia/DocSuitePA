using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using VecompSoftware.DocSuiteWeb.EntityMapper;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.PECMails
{
    public class PECMailLogFinder : BaseFinder<PECMailLog, PECMailLogHeader>
    {
        #region Properties
       
        public PECMail PECMail { get; set; }
        public List<int> PECMailIDIn { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string LogType { get; set; }
        public string DescriptionLike { get; set; }

        #endregion

        #region Constructors
        public PECMailLogFinder(string dbName, IEntityMapper<PECMailLog, PECMailLogHeader> mapper)
            : base(dbName, mapper)
        { }

        public PECMailLogFinder(IEntityMapper<PECMailLog, PECMailLogHeader> mapper)
            : this(System.Enum.GetName(typeof(EnvironmentDataCode), EnvironmentDataCode.ProtDB), mapper)
        { }

        #endregion

        #region Methods
        
        protected override IQueryOver<PECMailLog, PECMailLog> CreateQueryOver()
        {
            return NHibernateSession.QueryOver<PECMailLog>();
        }

        /// <summary>
        /// Verifica se la data di inizio e di fine sono nell'ordine corretto
        /// </summary>
        private void innerDateTimeRange()
        {
            if (!(this.DateFrom.HasValue && this.DateTo.HasValue))
                return;
            if (this.DateTo.Value.AddDays(1).AddSeconds(-1) < this.DateFrom)
            {
                DateTime? tempFrom = this.DateFrom;
                this.DateFrom = this.DateTo;
                this.DateTo = tempFrom;
            }
        }

        /// <summary>
        /// Crea i criteri base per l'interrogazione
        /// </summary>
        /// <param name="queryOver"></param>
        /// <returns></returns>
        protected override IQueryOver<PECMailLog, PECMailLog> DecorateCriteria(IQueryOver<PECMailLog, PECMailLog> queryOver)
        {
            if (PECMail != null)
                queryOver.Where(x => x.Mail == PECMail);
            if (PECMailIDIn != null && PECMailIDIn.Count > 0)
                queryOver.AndRestrictionOn(x => x.Id).IsIn(PECMailIDIn);
            if (!string.IsNullOrEmpty(LogType))
                queryOver.Where(x => x.Type == LogType);
            if (!string.IsNullOrEmpty(DescriptionLike))
                queryOver.WhereRestrictionOn(x => x.Description).IsInsensitiveLike(DescriptionLike, MatchMode.Anywhere);

            innerDateTimeRange();
            if (DateFrom.HasValue)
                queryOver.Where(x => x.Date > DateFrom);
            if (DateTo.HasValue)
                queryOver.Where(x => x.Date < DateTo.Value.AddDays(1).AddSeconds(-1));
            return queryOver;
        }

        public override int Count()
        {
            IQueryOver<PECMailLog, PECMailLog> queryOver = CreateQueryOver();
            queryOver = DecorateCriteria(queryOver);
            queryOver = AttachFilterExpressions(queryOver);
            
            return queryOver.Select(Projections.CountDistinct<PECMailLog>(x => x.Id))
                            .FutureValue<int>()
                            .Value;
        }
        #endregion
    }
}