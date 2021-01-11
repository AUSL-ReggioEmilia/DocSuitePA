using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentArchives
{
    public class DocumentSeriesValidator : ObjectValidator<DocumentSeries, DocumentSeriesValidator>, IDocumentSeriesValidator
    {
        #region [ Constructor ]
        public DocumentSeriesValidator(ILogger logger, IDocumentSeriesValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        { }

        #endregion

        #region[ Properties ]

        public int EntityId { get; set; }

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
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }

        #endregion

        #region[ Navigation Properties ]
        public Container Container { get; set; }
        public ICollection<DocumentSeriesItem> DocumentSeriesItems { get; set; }
        public ICollection<DocumentSeriesConstraint> DocumentSeriesConstraints { get; set; }
        public ICollection<ResolutionKindDocumentSeries> ResolutionKindDocumentSeries { get; set; }
        //da aggiungere quando verrà mappata l'entità DocumentSeriesFamily
        //public DocumentSeriesFamily DocumentSeriesFamily { get; set; }

        #endregion
    }
}