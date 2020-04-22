using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.UDS
{
    public class UDSSchemaRepository : DomainObject<Guid>, IAuditable
    {
        #region Constructors

        protected UDSSchemaRepository() : base()
        {

        }

        public UDSSchemaRepository(string userName)
            : this()
        {
            this.UDSRepositories = new Collection<UDSRepository>();
            RegistrationDate = DateTimeOffset.UtcNow;
            RegistrationUser = userName;
        }

        #endregion

        #region [ Properties ]

        public virtual string SchemaXML { get; set; }

        public virtual short Version { get; set; }

        public virtual DateTimeOffset ActiveDate { get; set; }

        public virtual DateTimeOffset? ExpiredDate { get; set; }

        public virtual DateTimeOffset? LastChangedDate { get; set; }

        public virtual string LastChangedUser { get; set; }

        public virtual DateTimeOffset RegistrationDate { get; set; }

        public virtual string RegistrationUser { get; set; }

        public virtual ICollection<UDSRepository> UDSRepositories { get; set; }       

        #endregion
    }
}
