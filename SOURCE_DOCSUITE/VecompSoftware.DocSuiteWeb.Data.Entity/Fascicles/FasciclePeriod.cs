using System;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Fascicles
{
    public class FasciclePeriod : AuditableDomainObject<Guid>, ISupportBooleanLogicDelete
    {
        #region [ Constructors ]
        /// <summary>
        /// Costruttore vuoto ereditato dalla classe base
        /// </summary>
        protected FasciclePeriod() : base()
        {

        }
        #endregion

        #region [ Properties ]

        public virtual bool IsActive { get; set; }

        public virtual string PeriodName { get; set; }

        public virtual double PeriodDays { get; set; }

        #endregion
    }
}
