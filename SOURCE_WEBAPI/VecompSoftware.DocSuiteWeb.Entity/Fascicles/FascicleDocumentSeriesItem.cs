using System;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Entity.Fascicles
{
    public class FascicleDocumentSeriesItem : DSWBaseEntityFascicolable<DocumentSeriesItem>, IDSWEntityFascicolable<DocumentSeriesItem>
    {
        #region [ Constructor ]

        public FascicleDocumentSeriesItem() : this(Guid.NewGuid()) { }

        public FascicleDocumentSeriesItem(Guid uniqueId)
            : base(uniqueId)
        { }

        #endregion
    }
}
