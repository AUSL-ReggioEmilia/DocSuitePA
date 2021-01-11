using System;
using System.Collections.Generic;
using iTextSharp.text.pdf.security;
using VecompSoftware.Commons.Infos;

namespace VecompSoftware.Helpers.iTextSharp
{
    public class PdfSignInfo : SignInfo
    {

        public PdfSignInfo(PdfPKCS7 pkcs7)
        {
            SignType = SignTypes.PAdES.ToString();
            SignDate = pkcs7.SignDate;
            Reason = pkcs7.Reason;
            try
            {
                IsVerified = pkcs7.Verify();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Verifica validità fallita: {0}", ex.Message));
            }

            Certificate = new SigningCertificate()
            {
                SerialNumber = pkcs7.SigningCertificate.SerialNumber.IntValue.ToString("X"),
                NotBefore = pkcs7.SigningCertificate.NotBefore,
                NotAfter = pkcs7.SigningCertificate.NotAfter
            };

            var issuerFields = CertificateInfo.GetIssuerFields(pkcs7.SigningCertificate);
            Certificate.Issuer = new IssuerDN()
            {
                SerialNumber = issuerFields.GetField("SN"),
                Organization = issuerFields.GetField("O"),
                OrganizationUnit = issuerFields.GetField("OU")
            };

            var subjectFields = CertificateInfo.GetSubjectFields(pkcs7.SigningCertificate);
            Certificate.Subject = new SubjectDN()
            {
                SerialNumber = subjectFields.GetField("SN"),
                GivenName = subjectFields.GetField("GIVENNAME"),
                Surname = subjectFields.GetField("SURNAME"),
                CommonName = subjectFields.GetField("CN"),
                Organization = subjectFields.GetField("O")
            };

            if (pkcs7.TimeStampToken != null)
                Children = new List<SignInfo>() { new TsSignInfo(pkcs7) };
        }

    }
}
