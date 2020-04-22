using System;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Commons
{
    public class CategoryFascicleRight : AuditableDomainObject<Guid>
    {
        #region [ Constructors ]
        public CategoryFascicleRight() : base()
        {

        }

        public CategoryFascicleRight(string username)
            : this()
        {
            RegistrationDate = DateTimeOffset.UtcNow;
            RegistrationUser = username;
        }
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Navigation Properties ]

        public virtual CategoryFascicle CategoryFascicle { get; set; }

        public virtual Role Role { get; set; }

        public virtual Container Container { get; set; }

        #endregion
    }
}
