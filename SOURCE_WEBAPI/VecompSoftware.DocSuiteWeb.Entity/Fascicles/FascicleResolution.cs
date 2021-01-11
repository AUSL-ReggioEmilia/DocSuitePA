using System;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Entity.Fascicles
{
    public class FascicleResolution : DSWBaseEntityFascicolable<Resolution>, IDSWEntityFascicolable<Resolution>
    {
        #region [ Constructor ]

        public FascicleResolution() : this(Guid.NewGuid()) { }

        public FascicleResolution(Guid uniqueId)
            : base(uniqueId)
        { }

        #endregion
    }
}
