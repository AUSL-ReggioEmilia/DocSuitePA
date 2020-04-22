using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BiblosDS.Library.Common.Objects;
using CCypher;
using System.IO;
using System.ComponentModel;
using System.Collections;
using BiblosDS.WCF.DigitalSign;
using System.Net;

namespace BiblosDS.Library.Common.CompEd
{
    public class CompEdService
    {
        static Digest Digest;

        public List<DocumentCertificate> GetDocumentCertificates(byte[] blob)
        {
            // usa la 
            List<DocumentCertificate> result = new List<DocumentCertificate>();
            using (CompEdLib p7m = new CompEdLib())
            {
                SimplyCert firstExp = new SimplyCert();
                Hashtable SimCrtLst;
                String rt = p7m.GetAllExpiryDate(blob, out firstExp, out SimCrtLst);

                foreach (SimplyCert thisCert in SimCrtLst.Values)
                {
                    DocumentCertificate thisDocCert = new DocumentCertificate(); 
                    thisDocCert.DateExpiration = thisCert.Expiry ; 
                    thisDocCert.DateValidFrom = thisCert.ValidFrom ; 
                    thisDocCert.Description = thisCert.Description ; 
                    thisDocCert.Email = thisCert.eMail ; 
                    thisDocCert.FiscalCode = thisCert.FiscalCode ; 
                    thisDocCert.HeaderInfo = thisCert.HeaderInfo ; 
                    thisDocCert.IsOnDisk = false ; 
                    thisDocCert.Issuer = thisCert.Issuer ; 
                    thisDocCert.Level = thisCert.Level ; 
                    thisDocCert.Loaded = false ; 
                    thisDocCert.Role = thisCert.Role ;
                    thisDocCert.Type = thisCert.Type;
                    thisDocCert.Id = thisCert.Id;
                    thisDocCert.CertificateVersion = thisCert.Version;
                    thisDocCert.SerialNumber = thisCert.SerialNumber; 

                    result.Add(thisDocCert); 
                }
            }

            return result; 
        }

        public List<DocumentCertificate> GetDocumentCertificates(string filepathname)
        {
            List<DocumentCertificate> retval = null;
            if (!string.IsNullOrEmpty(filepathname) && File.Exists(filepathname))
            {
                retval = this.GetDocumentCertificates(File.ReadAllBytes(filepathname));
            }
            else
            {
                throw new Exception(this.GetType().Name + "::GetDocumentCertificates(filename)= invalid file path or name");
            }
            return retval;
        }

        /// <summary>
        /// torna l'elenco dei certificati di firma e il certificato con la scadenza più vicina
        /// </summary>
        /// <param name="fileName">nome del file</param>
        /// <param name="Content">contenuto</param>
        /// <param name="FirstCertificate">ritorna il primo certificato in scadenza</param>
        /// <returns>elenco dei certificati di firma</returns>
        public BindingList<DocumentCertificate> GetAllExpireDates(string fileName, DocumentContent Content, out DocumentCertificate FirstCertificate)
        {
            BindingList<DocumentCertificate> result = new BindingList<DocumentCertificate>();
            using (CompEdLib p7m = new CompEdLib())
            {
                SimplyCert firstExp = new SimplyCert();
                Hashtable SimCrtLst;
                String rt = p7m.GetAllExpiryDate(Content.Blob, out firstExp, out SimCrtLst);
                
                
                if (rt.Length > 0)
                    throw new Exception(rt);
                
                FirstCertificate = new DocumentCertificate(firstExp.Level, firstExp.Type,
                    firstExp.FiscalCode, firstExp.Role, firstExp.Description, firstExp.Issuer,
                    firstExp.eMail, firstExp.Expiry, firstExp.ValidFrom);

                DocumentCertificate certificate;
                foreach (System.Collections.DictionaryEntry obj in SimCrtLst)
                {
                    SimplyCert crtlo = (SimplyCert)obj.Value;
                    certificate = new DocumentCertificate(crtlo.Level, crtlo.Type,
                        crtlo.FiscalCode, crtlo.Role, crtlo.Description,
                        crtlo.Issuer, crtlo.eMail, crtlo.Expiry, crtlo.ValidFrom);
                    certificate.HeaderInfo = crtlo.HeaderInfo;
                    result.Add(certificate);
                }
            }
            return result;
        }

        public CompEdService()
        {
            if (CompEdService.Digest == null)
                CompEdService.Digest = new Digest();
        }

        /// <summary>
        /// ritorna il numero di firme del documento
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        /// <remarks>Suporta PADES</remarks>
        public int GetSignatureCount(DocumentContent content)
        {
            using (CompEdLib p7m = new CompEdLib())
            {
                return p7m.GetSignatureCount(content.Blob);
            }
        }
    }
}
