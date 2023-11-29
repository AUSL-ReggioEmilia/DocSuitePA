using System;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Commons
{
    public class JeepServiceHost : DomainObject<Guid>, ISupportBooleanLogicDelete
    {
        #region Constructors
        /// <summary>
        /// Costruttore vuoto ereditato dalla classe base
        /// </summary>
        protected JeepServiceHost() : base()
        {
            
        }
        public JeepServiceHost(string UserName)
            : this()
        {
            RegistrationDate = DateTimeOffset.UtcNow;
            RegistrationUser = UserName;
        }
        #endregion

        #region Properties

        public virtual string Hostname { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual bool IsDefault { get; set; }

    
        public virtual string RegistrationUser { get; set; }

        public virtual DateTimeOffset RegistrationDate { get; set; }
        public virtual string LastChangedUser {get; set;}

        public virtual DateTimeOffset? LastChangedDate { get; set; }
        #endregion
    }
}
