using System;
using VecompSoftware.DocSuiteWeb.Data.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Commons
{
    public class CategoryFascicle : AuditableDomainObject<Guid>
    {
        #region [ Constructors ]
        public CategoryFascicle(): base()
        {

        }

        public CategoryFascicle(string username)
            : this()
        {
            RegistrationDate = DateTimeOffset.UtcNow;
            RegistrationUser = username;
        }
        #endregion

        #region [ Properties ]

        public virtual FascicleType FascicleType { get; set; }

        public virtual int DSWEnvironment { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual Category Category { get; set; }

        public virtual FasciclePeriod FasciclePeriod { get; set; }

        public virtual Contact Manager { get; set; }

        #endregion
    }
}
