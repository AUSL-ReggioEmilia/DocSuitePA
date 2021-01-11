using CCypher;
using System;
using VecompSoftware.Common;

namespace VecompSoftware.CompEd
{
    public class TSSignInfo : SignInfo
    {

        #region [ Constructors ]

        public TSSignInfo(DigestSession digest, int p7x, int ts, int index)
        {
            SignType =  SignTypes.TimeStamp.ToString();
            SignDate = digest.TSGetDateAndTime(ts);
            IsVerified = digest.TSVerify(ts);

            Certificate = new SigningCertificate()
            {
                SerialNumber = digest.TSGetTSPInfo(ts, enumCertInfoCategory.CC_GENERAL, enumCertInfoItem.CI_SERIALNUMBER),
                NotBefore = digest.TSGetTSPInfo(ts, enumCertInfoCategory.CC_GENERAL, enumCertInfoItem.CI_VALID_FROM).ChangeType<DateTime>(),
                NotAfter = digest.TSGetTSPInfo(ts, enumCertInfoCategory.CC_GENERAL, enumCertInfoItem.CI_VALID_TO).ChangeType<DateTime>()
            };

            Certificate.Issuer = new IssuerDN()
            {
                Organization = digest.TSGetTSPInfo(ts, enumCertInfoCategory.CC_ISSUER, enumCertInfoItem.CI_NAME)
            };

            Certificate.Subject = new SubjectDN()
            {
                CommonName = digest.TSGetTSPInfo(ts, enumCertInfoCategory.CC_SUBJECT, enumCertInfoItem.CI_NAME)
            };
        }

        #endregion

    }
}
