using System;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Repository.Entity;

namespace VecompSoftware.DocSuiteWeb.Entity.Protocols
{
    public class ProtocolContact : DSWBaseEntity, IUnauditableEntity
    {
        #region [ Constructor ]

        public ProtocolContact() : this(Guid.NewGuid()) { }
        public ProtocolContact(Guid uniqueId)
            : base(uniqueId) { }
        #endregion

        #region[ Properties ]

        public short Year { get; set; }
        public int Number { get; set; }

        // La propretà ComunicationType dovrà essere dichiarata come enumeratore
        public string ComunicationType { get; set; }
        public string Type { get; set; }

        #endregion

        #region[ Navigation Properties ]
        public virtual Protocol Protocol { get; set; }
        public virtual Contact Contact { get; set; }
        #endregion
    }
}
