using System;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Entity.Fascicles
{
    public class FascicleProtocol : DSWBaseEntityFascicolable<Protocol>, IDSWEntityFascicolable<Protocol>
    {
        #region [ Constructor ]

        public FascicleProtocol() : this(Guid.NewGuid()) { }

        public FascicleProtocol(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

    }
}
