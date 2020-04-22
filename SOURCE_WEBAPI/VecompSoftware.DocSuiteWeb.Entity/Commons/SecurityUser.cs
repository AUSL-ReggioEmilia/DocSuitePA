using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Desks;

namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{

    public class SecurityUser : DSWBaseEntity
    {
        #region [ Constructor ]
        public SecurityUser() : this(Guid.NewGuid()) { }

        public SecurityUser(Guid uniqueId)
            : base(uniqueId)
        {
            DeskRoleUsers = new HashSet<DeskRoleUser>();
            //PECMailBoxUsers = new HashSet<PECMailBoxUsers>();
        }
        #endregion

        #region [ Properties ]
        /// <summary>
        /// Account name
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// Descrizione 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Dominio utente
        /// </summary>
        public string UserDomain { get; set; }

        #endregion

        #region [ Navigation Properties ]
        /// <summary>
        /// Collezione di ruoli per il tavolo
        /// </summary>
        public virtual SecurityGroup Group { get; set; }

        /// <summary>
        /// Collezione di ruoli per il tavolo
        /// </summary>
        public virtual ICollection<DeskRoleUser> DeskRoleUsers { get; set; }


        /// <summary>
        /// 
        /// </summary>
        //public virtual ICollection<PECMailBoxUsers> PECMailBoxUsers { get; set; }
        #endregion
    }
}
