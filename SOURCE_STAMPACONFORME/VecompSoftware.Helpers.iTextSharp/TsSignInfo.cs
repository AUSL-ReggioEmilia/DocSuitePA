using System;
using System.Linq;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using VecompSoftware.Commons.Infos;

namespace VecompSoftware.Helpers.iTextSharp
{
    public class TsSignInfo : SignInfo
    {
        #region [ Constants ]

        private const string OID_CN = "2.5.4.3";
        private const string OID_OU = "2.5.4.11";
        private const string OID_O = "2.5.4.10";
        private const string OID_C = "2.5.4.6";

        #endregion

        public TsSignInfo(PdfPKCS7 pkcs7)
        {
            SignType = "TimeStamp";
            SignDate = pkcs7.TimeStampDate;
            try
            {
                IsVerified = pkcs7.VerifyTimestampImprint();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Verifica validità fallita: {0}", ex.Message));
            }

            Certificate = new SigningCertificate()
            {
                SerialNumber = pkcs7.TimeStampToken.SignerID.SerialNumber.IntValue.ToString("X"),
            };
            if (pkcs7.TimeStampToken.SignerID.Certificate != null)
            {
                Certificate.NotBefore = pkcs7.TimeStampToken.SignerID.Certificate.NotBefore;
                Certificate.NotAfter = pkcs7.TimeStampToken.SignerID.Certificate.NotAfter;
            }

            var tsa = (X509Name)pkcs7.TimeStampToken.TimeStampInfo.Tsa.Name;
            Certificate.Issuer = new IssuerDN()
            {
                Organization = tsa.GetValues(new DerObjectIdentifier(OID_O)).ToArray().FirstOrDefault().ToString(),
                OrganizationUnit = tsa.GetValues(new DerObjectIdentifier(OID_OU)).ToArray().FirstOrDefault().ToString()
            };

            Certificate.Subject = new SubjectDN()
            {
                CommonName = tsa.GetValues(new DerObjectIdentifier(OID_CN)).ToArray().FirstOrDefault().ToString()
            };
        }
    }
}
