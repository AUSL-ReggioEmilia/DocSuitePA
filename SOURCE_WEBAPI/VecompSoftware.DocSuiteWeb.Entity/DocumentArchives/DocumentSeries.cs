using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Entity.DocumentArchives
{
    public class DocumentSeries : DSWBaseEntity
    {
        #region [ Constructor ]

        public DocumentSeries() : this(Guid.NewGuid()) { }
        public DocumentSeries(Guid uniqueId)
            : base(uniqueId)
        {
            DocumentSeriesItems = new HashSet<DocumentSeriesItem>();
            DocumentSeriesConstraints = new HashSet<DocumentSeriesConstraint>();
            ResolutionKindDocumentSeries = new HashSet<ResolutionKindDocumentSeries>();
        }
        #endregion

        #region[ Properties ]

        public string Name { get; set; }
        public bool PublicationEnabled { get; set; }
        public bool? SubsectionEnabled { get; set; }
        public bool? RoleEnabled { get; set; }
        public bool? AllowNoDocument { get; set; }
        public bool? AllowAddDocument { get; set; }
        public bool? AttributeSorting { get; set; }
        public bool? AttributeCache { get; set; }
        public int? SortOrder { get; set; }

        //da togliere quando verrà mappata l'entità DocumentSeriesFamily
        public int? IdDocumentSeriesFamily { get; set; }

        #endregion

        #region[ Navigation Properties ]
        public virtual Container Container { get; set; }
        public virtual ICollection<DocumentSeriesItem> DocumentSeriesItems { get; set; }
        public virtual ICollection<DocumentSeriesConstraint> DocumentSeriesConstraints { get; set; }
        public virtual ICollection<ResolutionKindDocumentSeries> ResolutionKindDocumentSeries { get; set; }
        #endregion
    }
}
