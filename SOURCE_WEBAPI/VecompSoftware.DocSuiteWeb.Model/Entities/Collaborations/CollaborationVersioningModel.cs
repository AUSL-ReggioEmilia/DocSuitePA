using System;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations
{
    public class CollaborationVersioningModel
    {
        #region [ Constructor ]

        public CollaborationVersioningModel() { }

        #endregion

        #region [ Properties ]

        public bool? CheckedOut { get; set; }

        public DateTimeOffset? CheckOutDate { get; set; }

        public string CheckOutUser { get; set; }

        public short CollaborationIncremental { get; set; }

        public string DocumentName { get; set; }

        public string DocumentGroup { get; set; }

        public string RegistrationUser { get; set; }

        public Guid? IdCollaborationVersioning { get; set; }

        public int IdDocument { get; set; }

        public short Incremental { get; set; }

        public bool? IsActive { get; set; }

        public byte[] DocumentContent { get; set; }

        public DocumentModel Document { get; set; }



        #endregion

        #region [ Methods ]
        #endregion
    }
}
