using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Entity.Resolutions
{
    public class ResolutionKind : DSWBaseEntity
    {
        #region [ Constructor ]
        public ResolutionKind() : this(Guid.NewGuid()) { }
        public ResolutionKind(Guid uniqueId)
            : base(uniqueId)
        {
            Resolutions = new HashSet<Resolution>();
            ResolutionKindDocumentSeries = new HashSet<ResolutionKindDocumentSeries>();
        }
        #endregion

        #region [ Properties ]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool AmountEnabled { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual ICollection<Resolution> Resolutions { get; set; }
        public virtual ICollection<ResolutionKindDocumentSeries> ResolutionKindDocumentSeries { get; set; }
        #endregion
    }
}
