using CCypher;
using System;
using System.Linq;
using VecompSoftware.Common;

namespace VecompSoftware.CompEd
{
    public class P7kSignInfo : SignInfo
    {

        #region [ Constants ]

        protected const string PKCS7FILEHEADERDESCRIPTION = "PKCS7-File-HeaderDescription";

        #endregion

        #region [ Constructors ]

        public P7kSignInfo(DigestSession digest, int pades, int p7k, int x509, int num, SignTypes signType)
        {
            SignType = signType.ToString();
            SignDate = (DateTime?)digest.P7kGetSignAttribute(p7k, num, "signingTime");
            var unstructuredName = digest.P7kGetSignAttribute(p7k, false, num, "unstructuredName");
            Reason = GetReason((string)unstructuredName);

            switch (signType)
            {
                case SignTypes.PAdES:
                    IsVerified = digest.PADES_Verify(pades);
                    break;
                default:
                    IsVerified = digest.P7kVerifySignature(p7k, num);
                    break;
            }

            Certificate = new SigningCertificate()
            {
                SerialNumber = digest.X509GetInfo(x509, enumCertInfoCategory.CC_GENERAL, enumCertInfoItem.CI_SERIALNUMBER),
                NotBefore = digest.X509GetInfo(x509, enumCertInfoCategory.CC_GENERAL, enumCertInfoItem.CI_VALID_FROM).ChangeType<DateTime>(),
                NotAfter = digest.X509GetInfo(x509, enumCertInfoCategory.CC_GENERAL, enumCertInfoItem.CI_VALID_TO).ChangeType<DateTime>()
            };

            Certificate.Issuer = new IssuerDN()
            {
                SerialNumber = digest.X509GetInfo(x509, enumCertInfoCategory.CC_ISSUER, enumCertInfoItem.CI_SERIALNUMBER),
                Organization = digest.X509GetInfo(x509, enumCertInfoCategory.CC_ISSUER, enumCertInfoItem.CI_NAME)
            };

            Certificate.Subject = new SubjectDN()
            {
                SerialNumber = digest.X509GetInfo(x509, enumCertInfoCategory.CC_SUBJECT, enumCertInfoItem.CI_FISCALCODE),
                GivenName = digest.X509GetInfo(x509, enumCertInfoCategory.CC_SUBJECT, enumCertInfoItem.CI_FIRSTNAME),
                Surname = digest.X509GetInfo(x509, enumCertInfoCategory.CC_SUBJECT, enumCertInfoItem.CI_LASTNAME),
                CommonName = digest.X509GetInfo(x509, enumCertInfoCategory.CC_SUBJECT, enumCertInfoItem.CI_NAME)
            };
        }
        public P7kSignInfo(DigestSession digest, int p7k, int x509, int num) : this(digest, 0, p7k, x509, num, SignTypes.CAdES) { }

        #endregion

        #region [ Methods ]

        protected string GetReason(string unstructuredName)
        {
            if (string.IsNullOrWhiteSpace(unstructuredName))
                return null;

            return unstructuredName.ToPairs(';', '=')
                .FirstOrDefault(p => p.Key.EqualsIgnoreCase(PKCS7FILEHEADERDESCRIPTION)).Value;
        }

        #endregion

    }
}
