using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Entity.DocumentArchives
{
    public class DocumentSeriesConstraint : DSWBaseEntity
    {
        #region [ Constructor ]
        public DocumentSeriesConstraint() : this(Guid.NewGuid()) { }
        public DocumentSeriesConstraint(Guid uniqueId)
            : base(uniqueId)
        {
            ResolutionKindDocumentSeries = new HashSet<ResolutionKindDocumentSeries>();
        }
        #endregion

        #region[ Properties ]
        public string Name { get; set; }
        #endregion

        #region[ Navigation Properties ]
        public virtual DocumentSeries DocumentSeries { get; set; }
        public virtual ICollection<ResolutionKindDocumentSeries> ResolutionKindDocumentSeries { get; set; }
        #endregion
    }
}
