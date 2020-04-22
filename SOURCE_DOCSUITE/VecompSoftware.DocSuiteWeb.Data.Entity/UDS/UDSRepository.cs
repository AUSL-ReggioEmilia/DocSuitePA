using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.UDS
{
    public class UDSRepository : DomainObject<Guid>, IAuditable
    {
        #region Constructors

        protected UDSRepository() : base()
        {

        }

        public UDSRepository(string userName)
            : this()
        {
            this.RoleUsers = new HashSet<RoleUser>();
            RegistrationDate = DateTimeOffset.UtcNow;
            RegistrationUser = userName;
        }

        #endregion

        #region [ Properties ]

        public virtual string Name { get; set; }

        public virtual string Alias { get; set; }

        public virtual short SequenceCurrentYear { get; set; }

        public virtual int SequenceCurrentNumber { get; set; }

        public virtual int DSWEnvironment { get; set; }

        public virtual string ModuleXML { get; set; }

        public virtual short Version { get; set; }

        public virtual DateTimeOffset ActiveDate { get; set; }

        public virtual DateTimeOffset? ExpiredDate { get; set; }

        /// <summary>
        /// Stato di una UDS
        /// 1 = Bozza
        /// 2 = Confermata
        /// </summary>
        public virtual UDSRepositoryState Status { get; set; }

        public virtual DateTimeOffset? LastChangedDate { get; set; }

        public virtual string LastChangedUser { get; set; }

        public virtual DateTimeOffset RegistrationDate { get; set; }

        public virtual string RegistrationUser { get; set; }

        #endregion

        #region [ Navigation Properties ]        

        public virtual Container Container { get; set; }

        public virtual UDSSchemaRepository UDSSchemaRepository { get; set; }

        public virtual ICollection<RoleUser> RoleUsers { get; set; }


        #endregion
    }
}
