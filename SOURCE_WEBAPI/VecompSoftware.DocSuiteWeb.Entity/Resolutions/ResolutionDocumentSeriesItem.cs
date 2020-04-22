using System;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Entity.Resolutions
{
    public class ResolutionDocumentSeriesItem : DSWBaseEntity
  
    {
        #region [ Constructor ]

        public ResolutionDocumentSeriesItem() : this(Guid.NewGuid()) { }
        public ResolutionDocumentSeriesItem(Guid uniqueId) : base(uniqueId)
        {
        }
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Navigation Properties ]
        public DocumentSeriesItem DocumentSeriesItem { get; set; }
        public Resolution Resolution { get; set; }
        #endregion
    }
}
