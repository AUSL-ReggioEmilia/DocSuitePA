using System;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Commons
{
    public class JeepServiceHost : DomainObject<Guid>, ISupportLogicDelete
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
            RegistrationDate = DateTime.UtcNow;
            RegistrationUser = UserName;
        }
        #endregion

        #region Properties

        public virtual string Hostname { get; set; }

        public virtual short IsActive { get; set; }

        public virtual bool IsDefault { get; set; }

    
        public virtual string RegistrationUser { get; set; }

        public virtual DateTime? RegistrationDate { get; set; }
        #endregion
    }
}
