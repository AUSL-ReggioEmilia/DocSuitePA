using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.PECMails
{
    public class PECMailAttachmentModel
    {
        #region [ Constructor ]
        public PECMailAttachmentModel()
        {
        }
        #endregion

        #region [ Proprieties ]
        public int EntityId { get; set; }
        public string AttachmentName { get; set; }
        public bool IsMain { get; set; }
        public Guid? IDDocument { get; set; }
        public long? Size { get; set; }
        public Guid UniqueId { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        #endregion

        #region [ Methods ]

        #endregion
    }
}
