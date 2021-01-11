using System;
using VecompSoftware.Commons.Infos;
using VecompSoftware.Helpers.Reporting;

namespace VecompSoftware.Services.Reporting
{
    [Serializable]
    public class SignContentReport : SignInfo, IReport
    {

        #region [ Constructors ]

        public SignContentReport(SignInfo signInfo)
        {
            this.SignType = signInfo.SignType;
            this.SignDate = signInfo.SignDate;
            this.Reason = signInfo.Reason;
            this.IsVerified = signInfo.IsVerified;
            this.Certificate = signInfo.Certificate;
            this.Children = signInfo.Children;
        }

        #endregion

        #region [ Properties ]

        public int Id { get; set; }
        public int RowCount { get; set; }

        public string Heading { get; set; }
        public string SHA1 { get; set; }
        public int Level { get; set; }

        #endregion

    }
}
