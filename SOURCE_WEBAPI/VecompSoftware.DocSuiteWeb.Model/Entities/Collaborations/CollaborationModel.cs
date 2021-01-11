using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations
{
    public class CollaborationModel : IContentBase
    {
        #region [ Constructor ]

        public CollaborationModel()
        {
            CollaborationVersionings = new HashSet<CollaborationVersioningModel>();
            CollaborationUsers = new HashSet<CollaborationUserModel>();
            CollaborationSigns = new HashSet<CollaborationSignModel>();
        }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }

        public DateTime? AlertDate { get; set; }

        public string DocumentType { get; set; }

        public int? IdCollaboration { get; set; }

        public string IdPriority { get; set; }

        public string IdStatus { get; set; }

        public DateTime? MemorandumDate { get; set; }

        public string Note { get; set; }

        public int? Number { get; set; }

        public DateTime? PublicationDate { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public string PublicationUser { get; set; }

        public string RegistrationName { get; set; }

        public string RegistrationUser { get; set; }

        public string Account { get; set; }

        public string DestinationFirst { get; set; }

        public short? SignCount { get; set; }

        public string Subject { get; set; }

        public short? Year { get; set; }

        public string TemplateName { get; set; }

        public ICollection<CollaborationVersioningModel> CollaborationVersionings { get; set; }

        public ICollection<CollaborationUserModel> CollaborationUsers { get; set; }

        public ICollection<CollaborationSignModel> CollaborationSigns { get; set; }

        public DocumentSeriesItemModel DocumentSeriesItem { get; set; }

        public ResolutionModel Resolution { get; set; }

        public CollaborationProtocolModel Protocol { get; set; }

        #endregion

        #region [ Methods ]

        public bool HasProtocol()
        {
            return (Year.HasValue && Year.Value > 0) && (Number.HasValue && Number.Value > 0);
        }

        public bool HasResolution()
        {
            return Resolution != null;
        }

        public bool HasSeries()
        {
            return DocumentSeriesItem != null;
        }

        public bool HasDocumentExtracted()
        {
            if (CollaborationVersionings == null || !CollaborationVersionings.Any())
            {
                return false;
            }

            return CollaborationVersionings.Any(x => x.CheckedOut == true);
        }

        public int VersioningCount()
        {
            if (CollaborationVersionings == null || !CollaborationVersionings.Any())
            {
                return 0;
            }

            return CollaborationVersionings.Count(x => !x.DocumentName.Contains(".p7m"));
        }
        #endregion
    }
}
