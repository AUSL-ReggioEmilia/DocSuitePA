using System;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Entity.Resolutions
{
    public class ResolutionContact : DSWBaseEntity
    {
        #region [ Constructor ]

        public ResolutionContact() : this(Guid.NewGuid()) { }
        public ResolutionContact(Guid uniqueId)
            : base(uniqueId) { }
        #endregion

        #region[ Properties ]

        public int IdResolution { get; set; }

        // La propretà ComunicationType dovrà essere dichiarata come enumeratore
        public string ComunicationType { get; set; }
        public short? Incremental { get; set; }

        #endregion

        #region[ Navigation Properties ]
        public virtual Resolution Resolution { get; set; }
        public virtual Contact Contact { get; set; }
        #endregion
    }
}
