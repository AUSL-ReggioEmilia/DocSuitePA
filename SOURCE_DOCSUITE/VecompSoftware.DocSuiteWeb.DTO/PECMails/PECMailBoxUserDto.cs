using System;

namespace VecompSoftware.DocSuiteWeb.DTO.PECMails
{
    public class PECMailBoxUserDto
    {
        #region [ Constructor ]
        public PECMailBoxUserDto() { }
        #endregion

        #region [ Properties ]
        public Guid? Id { get; set; }
        public string Account { get; set; }
        public int? SecurityId { get; set; }
        public string SecurityAccount { get; set; }
        public short? MailBoxId { get; set; }
        public string MailBoxName { get; set; }
        #endregion
    }
}
