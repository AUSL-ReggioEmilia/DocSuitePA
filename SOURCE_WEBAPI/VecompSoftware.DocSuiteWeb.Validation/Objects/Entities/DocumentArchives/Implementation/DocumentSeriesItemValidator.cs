using System;
using System.Collections.Generic;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentArchives
{
    public class DocumentSeriesItemValidator : ObjectValidator<DocumentSeriesItem, DocumentSeriesItemValidator>, IDocumentSeriesItemValidator
    {
        #region [ Constructor ]
        public DocumentSeriesItemValidator(ILogger logger, IDocumentSeriesItemValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        {
            DocumentSeriesItemLogs = new HashSet<DocumentSeriesItemLog>();
        }

        #endregion

        #region[ Properties ]

        public int EntityId { get; set; }
        public Guid UniqueId { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public int? Year { get; set; }
        public int? Number { get; set; }
        public Guid? IdMain { get; set; }
        public Guid? IdAnnexed { get; set; }
        public Guid? IdUnpublishedAnnexed { get; set; }
        public DateTime? PublishingDate { get; set; }
        public DateTime? RetireDate { get; set; }
        public DocumentSeriesItemStatus? Status { get; set; }
        public string Subject { get; set; }
        public bool? Priority { get; set; }
        public bool? IsActive { get; set; }
        public int? IdDocumentSeriesSubsection { get; set; }
        public Guid? DematerialisationChainId { get; set; }
        public bool? HasMainDocument { get; set; }
        public string ConstraintValue { get; set; }

        #endregion

        #region[ Navigation Properties ]
        public DocumentSeries DocumentSeries { get; set; }
        public Category Category { get; set; }
        public Location Location { get; set; }
        public Location LocationAnnexed { get; set; }
        public Location LocationUnpublishedAnnexed { get; set; }
        public ICollection<DocumentSeriesItemLog> DocumentSeriesItemLogs { get; set; }

        //da aggiungere quando verranno mappate l'entità DocumentSeriesSubsection
        //public DocumentSeriesSubsection DocumentSeriesSubsection { get; set; }


        #endregion
    }
}