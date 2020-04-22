using System;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Entity.Resolutions
{
    public class ResolutionKindDocumentSeries : DSWBaseEntity
    {
        #region [ Constructor ]
        public ResolutionKindDocumentSeries() : this(Guid.NewGuid()) { }
        public ResolutionKindDocumentSeries(Guid uniqueId)
            : base(uniqueId)
        {

        }
        #endregion

        #region [ Properties ]
        public bool DocumentRequired { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual ResolutionKind ResolutionKind { get; set; }
        public virtual DocumentSeries DocumentSeries { get; set; }
        public virtual DocumentSeriesConstraint DocumentSeriesConstraint { get; set; }
        #endregion
    }
}
