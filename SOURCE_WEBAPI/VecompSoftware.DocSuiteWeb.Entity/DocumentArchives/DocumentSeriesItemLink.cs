using System;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Entity.DocumentArchives
{

    public class DocumentSeriesItemLink : DSWBaseEntity
    {
        #region [ Constructor ]

        public DocumentSeriesItemLink() : this(Guid.NewGuid()) { }
        public DocumentSeriesItemLink(Guid uniqueId)
            : base(uniqueId)
        {

        }
        #endregion

        #region [ Properties ]   
        public string LinkType { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public virtual DocumentSeriesItem DocumentSeriesItem { get; set; }
        public virtual Resolution Resolution { get; set; }
        #endregion
    }
}
