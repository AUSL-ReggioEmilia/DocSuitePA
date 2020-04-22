using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations
{
    public class CollaborationTableValuedModel
    {
        #region [ Constructor ]

        public CollaborationTableValuedModel()
        {
        }

        #endregion

        #region [ Properties ]

        public DateTime? AlertDate { get; set; }

        public string DocumentType { get; set; }

        public int IdCollaboration { get; set; }

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

        #region [ CollaborationSigns ]

        public Guid? CollaborationSign_IdCollaborationSign { get; set; }

        public short CollaborationSign_Incremental { get; set; }

        public bool CollaborationSign_IsActive { get; set; }

        public bool? CollaborationSign_IsRequired { get; set; }

        public string CollaborationSign_SignName { get; set; }

        public string CollaborationSign_SignUser { get; set; }

        public DateTime? CollaborationSign_SignDate { get; set; }

        public bool? CollaborationSign_IsAbsent { get; set; }

        #endregion

        #region [ CollaborationUsers ]

        public Guid? CollaborationUser_IdCollaborationUser { get; set; }

        public bool CollaborationUser_DestinationFirst { get; set; }

        public string CollaborationUser_DestinationName { get; set; }

        #endregion

        #region [ CollaborationVersionings ]

        public Guid? CollaborationVersioning_IdCollaborationVersioning { get; set; }

        public string CollaborationVersioning_DocumentName { get; set; }

        public short CollaborationVersioning_CollaborationIncremental { get; set; }

        public string CollaborationVersioning_RegistrationUser { get; set; }

        public short CollaborationVersioning_Incremental { get; set; }

        #endregion

        #region [ DocumentSeriesItem ]
        public int? DocumentSeriesItem_IdDocumentSeriesItem { get; set; }

        public int? DocumentSeriesItem_Number { get; set; }

        public int? DocumentSeriesItem_Year { get; set; }

        #endregion

        #region [ Resolution ]

        public int? Resolution_IdResolution { get; set; }

        public int? Resolution_Number { get; set; }

        public short? Resolution_Year { get; set; }

        public DateTime? Resolution_PublishingDate { get; set; }

        public string Resolution_ServiceNumber { get; set; }

        public DateTime? Resolution_AdoptionDate { get; set; }

        #endregion

        #endregion

        #region [ Methods ]

        #endregion
    }
}
