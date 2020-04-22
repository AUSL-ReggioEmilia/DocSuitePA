using System;
using VecompSoftware.DocSuiteWeb.Repository.Entity;

namespace VecompSoftware.DocSuiteWeb.Entity.PECMails
{
    public class PECMailLog : DSWBaseEntity, IUnauditableEntity
    {
        #region [ Constructor ]

        public PECMailLog() : this(Guid.NewGuid()) { }

        public PECMailLog(Guid uniqueId)
            : base(uniqueId)
        {
        }

        #endregion

        #region [ Properties ]        

        public string Description { get; set; }

        public string LogType { get; set; }

        public DateTime LogDate { get; set; }

        public string SystemComputer { get; set; }

        public short? Severity { get; set; }

        public string Hash { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public PECMail PECMail { get; set; }

        #endregion
    }
}
